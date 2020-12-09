﻿using System;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct NormalizedToeplitzMatrix : IReadOnlyToeplitzMatrix {
        private readonly float[] _values;

        private NormalizedToeplitzMatrix(int size, float[] values) => (Size, _values) = (size, values);

        public static NormalizedToeplitzMatrix Create(int size) {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, Resources.NormalizedToeplitzMatrix_InvalidSize);

            var values = new float[(size - 1) * 2 + 1];
            values[size - 1] = 1f;
            return new NormalizedToeplitzMatrix(size, values);
        }

        public static NormalizedToeplitzMatrix Create(float[] values) {
            if (values.Length % 2 != 1)
                throw new ArgumentException(Resources.NormalizedToeplitzMatrix_InvalidArrayLength, nameof(values));
            var size = (values.Length - 1) / 2 + 1;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (values[size - 1] != 1.0f)
                throw new ArgumentException(string.Format(Resources.NormalizedToeplitzMatrix_ArrayDataNotNormilized, size), nameof(values));
            
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
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_IndexOutOfRange);

            return _values[index + (Size - 1)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int index, float value) {
            if (index == 0)
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_CantSetMainDiagonal);
            if (index >= Size || index <= -Size)
                throw new IndexOutOfRangeException(Resources.NormalizedToeplitzMatrix_IndexOutOfRange);

            _values[index + (Size - 1)] = value;
        }

        public float[] GetValues() {
            var copy = new float[_values.Length];
            _values.CopyTo(copy, 0);
            return copy;
        }
        
        public bool IsInitialized => _values != null;
    }
}