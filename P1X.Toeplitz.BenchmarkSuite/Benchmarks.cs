using System;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Single;

namespace P1X.Toeplitz.BenchmarkSuite {
    [MemoryDiagnoser]
    public class Benchmarks {
        [Params(8, 32, 128, 512)]
        public int N;
        
        private NormalizedToeplitzMatrixSingle _matrix;

        private float[] _rightValues;
        private float[] _resultValues;
        private VectorSingle _rightVector;
        private DenseMatrix _mnMatrix;
        private DenseVector _mnRightVector;
        private DenseVector _mnResult;

        [GlobalSetup]
        public void Setup() {
            static float R(float t) => MathF.Abs(MathF.Cos(10 * t) * MathF.Exp(-t * t));

            _matrix = NormalizedToeplitzMatrixSingle.Create(N);
            for (var i = 1; i < N; i++)
                _matrix[i] = _matrix[-i] = R(i);

            _mnMatrix = new DenseMatrix(N);
            for (var i = 0; i < N; i++)
            for (var j = 0; j < N; j++)
                _mnMatrix[i, j] = _matrix[Math.Abs(i - j)];

            _rightValues = new float[N];
            for (var i = 0; i < N; i++) 
                _rightValues[i] = R(0.5f - N / 2f + i);

            _rightVector = new VectorSingle(_rightValues);
            _mnRightVector = new DenseVector(_rightValues);
            
            _resultValues = new float[N];

            _mnResult = new DenseVector(N);
        }
        
        [Benchmark]
        public float[] MainSolver() {
            new SingleSolver<NormalizedToeplitzMatrixSingle, VectorSingle>(N).Solve(_matrix, _rightVector, _resultValues);
            return _resultValues;
        }
        
        [Benchmark(Baseline = true)]
        public DenseVector MathNetNativeMkl() {
            Control.UseNativeMKL();
            _mnMatrix.Solve(_mnRightVector, _mnResult);
            return _mnResult;
        }
    }
}