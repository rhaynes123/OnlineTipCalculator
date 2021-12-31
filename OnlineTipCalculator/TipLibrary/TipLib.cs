using System;

namespace TipLibrary
{
    public static class TipLib
    {
        /// <summary>
        /// This method will calculate 15 % of the bill
        /// </summary>
        /// <param name="billamount"></param>
        /// <returns>double</returns>
        public static double CalculateTip(this double billamount)
        {
            double PreTip = Math.Round(billamount / 10, 2);
            double Tip = Math.Round(PreTip + (PreTip / 2), 2);
            return Tip;
        }
    }
}
