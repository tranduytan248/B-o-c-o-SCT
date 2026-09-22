using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CenIT.Libs.VNPTSmartCA.Helpers
{
    public static class FormUrlEncodedHelper
    {
        public static Dictionary<string, object> GenFormEncode<T>(T objData, bool getCustomAttr = false,
            Type attrType = null)
        {
            var lstProName = new Dictionary<string, object>();
            if (!getCustomAttr)
            {
                var props = TypeDescriptor.GetProperties(typeof(T));
                for (var i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    var p = objData.GetType().GetProperty(prop.Name);
                    lstProName.Add(prop.Name, p?.GetValue(objData));
                }
            }
            else
            {
                if (attrType == null) return null;

                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                foreach (var attr in prop.CustomAttributes)
                {
                    if (attrType != attr.AttributeType) continue;
                    var p = objData.GetType().GetProperty(prop.Name);
                    lstProName.Add(attr.NamedArguments?[0].TypedValue.Value.ToString() ?? "", p?.GetValue(objData));
                }
            }

            return lstProName;
        }
    }
}