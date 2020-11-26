using System;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct NormalizedToeplitzMatrix {
        private readonly float[] _values;

        private NormalizedToeplitzMatrix(int size, float[] values) => (Size, _values) = (size, values);

        public static NormalizedToeplitzMatrix Create(int size) {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Size should be grater then 1.");
            
            return new NormalizedToeplitzMatrix(size, new float[(size - 1) * 2]);
        }

        public static NormalizedToeplitzMatrix Create(float[] values) {
            if (values.Length % 2 != 0)
                throw new ArgumentException("Array length should be even.", nameof(values));
            
            var size = values.Length / 2 + 1;
            return new NormalizedToeplitzMatrix(size, values);
        }

        public int Size { get; }

        public float this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Get(int index) {
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException("The index should be between -N and N, where N is the size of the matrix.");
            if (index == 0)
                return 1;

            return index < 0 
                ? _values[-index - 1] 
                : _values[index - 1 + (Size - 1)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int index, float value) {
            if (index == 0)
                throw new IndexOutOfRangeException("Can't set main diagonal values for the normalized matrix.");
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException("The index should be between -N and N, where N is the size of the matrix.");

            if (index < 0)
                _values[-index - 1] = value;
            else
                _values[index - 1 + (Size - 1)] = value;
        }

        public bool IsInitialized => _values != null;
    }
}