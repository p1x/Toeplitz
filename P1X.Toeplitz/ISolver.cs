namespace P1X.Toeplitz {
    public interface ISolver<in TMatrix, in TVectorB, in TVectorA> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix
        where TVectorB : struct, IReadOnlyVector 
        where TVectorA : struct, IVector {
        void Solve(TMatrix matrix, TVectorB rightVector, TVectorA resultVector);
    }
}