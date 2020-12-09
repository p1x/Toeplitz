﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace P1X.Toeplitz {
    using SN = System.Numerics;
    
    /// <summary>
    /// Main implementation of Zohar-Trench algorithm.
    /// https://doi.org/10.1145/321812.321822
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class Solver : ISolver<NormalizedToeplitzMatrix, Vector> {
        private float[] _e; // reversed vector
        private float[] _g;
        private float _lambda = 1f;
        private int _index;
        
        private float[] _columnCache;
        private float[] _rowCache;

        private int _vectorsLength;
        
        public Solver(int maxMatrixSize) {
            var vCount = System.Numerics.Vector<float>.Count;
            _vectorsLength = vCount * (int) Math.Ceiling((maxMatrixSize - 1) / (float) vCount);
            _e = new float[_vectorsLength + vCount]; 
            _g = new float[_vectorsLength];
            _columnCache = new float[_vectorsLength + vCount];
            _rowCache = new float[_vectorsLength];
        }

        public static int ResultVectorMultiplier => SN.Vector<float>.Count;
        
        private void ResizeArrays() {
            ResizeArray(ref _e, SN.Vector<float>.Count, 0);
            ResizeArray(ref _g, 0, 0);
            ResizeArray(ref _columnCache, SN.Vector<float>.Count, _vectorsLength);
            ResizeArray(ref _rowCache, 0, 0);

            _vectorsLength *= 2;
        }

        private void ResizeArray(ref float[] array, int additionalSpace, int copyOffset) {
            var newArray = new float[_vectorsLength * 2 + additionalSpace];
            array.CopyTo(newArray, copyOffset);
            array = newArray;
        }

        public void Solve(NormalizedToeplitzMatrix matrix, Vector rightVector, float[] resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException(Resources.Solver_MatrixNotInitialized, nameof(matrix));
            if (rightVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector == null)
                throw new ArgumentNullException(nameof(resultVector));
            if (matrix.Size != rightVector.Size)
                throw new ArgumentException(Resources.Solver_InvalidVectorSize, nameof(rightVector));
            if (matrix.Size != resultVector.Length)
                throw new ArgumentException(Resources.Solver_InvalidVectorSize, nameof(resultVector));
            

            SolveUnchecked(matrix, rightVector, resultVector);
        }

        private void SolveUnchecked(NormalizedToeplitzMatrix matrix, Vector d, float[] s) {
            var vCount = System.Numerics.Vector<float>.Count;
            
            var size = vCount * (int) Math.Ceiling(s.Length / (float) vCount);
            var s2 = s.Length == size ? s : new float[size];
            
            s2[0] = d[0];
            while (_index < matrix.Size - 1) 
                IterateUnchecked(matrix, d, s2);
            
            if (s2 != s) 
                Array.Copy(s2, s, s.Length);
        }

        public void Iterate(NormalizedToeplitzMatrix matrix, Vector rightVector, float[] resultVector) {
            if (!matrix.IsInitialized)
                throw new ArgumentException(Resources.Solver_MatrixNotInitialized, nameof(matrix));
            if (rightVector.Equals(default(Vector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector == null)
                throw new ArgumentNullException(nameof(resultVector));

            var minSize = _index + 2;
            if (matrix.Size < minSize)
                throw new ArgumentException(Resources.Solver_InsufficientMatrixSize, nameof(matrix));
            if (rightVector.Size < minSize)
                throw new ArgumentException(Resources.Solver_InsufficientVectorSize, nameof(rightVector));

            var vSize = System.Numerics.Vector<float>.Count;
            var minResultSize = vSize * Math.Max(1, (int) Math.Ceiling(minSize / (float) vSize));
            if (resultVector.Length < minResultSize)
                throw new ArgumentException(Resources.Solver_InsufficientVectorSize, nameof(resultVector));
            
            IterateUnchecked(matrix, rightVector, resultVector);
        }

        private void IterateUnchecked(NormalizedToeplitzMatrix matrix, Vector rightVector, float[] resultVector) {
            if (_index >= _vectorsLength)
                ResizeArrays();

            if (_index == 0) 
                CalculateIntermediateResult(_index, rightVector, resultVector);

            _columnCache[GetColumnCacheIndex(_index + 1)] = matrix[_index + 1];
            _rowCache[_index] = matrix[-(_index + 1)];
            
            CalculateInversion(_index);
            CalculateIntermediateResult(_index + 1, rightVector, resultVector);
            
            _index++;
        }

        // Returns starting column index for iteration. It's needed because column is reversed
        private int GetColumnCacheIndex(int iteration) => _columnCache.Length - (SN.Vector<float>.Count - 1) - iteration;

        private void CalculateIntermediateResult(int i, Vector d, float[] s) {
            var iColumnCache = GetColumnCacheIndex(i);

            var theta = d[i];
            for (var j = 0; j < i; j+= SN.Vector<float>.Count) {
                var sv = new SN.Vector<float>(s, j);
                var cv = new SN.Vector<float>(_columnCache, iColumnCache + j);
                var sum = SN.Vector.Dot(sv, cv);
                
                theta -= sum;
            }

            var thetaOverLambda = new SN.Vector<float>(theta / _lambda);

            for (var j = 0; j < i; j+= SN.Vector<float>.Count) {
                var ev = new SN.Vector<float>(_e, j);
                var sv = new SN.Vector<float>(s, j);
                
                var r = sv + ev * thetaOverLambda;
                r.CopyTo(s, j);
            }
            s[i] = thetaOverLambda[0];
        }

        private void CalculateInversion(int i) {
            var eta = CalculateEta(i);
            var gamma = CalculateGamma(i);

            CalculateInversionVectors(i, eta, gamma);

            _lambda -= eta * gamma / _lambda;
        }

        private float CalculateEta(int i) {
            var eta = -_rowCache[0 + i];
            for (var j = 0; j < i; j += SN.Vector<float>.Count) {
                var ev = new SN.Vector<float>(_e, j);
                var rv = new SN.Vector<float>(_rowCache, j);
                var sum = SN.Vector.Dot(ev, rv);

                eta -= sum;
            }

            return eta;
        }

        private float CalculateGamma(int i) {
            var iColumnCache = GetColumnCacheIndex(i);

            var gamma = -_columnCache[iColumnCache - 1];
            for (var j = 0; j < i; j+= SN.Vector<float>.Count) {
                var gv = new SN.Vector<float>(_g, j);
                var cv = new SN.Vector<float>(_columnCache, iColumnCache + j);
                var sum = SN.Vector.Dot(gv, cv);

                gamma -= sum;
            }
            
            return gamma;
        }

        private void CalculateInversionVectors(int i, float eta, float gamma) {
            var etaOverLambda = new SN.Vector<float>(eta / _lambda);
            var gammaOverLambda = new SN.Vector<float>(gamma / _lambda);

            var ev = new SN.Vector<float>(_e, 0); 
            for (var j = 0; j < i; j += SN.Vector<float>.Count) {
                var gv = new SN.Vector<float>(_g, j);

                var gvNew = gv + gammaOverLambda * ev;
                var evNew = ev + etaOverLambda * gv;

                ev = new SN.Vector<float>(_e, j + SN.Vector<float>.Count);

                gvNew.CopyTo(_g, j);
                evNew.CopyTo(_e, j + 1);
            }
            
            _e[0] = etaOverLambda[0];
            _g[i] = gammaOverLambda[0];
        }
    }
}