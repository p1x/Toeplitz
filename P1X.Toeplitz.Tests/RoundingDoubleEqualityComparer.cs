using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace P1X.Toeplitz.Tests {
    public class RoundingDoubleEqualityComparer : IEqualityComparer<double> {
        public RoundingDoubleEqualityComparer(int roundToDigits) => RoundToDigits = roundToDigits;

        public int RoundToDigits { get; }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(double x, double y) => 
            Math.Round(x, RoundToDigits) == Math.Round(y, RoundToDigits);

        public int GetHashCode(double x) => Math.Round(x, RoundToDigits).GetHashCode();
    }
}