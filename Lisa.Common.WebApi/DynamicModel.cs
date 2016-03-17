using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
                    Properties.Add(name, value);
                }
                else
                {
                    Properties[property.Key] = value;
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
            if (property.Key == null)
            {
                return base.TryGetMember(binder, out result);
            }

            result = property.Value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Properties[binder.Name] = value;
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

        private object _metadata;
    }
}