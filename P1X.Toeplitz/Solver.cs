using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace P1X.Toeplitz {
    using SN = System.Numerics;
    
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
        
        private float[] _columnCache;

        public Solver(int maxMatrixSize) {
            var count = System.Numerics.Vector<float>.Count;
            var size = count * (int) Math.Ceiling((maxMatrixSize - 1) / (float) count);
            _e = new float[size]; 
            _g = new float[size];
            _columnCache = new float[size + System.Numerics.Vector<float>.Count];
        }

        private void ResizeArrays() {
            var vectorsLength = _e.Length;
            if (_index < _e.Length)
                return;
            
            var newE = new float[vectorsLength * 2];
            var newG = new float[vectorsLength * 2];
            var newColumn = new float[vectorsLength * 2 + System.Numerics.Vector<float>.Count];
                
            _e.CopyTo(newE, 0);
            _g.CopyTo(newG, 0);
            _columnCache.CopyTo(newColumn, vectorsLength);

            _e = newE;
            _g = newG;
            _columnCache = newColumn;
        }

        public void Solve(NormalizedToeplitzMatrix matrix, Vector rightVector, Vector resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException("The matrix should be initialized (non-default).", nameof(matrix));
            if (rightVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(resultVector));

            SolveUnchecked(matrix.GetUnchecked(), rightVector, resultVector);
        }

        private void SolveUnchecked(NormalizedToeplitzMatrixUnchecked matrix, Vector d, Vector s) {
            var count = System.Numerics.Vector<float>.Count;
            
            var size2 = count * (int) Math.Ceiling(s.Values.Length / (float) count);

            var s2 = s;
            if (s.Values.Length != size2) 
                s2 = new Vector(new float[size2]);

            s2[0] = d[0];

            while (_index < matrix.Size - 1) 
                IterateUnchecked(matrix, d, s2);

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
            
            if (_index == 0) {
                _columnCache[GetColumnCacheIndex(_index)] = matrix[_index];
                CalculateIntermediateResult(_index, rightVector, resultVector);
            }

            _columnCache[GetColumnCacheIndex(_index + 1)] = matrix[_index + 1];
            CalculateInversion(matrix, _index);
            CalculateIntermediateResult(_index + 1, rightVector, resultVector);
            
            _index++;
        }

        // Returns starting column index for provided iteration
        private int GetColumnCacheIndex(int iteration) => 
            _columnCache.Length - (SN.Vector<float>.Count - 1) - iteration;

        private void CalculateIntermediateResult(int i, Vector d, Vector s) {
            var iColumnCache = GetColumnCacheIndex(i);
            
            var sValues = s.Values;
            var theta = d[i];
            for (var j = 0; j < i; j+= SN.Vector<float>.Count) {
                var sv = new SN.Vector<float>(sValues, j);
                var cv = new SN.Vector<float>(_columnCache, iColumnCache + j);
                var sum = SN.Vector.Dot(sv * cv, SN.Vector<float>.One);
                
                theta -= sum;
            }

            var thetaOverLambda = theta / _lambda;

            for (var j = 0; j < i; j+= SN.Vector<float>.Count) {
                var ev = new SN.Vector<float>(_e, j);
                var sv = new SN.Vector<float>(sValues, j);
                
                var r = sv + ev * thetaOverLambda;
                r.CopyTo(sValues, j);
            }
            s[i] = thetaOverLambda;
        }

        private void CalculateInversion(NormalizedToeplitzMatrixUnchecked matrix, int i) {
            var eta = CalculateEta(matrix, i);
            var gamma = CalculateGamma(matrix, i);

            var etaOverLambda = eta / _lambda;
            var gammaOverLambda = gamma / _lambda;

            CalculateInversionVectors(i, etaOverLambda, gammaOverLambda);

            _lambda -= eta * gammaOverLambda;
        }

        private float CalculateEta(NormalizedToeplitzMatrixUnchecked matrix, int i) {
            var eta = -matrix[-(i + 1)];
            for (var j = 0; j < i; j++)
                eta -= matrix[-(j + 1)] * _e[j];
            return eta;
        }

        private float CalculateGamma(NormalizedToeplitzMatrixUnchecked matrix, int i) {
            var gamma = -matrix[i + 1];
            for (var j = 0; j < i; j++)
                gamma -= _g[j] * matrix[i - j];
            return gamma;
        }

        private void CalculateInversionVectors(int i, float etaOverLambda, float gammaOverLambda) {
            var ejPrev = GetSet(ref _e[0], etaOverLambda);
            for (var j = 0; j < i; j++) {
                var gjPrev = GetSet(ref _g[j], _g[j] + gammaOverLambda * ejPrev);
                ejPrev = GetSet(ref _e[j + 1], ejPrev + etaOverLambda * gjPrev);
            }
            _g[i] = gammaOverLambda;
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