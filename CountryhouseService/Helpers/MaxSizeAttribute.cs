using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryhouseService.Helpers
{
    public class MaxSizeAttribute : ValidationAttribute
    {
        public long MaxSize { get; set; }

        public MaxSizeAttribute(long maxSize)
        {
            MaxSize = maxSize;
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
                    if (formFile.Length > MaxSize)
                    {
                        return new ValidationResult(ErrorMessage ?? "The file size is too large");
                    }
                }
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (formFile.Length > MaxSize)
                {
                    return new ValidationResult(ErrorMessage ?? "The file size is too large");
                }
                return ValidationResult.Success;
            }
            else return new ValidationResult("The property wasn't of type IFormFile or IFormFileCollection");
        }
    }
}