namespace P1X.Toeplitz {
    public interface IVector : IReadOnlyVector {
        new float this[int index] { get; set; }
    }
}