using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace HouseRentManagementSystem.ViewModel
{
    public class Register
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //Required attribute implements validation on Model item that this fields is mandatory for user
        [Required]
        //We are also implementing Regular expression to check if email is valid like a1@test.com

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]


        public string Email { get; set; }


        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$", ErrorMessage = "Password id is not valid")]
        public string Password { get; set; }


        [NotMapped]
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }


        [Required]
        [RegularExpression(@"^([0]|\+91)?[6789]\d{9}$", ErrorMessage = "Mobile No id is not valid")]
        [Display(Name = "Mobile No")]
        public long PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^[2-9]{1}[0-9]{11}$", ErrorMessage = "Aadhar No id is not valid")]
        [Display(Name = "Aadhar No")]
        public long Aadharno { get; set; }

        public byte[] ProfileImage { get; set;}

    }
}