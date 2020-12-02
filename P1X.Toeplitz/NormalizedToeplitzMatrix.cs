using System;

namespace P1X.Toeplitz {
    public readonly struct NormalizedToeplitzMatrix : IToeplitzMatrix {
        private readonly NormalizedToeplitzMatrixUnchecked _matrix;

        private NormalizedToeplitzMatrix(NormalizedToeplitzMatrixUnchecked matrix) => _matrix = matrix;

        public static NormalizedToeplitzMatrix Create(int size) {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Size should be grater then 1.");

            var values = new float[(size - 1) * 2 + 1];
            values[size - 1] = 1f;
            return new NormalizedToeplitzMatrix(new NormalizedToeplitzMatrixUnchecked(size, values));
        }

        public static NormalizedToeplitzMatrix Create(float[] values) {
            if (values.Length % 2 != 1)
                throw new ArgumentException("Array length should be odd.", nameof(values));
            var size = (values.Length - 1) / 2 + 1;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (values[size - 1] != 1.0f)
                throw new ArgumentException($"Normalized matrix should contains 1 at main diagonal (at {size} index of the array).", nameof(values));
            
            return new NormalizedToeplitzMatrix(new NormalizedToeplitzMatrixUnchecked(size, values));
        }

        public int Size => _matrix.Size;

        public float this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        private float Get(int index) {
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException("The index should be between -N and N, where N is the size of the matrix.");

            return _matrix[index];
        }

        private void Set(int index, float value) {
            if (index == 0)
                throw new IndexOutOfRangeException("Can't set main diagonal values for the normalized matrix.");
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException("The index should be between -N and N, where N is the size of the matrix.");

            _matrix[index] = value;
        }

        public bool IsInitialized => _matrix.IsInitialized;
        
        public NormalizedToeplitzMatrixUnchecked GetUnchecked() => _matrix;
    }
}