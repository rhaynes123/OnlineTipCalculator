using System;
using System.Threading.Tasks;
using OnlineTipCalculator.Models;
using OnlineTipCalculator.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineTipCalculator.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CalculationRepository> _logger;

        public CalculationRepository(ApplicationDbContext dbContext
            , ILogger<CalculationRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(_dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<IList<Calculation>> GetCalculationsAsync()
        {
            return await _dbContext.Calculations.ToListAsync();
        }

        public async Task<bool> SaveCalculation(Calculation calculation)
        {
            try
            {
                await _dbContext.Calculations.AddAsync(calculation);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Calculation could not be saved. See details below \n {ex}");
                return false;
            }
        }
    }
}
