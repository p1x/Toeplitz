using System;
using System.Collections.Generic;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class DoubleSolverTests : SolverTestsBase<NormalizedToeplitzMatrixDouble, VectorDouble, double> {
        protected override ISolver<NormalizedToeplitzMatrixDouble, VectorDouble, double> GetSolver() => new DoubleSolver<NormalizedToeplitzMatrixDouble, VectorDouble>(2);

        protected override NormalizedToeplitzMatrixDouble NewMatrix(int size) => NormalizedToeplitzMatrixDouble.Create(size);

        protected override NormalizedToeplitzMatrixDouble NewMatrix(double[] data) => NormalizedToeplitzMatrixDouble.Create(data);

        protected override VectorDouble NewVector(double[] data) => new VectorDouble(data);

        protected override IEqualityComparer<double> GetRoundingComparer() => new RoundingDoubleEqualityComparer(5);
        protected override double[] ConvertArray(float[] data) => Array.ConvertAll(data, f => (double) f);

        [Fact]
        public void CanCreate() {
            var solver = new DoubleSolver<NormalizedToeplitzMatrixDouble, VectorDouble>(2);
            Assert.NotNull(solver);
        }
    }
}