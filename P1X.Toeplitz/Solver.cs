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
            var e = new float[L.Size - 1]; // reversed vector
            var g = new float[L.Size - 1];
            
            s[0] = d[0];
            var lambda = 1f;
            
            for (var i = 0; i < L.Size - 1; i++) {
                CalculateHelpers(L, i, e, g, ref lambda);
                CalculateIntermediateResult(L, i + 1, d, e, lambda, s);
            }
        }

        private static void CalculateHelpers(NormalizedToeplitzMatrix L, int i, float[] e, float[] g, ref float lambda) {
            var (eta, gamma) = CalculateHelperValues(L, i, e, g);

            var etaOverLambda = eta / lambda;
            var gammaOverLambda = gamma / lambda;

            CalculateHelperVectors(i, e, g, etaOverLambda, gammaOverLambda);

            lambda -= eta * gammaOverLambda;
        }

        private static void CalculateIntermediateResult(NormalizedToeplitzMatrix L, int i, float[] d, float[] e, float lambda, float[] s) {
            var theta = d[i];
            for (var j = 0; j < i; j++)
                theta -= s[j] * L[i - j];

            var thetaOverLambda = theta / lambda;
            for (var j = 0; j < i; j++)
                s[j] = s[j] + thetaOverLambda * e[j];
            s[i] = thetaOverLambda;
        }

        private static (float eta, float gamma) CalculateHelperValues(NormalizedToeplitzMatrix L, int i, float[] e, float[] g) {
            var eta = -L[-(i + 1)];
            var gamma = -L[i + 1];

            for (var j = 0; j < i; j++) {
                eta -= L[-(j + 1)] * e[j];
                gamma -= g[j] * L[i - j];
            }

            return (eta, gamma);
        }

        private static void CalculateHelperVectors(int i, float[] e, float[] g, float etaOverLambda, float gammaOverLambda) {
            var ejPrev = e[0]; // e[j] from prev iteration
            e[0] = etaOverLambda;
            for (var j = 0; j < i; j++) {
                var gjPrev = g[j];

                var ej1 = ejPrev + etaOverLambda * gjPrev;
                g[j] = gjPrev + gammaOverLambda * ejPrev;

                ejPrev = e[j + 1];
                e[j + 1] = ej1;
            }
            g[i] = gammaOverLambda;
        }
    }
}