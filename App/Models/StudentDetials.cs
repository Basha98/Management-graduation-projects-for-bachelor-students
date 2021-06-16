using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Models
{
    public class StudentDetials
    {

        [Required]

        [Display(Name = "Full Name")]


        [StringLength(50, ErrorMessage = "The length must be at most 50 characters.", MinimumLength = 1)]
        public string FullName { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Description = "Desc: Password must be at least 8 characters and contain one or more non-letter characters.", Prompt = "**")]
        [DisplayFormat(NullDisplayText = "Enter password...")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*[^A-Za-z])[\\s\\S]{8,}$", ErrorMessage = "Password must be at least 8 characters and contain one or more non-letter characters.")]
        [AdditionalMetadata("Tooltip", "Password must be at least 8 characters and contain one or more non-letter characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [Display(Name = "Entry Code of Project")]
        public string Token { get; set; }
    }
}