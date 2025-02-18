using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInBytes;
        private readonly int _minSizeInBytes;

        public MaxFileSizeAttribute(int maxSizeInMB, int minSize)
        {
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024;
            _minSizeInBytes = minSize * 1024 * 1024;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null && file.Length < _minSizeInBytes)
            {
                return new ValidationResult($"Kích thước ảnh không được nhỏ hơn {_minSizeInBytes}MB.");
            }
            if (file != null && file.Length > _maxSizeInBytes)
            {
                return new ValidationResult($"Kích thước ảnh không được vượt quá {_maxSizeInBytes}MB.");
            }
            return ValidationResult.Success;
        }
    }
}
