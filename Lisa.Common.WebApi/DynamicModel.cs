using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Common.WebApi
{
    public class DynamicModel : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _properties[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
    }
}