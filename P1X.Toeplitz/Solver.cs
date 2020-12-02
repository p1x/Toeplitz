using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    /// <summary>
    /// Main implementation of Zohar-Trench algorithm.
    /// https://doi.org/10.1145/321812.321822
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class Solver : ISolver<NormalizedToeplitzMatrix> {
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
            
            SolveUnchecked(matrix.GetUnchecked(), rightVector, resultVector);
        }
        
        private void SolveUnchecked(NormalizedToeplitzMatrixUnchecked matrix, float[] d, float[] s) {
            var e = new float[matrix.Size - 1]; // reversed vector
            var g = new float[matrix.Size - 1];
            
            s[0] = d[0];
            var lambda = 1f;
            
            for (var i = 0; i < matrix.Size - 1; i++) {
                CalculateInversion(matrix, i, e, g, ref lambda);
                CalculateIntermediateResult(matrix, i + 1, d, e, lambda, s);
            }
        }

        private static void CalculateIntermediateResult(NormalizedToeplitzMatrixUnchecked matrix, int i, float[] d, float[] e, float lambda, float[] s) {
            var theta = d[i];
            for (var j = 0; j < i; j++)
                theta -= s[j] * matrix[i - j];

            var thetaOverLambda = theta / lambda;
            for (var j = 0; j < i; j++)
                s[j] = s[j] + thetaOverLambda * e[j];
            s[i] = thetaOverLambda;
        }

        private static void CalculateInversion(NormalizedToeplitzMatrixUnchecked matrix, int i, float[] e, float[] g, ref float lambda) {
            var eta = CalculateEta(matrix, i, e);
            var gamma = CalculateGamma(matrix, i, g);

            var etaOverLambda = eta / lambda;
            var gammaOverLambda = gamma / lambda;

            CalculateInversionVectors(i, e, g, etaOverLambda, gammaOverLambda);

            lambda -= eta * gammaOverLambda;
        }

        private static float CalculateEta(NormalizedToeplitzMatrixUnchecked matrix, int i, float[] e) {
            var eta = -matrix[-(i + 1)];
            for (var j = 0; j < i; j++)
                eta -= matrix[-(j + 1)] * e[j];
            return eta;
        }

        private static float CalculateGamma(NormalizedToeplitzMatrixUnchecked L, int i, float[] g) {
            var gamma = -L[i + 1];
            for (var j = 0; j < i; j++)
                gamma -= g[j] * L[i - j];
            return gamma;
        }

        private static void CalculateInversionVectors(int i, float[] e, float[] g, float etaOverLambda, float gammaOverLambda) {
            var ejPrev = GetSet(ref e[0], etaOverLambda);
            for (var j = 0; j < i; j++) {
                var gjPrev = GetSet(ref g[j], g[j] + gammaOverLambda * ejPrev);
                ejPrev = GetSet(ref e[j + 1], ejPrev + etaOverLambda * gjPrev);
            }
            g[i] = gammaOverLambda;
        }

        // Set new value and return original value
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetSet(ref float target, in float value) {
            var result = target;
            target = value;
            return result;
        }
    }
}