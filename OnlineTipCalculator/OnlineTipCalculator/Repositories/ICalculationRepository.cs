using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineTipCalculator.Models;

namespace OnlineTipCalculator.Repositories
{
    public interface ICalculationRepository
    {
        Task<bool> SaveCalculation(Calculation calculation);
        Task<IList<Calculation>> GetCalculationsAsync();
    }
}
