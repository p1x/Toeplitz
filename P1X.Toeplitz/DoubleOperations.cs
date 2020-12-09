namespace P1X.Toeplitz {
    public readonly struct DoubleOperations : IOperations<double> {
        public double Negate(double a) => -a;

        public double Subtract(double a, double b) => a - b;

        public double Multiply(double a, double b) => a * b;

        public double Divide(double a, double b) => a / b;

        public double One() => 1d;
    }
}