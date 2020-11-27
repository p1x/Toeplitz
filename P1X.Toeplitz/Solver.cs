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
        
        /// <summary>
        /// Direct implementation of Zohar-Trench algorithm.
        /// https://doi.org/10.1145/321812.321822
        /// </summary>
        private static void SolveCore(NormalizedToeplitzMatrix L, float[] d, float[] s) {
            var ePrev = new float[L.Size - 1]; // reversed vector
            var gPrev = new float[L.Size - 1];
            var eNext = new float[L.Size - 1];
            var gNext = new float[L.Size - 1];
            s[0] = d[0];
            ePrev[0] = -L[-1];
            gPrev[0] = -L[1];
            var lambda = 1 - L[-1] * L[1];

            for (var i = 1; i < L.Size; i++) {
                var theta = d[i];
                for (var j = 0; j < i; j++) 
                    theta -= s[j] * L[i - j];

                var thetaOverLambda = theta / lambda;
                for (var j = 0; j < i; j++) 
                    s[j] = s[j] + thetaOverLambda * ePrev[j];
                s[i] = thetaOverLambda;

                if (i == L.Size - 1)
                    break;
                
                var eta = -L[-(i + 1)];
                var gamma = -L[i + 1];

                for (var j = 0; j < i; j++) {
                    eta -= L[-(j + 1)] * ePrev[j];
                    gamma -= gPrev[j] * L[i - j];
                }

                var etaOverLambda = eta / lambda;
                var gammaOverLambda = gamma / lambda;
                eNext[0] = etaOverLambda;
                for (var j = 0; j < i; j++) {
                    eNext[j + 1] = ePrev[j] + etaOverLambda * gPrev[j];
                    gNext[j] = gPrev[j] + gammaOverLambda * ePrev[j];
                }
                gNext[i] = gammaOverLambda;
                
                Swap(ref ePrev, ref eNext);
                Swap(ref gPrev, ref gNext);

                lambda -= eta * gammaOverLambda;
            }
        }

        private static void Swap(ref float[] a, ref float[] b) {
            var t = a;
            a = b;
            b = t;
        }
    }
}