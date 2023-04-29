using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Helpers
{
    public class TypeRequiredAttribute : ValidationAttribute
    {
        public string[] AllowedTypes { get; set; }
        public TypeRequiredAttribute(params string[] allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (value is IFormFileCollection formFileCollection)
            {
                foreach (IFormFile formFile in formFileCollection)
                {
                    if (!Array.Exists(AllowedTypes, t => t == formFile.ContentType))
                    {
                        return new ValidationResult(ErrorMessage ?? "The file type is invalid");
                    }
                }
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (!AllowedTypes.Contains(formFile.ContentType))
                {
                    return new ValidationResult(ErrorMessage ?? "The file type is invalid");
                }
                return ValidationResult.Success;
            }
            else return new ValidationResult("The property wasn't of type IFormFile or IFormFileCollection");
        }
    }
}