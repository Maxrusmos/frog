using System;
using System.Collections.Generic;
using System.Text;

namespace CryptographyLabs.Extensions
{
    public static class MathEx
    {
        public static double Sum(IEnumerable<double> values)
        {
            double sum = 0;
            foreach (double value in values)
            {
                sum += value;
            }
            return sum;
        }
    }
}
