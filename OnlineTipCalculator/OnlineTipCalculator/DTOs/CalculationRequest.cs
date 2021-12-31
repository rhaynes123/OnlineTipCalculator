using System;
using System.ComponentModel.DataAnnotations;
using OnlineTipCalculator.Models;

namespace OnlineTipCalculator.DTOs
{
    public record CalculationRequest()
    {
        [Required]
        public int Id { get; init; }
        [Required, Display(Name ="Bill Amount"),Range(1, double.MaxValue, ErrorMessage = "Result can not be zero and below")]
        public double BillAmount { get; init; }
        [Required, Display(Name ="Type of Tip")]
        public TipType TipType { get; init; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
