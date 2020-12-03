using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    /// <summary>
    /// Main implementation of Zohar-Trench algorithm.
    /// https://doi.org/10.1145/321812.321822
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class Solver : ISolver<NormalizedToeplitzMatrix, Vector, Vector> {
        private float[] _e; // reversed vector
        private float[] _g;
        private float _lambda = 1f;
        private int _index = 0;
        
        public Solver(int maxMatrixSize) {
            _e = new float[maxMatrixSize - 1]; 
            _g = new float[maxMatrixSize - 1];
        }
        
        public static void Solve(NormalizedToeplitzMatrix matrix, Vector rightVector, Vector resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException("The matrix should be initialized (non-default).", nameof(matrix));
            if (rightVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(resultVector));

            SolveUnchecked(matrix.GetUnchecked(), rightVector, resultVector);
        }

        private static void SolveUnchecked(NormalizedToeplitzMatrixUnchecked matrix, Vector d, Vector s) {
            var count = System.Numerics.Vector<float>.Count;
            var size = count * (int) Math.Ceiling((matrix.Size - 1) / (float) count);
            var size2 = count * (int) Math.Ceiling(s.Values.Length / (float) count);

            var s2 = s;
            if (s.Values.Length != size2) 
                s2 = new Vector(new float[size2]);

            s2[0] = d[0];

            var e = new float[size];
            var g = new float[size];
            var lambda = 1f;
            for (var i = 0; i < matrix.Size - 1; i++) {
                CalculateInversion(matrix, i, e, g, ref lambda);
                CalculateIntermediateResult(matrix, i + 1, d, s2, e, lambda);
            }

            if (s2.Values != s.Values) 
                Array.Copy(s2.Values, s.Values, s.Values.Length);
        }
        
        public void Iterate(in NormalizedToeplitzMatrix matrix, in Vector rightVector, in Vector resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException("The matrix should be initialized (non-default).", nameof(matrix));
            if (rightVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(resultVector));
            
            IterateUnchecked(matrix.GetUnchecked(), rightVector, resultVector);
        }

        private void IterateUnchecked(NormalizedToeplitzMatrixUnchecked matrix, Vector rightVector, Vector resultVector) {
            ResizeArrays();
            
            if (_index == 0)
                CalculateIntermediateResult(matrix, 0, rightVector, resultVector, _e, _lambda);
            
            CalculateInversion(matrix, _index, _e, _g, ref _lambda);
            CalculateIntermediateResult(matrix, _index + 1, rightVector, resultVector, _e, _lambda);
            
            _index++;
        }

        private void ResizeArrays() {
            if (_index <= _e.Length - 1)
                return;
            
            var newE = new float[_e.Length * 2];
            var newG = new float[_g.Length * 2];
                
            _e.CopyTo(newE, 0);
            _g.CopyTo(newG, 0);

            _e = newE;
            _g = newG;
        }

        private static void CalculateIntermediateResult(NormalizedToeplitzMatrixUnchecked matrix, int i, Vector d, Vector s, float[] e, float lambda) {
            var theta = d[i];
            for (var j = 0; j < i; j++)
                theta -= s[j] * matrix[i - j];

            var thetaOverLambda = theta / lambda;

            var count = System.Numerics.Vector<float>.Count;
            for (var j = 0; j < i; j+= count) {
                var ev = new System.Numerics.Vector<float>(e, j);
                var sv = new System.Numerics.Vector<float>(s.Values, j);
                var r = sv + ev * thetaOverLambda;
                r.CopyTo(s.Values, j);
            }
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

        private static float CalculateGamma(NormalizedToeplitzMatrixUnchecked matrix, int i, float[] g) {
            var gamma = -matrix[i + 1];
            for (var j = 0; j < i; j++)
                gamma -= g[j] * matrix[i - j];
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

        void ISolver<NormalizedToeplitzMatrix, Vector, Vector>.Solve(NormalizedToeplitzMatrix matrix, Vector rightVector, Vector resultVector) => 
            Solve(matrix, rightVector, resultVector);
    }
}