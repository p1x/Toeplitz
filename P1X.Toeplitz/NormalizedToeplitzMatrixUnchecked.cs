using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    public readonly struct NormalizedToeplitzMatrixUnchecked : IToeplitzMatrix {
        private readonly float[] _values;
        
        internal NormalizedToeplitzMatrixUnchecked(int size, float[] values) => (Size, _values) = (size, values);
        
        public float this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Get(int index) => _values[index + (Size - 1)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int index, float value) => _values[index + (Size - 1)] = value;

        public int Size { get; }
        
        public bool IsInitialized => _values != null;
    }
}