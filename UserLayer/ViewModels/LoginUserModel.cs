using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserLayer.ViewModels
{
    public class LoginUserModel
    {
        public string UsernameEmail { get; set; }
        public string Passwd { get; set; }
        public bool RememberMe{ get; set; }
    }
}
