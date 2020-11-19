using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UserBusinessLayer.ViewModels
{
    public class LoginViewModel
    {
        public bool LoginValidator { get; set; }

        [Required(ErrorMessage = "Please enter username or email")]
        public string UsernameEmail { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [DataType(DataType.Password)]
        public string Passwd { get; set; }

        public bool RememberMe { get; set; }
    }
}
