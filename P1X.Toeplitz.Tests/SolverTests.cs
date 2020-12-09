using System;
using System.Collections.Generic;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class SolverTests : SolverTestsBase {
        protected override ISolver<NormalizedToeplitzMatrix, Vector> GetSolver() => new Solver(4);

        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public void Iterate_ValidResult(float[] matrixValues, float[] rightValues, float[] expectedResult) {
            var solver = new Solver(2);
            var matrix = NormalizedToeplitzMatrix.Create(matrixValues);
            var resultVector = new float[matrix.Size];
            var rightVector = new Vector(rightValues);

            for (var i = 0; i < matrix.Size - 1; i++) 
                solver.Iterate(matrix, rightVector, resultVector);
            
            Assert.Equal(expectedResult, resultVector, new RoundingSingleEqualityComparer(5));
        }
        
        [Fact]
        public void IterateDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => new Solver(2).Iterate(NormalizedToeplitzMatrix.Create(2), default, new float[2]));
            Assert.Throws<ArgumentNullException>(() => new Solver(2).Iterate(NormalizedToeplitzMatrix.Create(2), new Vector(new float[2]), default));
            Assert.Throws<ArgumentException>(() => new Solver(2).Iterate(default, new Vector(new float[2]), new float[2]));
        }
        
        [Fact]
        public void IterateMatrixInsufficientSize_ThrowException() {
            var solver = new Solver(2);
            var matrix = NormalizedToeplitzMatrix.Create(2);
            var size = Solver.ResultVectorMultiplier;
            var b = new Vector(new float[size]);
            var x = new float[size];
            solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateRightVectorInsufficientSize_ThrowException() {
            var solver = new Solver(2);
            var size = Solver.ResultVectorMultiplier;
            var matrix = NormalizedToeplitzMatrix.Create(size * 2);
            var b = new Vector(new float[size]);
            var x = new float[size * 2];
            for (var i = 0; i < size - 1; i++) 
                solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateResultVectorInsufficientSize_ThrowException() {
            var solver = new Solver(2);
            var size = Solver.ResultVectorMultiplier;
            var matrix = NormalizedToeplitzMatrix.Create(size * 2);
            var b = new Vector(new float[size * 2]);
            var x = new float[size];
            for (var i = 0; i < size - 1; i++) 
                solver.Iterate(matrix, b, x);
            
            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
        
        [Fact]
        public void IterateResultVectorInsufficientSize2_ThrowException() {
            var solver = new Solver(2);
            var size = System.Numerics.Vector<float>.Count;
            var matrix = NormalizedToeplitzMatrix.Create(size);
            var b = new Vector(new float[size]);
            var x = new float[size - 1];

            Assert.Throws<ArgumentException>(() => solver.Iterate(matrix, b, x));
        }
    }

    public class NaiveSolverTests : SolverTestsBase {
        protected override ISolver<NormalizedToeplitzMatrix, Vector> GetSolver() => new NaiveSolver();
    }
    
    public abstract class SolverTestsBase {
        protected abstract ISolver<NormalizedToeplitzMatrix, Vector> GetSolver();
        
        [Fact]
        public void SolveDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), default, new float[2]));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new Vector(new float[2]), default));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(default, new Vector(new float[2]), new float[2]));
        }
        
        [Fact]
        public void SolveDifferentVectorAndMatrixSizes_ThrowException() {
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(3), new Vector(new float[2]), new float[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new Vector(new float[3]), new float[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new Vector(new float[2]), new float[3]));
        }

        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public void Solve_ValidResult(float[] matrixValues, float[] rightVector, float[] expectedResult) {
            var matrix = NormalizedToeplitzMatrix.Create(matrixValues);
            var resultVector = new float[matrix.Size];
            
            GetSolver().Solve(matrix, new Vector(rightVector), resultVector);
            
            Assert.Equal(expectedResult, resultVector, new RoundingSingleEqualityComparer(5));
        }
        
        [Theory]
        [MemberData(nameof(GetValidTestDataBySize), new [] {8, 16, 32})]
        public void SolveLarge_ValidResult(float[] matrixValues, float[] rightVector) {
            var matrix = NormalizedToeplitzMatrix.Create(matrixValues);
            var resultVector = new float[matrix.Size];
            
            GetSolver().Solve(matrix, new Vector(rightVector), resultVector);
            
            var expectedResult = new float[rightVector.Length];
            Assert.NotEqual(expectedResult, resultVector, new RoundingSingleEqualityComparer(6));
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
                var matrix = NormalizedToeplitzMatrix.Create(n);
                for (var i = 1; i < n; i++)
                    matrix[i] = matrix[-i] = R(i);

                var rightValues = new float[n];
                for (var i = 0; i < n; i++)
                    rightValues[i] = R(0.5f - n / 2f + i);

                yield return new object[] { matrix.GetValues(), rightValues };
            }
        }
    }
}