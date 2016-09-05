using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Models.UI
{
    [AttributeUsage(AttributeTargets.Property)]
    class UIBindingAttribute : Attribute
    {
        public string xmlAttributeName;
        public bool isNestedElement;

        public UIBindingAttribute(string xmlAttributeName, bool isNestedElement = false)
        {
            this.xmlAttributeName = xmlAttributeName;
            this.isNestedElement = isNestedElement;
        }
    }
}
