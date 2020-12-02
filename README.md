## About
This is an implementation of the Zohar-Trench algorithm for solving a set of linear Toeplitz equations, based on the original article by S. Zohar "The Solution of a Toeplitz Set of Linear Equations" (https://doi.org/10.1145/321812.321822). There is also an option for an iterative solution that gives a valid result for increasing sub-matrix size.

For more info on the alghoritm and its limitations see the referenced paper. There is also many papers about alghtoritm numerical stability.

## Performance
The solution includes a small benchmark to compare an optimized, simple and MathNet.Numerics+MKL implementation. Although MathNet does not implement Toeplitz matrices and algorithms optimized for them, it is still worth the comparison. Here are my benchmark result:

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i5-6600 CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.302
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT


```
|           Method |   N |           Mean |        Error |       StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|----------------- |---- |---------------:|-------------:|-------------:|------:|--------:|---------:|---------:|---------:|----------:|
|       **MainSolver** |   **8** |       **578.9 ns** |      **2.32 ns** |      **2.17 ns** |  **0.10** |    **0.00** |   **0.0858** |        **-** |        **-** |     **272 B** |
|      NaiveSolver |   8 |       751.6 ns |      2.53 ns |      2.37 ns |  0.13 |    0.00 |   0.3519 |        - |        - |    1104 B |
| MathNetNativeMkl |   8 |     5,591.0 ns |     28.57 ns |     26.72 ns |  1.00 |    0.00 |   0.3052 |        - |        - |     968 B |
|                  |     |                |              |              |       |         |          |          |          |           |
|       **MainSolver** |  **32** |     **9,417.0 ns** |     **43.28 ns** |     **40.48 ns** |  **0.27** |    **0.00** |   **0.1373** |        **-** |        **-** |     **464 B** |
|      NaiveSolver |  32 |    12,925.8 ns |     34.63 ns |     32.39 ns |  0.37 |    0.00 |   2.7618 |        - |        - |    8688 B |
| MathNetNativeMkl |  32 |    34,788.1 ns |    112.81 ns |    105.52 ns |  1.00 |    0.00 |   1.5259 |        - |        - |    4904 B |
|                  |     |                |              |              |       |         |          |          |          |           |
|       **MainSolver** | **128** |   **196,113.0 ns** |    **388.41 ns** |    **363.32 ns** |  **1.06** |    **0.00** |   **0.2441** |        **-** |        **-** |    **1232 B** |
|      NaiveSolver | 128 |   251,840.0 ns |    891.94 ns |    834.32 ns |  1.36 |    0.01 |  34.1797 |        - |        - |  108160 B |
| MathNetNativeMkl | 128 |   184,660.8 ns |    738.73 ns |    691.01 ns |  1.00 |    0.00 |  20.5078 |        - |        - |   66728 B |
|                  |     |                |              |              |       |         |          |          |          |           |
|       **MainSolver** | **512** |   **910,067.0 ns** |  **3,061.63 ns** |  **2,863.85 ns** |  **0.72** |    **0.01** |   **0.9766** |        **-** |        **-** |    **4305 B** |
|      NaiveSolver | 512 | 1,680,356.0 ns |  5,395.66 ns |  5,047.11 ns |  1.34 |    0.02 | 513.6719 |        - |        - | 1612112 B |
| MathNetNativeMkl | 512 | 1,255,668.4 ns | 19,334.47 ns | 18,085.48 ns |  1.00 |    0.00 | 248.0469 | 248.0469 | 248.0469 | 1052788 B |
