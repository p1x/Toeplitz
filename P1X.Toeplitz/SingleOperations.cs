namespace P1X.Toeplitz {
    public readonly struct SingleOperations : IOperations<float> {
        public float Negate(float a) => -a;

        public float Subtract(float a, float b) => a - b;

        public float Multiply(float a, float b) => a * b;

        public float Divide(float a, float b) => a / b;

        public float One() => 1f;
    }
}