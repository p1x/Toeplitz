using System;

namespace P1X.Toeplitz {
    public interface IReadOnlyToeplitzMatrix<out T> where T : struct, IFormattable, IEquatable<T>, IComparable<T> {
        T this[int index] { get; }
        
        int Size { get; }
    }
}