using System.ComponentModel.DataAnnotations;

namespace NavagisProjRegistration.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Client Secret")]
        public string ClientSecret { get; set; }

        
        [Required]
        [StringLength(255)]
        [Display(Name = "Service Account Email")]
        public string ServiceAccountEmail { get; set; }

    }
}