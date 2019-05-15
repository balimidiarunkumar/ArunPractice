using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracticeExercise.Models
{

    public class UserViewModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "This is required")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "This is required")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "This is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter 10 digits.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { get; set; }
        public string RoleId { get; set; }

    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "This is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "This is required")]
        public string Password { get; set; }
    }

    public class ServiceResponse
    {
        public string status { get; set; }
        public string errorMessage { get; set; }
        public string refId { get; set; }
    }


}