using System;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class SolverTests : SolverTestsBase {
        protected override ISolver GetSolver() => new Solver();
    }

    public class NaiveSolverTests : SolverTestsBase {
        protected override ISolver GetSolver() => new NaiveSolver();
    }
    
    public abstract class SolverTestsBase {
        protected abstract ISolver GetSolver();
        
        [Fact]
        public void SolveDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), null, new float[2]));
            Assert.Throws<ArgumentNullException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new float[2], null));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(default, new float[2], new float[2]));
        }

        [Fact]
        public void SolveDifferentVectorAndMatrixSizes_ThrowException() {
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(3), new float[2], new float[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new float[3], new float[2]));
            Assert.Throws<ArgumentException>(() => GetSolver().Solve(NormalizedToeplitzMatrix.Create(2), new float[2], new float[3]));
        }

        [Theory]
        // Wolfram Alpha query:
        // ToeplitzMatrix[{1, 2, 3}, {1, 2, 3}] . {x1, x2, x3} == {14, 10, 10}
        [InlineData(new [] { 2f, 3f, 2f, 3f }, new[] { 14f, 10f, 10f }, new[] { 1f, 2f, 3f })]
        // ToeplitzMatrix[{1, 0.5, 0.3}, {1, 0.2, 0.4}] . {x1, x2, x3} = {0.52, 0.62, 0.86}
        [InlineData(new [] { 0.2f, 0.4f, 0.5f, 0.3f }, new[] { 0.52f, 0.62f, 0.86f }, new[] { 0.2f, 0.4f, 0.6f })]
        // ToeplitzMatrix[{1, 0.2, 0.3, 0.4}, {1, 0.2, 0.3, 0.4}] . {x1, x2, x3, x4} == {0.39, 0.4, 0.45, 0.56}
        [InlineData(new [] { 0.2f, 0.3f, 0.4f, 0.2f, 0.3f, 0.4f }, new[] { 0.39f, 0.4f, 0.45f, 0.56f }, new[] { 0.1f, 0.2f, 0.3f, 0.4f })]
        // ToeplitzMatrix[{1, 0.2, 0.3, 0.4}, {1, 0.5, 0.6, 0.7}] . {x1, x2, x3, x4} == {0.66, 0.61, 0.57, 0.56}
        [InlineData(new [] { 0.5f, 0.6f, 0.7f, 0.2f, 0.3f, 0.4f }, new[] { 0.66f, 0.61f, 0.57f, 0.56f }, new[] { 0.1f, 0.2f, 0.3f, 0.4f })]
        public void Solve_ValidResult(float[] matrixValues, float[] rightVector, float[] expectedResult) {
            var matrix = NormalizedToeplitzMatrix.Create(matrixValues);
            var resultVector = new float[matrix.Size];
            
            GetSolver().Solve(matrix, rightVector, resultVector);
            
            Assert.Equal(expectedResult, resultVector, new RoundingSingleEqualityComparer(6));
        }
    }
}