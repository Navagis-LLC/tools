using System.ComponentModel.DataAnnotations;

namespace RegisterProject_Spice.Pages.Models
{
    public class AdminUser
    {
        [Key]
        public int? Id { get; set; }


        [StringLength(70)]
        [Display(Name = "Username (Email)")]
        [Required(ErrorMessage = "The username is required.")]
        [EmailAddress(ErrorMessage = "The username should be a valid email address.")]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Is super admin")]
        public bool IsAdmin { get; set; }
    }
}
