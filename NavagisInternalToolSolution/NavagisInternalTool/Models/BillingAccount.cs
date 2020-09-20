using System.ComponentModel.DataAnnotations;

namespace NavagisInternalTool.Models
{
    public class BillingAccount
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Billing Account")]
        public string BillingAccountName { get; set; }


        [StringLength(255)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}