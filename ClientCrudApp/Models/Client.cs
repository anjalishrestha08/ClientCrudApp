using System.ComponentModel.DataAnnotations;

namespace ClientCrudApp.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name cannot be longer than 30 characters")]
        [Display(Name = "Full Name")]
        public required string Name { get; set; }

        public enum GenderType { Male, Female, Other }
        [Required(ErrorMessage = "Gender is required")]
        public required GenderType Gender { get; set; }


        [Required(ErrorMessage = "Country code is required")]
        [Display(Name = "Country Code")]
        [RegularExpression(@"^\+\d{0,4}$", ErrorMessage = "Invalid country code format")]
        public required string CountryCode { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9\s\-]{7,15}$", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone No.")]
        public required string Phone { get; set; }

        [Display(Name = "Phone Number")]
        public string FullPhone => $"{CountryCode} {Phone}";



        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid Email")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }


        [Required(ErrorMessage = "Address is required")]
        [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        public required string Address { get; set; }


        [Required(ErrorMessage = "Nationality is required")]
        [MaxLength(50, ErrorMessage = "Nationality cannot exceed 50 characters")]
        public required string Nationality { get; set; }


        [Required(ErrorMessage = "D.O.B is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Client), nameof(ValidateDOB))]
        public required DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Education Background is required")]
        [MaxLength(255, ErrorMessage = "Education Background Cannot exceed 255 characters")]
        [Display(Name = "Education Background")]
        public required string EducationBackground { get; set; }


        public enum ContactMode { Email, Phone, None }

        [Required(ErrorMessage = "Preferred Model of Contact is required")]
        [Display(Name = "Preferred Mode of Contact")]
        public required ContactMode PreferredModeOfContact { get; set; }

        public static ValidationResult? ValidateDOB(DateTime dob, ValidationContext context)
        {
            if (dob >= DateTime.Now)
            {
                return new ValidationResult("Date of Birth cannot be in future");
            }
            if (dob < new DateTime(1940, 1, 1))
            {
                return new ValidationResult("Date of Birth cannot be before 1940.");
            }
            return ValidationResult.Success;
        }
    }
}
