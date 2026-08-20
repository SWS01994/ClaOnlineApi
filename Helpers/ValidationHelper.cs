using System.Collections.Generic;

namespace WebAPI.Helpers
{
    public class ValidationResult
    {
        public List<string> Errors { get; set; }

        public ValidationResult()
        {
            this.Errors = new List<string>();
        }

        public bool IsValid
        {
            get { return this.Errors.Count == 0; }
        }

        public void AddIfEmpty(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
            {
                this.Errors.Add(string.Format("{0} is required.", fieldName));
            }
        }

        public void Add(string message)
        {
            this.Errors.Add(message);
        }
    }
}