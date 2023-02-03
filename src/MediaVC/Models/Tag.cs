using System.Text.RegularExpressions;

using MediaVC.Validation;

namespace MediaVC.Models
{
    public class Tag : IValidable
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Validate(IValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Name))
            {
                validationContext.AddError(nameof(Name), "Cannot be empty.");
                return false;
            }

            if(!Regex.IsMatch(Name, @"^[a-z_\.\-]$"))
            {
                validationContext.AddError(nameof(Name), "Cannot be empty.");
                return false;
            }

            if(Id != Name.GetHashCode())
            {
                validationContext.AddError(nameof(Id), "Need to be hashcode of Name.");
                return false;
            }

            return true;
        }
    }
}
