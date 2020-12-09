namespace P1X.Toeplitz {
    public class SingleSolver : Solver<NormalizedToeplitzMatrixSingle, VectorSingle, SingleOperations, float> {
        public SingleSolver(int expectedMatrixSize) : base(expectedMatrixSize) { }
    }
}