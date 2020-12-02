namespace P1X.Toeplitz {
    public interface IToeplitzMatrix : IReadOnlyToeplitzMatrix {
        new float this[int index] { get; set; }
    }
}