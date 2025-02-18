using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensions(FileType extensions)
        {
            _extensions = extensions.GetExtentionList();
        }

        public AllowedExtensions(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                return ValidateFile(file);
            }

            if (value is IFormFile[] files && files != null && files.Length > 0)
            {
                foreach (var f in files)
                {
                    var validationResult = ValidateFile(f);
                    if (validationResult != ValidationResult.Success)
                    {
                        return validationResult;
                    }
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateFile(IFormFile file)
        {
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage(file.FileName));
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string fileName)
        {
            return $"File {fileName} is invalid because {Path.GetExtension(fileName)} extension not allowed.";
        }
    }
}
