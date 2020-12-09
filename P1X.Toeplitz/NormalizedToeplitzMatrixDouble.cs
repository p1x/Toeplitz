using System;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct NormalizedToeplitzMatrixDouble : IReadOnlyToeplitzMatrix<double> {
        private readonly double[] _values;

        private NormalizedToeplitzMatrixDouble(int size, double[] values) => (Size, _values) = (size, values);

        public static NormalizedToeplitzMatrixDouble Create(int size) {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, Resources.NormalizedToeplitzMatrix_InvalidSize);

            var values = new double[(size - 1) * 2 + 1];
            values[size - 1] = 1f;
            return new NormalizedToeplitzMatrixDouble(size, values);
        }

        public static NormalizedToeplitzMatrixDouble Create(double[] values) {
            if (values.Length % 2 != 1)
                throw new ArgumentException(Resources.NormalizedToeplitzMatrix_InvalidArrayLength, nameof(values));
            var size = (values.Length - 1) / 2 + 1;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (values[size - 1] != 1.0f)
                throw new ArgumentException(string.Format(Resources.NormalizedToeplitzMatrix_ArrayDataNotNormilized, size), nameof(values));
            
            return new NormalizedToeplitzMatrixDouble(size, values);
        }

        public int Size { get; }

        public double this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Get(int index) {
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_IndexOutOfRange);

            return _values[index + (Size - 1)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int index, double value) {
            if (index == 0)
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_CantSetMainDiagonal);
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_IndexOutOfRange);

            _values[index + (Size - 1)] = value;
        }

        public bool IsInitialized => _values != null;
    }
}