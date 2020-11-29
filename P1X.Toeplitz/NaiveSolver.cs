using System;

namespace P1X.Toeplitz {
    /// <summary>
    /// Direct "naive" implementation of Zohar-Trench algorithm.
    /// https://doi.org/10.1145/321812.321822
    /// </summary>
    public class NaiveSolver : ISolver {
        public void Solve(NormalizedToeplitzMatrix matrix, float[] rightVector, float[] resultVector) {
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
            var sPrev = new float[] { d[0] };
            var ePrev = new float[] { -L[-1] }; // already reversed
            var gPrev = new float[] { -L[1] };
            var lambdaPrev = 1 - L[-1] * L[1];

            for (var i = 1; i < L.Size; i++) {
                var theta = d[i];
                for (var j = 0; j < i; j++) 
                    theta -= sPrev[j] * L[i - j];

                var sNext = new float[i + 1];
                for (var j = 0; j < i; j++) 
                    sNext[j] = sPrev[j] + theta / lambdaPrev * ePrev[j];
                sNext[i] = theta / lambdaPrev;
                
                if (i != L.Size - 1) {
                    var eta = -L[-(i + 1)];
                    for (var j = 0; j < i; j++)
                        eta -= L[-(j + 1)] * ePrev[j];

                    var gamma = -L[i + 1];
                    for (var j = 0; j < i; j++)
                        gamma -= gPrev[j] * L[i - j];

                    var eNext = new float[i + 1];
                    eNext[0] = eta / lambdaPrev;
                    for (var j = 0; j < i; j++)
                        eNext[j + 1] = ePrev[j] + eta / lambdaPrev * gPrev[j];

                    var gNext = new float[i + 1];
                    for (var j = 0; j < i; j++)
                        gNext[j] = gPrev[j] + gamma / lambdaPrev * ePrev[j];
                    gNext[i] = gamma / lambdaPrev;
                    
                    var lambdaNext = lambdaPrev - eta * gamma / lambdaPrev;
                    
                    ePrev = eNext;
                    gPrev = gNext;
                    lambdaPrev = lambdaNext;
                } 
                sPrev = sNext;
            }

            for (var i = 0; i < s.Length; i++) 
                s[i] = sPrev[i];
        }
    }
}