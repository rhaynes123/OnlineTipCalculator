using NUnit.Framework;
using Microsoft.Extensions.Logging;
using OnlineTipCalculator.Models;
using OnlineTipCalculator.DTOs;
using OnlineTipCalculator.Repositories;
using OnlineTipCalculator.Controllers;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using System.Collections.Generic;

namespace OnlineTipCalculatorTest
{
    public class CalculationControllerTests
    {
        private readonly Mock<ILogger<CalculationController>> mockLogger = new();
        private readonly Mock<ICalculationRepository> mockCalculationRepository = new();
        private readonly Mock<IDataProtectionProvider> mockDataProtectionProvider = new();
        private readonly Mock<IDataProtector> mockDataProtector = new();
        private ICalculationRepository calculationRepository;
        private ILogger<CalculationController> logger;
        private IDataProtectionProvider dataProtectionProvider;
        [SetUp]
        public void Setup()
        {
            var TestCalcs = new List<Calculation>()
            {
                new Calculation
                {
                    Id =1,
                    UserId = "22be4874-5268-4213-b2ee-18ade0f5b195",
                    ResultAmount = "cHJvdGVjdGVkRGF0YQ"
                }
            };
            mockCalculationRepository.Setup(r => r.GetCalculationsAsync()).ReturnsAsync(TestCalcs);
            mockCalculationRepository.Setup(r => r.SaveCalculation(It.IsAny<Calculation>())).ReturnsAsync(true);
            mockDataProtector.Setup(sut => sut.Protect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("protectedData"));
            mockDataProtector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("protectedData"));
            mockDataProtectionProvider.Setup(r=>r.CreateProtector(It.IsAny<string>())).Returns(mockDataProtector.Object);
            dataProtectionProvider = mockDataProtectionProvider.Object;
            calculationRepository = mockCalculationRepository.Object;
            logger = mockLogger.Object;
        }

        [Test]
        public async Task RunCalculationWillReturn200OnSave()
        {
            //Arrange
            var controller = new CalculationController(logger, calculationRepository, dataProtectionProvider);
            var TestCalc = new CalculationRequest {Id =1 ,BillAmount = 37.50, TipType = TipType.SitDownRestaurant };
            //Act
            var result = await controller.RunCalculationAsync(TestCalc) as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.Created);
        }

        [Test]
        public async Task RunCalculationWillReturnEncrpytedData()
        {
            //Arrange
            var expected = "cHJvdGV";
            var controller = new CalculationController(logger, calculationRepository, dataProtectionProvider);
            var TestCalc = new CalculationRequest { Id = 1, BillAmount = 37.50, TipType = TipType.SitDownRestaurant };
            //Act
            var result = await controller.RunCalculationAsync(TestCalc) as ObjectResult;
            var actual = (Calculation)result.Value;
            //Assert
            Assert.IsTrue(actual.ResultAmount.Contains(expected));
        }

        [Test]
        public async Task GetCalculationWillReturnData()
        {
            //Arrange
           
            var controller = new CalculationController(logger, calculationRepository, dataProtectionProvider);
            
            //Act
            var result = await controller.GetCalculationsByUserIdAsync("22be4874-5268-4213-b2ee-18ade0f5b195") as OkObjectResult;
            var actual = (IEnumerable<CalculationResponse>)result.Value;
            //Assert

            Assert.IsNotEmpty(actual);
        }
    }
}
