namespace P1X.Toeplitz {
    public interface ISolver {
        void Solve(NormalizedToeplitzMatrix matrix, float[] rightVector, float[] resultVector);
    }
}