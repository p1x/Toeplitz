using System;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Single;

namespace P1X.Toeplitz.BenchmarkSuite {
    [MemoryDiagnoser]
    public class Benchmarks {
        [Params(8, 32, 1024)]
        public int N;
        
        private NormalizedToeplitzMatrix _matrix;

        private float[] _rightVector;
        private float[] _result;
        private DenseMatrix _mnMatrix;
        private DenseVector _mnRightVector;
        private DenseVector _mnResult;

        [GlobalSetup]
        public void Setup() {
            static float R(float t) => MathF.Abs(MathF.Cos(10 * t) * MathF.Exp(-t * t));

            _matrix = NormalizedToeplitzMatrix.Create(N);
            for (var i = 1; i < N; i++)
                _matrix[i] = _matrix[-i] = R(i);

            _mnMatrix = new DenseMatrix(N);
            for (var i = 0; i < N; i++)
            for (var j = 0; j < N; j++)
                _mnMatrix[i, j] = _matrix[Math.Abs(i - j)];

            _rightVector = new float[N];
            for (var i = 0; i < N; i++) 
                _rightVector[i] = R(0.5f - N / 2f + i);

            _mnRightVector = new DenseVector(_rightVector);
            
            _result = new float[N];
            
            _mnResult = new DenseVector(N);
        }
        
        [Benchmark]
        public float[] MainSolver() {
            new Solver().Solve(_matrix, _rightVector, _result);
            return _result;
        }
        
        [Benchmark]
        public float[] NaiveSolver() {
            new Solver().Solve(_matrix, _rightVector, _result);
            return _result;
        }

        [Benchmark(Baseline = true)]
        public DenseVector MathNetManged() {
            Control.UseManaged();
            _mnMatrix.Solve(_mnRightVector, _mnResult);
            return _mnResult;
        }
        
        [Benchmark]
        public DenseVector MathNetNativeMkl() {
            Control.UseNativeMKL();
            _mnMatrix.Solve(_mnRightVector, _mnResult);
            return _mnResult;
        }
    }
}