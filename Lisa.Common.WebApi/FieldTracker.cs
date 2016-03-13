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
            // This check works because the validator runs all validations with a dummy property
            // first. In that first pass, fields never get marked as present and this check relies
            // on that fact.
            else if (field.Value == FieldStatus.Optional)
            {
                throw new InvalidOperationException($"Cannot mark field '{fieldName}' as required, because it is already marked as optional.");
            }
        }

        public void MarkOptional(string fieldName)
        {
            var field = FindField(fieldName);
            if (field.Key == null)
            {
                _fields[fieldName] = FieldStatus.Optional;
            }
            // This check works because the validator runs all validations with a dummy property
            // first. In that first pass, fields never get marked as present and this check relies
            // on that fact.
            else if (field.Value == FieldStatus.Required)
            {
                throw new InvalidOperationException($"Cannot mark field '{fieldName}' as optional, because it is already marked as required.");
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