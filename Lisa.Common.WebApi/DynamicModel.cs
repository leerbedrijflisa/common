using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace Lisa.Common.WebApi
{
    public class DynamicModel : DynamicObject
    {
        public DynamicModel()
        {
        }

        public object this[string name]
        {
            get
            {
                if (name.Contains("."))
                {
                    return GetValueOfNestedProperty(name, this);
                }

                var property = Properties.SingleOrDefault(p => string.Equals(p.Key, name, StringComparison.OrdinalIgnoreCase));
                if (property.Key == null)
                {
                    throw new KeyNotFoundException($"A property with the name {name} does not exist.");
                }

                return property.Value;
            }
            set
            {
                var property = Properties.SingleOrDefault(p => string.Equals(p.Key, name, StringComparison.OrdinalIgnoreCase));

                if (property.Key == null)
                {
                    Properties.Add(name, NormalizeValue(value));
                }
                else
                {
                    Properties[property.Key] = NormalizeValue(value);
                }
            }
        }

        public bool Contains(string name)
        {
            return Properties.Any(p => string.Equals(p.Key, name, StringComparison.OrdinalIgnoreCase));
        }

        public object GetMetadata()
        {
            return _metadata;
        }

        public void SetMetadata(object metadata)
        {
            _metadata = metadata;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = Properties.SingleOrDefault(p => string.Equals(p.Key, binder.Name, StringComparison.OrdinalIgnoreCase));
            result = property.Key == null ? null : property.Value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Properties[binder.Name] = NormalizeValue(value);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Properties.Keys;
        }

        internal IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        // Only copies the properties and not the metadata, because that's enough for the
        // validator. And because I don't know how to make a deep copy of an arbitrary object.
        internal DynamicModel Copy()
        {
            return new DynamicModel(Properties);
        }

        private DynamicModel(IDictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                Properties.Add(property.Key, property.Value);
            }
        }

        private object NormalizeValue(object value)
        {
            if (value is JValue)
            {
                return FromJValue((JValue) value);
            }
            else if (value is JObject)
            {
                var nestedModel = new DynamicModel();
                foreach (JProperty child in ((JObject) value).Properties())
                {
                    nestedModel[child.Name] = child.Value;
                }
                return nestedModel;
            }

            return value;
        }

        private object GetValueOfNestedProperty(string propertyName, object obj)
        {
            if (obj == null || propertyName.Length == 0)
            {
                return obj;
            }

            string name;
            int dotPosition = propertyName.IndexOf('.');
            if (dotPosition < 0)
            {
                name = propertyName;
                dotPosition = propertyName.Length - 1;
            }
            else
            {
                name = propertyName.Substring(0, dotPosition);
            }

            object value;
            if (obj is IDynamicMetaObjectProvider)
            {
                // Code adapted from http://stackoverflow.com/a/7108263
                var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, name, obj.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
                value =  callsite.Target(callsite, obj);
            }
            else
            {
                var propertyInfo = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                value = propertyInfo?.GetValue(obj);
            }

            string subPropertyName = propertyName.Substring(dotPosition + 1);
            return GetValueOfNestedProperty(subPropertyName, value);
        }

        private object FromJValue(JValue value)
        {
            switch (value.Type)
            {
                case JTokenType.String:
                    return value.Value<string>();

                case JTokenType.Float:
                    return value.Value<double>();

                case JTokenType.Boolean:
                    return value.Value<bool>();

                case JTokenType.Date:
                    return value.Value<DateTime>();

                case JTokenType.Integer:
                    return value.Value<int>();

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        private object _metadata;
    }
}