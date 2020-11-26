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
            // ToeplitzMatrix[{1, 2, 3}, {1, 2, 3}] . {x1, x2, x3} == {14, 10, 10}
            // Result: x1 == 1 && x2 == 2 && x3 == 3
            var matrix = NormalizedToeplitzMatrix.Create(3);
            matrix[1] = matrix[-1] = 2;
            matrix[2] = matrix[-2] = 3;
            var rightVector = new float[] { 14, 10, 10 };
            var resultVector = new float[3];
            
            Solver.Solve(matrix, rightVector, resultVector);
            
            Assert.Equal(1, resultVector[0]);
            Assert.Equal(2, resultVector[1]);
            Assert.Equal(3, resultVector[2]);
        }
    }
}