namespace P1X.Toeplitz {
    public interface IOperations<T> {
        T Negate(T a);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b);
        T One();
    }
}