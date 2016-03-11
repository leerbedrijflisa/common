using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public class DynamicModel : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var memberName = binder.Name.ToLowerInvariant();
            if (!_properties.ContainsKey(memberName))
            {
                return base.TryGetMember(binder, out result);
            }

            result = _properties[memberName];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var memberName = binder.Name.ToLowerInvariant();
            _properties[memberName] = value;
            return true;
        }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
    }
}