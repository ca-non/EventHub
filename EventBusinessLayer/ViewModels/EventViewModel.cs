using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace EventBusinessLayer.ViewModels
{
    public class EventViewModel
    {
        [Required(ErrorMessage = "Title cannot be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Location cannot be empty")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid character/s entered for location")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Time cannot be empty")]
        [RegularExpression(@"^[0-2]{1}[0-9]{1}:[0-5]{1}[0-9]{1}$", ErrorMessage = "Please follow the format HH:MM")]
        public string Time { get; set; }

        [Required(ErrorMessage = "Date cannot be empty")]
        [RegularExpression(@"^[0-1]{1}[0-2]{1}\/[0-3]{1}[0-9]{1}\/[0-2]{1}[0-9]{1}[0-9]{1}[0-9]{1}$", ErrorMessage = "Please follow the format MM/DD/YYYY")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Description cannot be empty")]
        public string Description { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }
}
