using System;
using OnlineTipCalculator.Models;

namespace OnlineTipCalculator.DTOs
{
    public record CalculationResponse(int Id, string ResultAmount, TipType TipType, string CreatedDateTime);
    
}
