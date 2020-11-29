using BenchmarkDotNet.Running;

namespace P1X.Toeplitz.BenchmarkSuite {
    public class Program {
        public static void Main(string[] args) {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }
}