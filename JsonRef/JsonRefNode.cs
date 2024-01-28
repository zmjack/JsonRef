using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace JsonRef
{
    public class JsonRefNode : Dictionary<string, object>
    {
        public Guid Ref { get; }

        internal static PropertyInfo[] GetJsonProperties(Type type)
        {
            PropertyInfo[] props =
            [
                .. from prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                   where prop.GetCustomAttribute<JsonIgnoreAttribute>() is null
                   select prop,

                .. from prop in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)
                   where prop.GetCustomAttribute<JsonIncludeAttribute>() is not null
                   select prop,
            ];
            return props;
        }

        private static object GetPropValue(Dictionary<object, Guid> pointers, Dictionary<Guid, JsonRefElement> refs, Type type, object value)
        {
            if (value is null) return null;

            if (type.IsValueType || value is string)
            {
                return value;
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var len = (value as Array).Length;
                var arr = new object[len];

                foreach (var (i, obj) in (value as Array).Pairs())
                {
                    arr[i] = GetPropValue(pointers, refs, elementType, obj);
                }
                return arr;
            }
            else
            {
                if (pointers.TryGetValue(value, out Guid guid))
                {
                    return new JsonRefObject
                    {
                        Ref = guid,
                    };
                }
                else
                {
                    var node = new JsonRefNode(pointers, refs, type, value);
                    return new JsonRefObject
                    {
                        Ref = node.Ref,
                    };
                }
            }
        }

        internal JsonRefNode(Dictionary<object, Guid> pointers, Dictionary<Guid, JsonRefElement> refs, Type type, object value)
        {
            var guid = Guid.NewGuid();

            Ref = guid;
            pointers[value] = guid;
            refs[guid] = new JsonRefElement
            {
                Type = type.ToString(),
                Value = this,
            };

            foreach (var prop in GetJsonProperties(type))
            {
                var propType = prop.PropertyType;
                var key = StringEx.CamelCase(prop.Name);
                var subValue = prop.GetValue(value);
                this[key] = GetPropValue(pointers, refs, propType, subValue);
            }
        }
    }

}
