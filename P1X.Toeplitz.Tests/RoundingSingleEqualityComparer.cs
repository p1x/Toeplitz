using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace P1X.Toeplitz.Tests {
    public class RoundingSingleEqualityComparer : IEqualityComparer<float> {
        public RoundingSingleEqualityComparer(int roundToDigits) => RoundToDigits = roundToDigits;

        public int RoundToDigits { get; }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(float x, float y) => 
            MathF.Round(x, RoundToDigits) == MathF.Round(y, RoundToDigits);

        public int GetHashCode(float x) => MathF.Round(x, RoundToDigits).GetHashCode();
    }
}