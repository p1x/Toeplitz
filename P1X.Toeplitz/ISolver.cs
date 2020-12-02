namespace P1X.Toeplitz {
    public interface ISolver<in T> where T : IReadOnlyToeplitzMatrix {
        void Solve(T matrix, float[] rightVector, float[] resultVector);
    }
}