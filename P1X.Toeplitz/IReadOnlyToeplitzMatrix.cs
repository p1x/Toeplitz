namespace P1X.Toeplitz {
    public interface IReadOnlyToeplitzMatrix {
        float this[int index] { get; }
        
        int Size { get; }
    }
}