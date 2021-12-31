using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineTipCalculator.Models;
using OnlineTipCalculator.DTOs;
using OnlineTipCalculator.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using TipLibrary;
using Microsoft.AspNetCore.Http;

namespace OnlineTipCalculator.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CalculationController : Controller
    {

        private readonly ILogger<CalculationController> _logger;
        private readonly ICalculationRepository _calculationRepository;
        private readonly IDataProtector _dataProtector;

        public CalculationController(ILogger<CalculationController> logger
            , ICalculationRepository calculationRepository
            , IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _calculationRepository = calculationRepository ?? throw new ArgumentNullException(nameof(_calculationRepository));
            _dataProtector = dataProtectionProvider.CreateProtector("CalculationResultProtector")
                ?? throw new ArgumentNullException(nameof(_dataProtector));
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// GetCalculationsByUserIdAsync
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Collection Of Calculation Reponses at status code 200</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("History")]
        public async Task<IActionResult> GetCalculationsByUserIdAsync([FromBody]string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest();
            }
            IEnumerable<Calculation> calculations = new List<Calculation>();
            try
            {
                calculations = (await _calculationRepository.GetCalculationsAsync()).Where(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured fetching calculation history. See exception details: \n{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured fetching calculation history.");
            }
            IEnumerable<CalculationResponse> results = calculations
                .Select( c => new CalculationResponse(c.Id,
                _dataProtector.Unprotect(c.ResultAmount),
                c.TipType,
                c.CreatedDateTime.ToString("MM/dd/yyyy hh:mm tt")));

            return Ok(results.OrderByDescending(r=>r.Id));
        }
        /// <summary>
        /// RunCalculationAsync
        /// </summary>
        /// <param name="calculationRequest"></param>
        /// <returns>A created calculation at status code 201</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost(nameof(RunCalculationAsync))]
        public async Task<IActionResult> RunCalculationAsync([FromBody] CalculationRequest calculationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            double result;
            try
            {
                result = calculationRequest.BillAmount.CalculateTip();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Calculation failed. See exception details: \n{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured calculating tip.");
            }
            Calculation calculation = new();
            try
            {
                calculation = new()
                {
                    Id = calculationRequest.Id,
                    ResultAmount = _dataProtector.Protect($"{result}"),
                    TipType = calculationRequest.TipType,
                    UserId = calculationRequest.UserId,
                    CreatedDateTime = calculationRequest.CreatedDateTime,

                };
                await _calculationRepository.SaveCalculation(calculation);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Saving Calculation failed. See exception details: \n{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured saving tip calculation.");
            }
            return CreatedAtAction(nameof(RunCalculationAsync), calculation);
        }
        

    }
}