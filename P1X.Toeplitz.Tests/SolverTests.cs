using System;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class SolverTests {
        [Fact]
        public void SolveDefaultParameters_ThrowException() {
            Assert.Throws<ArgumentNullException>(() => Solver.Solve(NormalizedToeplitzMatrix.Create(2), null, new float[2]));
            Assert.Throws<ArgumentNullException>(() => Solver.Solve(NormalizedToeplitzMatrix.Create(2), new float[2], null));
            Assert.Throws<ArgumentException>(() => Solver.Solve(default, new float[2], new float[2]));
        }

        [Fact]
        public void SolveDifferentVectorAndMatrixSizes_ThrowException() {
            Assert.Throws<ArgumentException>(() => Solver.Solve(NormalizedToeplitzMatrix.Create(3), new float[2], new float[2]));
            Assert.Throws<ArgumentException>(() => Solver.Solve(NormalizedToeplitzMatrix.Create(2), new float[3], new float[2]));
            Assert.Throws<ArgumentException>(() => Solver.Solve(NormalizedToeplitzMatrix.Create(2), new float[2], new float[3]));
        }

        [Fact]
        public void Solve_ValidResult() {
            // Wolfram Alpha query:
            // ToeplitzMatrix[{1, 0.5, 0.3}, {1, 0.2, 0.4}] . {x1, x2, x3} = {0.52, 0.62, 0.86}
            // Result: x1 == 0.2 && x2 == 0.4 && x3 == 0.6
            var matrix = NormalizedToeplitzMatrix.Create(3);
            matrix[1] = 0.5f;
            matrix[2] = 0.3f;
            matrix[-1] = 0.2f;
            matrix[-2] = 0.4f;
            var rightVector = new[] { 0.52f, 0.62f, 0.86f };
            var resultVector = new float[3];
            var expectedVector = new[] { 0.2f, 0.4f, 0.6f };
            
            Solver.Solve(matrix, rightVector, resultVector);
            
            Assert.Equal(expectedVector, resultVector, new RoundingSingleEqualityComparer(6));
        }
    }
}