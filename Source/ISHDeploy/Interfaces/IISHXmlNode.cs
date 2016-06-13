/**
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
﻿using System.Xml.Linq;

namespace ISHDeploy.Interfaces
{
    /// <summary>
    /// Represents ISH xml nodes used in configuration.
    /// </summary>
    public interface IISHXmlNode
	{
		/// <summary>
		/// Gets node comemnt if exiss
		/// </summary>
		XComment GetNodeComment();

		/// <summary>
		/// Converts node to XElement
		/// </summary>
		XElement ToXElement();
    }
}
