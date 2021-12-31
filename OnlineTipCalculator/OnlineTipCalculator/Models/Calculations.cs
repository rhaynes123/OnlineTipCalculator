using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineTipCalculator.Models
{
    public class Calculation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ResultAmount { get; set; }
        [Required, ForeignKey("TipTypeId")]
        public TipType TipType { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
