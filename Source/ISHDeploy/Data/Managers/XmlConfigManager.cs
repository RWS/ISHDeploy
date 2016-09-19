/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ISHDeploy.Business.Enums;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Performs different kinds of operations with xml file
    /// </summary>
    /// <seealso cref="IXmlConfigManager" />
    public class XmlConfigManager : IXmlConfigManager
    {
        #region Private constants

        /// <summary>
        /// The input configuration parameter xml path
        /// </summary>
        private const string InputConfigParamXmlPath = "inputconfig/param";

        /// <summary>
        /// The name xml attribute
        /// </summary>
        private const string NameXmlAttr = "name";

        /// <summary>
        /// The current value xml node
        /// </summary>
        private const string CurrentValueXmlNode = "currentvalue";

        #endregion

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Returns new instance of the <see cref="XmlConfigManager"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        public XmlConfigManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Returns dictionary with all parameters from inputparameters.xml file
        /// </summary>
        /// <param name="filePath">Path to inputparameters.xml file</param>
        /// <returns>Dictionary with parameters</returns>
        public Dictionary<string, string> GetAllInputParamsValues(string filePath)
        {
            _logger.WriteDebug("Retrieve input parameters", filePath);

            var doc = _fileManager.Load(filePath);
            var dictionary = new Dictionary<string, string>();

            var paramElements = doc.XPathSelectElements(InputConfigParamXmlPath);

            foreach (var paramElement in paramElements)
            {
                var name = paramElement.Attribute(XName.Get(NameXmlAttr)).Value;
                var currentValue = paramElement.XPathSelectElement(CurrentValueXmlNode).Value;

                dictionary.Add(name, currentValue);
            }

            _logger.WriteVerbose($"The input parameters from file `{filePath}` has been retrieved");

            return dictionary;
        }

        /// <summary>
        /// Removes single node or comment in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        public void RemoveSingleNode(string filePath, string xpath)
        {
            _logger.WriteDebug("Remove node", xpath, filePath);

            var doc = _fileManager.Load(filePath);

            var node = SelectSingleNode(ref doc, xpath);
            if (node == null)
            {
                _logger.WriteVerbose($"The file `{filePath}` does not contain node within the xpath `{xpath}`");
                return;
            }

            node.Remove();

            _fileManager.Save(filePath, doc);

            _logger.WriteVerbose($"The node within the xpath `{xpath}` has been removed from file `{filePath}`");
        }

        /// <summary>
        /// Removes nodes in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched nodes</param>
        public void RemoveNodes(string filePath, string xpath)
        {
            _logger.WriteDebug("Remove nodes", xpath, filePath);

            var doc = _fileManager.Load(filePath);

            var nodes = SelectNodes(ref doc, xpath).ToArray();
            if (nodes.Length == 0)
            {
                _logger.WriteVerbose($"The file `{filePath}` does not contain nodes within the xpath `{xpath}`");
                return;
            }

            nodes.Remove();

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"Nodes within the xpath `{xpath}` have been removed from file `{filePath}`");
        }

        /// <summary>
        /// Removes node in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        /// <param name="insertBeforeXpath">XPath to searched node</param>
        public void MoveBeforeNode(string filePath, string xpath, string insertBeforeXpath)
        {
            _logger.WriteDebug($"Move node before `{insertBeforeXpath}`", xpath, filePath);

            var doc = _fileManager.Load(filePath);

            var nodes = SelectNodes(ref doc, xpath).ToArray();
            if (nodes.Length == 0)
            {
                _logger.WriteVerbose($"The file `{filePath}` does not contain nodes within the xpath {xpath}");
                _logger.WriteWarning("Do not able to find target node to insert before");
                return;
            }

            XNode insertBeforeNode = null;
            if (string.IsNullOrEmpty(insertBeforeXpath))
            {
                // In this case we need to add node to the top of the document
                var parentNode = nodes.First().Parent;
                if (parentNode != null)
                {
                    insertBeforeNode = parentNode.FirstNode;
                }
            }
            else
            {
                insertBeforeNode = doc.XPathSelectElement(insertBeforeXpath);
            }

            if (insertBeforeNode == null)
            {
                _logger.WriteWarning($"{filePath} does not contain target node to insert before");
                return;
            }

            foreach (var nodeToMove in nodes.Reverse())
            {
                if (nodeToMove != insertBeforeNode)
                {
                    nodeToMove.Remove();
                    insertBeforeNode.AddBeforeSelf(nodeToMove);

                    insertBeforeNode = nodeToMove;
                }
            }

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The node within the xpath `{xpath}` has been moved before `{insertBeforeXpath}` in file `{filePath}`");
        }

        /// <summary>
	    /// Removes node in xml file that can be found by <paramref name="xpath"/>
	    /// </summary>
	    /// <param name="filePath">Path to the file that is modified</param>
	    /// <param name="xpath">XPath to searched node</param>
	    /// <param name="insertAfterXpath">XPath to searched node</param>
	    public void MoveAfterNode(string filePath, string xpath, string insertAfterXpath)
        {
            _logger.WriteDebug($"Move node after `{insertAfterXpath}`", xpath, filePath);

            var doc = _fileManager.Load(filePath);

            var nodes = SelectNodes(ref doc, xpath).ToArray();
            if (nodes.Length == 0)
            {
                _logger.WriteVerbose($"{filePath} does not contain nodes within the xpath {xpath}");
                _logger.WriteWarning("Not able to find the target node");
                return;
            }

            XNode insertAfterNode = null;
            if (string.IsNullOrEmpty(insertAfterXpath))
            {
                // In this case we need to add node to the bottom of the document
                var parentNode = nodes.First().Parent;
                if (parentNode != null)
                {
                    insertAfterNode = parentNode.LastNode;
                }
            }
            else
            {
                insertAfterNode = doc.XPathSelectElement(insertAfterXpath);
            }

            if (insertAfterNode == null)
            {
                _logger.WriteVerbose($"Do not able to find target node within the xpath `{insertAfterXpath}` in file `{filePath}` to insert after it the node `{xpath}`");
                _logger.WriteWarning("Not able to find the target node");
                return;
            }

            foreach (var nodeToMove in nodes)
            {
                if (nodeToMove != insertAfterNode)
                {
                    nodeToMove.Remove();
                    insertAfterNode.AddAfterSelf(nodeToMove);

                    insertAfterNode = nodeToMove;
                }
            }

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The node within the xpath `{xpath}` has been moved after `{insertAfterXpath}` in file `{filePath}`");
        }

        /// <summary>
        /// Comments node in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        /// <param name="encodeInnerXml">True if content of the comment should be encoded; otherwise False.</param>
        public void CommentNode(string filePath, string xpath, bool encodeInnerXml = false)
        {
            _logger.WriteDebug($"Comment{(encodeInnerXml ? " and encode" : "")} xml node", xpath, filePath);

            var doc = _fileManager.Load(filePath);

            var uncommentedNode = doc.XPathSelectElement(xpath);

            if (uncommentedNode == null)
            {
                _logger.WriteVerbose($"{filePath} does not contain uncommented node within the xpath {xpath}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            if (encodeInnerXml)
            {
                uncommentText = XmlEncode(uncommentText);
            }

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The node within the xpath `{xpath}` has been commented{(encodeInnerXml ? " and encoded" : "")} in file `{filePath}`");
        }

        /// <summary>
        /// Comments all nodes that has <paramref name="searchPattern"/> right above it
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node</param>
        public void CommentNodesByPrecedingPattern(string filePath, string searchPattern)
        {
            _logger.WriteDebug($"Comment all nodes that have `{searchPattern}`", filePath);

            var doc = _fileManager.Load(filePath);

            var patternNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern)).ToArray();

            if (!patternNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            XComment commentedNode = null;

            foreach (var patternNode in patternNodes)
            {
                var uncommentedNode = patternNode.NextNode;

                if (uncommentedNode == null)
                {
                    throw new WrongXmlStructureException(filePath, $"Comment pattern has no following nodes after searched comment '{searchPattern}'");
                }

                if (uncommentedNode.NodeType == XmlNodeType.Comment)
                {
                    _logger.WriteVerbose($"{filePath} contains already commented node following after pattern {searchPattern}");
                    continue;
                }

                var uncommentText = XmlEncode(uncommentedNode.ToString());

                commentedNode = new XComment(uncommentText);

                uncommentedNode.ReplaceWith(commentedNode);
            }

            if (commentedNode != null) // means that file was changed
            {
                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"All nodes that have `{searchPattern}` have been commented in file `{filePath}`");
            }
        }

        /// <summary>
        /// Uncomments all nodes that has <paramref name="searchPattern"/> right above it.
        /// </summary>
        /// <param name="filePath">Path to the file that is modified.</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node.</param>
        public void UncommentNodesByPrecedingPattern(string filePath, string searchPattern)
        {
            _logger.WriteDebug($"Unomment all nodes that have `{searchPattern}`", filePath);

            var doc = _fileManager.Load(filePath);

            var patternNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern)).ToArray();

            if (!patternNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            var isFileChanged = false;

            foreach (var patternNode in patternNodes)
            {
                XNode commentedNode = patternNode.NextNode;

                if (commentedNode == null)
                {
                    throw new WrongXmlStructureException(filePath, $"Comment pattern has no following nodes after searched comment '{searchPattern}'");
                }

                if (commentedNode.NodeType != XmlNodeType.Comment)
                {
                    _logger.WriteVerbose($"{filePath} contains already uncommented node following after pattern {searchPattern}");
                    continue;
                }

                if (!TryUncommentNode(commentedNode, ref doc))
                {
                    throw new WrongXmlStructureException(filePath, $"Was not able to uncomment the following node: {commentedNode}");
                }
                isFileChanged = true;
            }

            if (isFileChanged)
            {
                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"All nodes that have `{searchPattern}` have been uncommented in file `{filePath}`");
            }
        }

        /// <summary>
        /// Uncomment multiple nodes that can be found by inner pattern
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is inside commented node</param>
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        public void UncommentNodesByInnerPattern(string filePath, string searchPattern, bool decodeInnerXml = false)
        {
            _logger.WriteDebug($"Uncomment{(decodeInnerXml ? " and decoding" : "")} all xml nodes that can be found by `{searchPattern}`", filePath);

            var doc = _fileManager.Load(filePath);

            var searchedNodes = doc.DescendantNodes()
                .Where(node => node.ToString().Contains(searchPattern)).ToArray();

            if (!searchedNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            var commentedNodes = searchedNodes.Where(node => node.NodeType == XmlNodeType.Comment).ToArray();

            if (!commentedNodes.Any())
            {
                _logger.WriteVerbose($"{filePath} contains already uncommented node by searched pattern '{searchPattern}'.");
                return;
            }

            foreach (var commentedNode in commentedNodes)
            {
                if (!TryUncommentNode(commentedNode, ref doc, decodeInnerXml))
                {
                    throw new WrongXmlStructureException(filePath, $"Was not able to uncomment the following node: {commentedNode}");
                }
            }

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"All xml nodes that can be found by `{searchPattern}` have been uncommented{(decodeInnerXml ? " and decoded" : "")} in file `{filePath}`");
        }

        /// <summary>
        /// Set attribute value by attribute XPath
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="attributeXpath">XPath the attribute that will be modified</param>
        /// <param name="value">Attribute new value</param>
        public void SetAttributeValue(string filePath, string attributeXpath, string value)
        {
            _logger.WriteDebug($"Set new value `{value}` for attribute `{attributeXpath}`", filePath);

            var doc = _fileManager.Load(filePath);
            var attr = ((IEnumerable<object>)doc.XPathEvaluate(attributeXpath)).OfType<XAttribute>().SingleOrDefault();
            if (attr == null)
            {
                // TODO: Create TryGetElementByXPath action or something similar to use it before run SetAttributeValue to avoid access to nonexistent elements
                // and change WriteVerbose on "throw new WrongXPathException(filePath, attributeXpath);" 
                _logger.WriteVerbose($"{filePath} does not contain attribute at '{attributeXpath}'.");
                return;
            }

            attr.SetValue(value);
            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The value of attribute `{attributeXpath}` has been set to '{value}' in file `{filePath}`");
        }

        /// <summary>
        /// Set xml node
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath that is searched</param>
        /// <param name="xNode">The xml node from ISH configuration.</param>
        /// <param name="replaceIfExists">if set to <c>true</c> replaces existing node if exists, otherwise does nothing.</param>
        public void SetNode(string filePath, string xpath, IISHXmlNode xNode, bool replaceIfExists = true)
        {
            _logger.WriteDebug("Set node", xpath, filePath);

            var doc = _fileManager.Load(filePath);
            XNode newNode = xNode.ToXElement();
            var existingElement = doc.XPathSelectElement(xpath);
            if (existingElement == null)
            {
                // We need to take the parent node
                var parentXPath = Regex.Replace(xpath, @"\/([^\/])*$", "");
                var parentNode = doc.XPathSelectElement(parentXPath);
                if (parentNode == null)
                {
                    throw new WrongXmlStructureException(filePath, $"There are no parent node for XPath '{xpath}' were found.");
                }

                parentNode.Add(newNode);
            }
            else if (replaceIfExists)
            {
                existingElement.ReplaceWith(newNode);
            }
            else
            {
                _logger.WriteDebug($"No modifications was done to the file `{filePath}`");
                return;
            }

            // Check if node does not have a comment
            if (newNode.PreviousNode == null || newNode.PreviousNode.NodeType != XmlNodeType.Comment)
            {
                var comment = xNode.GetNodeComment();
                if (comment != null)
                {
                    newNode.AddBeforeSelf(comment);
                }
            }

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The node with xpath `{xpath}` has been set in file `{filePath}`");
        }

        /// <summary>
        /// Inserts a new node before specified one.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        /// <exception cref="WrongXPathException"></exception>
        public void InsertBeforeNode(string filePath, string xpath, string xmlString)
        {
            _logger.WriteDebug($"Insert new node before `{xpath}`", filePath);

            var doc = _fileManager.Load(filePath);

            var relativeElement = doc.XPathSelectElement(xpath);
            if (relativeElement?.Parent == null)
            {
                throw new WrongXPathException(filePath, xpath);
            }

            var newElement = XElement.Parse(xmlString);
            XNodeEqualityComparer equalityComparer = new XNodeEqualityComparer();
            foreach (var node in relativeElement.Parent.Nodes())
            {
                if (equalityComparer.Equals(node, newElement))
                {
                    _logger.WriteVerbose($"The element with xpath '{xpath}' already contains element '{xmlString}' before it.");
                    return;
                }
            }

            relativeElement.AddBeforeSelf(newElement);

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The new node has been inserted before node with xpath `{xpath}` in file `{filePath}`");
        }

        /// <summary>
        /// Inserts or update the element of UI.
        /// </summary>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        public void InsertOrUpdateUIElement(string filePath, BaseUIElement model)
        {
            _logger.WriteDebug("Insert/Update UI element", filePath);
            var doc = _fileManager.Load(filePath);
            var element = XElement.Parse(Serialize(model));

            var found = FindElement(doc, model.NameOfRootElement, model.NameOfItem, model.KeyAttribute, element.Attribute(model.KeyAttribute).Value);

            if (found == null)
            {
                _logger.WriteDebug("Insert UI element", $"<{model.NameOfItem} {model.KeyAttribute}=`{element.Attribute(model.KeyAttribute).Value}`>", filePath);
                var memberList = doc.Element(model.NameOfRootElement).Elements(model.NameOfItem).ToList();
                if (!memberList.Any())
                {
                    doc.Element(model.NameOfRootElement).Add(element);
                }
                else
                {
                    memberList.Last().AddAfterSelf(element);
                }

                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"The new element has been inserted in file `{filePath}`");
            }
            else
            {
                _logger.WriteDebug("Update UI element", $"<{model.NameOfItem} {model.KeyAttribute}=`{element.Attribute(model.KeyAttribute).Value}`>", filePath);
                found.ReplaceWith(element);

                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"The element has been updated in file `{filePath}`");
            }
        }

        /// <summary>
        /// Removes the element of UI.
        /// </summary>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        public void RemoveUIElement(string filePath, BaseUIElement model)
        {
            _logger.WriteDebug($"Remove UI element {model.NameOfItem}", filePath);

            var doc = _fileManager.Load(filePath);

            var element = XElement.Parse(Serialize(model));

            _logger.WriteDebug("Remove UI element", $"<{model.NameOfItem} {model.KeyAttribute}=`{element.Attribute(model.KeyAttribute).Value}`>", filePath);

            var found = FindElement(doc, model.NameOfRootElement, model.NameOfItem, model.KeyAttribute, element.Attribute(model.KeyAttribute).Value);

            if (found != null)
            {
                found.Remove();
                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"The element `{element.Attribute(model.KeyAttribute).Value}` has been removed from file `{filePath}`");
            }
        }

        /// <summary>
        /// Moves the UI element.
        /// </summary>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        /// <param name="direction">The direction to move.</param>
        /// <param name="after">The id of element to move after it.</param>
        /// <exception cref="System.Exception">
        /// Could not find source element
        /// or
        /// Could not find target element
        /// or
        /// Unknown operation
        /// </exception>
        public void MoveUIElement(string filePath, BaseUIElement model, MoveElementDirection direction, string after)
        {
            _logger.WriteDebug($"Move UI element {model.NameOfItem}", filePath);
            var doc = _fileManager.Load(filePath);

            var element = XElement.Parse(Serialize(model));

            _logger.WriteDebug($"Move UI element <{model.NameOfItem} {model.KeyAttribute}=`{element.Attribute(model.KeyAttribute).Value}`> {(direction == MoveElementDirection.After ? $"{direction} {after}" : $"to {direction} position")}", filePath);

            var found = FindElement(doc, model.NameOfRootElement, model.NameOfItem, model.KeyAttribute, element.Attribute(model.KeyAttribute).Value);

            if (found != null)
            {
                string verboseMessage = "";
                switch (direction)
                {
                    case MoveElementDirection.First:
                        doc.Element(model.NameOfRootElement).AddFirst(found);
                        verboseMessage = "The UI element has been moved to the first position";
                        break;
                    case MoveElementDirection.Last:
                        var lastElement = doc.Element(model.NameOfRootElement).Elements(model.NameOfItem).LastOrDefault();
                        if (lastElement != null)
                        {
                            lastElement.AddAfterSelf(found);
                        }
                        else
                        {
                            doc.Element(model.NameOfRootElement).Add(found);
                        }
                        verboseMessage = "The UI element has been moved to the last position";
                        break;
                    case MoveElementDirection.After:
                        var afterElement = FindElement(doc, model.NameOfRootElement, model.NameOfItem, model.KeyAttribute, after);
                        if (afterElement == null)
                        {
                            verboseMessage = $"Do not able to find target element `{after}` to insert after it the node `{element.Attribute(model.KeyAttribute).Value}`";
                            _logger.WriteWarning("Not able to find the target node");
                        }
                        else
                        {
                            afterElement.AddAfterSelf(found);
                            verboseMessage = $"The UI element has been moved after {after}";
                        }
                        break;
                }

                found.Remove();
                _fileManager.Save(filePath, doc);
                _logger.WriteVerbose($"{verboseMessage} in file {filePath}");
            }
        }

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="nameOfRootElement">The name of root element.</param>
        /// <param name="nameOfItem">The name of item.</param>
        /// <param name="keyAttribute">The key attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private XElement FindElement(XDocument doc, string nameOfRootElement, string nameOfItem, string keyAttribute, string value)
        {
            var element = doc.Element(nameOfRootElement)
                           .Elements(nameOfItem)
                           .FirstOrDefault(item => item.Attribute(keyAttribute).Value == value);

            if (element == null)
            {
                _logger.WriteDebug("Does not contain element", $"<{nameOfItem} {keyAttribute}=`{value}`>");
                _logger.WriteVerbose($"The file does not contain item with identifier {value}");
            }

            return element;
        }

        /// <summary>
        /// Set element value.
        /// </summary>
        /// <param name="filePath">Path to the file that is modified.</param>
        /// <param name="xpath">XPath of searched element.</param>
        /// <param name="value">The new value of element.</param>
        public void SetElementValue(string filePath, string xpath, string value)
        {
            _logger.WriteDebug($"Set new value '{value}' for element `{xpath}`", filePath);

            var doc = _fileManager.Load(filePath);

            var element = doc.XPathSelectElement(xpath);

            if (element == null)
            {
                _logger.WriteWarning($"{filePath} does not contain element '{xpath}'.");
                return;
            }
            element.SetValue(value);

            _fileManager.Save(filePath, doc);
            _logger.WriteVerbose($"The new value '{value}' has been set for element with xpath `{xpath}` in file `{filePath}`");
        }

        /// <summary>
        /// Gets the value from element found by xpath.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <returns>The element value.</returns>
        public string GetValue(string filePath, string xpath)
        {
            _logger.WriteDebug($"Get value of element `{xpath}`", filePath);

            var doc = _fileManager.Load(filePath);
            var node = ((IEnumerable<object>)doc.XPathEvaluate(xpath)).SingleOrDefault();
            if (node == null)
            {
                throw new WrongXPathException(filePath, xpath);
            }

            if (node is XAttribute)
            {
                var attr = (XAttribute)node;
                _logger.WriteVerbose($"The value of element with xpath `{xpath}` has been retrieved from file `{filePath}`");
                return attr.Value;
            }

            var element = (XElement)node;
            _logger.WriteVerbose($"The value of element with xpath `{xpath}` has been retrieved from file `{filePath}`");
            return element.Value;
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Serialize<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            var serializer = new XmlSerializer(value.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value, ns);
                }
                return textWriter.ToString();
            }
        }
        #region private methods

        /// <summary>
        /// Tries to uncomment node.
        /// </summary>
        /// <param name="commentedNode">The commented node.</param>
        /// <param name="doc">The document where changes should take place.</param>
        /// <returns>True if operation succeeded; otherwise False.</returns>
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        private bool TryUncommentNode(XNode commentedNode, ref XDocument doc, bool decodeInnerXml = false)
        {
            var commentText = commentedNode.ToString().TrimStart('<').TrimEnd('>');
            var startIndex = commentText.IndexOf('<');
            var endIndex = commentText.LastIndexOf('>');

            if (startIndex < 0 || endIndex < 0)
            {
                return false;
            }

            commentText = commentText.Substring(startIndex, endIndex - startIndex + 1);

            if (decodeInnerXml)
            {
                commentText = XmlDecode(commentText);
            }

            try
            {
                // XElement.Replace cannot be used because of possible unknown namespaces inside the comment.
                var commentedDocXmlString = doc.ToString();
                var replacedDocXmlString = commentedDocXmlString.Replace(commentedNode.ToString(), commentText);

                doc = XDocument.Parse(replacedDocXmlString);
            }
            catch (XmlException ex)
            {
                _logger.WriteWarning($"Replaced content can`t be parsed as XML, with following message {ex.Message}");
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Evaluates node from XPath
        /// </summary>
        /// <param name="doc">The document to node lookup.</param>
        /// <param name="xPath">The xPath of node to be evaluated.</param>
        /// <returns>
        /// XNode instance from document.
        /// </returns>
        private XNode SelectSingleNode(ref XDocument doc, string xPath)
        {
            return SelectNodes(ref doc, xPath).SingleOrDefault();
        }

        /// <summary>
        /// Evaluates nodes from XPath
        /// </summary>
        /// <param name="doc">The document to node lookup.</param>
        /// <param name="xPath">The xPath of node to be evaluated.</param>
        /// <returns>
        /// IEnumerable of XNodes from document.
        /// </returns>
        private IEnumerable<XNode> SelectNodes(ref XDocument doc, string xPath)
        {
            return ((IEnumerable<object>)doc.XPathEvaluate(xPath)).OfType<XNode>();
        }

        /// <summary>
        /// Replaces some characters that might cause issues when commenting/uncommenting xml fragment.
        /// </summary>
        /// <param name="text">The string that should be encoded.</param>
        /// <returns>Encoded string.</returns>
        private string XmlEncode(string text)
        {
            return text.Replace(@"\", @"\\").Replace(@"--", @"-\-");
        }

        /// <summary>
        /// Replaces some characters back that where changed by <see cref="XmlEncode"/>.
        /// </summary>
        /// <param name="text">The string that should be decoded.</param>
        /// <returns>Decoded string.</returns>
        private string XmlDecode(string text)
        {
            return text.Replace(@"-\-", @"--").Replace(@"\\", @"\");
        }

        #endregion
    }
}
