using System;

namespace P1X.Toeplitz {
    public interface ISolver<in TMatrix, in TVector, in TValue> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix<TValue>
        where TVector : struct, IReadOnlyVector<TValue>
        where TValue : struct, IFormattable, IEquatable<TValue>, IComparable<TValue> {
        void Solve(TMatrix matrix, TVector rightVector, TValue[] resultVector);

        void Iterate(TMatrix matrix, TVector rightVector, TValue[] resultVector);

        /// <summary>
        /// Multiplier for result vector used in <see cref="Iterate"/>
        /// </summary>
        int ResultVectorMultiplier { get; }
    }
}