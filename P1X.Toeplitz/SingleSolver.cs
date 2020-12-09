namespace P1X.Toeplitz {
    public class SingleSolver<TMatrix, TVector> : Solver<TMatrix, TVector, SingleOperations, float> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix<float> 
        where TVector : struct, IReadOnlyVector<float> {
        public SingleSolver(int expectedMatrixSize) : base(expectedMatrixSize) { }
    }
}