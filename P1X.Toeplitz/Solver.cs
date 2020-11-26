using System;

namespace P1X.Toeplitz {
    public static class Solver {
        public static void Solve(NormalizedToeplitzMatrix matrix, float[] rightVector, float[] resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException("The matrix should be initialized (non-default).", nameof(matrix));
            if (rightVector == null)
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector == null)
                throw new ArgumentNullException(nameof(resultVector));

            if (rightVector.Length != matrix.Size)
                throw new ArgumentException("Vectors and the matrix should be the same size.", nameof(rightVector));
            if (resultVector.Length != matrix.Size)
                throw new ArgumentException("Vectors and the matrix should be the same size.", nameof(resultVector));
            
            SolveCore(matrix, rightVector, resultVector);
        }
        
        private static void SolveCore(NormalizedToeplitzMatrix L, float[] d, float[] s) {
            
        }
    }
}