using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NavagisInternalTool.Models
{
    public class AdminResetPassword
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Index("ResetCode_Index", IsUnique = true)]
        public string ResetCode { get; set; }

        [Required]
        public int AdminUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        public AdminResetPassword()
        {
            this.ReportDate = DateTime.UtcNow;
        }
    }
}