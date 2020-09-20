using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace NavagisInternalTool.Models
{
    public class AdminUser
    {
        public int? Id { get; set; }

       
        [StringLength(70)]
        [Index("Username_Index", IsUnique = true)]
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

        [Required]
        public bool IsAdmin { get; set; }

    }
}