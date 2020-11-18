using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UserBusinessLayer.ViewModels
{
    public class RegisterViewModel
    {
        [RegularExpression(@"^\w+$", ErrorMessage = "Username can contain only letters, numbers and _ ")]
        [Required(ErrorMessage = "Username is required")]
        [StringLength(200, ErrorMessage = "Username is too long")]
        [Remote("IsUsernameAvailable", "Home", ErrorMessage="Username already taken")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", ErrorMessage = "Invalid email entered")]
        [DataType(DataType.EmailAddress)]
        [Remote("IsEmailAvailable", "Home", ErrorMessage = "Email already taken")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "Emails don't match")]
        public string ConfirmEmail { get; set; }

        [RegularExpression(@"^[\w@$#*!]+$", ErrorMessage = "Invlaid character entered for password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password should be longer than 6 characters")]
        public string Passwd { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Passwd", ErrorMessage = "Passwords don't match")]
        public string ConfirmPasswd { get; set; }
    }
}
