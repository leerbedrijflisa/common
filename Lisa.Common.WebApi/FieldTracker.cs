using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    internal class FieldTracker
    {
        public IEnumerable<string> MissingFields
        {
            get
            {
                return from field in _fields
                       where field.Value == FieldStatus.Required
                       select field.Key;
            }
        }

        public bool IsValid(string fieldName)
        {
            return _fields.Any(f => string.Equals(f.Key, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public void MarkRequired(string fieldName)
        {
            var field = FindField(fieldName);
            if (field.Key == null)
            {
                _fields[fieldName] = FieldStatus.Required;
            }
        }

        public void MarkOptional(string fieldName)
        {
            var field = FindField(fieldName);
            if (field.Key == null)
            {
                _fields[fieldName] = FieldStatus.Optional;
            }
        }

        public void MarkPresent(string fieldName)
        {
            _fields[fieldName] = FieldStatus.Present;
        }

        private KeyValuePair<string, FieldStatus> FindField(string fieldName)
        {
            return _fields.SingleOrDefault(f => string.Equals(f.Key, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        private Dictionary<string, FieldStatus> _fields = new Dictionary<string, FieldStatus>();
    }
}