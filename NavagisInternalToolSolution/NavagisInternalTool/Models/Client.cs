using System.ComponentModel.DataAnnotations;

namespace NavagisInternalTool.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public BillingAccount BillingAccount { get; set; }

        [Required]
        public int BillingAccountId { get; set; }
    }
}