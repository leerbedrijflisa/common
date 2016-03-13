using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public class DynamicModel : DynamicObject
    {
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

        private object _metadata;
    }
}