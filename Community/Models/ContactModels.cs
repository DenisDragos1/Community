using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Community.Models
{
    public class ContactModels
    {
        [Required(ErrorMessage = "First Name is required")]

        public string FirstName { get; set; }
        public string Supject { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}