using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public class DynamicModel : DynamicObject
    {
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
            var memberName = binder.Name.ToLowerInvariant();
            var property = _properties.SingleOrDefault(p => p.Key.ToLowerInvariant() == memberName);
            if (property.Key == null)
            {
                return base.TryGetMember(binder, out result);
            }

            result = property.Value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private object _metadata;
    }
}