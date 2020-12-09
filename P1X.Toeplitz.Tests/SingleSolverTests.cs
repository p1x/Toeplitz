using System.Collections.Generic;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class SingleSolverTests : SolverTestsBase<NormalizedToeplitzMatrixSingle, VectorSingle, float> {
        protected override ISolver<NormalizedToeplitzMatrixSingle, VectorSingle, float> GetSolver() => new SingleSolver<NormalizedToeplitzMatrixSingle, VectorSingle>(2);

        protected override NormalizedToeplitzMatrixSingle NewMatrix(int size) => NormalizedToeplitzMatrixSingle.Create(size);

        protected override NormalizedToeplitzMatrixSingle NewMatrix(float[] data) => NormalizedToeplitzMatrixSingle.Create(data);

        protected override VectorSingle NewVector(float[] data) => new VectorSingle(data);

        protected override IEqualityComparer<float> GetRoundingComparer() => new RoundingSingleEqualityComparer(5);
        protected override float[] ConvertArray(float[] data) => data;

        [Fact]
        public void CanCreate() {
            var solver = new SingleSolver<NormalizedToeplitzMatrixSingle, VectorSingle>(2);
            Assert.NotNull(solver);
        }
    }
}