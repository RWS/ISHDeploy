using ISHDeploy.Business.Enums;
using ISHDeploy.Business.Operations.ISHUIOperation;
using ISHDeploy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISHDeploy.Models.UI
{
    abstract class UIBase
    {
        protected XElement element;
        protected string filePath;
        protected string rootPath;
        protected string childItemPath;
        protected string keyAttribute;
        internal void Set(ILogger Logger, ISHDeployment ISHDeployment)
        {
            CreateXMLNode();
            new SetUIOperation(
                Logger,
                ISHDeployment,
                filePath,
                rootPath,
                childItemPath,
                element,
                keyAttribute).Run();
        }
        internal void Remove(ILogger Logger, ISHDeployment ISHDeployment) {
            CreateXMLNode();
            new RemoveUIOperation(
                Logger,
                ISHDeployment,
                filePath,
                rootPath,
                childItemPath,
                element,
                keyAttribute).Run();
        }
        internal void Move(ILogger Logger, ISHDeployment ISHDeployment, OperationType operation, string after) { 
            new MoveUIOperation(
                    Logger,
                    ISHDeployment,
                    filePath,
                    rootPath,
                    childItemPath,
                    null,
                    keyAttribute,
                    operation,
                    after).Run();
        }
        private void CreateXMLNode()
        {// will create XML node in accordance with attribute bindidng
            Type type = this.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                if (value != null && property.IsDefined(typeof(UIBindingAttribute), false))
                {
                    var bindingyAttribute = (UIBindingAttribute)property.GetCustomAttributes(typeof(UIBindingAttribute), false).First();
                    if (bindingyAttribute.isNestedElement)
                    { // For nested XML elements
                        foreach (string nestedElement in (string[])value)
                        {
                            element.Add(new XElement(bindingyAttribute.xmlAttributeName, nestedElement));
                        }
                    }
                    else
                    { //Add XML attribute only
                        element.Add(new XAttribute(bindingyAttribute.xmlAttributeName, value));
                    }

                }
            }
        }
    }
}
