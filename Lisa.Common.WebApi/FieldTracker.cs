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
                       where ((field.Value & FieldStatus.Required) == FieldStatus.Required) && ((field.Value & FieldStatus.Present) == 0)
                       select field.Key;
            }
        }

        public bool Exists(string fieldName)
        {
            return _fields.Any(f => string.Equals(f.Key, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsOptional(string fieldName)
        {
            var field = FindField(fieldName);
            return field.Key != null && ((field.Value & FieldStatus.Optional) == FieldStatus.Optional);
        }

        public bool IsRequired(string fieldName)
        {
            var field = FindField(fieldName);
            return field.Key != null && ((field.Value & FieldStatus.Required) == FieldStatus.Required);
        }

        public bool IsAllowed(string fieldName)
        {
            var field = FindField(fieldName);
            return field.Key != null && ((field.Value & FieldStatus.Allowed) == FieldStatus.Allowed);
        }

        public void MarkRequired(string fieldName)
        {
            if (IsOptional(fieldName))
            {
                throw new InvalidOperationException($"Cannot mark field '{fieldName}' as required, because it is already marked as optional.");
            }

            AddStatus(fieldName, FieldStatus.Required);
        }

        public void MarkOptional(string fieldName)
        {
            if (IsRequired(fieldName))
            {
                throw new InvalidOperationException($"Cannot mark field '{fieldName}' as optional, because it is already marked as required.");
            }

            AddStatus(fieldName, FieldStatus.Optional);
        }

        public void MarkPresent(string fieldName)
        {
            AddStatus(fieldName, FieldStatus.Present);
        }

        public void MarkAllowed(string fieldName)
        {
            AddStatus(fieldName, FieldStatus.Allowed);
        }

        private void AddStatus(string fieldName, FieldStatus status)
        {
            var field = FindField(fieldName);
            if (field.Key == null)
            {
                _fields[fieldName] = status;
            }
            else
            {
                _fields[fieldName] |= status;
            }
        }

        private KeyValuePair<string, FieldStatus> FindField(string fieldName)
        {
            return _fields.SingleOrDefault(f => string.Equals(f.Key, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        private Dictionary<string, FieldStatus> _fields = new Dictionary<string, FieldStatus>();
    }
}