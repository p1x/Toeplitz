using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct VectorDouble : IReadOnlyVector<double> {
        private readonly double[] _values;
        
        public VectorDouble(double[] values) => _values = values;

        public double this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _values[index];
        }

        public int Size => _values.Length;
    }
}