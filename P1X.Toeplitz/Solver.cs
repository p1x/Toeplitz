using System;
using System.Diagnostics.CodeAnalysis;

namespace P1X.Toeplitz {
    using SN = System.Numerics;

    /// <summary>
    /// Implementation of Zohar-Trench algorithm.
    /// https://doi.org/10.1145/321812.321822
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class Solver<TMatrix, TVector, TOperations, T> : ISolver<TMatrix, TVector, T>
        where TMatrix : struct, IReadOnlyToeplitzMatrix<T> 
        where TVector : struct, IReadOnlyVector<T> 
        where TOperations : struct, IOperations<T>
        where T : struct, IFormattable, IEquatable<T>, IComparable<T> {
        private T[] _e; // reversed vector
        private T[] _g;
        private T _lambda = default(TOperations).One();
        private int _index;
        
        private T[] _columnCache;
        private T[] _rowCache;

        private int _vectorsLength;
        
        /// <summary>
        /// Create new solver with cache and intermediate data storage capacity calculated for given matrix size.
        /// </summary>
        /// <param name="expectedMatrixSize">Expected matrix size</param>
        public Solver(int expectedMatrixSize) {
            var vCount = System.Numerics.Vector<T>.Count;
            _vectorsLength = vCount * (int) Math.Ceiling((expectedMatrixSize - 1) / (float) vCount);
            _e = new T[_vectorsLength + vCount]; 
            _g = new T[_vectorsLength];
            _columnCache = new T[_vectorsLength + vCount];
            _rowCache = new T[_vectorsLength];
        }

        /// <summary>
        /// Multiplier for result vector used in <see cref="Iterate"/>
        /// </summary>
        public int ResultVectorMultiplier => SN.Vector<T>.Count;
        
        private void ResizeArrays() {
            ResizeArray(ref _e, SN.Vector<T>.Count, 0);
            ResizeArray(ref _g, 0, 0);
            ResizeArray(ref _columnCache, SN.Vector<T>.Count, _vectorsLength);
            ResizeArray(ref _rowCache, 0, 0);

            _vectorsLength *= 2;
        }

        private void ResizeArray(ref T[] array, int additionalSpace, int copyOffset) {
            var newArray = new T[_vectorsLength * 2 + additionalSpace];
            array.CopyTo(newArray, copyOffset);
            array = newArray;
        }

        /// <summary>
        /// Solve equation in form of A*x = b where A is coefficient Toeplitz matrix and x and b is vectors. 
        /// </summary>
        /// <param name="matrix">Coefficients matrix</param>
        /// <param name="rightVector">Right-hand size vector</param>
        /// <param name="resultVector">Result vector</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="matrix"/> is not initialized -or- 
        /// <paramref name="rightVector"/> or <paramref name="resultVector"/> size is not equal to <paramref name="matrix"/> size.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="rightVector"/> is <c>default</c> or <paramref name="resultVector"/> is <c>null</c>.</exception>
        public void Solve(TMatrix matrix, TVector rightVector, T[] resultVector) {
            if (matrix.Equals(default(TMatrix)))
                throw new ArgumentNullException(nameof(matrix));
            if (rightVector.Equals(default(TVector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector == null)
                throw new ArgumentNullException(nameof(resultVector));
            if (matrix.Size != rightVector.Size)
                throw new ArgumentException(Resources.Solver_InvalidVectorSize, nameof(rightVector));
            if (matrix.Size != resultVector.Length)
                throw new ArgumentException(Resources.Solver_InvalidVectorSize, nameof(resultVector));
            

            SolveUnchecked(matrix, rightVector, resultVector);
        }

        private void SolveUnchecked(TMatrix matrix, TVector d, T[] s) {
            var vCount = System.Numerics.Vector<T>.Count;
            
            var size = vCount * (int) Math.Ceiling(s.Length / (float) vCount);
            var s2 = s.Length == size ? s : new T[size];
            
            s2[0] = d[0];
            while (_index < matrix.Size - 1) 
                IterateUnchecked(matrix, d, s2);
            
            if (s2 != s) 
                Array.Copy(s2, s, s.Length);
        }

        /// <summary>
        /// Calculate one iteration. It's useful for some algorithms.
        /// First iterations, which is trivial, is skipped.
        /// Result vector size should be multiple of <see cref="ResultVectorMultiplier"/>.  
        /// </summary>
        /// <param name="matrix">Coefficients matrix</param>
        /// <param name="rightVector">Right-hand side vector</param>
        /// <param name="resultVector">Result vector</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="matrix"/> is not initialized or its size is insufficient for current iteration -or- 
        /// <paramref name="rightVector"/> size is insufficient for current iteration -or-
        /// <paramref name="resultVector"/> size is insufficient for current iteration.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="rightVector"/> is <c>default</c> or <paramref name="resultVector"/> is <c>null</c>.</exception>
        public void Iterate(TMatrix matrix, TVector rightVector, T[] resultVector) {
            if (matrix.Equals(default(TMatrix)))
                throw new ArgumentNullException(nameof(matrix));
            if (rightVector.Equals(default(TVector)))
                throw new ArgumentNullException(nameof(rightVector));
            if (resultVector == null)
                throw new ArgumentNullException(nameof(resultVector));

            var minSize = _index + 2;
            if (matrix.Size < minSize)
                throw new ArgumentException(Resources.Solver_InsufficientMatrixSize, nameof(matrix));
            if (rightVector.Size < minSize)
                throw new ArgumentException(Resources.Solver_InsufficientVectorSize, nameof(rightVector));

            var vSize = System.Numerics.Vector<T>.Count;
            var minResultSize = vSize * Math.Max(1, (int) Math.Ceiling(minSize / (float) vSize));
            if (resultVector.Length < minResultSize)
                throw new ArgumentException(Resources.Solver_InsufficientVectorSize, nameof(resultVector));
            
            IterateUnchecked(matrix, rightVector, resultVector);
        }

        private void IterateUnchecked(TMatrix matrix, TVector rightVector, T[] resultVector) {
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
        private int GetColumnCacheIndex(int iteration) => _columnCache.Length - (SN.Vector<T>.Count - 1) - iteration;

        private void CalculateIntermediateResult(int i, TVector d, T[] s) {
            var iColumnCache = GetColumnCacheIndex(i);

            var theta = d[i];
            var op = default(TOperations);
            for (var j = 0; j < i; j+= SN.Vector<T>.Count) {
                var sv = new SN.Vector<T>(s, j);
                var cv = new SN.Vector<T>(_columnCache, iColumnCache + j);
                var sum = SN.Vector.Dot(sv, cv);
                theta = op.Subtract(theta, sum);
            }

            var thetaOverLambda = new SN.Vector<T>(op.Divide(theta, _lambda));

            for (var j = 0; j < i; j+= SN.Vector<T>.Count) {
                var ev = new SN.Vector<T>(_e, j);
                var sv = new SN.Vector<T>(s, j);
                
                var r = sv + ev * thetaOverLambda;
                r.CopyTo(s, j);
            }
            s[i] = thetaOverLambda[0];
        }

        private void CalculateInversion(int i) {
            var eta = CalculateEta(i);
            var gamma = CalculateGamma(i);

            CalculateInversionVectors(i, eta, gamma);

            var op = default(TOperations);
            _lambda = op.Subtract(_lambda, op.Divide(op.Multiply(eta, gamma), _lambda));
        }

        private T CalculateEta(int i) {
            var op = default(TOperations);
            var eta = op.Negate(_rowCache[0 + i]);
            for (var j = 0; j < i; j += SN.Vector<T>.Count) {
                var ev = new SN.Vector<T>(_e, j);
                var rv = new SN.Vector<T>(_rowCache, j);
                var sum = SN.Vector.Dot(ev, rv);

                eta = op.Subtract(eta, sum);
            }

            return eta;
        }

        private T CalculateGamma(int i) {
            var iColumnCache = GetColumnCacheIndex(i);
            var op = default(TOperations);

            var gamma = op.Negate(_columnCache[iColumnCache - 1]);
            for (var j = 0; j < i; j+= SN.Vector<T>.Count) {
                var gv = new SN.Vector<T>(_g, j);
                var cv = new SN.Vector<T>(_columnCache, iColumnCache + j);
                var sum = SN.Vector.Dot(gv, cv);

                gamma = op.Subtract(gamma, sum);
            }
            
            return gamma;
        }

        private void CalculateInversionVectors(int i, T eta, T gamma) {
            var op = default(TOperations);

            var etaOverLambda = new SN.Vector<T>(op.Divide(eta, _lambda));
            var gammaOverLambda = new SN.Vector<T>(op.Divide(gamma, _lambda));

            var ev = new SN.Vector<T>(_e, 0); 
            for (var j = 0; j < i; j += SN.Vector<T>.Count) {
                var gv = new SN.Vector<T>(_g, j);

                var gvNew = gv + gammaOverLambda * ev;
                var evNew = ev + etaOverLambda * gv;

                ev = new SN.Vector<T>(_e, j + SN.Vector<T>.Count);

                gvNew.CopyTo(_g, j);
                evNew.CopyTo(_e, j + 1);
            }
            
            _e[0] = etaOverLambda[0];
            _g[i] = gammaOverLambda[0];
        }
    }
}