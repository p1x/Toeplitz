## About
This is an implementation of the Zohar-Trench algorithm for solving a set of linear Toeplitz equations, based on the original article by S. Zohar "The Solution of a Toeplitz Set of Linear Equations" (https://doi.org/10.1145/321812.321822). There is also an option for an iterative solution that gives a valid result for increasing sub-matrix size.

For more info on the algorithm and its limitations see the referenced paper. There is also many papers about algorithm numerical stability.

## Performance
The solution includes a small benchmark to compare with MathNet.Numerics+MKL implementation. Although MathNet does not implement Toeplitz matrices and algorithms optimized for them, it is still worth the comparison. Here are my results:

``` ini
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i5-6600 CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.302
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
```
|           Method |   N |           Mean |        Error |       StdDev |         Median | Ratio |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|----------------- |---- |---------------:|-------------:|-------------:|---------------:|------:|---------:|---------:|---------:|----------:|
|       **MainSolver** |   **8** |       **621.0 ns** |      **3.36 ns** |      **2.98 ns** |       **621.7 ns** |  **0.11** |   **0.1707** |        **-** |        **-** |     **536 B** |
| MathNetNativeMkl |   8 |     5,594.6 ns |     20.61 ns |     18.27 ns |     5,589.4 ns |  1.00 |   0.3052 |        - |        - |     968 B |
|                  |     |                |              |              |                |       |          |          |          |           |
|       **MainSolver** |  **32** |    **11,690.5 ns** |    **126.34 ns** |    **118.17 ns** |    **11,641.2 ns** |  **0.33** |   **0.2899** |        **-** |        **-** |     **920 B** |
| MathNetNativeMkl |  32 |    35,077.0 ns |    256.05 ns |    239.51 ns |    35,094.7 ns |  1.00 |   1.5259 |        - |        - |    4904 B |
|                  |     |                |              |              |                |       |          |          |          |           |
|       **MainSolver** | **128** |    **72,167.5 ns** |    **378.92 ns** |    **316.42 ns** |    **72,159.9 ns** |  **0.39** |   **0.7324** |        **-** |        **-** |    **2457 B** |
| MathNetNativeMkl | 128 |   185,198.8 ns |  1,108.50 ns |  1,036.89 ns |   184,818.0 ns |  1.00 |  20.5078 |        - |        - |   66728 B |
|                  |     |                |              |              |                |       |          |          |          |           |
|       **MainSolver** | **512** |   **219,324.7 ns** |  **1,027.25 ns** |    **910.63 ns** |   **219,068.5 ns** |  **0.17** |   **2.6855** |        **-** |        **-** |    **8603 B** |
| MathNetNativeMkl | 512 | 1,274,336.5 ns | 25,380.10 ns | 47,043.64 ns | 1,249,485.4 ns |  1.00 | 248.0469 | 248.0469 | 248.0469 | 1053296 B |
