﻿using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct Vector : IReadOnlyVector {
        private readonly float[] _values;
        
        public Vector(float[] values) => _values = values;

        public float this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _values[index];
        }

        public int Size => _values.Length;
    }
}