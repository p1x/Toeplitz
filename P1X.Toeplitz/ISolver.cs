namespace P1X.Toeplitz {
    public interface ISolver<in TMatrix, in TVectorB> 
        where TMatrix : struct, IReadOnlyToeplitzMatrix
        where TVectorB : struct, IReadOnlyVector {
        void Solve(TMatrix matrix, TVectorB rightVector, float[] resultVector);
    }
}