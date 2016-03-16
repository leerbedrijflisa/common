using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public class ValidationResult
    {
        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        public ICollection<Error> Errors { get; } = new List<Error>();

        internal void Merge(ValidationResult other)
        {
            List<Error> errors = (List<Error>) Errors;
            errors.AddRange(other.Errors);
        }
    }
}
