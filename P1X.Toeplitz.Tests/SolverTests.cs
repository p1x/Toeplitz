using System;
using System.Collections.Generic;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class SingleSolverTests : SolverTestsBase<NormalizedToeplitzMatrixSingle, VectorSingle, float> {
        protected override ISolver<NormalizedToeplitzMatrixSingle, VectorSingle, float> GetSolver() => new SingleSolver<NormalizedToeplitzMatrixSingle, VectorSingle>(2);

        protected override NormalizedToeplitzMatrixSingle NewMatrix(int size) => NormalizedToeplitzMatrixSingle.Create(size);

        protected override NormalizedToeplitzMatrixSingle NewMatrix(float[] data) => NormalizedToeplitzMatrixSingle.Create(data);

        protected override VectorSingle NewVector(float[] data) => new VectorSingle(data);

        protected override IEqualityComparer<float> GetRoundingComparer() => new RoundingSingleEqualityComparer(5);

        [Fact]
        public void SingleSolver_CanCreate() {
            var solver = new SingleSolver<NormalizedToeplitzMatrixSingle, VectorSingle>(2);
            Assert.NotNull(solver);
        }
    }

    public abstract class SolverTestsBase<TMatrix, TVector, T> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix<T> 
        where TVector : struct, IReadOnlyVector<T> 
        where T : struct, IFormattable, IEquatable<T>, IComparable<T> {
        protected abstract ISolver<TMatrix, TVector, T> GetSolver();

        protected abstract TMatrix NewMatrix(int size);
        protected abstract TMatrix NewMatrix(T[] data);
        protected abstract TVector NewVector(T[] data);
        protected abstract IEqualityComparer<T> GetRoundingComparer();
        
        [Fact]
        public void SolveDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NewMatrix(2), default, new T[2]));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NewMatrix(2), NewVector(new T[2]), default));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(default, NewVector(new T[2]), new T[2]));
        }
        
        [Fact]
        public void SolveDifferentVectorAndMatrixSizes_ThrowException() {
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NewMatrix(3), NewVector(new T[2]), new T[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NewMatrix(2), NewVector(new T[3]), new T[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NewMatrix(2), NewVector(new T[2]), new T[3]));
        }

        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public void Solve_ValidResult(T[] matrixValues, T[] rightVector, T[] expectedResult) {
            var matrix = NewMatrix(matrixValues);
            var resultVector = new T[matrix.Size];
            
            GetSolver().Solve(matrix, NewVector(rightVector), resultVector);
            
            Assert.Equal(expectedResult, resultVector, GetRoundingComparer());
        }
        
        [Theory]
        [MemberData(nameof(GetValidTestDataBySize), new [] {8, 16, 32})]
        public void SolveLarge_ValidResult(T[] matrixValues, T[] rightVector) {
            var matrix = NewMatrix(matrixValues);
            var resultVector = new T[matrix.Size];
            
            GetSolver().Solve(matrix, NewVector(rightVector), resultVector);
            
            var expectedResult = new T[rightVector.Length];
            Assert.NotEqual(expectedResult, resultVector, GetRoundingComparer());
        }

        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public void Iterate_ValidResult(T[] matrixValues, T[] rightValues, T[] expectedResult) {
            var solver = GetSolver();
            var matrix = NewMatrix(matrixValues);
            var resultVector = new T[solver.ResultVectorMultiplier * 2];
            var rightVector = NewVector(rightValues);

            for (var i = 0; i < matrix.Size - 1; i++) 
                solver.Iterate(matrix, rightVector, resultVector);

            var result = new T[matrix.Size];
            Array.Copy(resultVector, result, matrix.Size);
            
            Assert.Equal(expectedResult, result, GetRoundingComparer());
        }
        
        [Fact]
        public void IterateDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => GetSolver().Iterate(NewMatrix(2), default, new T[2]));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Iterate(NewMatrix(2), NewVector(new T[2]), default));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Iterate(default, NewVector(new T[2]), new T[2]));
        }
        
        [Fact]
        public void IterateMatrixInsufficientSize_ThrowException() {
            var solver = GetSolver();
            var matrix = NewMatrix(2);
            var size = solver.ResultVectorMultiplier;
            var b = NewVector(new T[size]);
            var x = new T[size];
            solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateRightVectorInsufficientSize_ThrowException() {
            var solver = GetSolver();
            var size = solver.ResultVectorMultiplier;
            var matrix = NewMatrix(size * 2);
            var b = NewVector(new T[size]);
            var x = new T[size * 2];
            for (var i = 0; i < size - 1; i++) 
                solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateResultVectorInsufficientSize_ThrowException() {
            var solver = GetSolver();
            var size = solver.ResultVectorMultiplier;
            var matrix = NewMatrix(size * 2);
            var b = NewVector(new T[size * 2]);
            var x = new T[size];
            for (var i = 0; i < size - 1; i++) 
                solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateResultVectorInsufficientSize2_ThrowException() {
            var solver = GetSolver();
            var size = solver.ResultVectorMultiplier;
            var matrix = NewMatrix(size);
            var b = NewVector(new T[size]);
            var x = new T[size - 1];

            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        public static IEnumerable<object[]> GetValidTestData() {
            // Wolfram Alpha query:
            // ToeplitzMatrix[{1, 2, 3}, {1, 2, 3}] . {x1, x2, x3} == {14, 10, 10}
            yield return new object[] { new[] { 3f, 2f, 1f, 2f, 3f }, new[] { 14f, 10f, 10f }, new[] { 1f, 2f, 3f } };
            // ToeplitzMatrix[{1, 0.5, 0.3}, {1, 0.2, 0.4}] . {x1, x2, x3} = {0.52, 0.62, 0.86}
            yield return new object[] { new [] { 0.4f, 0.2f, 1f, 0.5f, 0.3f }, new[] { 0.52f, 0.62f, 0.86f }, new[] { 0.2f, 0.4f, 0.6f }};
            // ToeplitzMatrix[{1, 0.2, 0.3, 0.4}, {1, 0.2, 0.3, 0.4}] . {x1, x2, x3, x4} == {0.39, 0.4, 0.45, 0.56}
            yield return new object[] { new [] { 0.4f, 0.3f, 0.2f, 1f, 0.2f, 0.3f, 0.4f }, new[] { 0.39f, 0.4f, 0.45f, 0.56f }, new[] { 0.1f, 0.2f, 0.3f, 0.4f }};
            // ToeplitzMatrix[{1, 0.2, 0.3, 0.4}, {1, 0.5, 0.6, 0.7}] . {x1, x2, x3, x4} == {0.66, 0.61, 0.57, 0.56}
            yield return new object[] { new [] { 0.7f, 0.6f, 0.5f, 1f, 0.2f, 0.3f, 0.4f }, new[] { 0.66f, 0.61f, 0.57f, 0.56f }, new[] { 0.1f, 0.2f, 0.3f, 0.4f }};
            // ToeplitzMatrix[{1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9}, {1, 0.21, 0.32, 0.43, 0.54, 0.65, 0.76, 0.87, 0.98}] . { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9}
            yield return new object[] { 
                new[] { 0.98f, 0.87f, 0.76f, 0.65f, 0.54f, 0.43f, 0.32f, 0.21f, 1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, 
                new[] { 3.18f, 2.796f, 2.454f, 2.175f, 1.98f, 1.89f, 1.926f, 2.109f, 2.46f }, 
                new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }
            };
        }

        public static IEnumerable<object[]> GetValidTestDataBySize(params int[] size) {
            static float R(float t) => MathF.Abs(MathF.Cos(10 * t) * MathF.Exp(-t * t));

            foreach (var n in size) {
                var matrix = new float[n * 2 - 1];//NewMatrix(n);
                for (var i = 1; i < n; i++) {
                    var j1 = n - 1 + i;
                    var j2 = n - 1 - i;
                    matrix[j1] = matrix[j2] = R(i);
                }
                matrix[n - 1] = 1f;

                var rightValues = new float[n];
                for (var i = 0; i < n; i++)
                    rightValues[i] = R(0.5f - n / 2f + i);

                yield return new object[] { matrix, rightValues };
            }
        }
    }
}