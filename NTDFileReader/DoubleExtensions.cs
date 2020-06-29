using System;
using System.Collections.Generic;
using System.Text;

namespace NTDFileReader {

    internal static class DoubleExtensions {

        public static double Increment(this double value, double increment, int numIncrements) {
            if (numIncrements == 0) return value;
            return (double)((decimal)increment * ((int)Math.Round(value / increment, MidpointRounding.AwayFromZero) + numIncrements));
        }


        public static double Increment(this double value, double increment, long numIncrements) {
            if (numIncrements == 0) return value;
            return (double)((decimal)increment * ((int)Math.Round(value / increment, MidpointRounding.AwayFromZero) + numIncrements));
        }
    }
}
