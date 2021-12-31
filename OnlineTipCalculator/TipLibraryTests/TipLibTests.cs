using System.Collections.Generic;
using NUnit.Framework;
using TipLibrary;
namespace TipLibraryTests
{
    public class TipLibTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCaseSource(nameof(TipResults))]
        public void CalculateTipReturns15Percent(double BillValue, double TipValue)
        {
            //Arrange
            double expected = TipValue;
            double value = BillValue;
            //Act
            double tip = value.CalculateTip();
            //Assert 
            Assert.AreEqual(expected,tip);
        }
        private static IEnumerable<TestCaseData> TipResults()
        {
            yield return new TestCaseData(37.50, 5.62);
        }
    }
}
