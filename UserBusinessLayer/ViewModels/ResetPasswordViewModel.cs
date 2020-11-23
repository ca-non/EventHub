using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UserBusinessLayer.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name ="New password")]
        [Required(ErrorMessage = "New password cannot be empty")]
        [RegularExpression(@"^[\w@$#*!]+$", ErrorMessage = "Invlaid character entered for password")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password should be longer than 6 characters")]
        public string Newpassword {get; set;}

        [Display(Name = "Confirm password")]
        [Compare("Newpassword", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string NewConfirmPassword { get; set; }
    }
}
