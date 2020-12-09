namespace P1X.Toeplitz {
    public class DoubleSolver<TMatrix, TVector> : Solver<TMatrix, TVector, DoubleOperations, double> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix<double> 
        where TVector : struct, IReadOnlyVector<double> {
        public DoubleSolver(int expectedMatrixSize) : base(expectedMatrixSize) { }
    }
}