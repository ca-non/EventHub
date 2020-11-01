using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EventHubMVC.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [RegularExpression(@"^\w+$")]
        [Required(ErrorMessage="Name is required son")]
        [StringLength(200, ErrorMessage ="Username is too long")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required son")]
        public string Email { get; set; }

        [Compare("Email", ErrorMessage ="Emails don't match")]
        public string ConfirmEmail { get; set; }

        [RegularExpression(@"^[\w@$#*!]+$")]
        [Required(ErrorMessage = "Password is required son")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password should be longer than 6 characters")]
        public string Passwd { get; set; }

        [DataType(DataType.Password)]
        [Compare("Passwd", ErrorMessage ="Passwords don't match")]
        public string ConfirmPasswd { get; set; }
    }
}
