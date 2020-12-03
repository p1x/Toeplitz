﻿using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct Vector : IVector {
        private readonly float[] _values;
        
        public Vector(float[] values) => _values = values;

        public float this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _values[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _values[index] = value;
        }

        internal float[] Values => _values;
    }
}