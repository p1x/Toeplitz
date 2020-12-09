using System;
using Xunit;

namespace P1X.Toeplitz.Tests {
    public class NormalizedToeplitzMatrixTests {
        [Fact]
        public void CreateWithSize_EmptyMatrix() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(2);
            
            Assert.Equal(2, matrix.Size);
        }

        [Fact]
        public void CreateWithZeroOrNegativeSize_ThrowsException() {
            Assert.Throws<ArgumentOutOfRangeException>(() => NormalizedToeplitzMatrixSingle.Create(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => NormalizedToeplitzMatrixSingle.Create(-2));
        }

        [Fact]
        public void CreateWithValues_MatrixWithSameValues() {
            var values = new float[] { 2, 3, 1, 4, 5 };
            
            var matrix = NormalizedToeplitzMatrixSingle.Create(values);
            
            Assert.Equal(3, matrix.Size);
            Assert.Equal(1, matrix[0]);
            Assert.Equal(values[0], matrix[-2]);
            Assert.Equal(values[1], matrix[-1]);
            Assert.Equal(values[2], matrix[0]);
            Assert.Equal(values[3], matrix[1]);
            Assert.Equal(values[4], matrix[2]);
        }

        [Fact]
        public void CreateWithNoValues_MatrixWithOneElement() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(new [] { 1f });
            
            Assert.Equal(1, matrix.Size);
            Assert.Equal(1, matrix[0]);
        }

        [Fact]
        public void CreateWithNotNormalizedValues_ThrowException() {
            Assert.Throws<ArgumentException>(() => NormalizedToeplitzMatrixSingle.Create(new float[] { 1, 2, 3 }));
        }
        
        [Fact]
        public void CreateWithEvenNumberOfValues_ThrowException() {
            Assert.Throws<ArgumentException>(() => NormalizedToeplitzMatrixSingle.Create(new float[4]));
        }

        [Fact]
        public void Get_ReturnValidValue() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(new float[] { 2, 1, 3 });
            
            Assert.Equal(2, matrix[-1]);
            Assert.Equal(3, matrix[1]);
        }

        [Fact]
        public void GetOutOfRangeIndex_ThrowException() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(2);
            
            Assert.Throws<IndexOutOfRangeException>(() => matrix[2]);
            Assert.Throws<IndexOutOfRangeException>(() => matrix[-2]);
        }
        
        [Fact]
        public void Set_ChangeValue() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(new float[] { 2, 1, 3 });

            matrix[-1] = 4;
            matrix[1] = 5;
            
            Assert.Equal(4, matrix[-1]);
            Assert.Equal(5, matrix[1]);
        }

        [Fact]
        public void SetZeroIndex_ThrowException() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(new float[] { 2, 1, 3 });

            Assert.Throws<IndexOutOfRangeException>(() => matrix[0] = 4);
        }

        [Fact]
        public void SetOutOfRangeIndex_ThrowException() {
            var matrix = NormalizedToeplitzMatrixSingle.Create(new float[] { 2, 1, 3 });
            
            Assert.Throws<IndexOutOfRangeException>(() => matrix[-2] = 4);
            Assert.Throws<IndexOutOfRangeException>(() => matrix[2] = 5);
        }

        [Fact]
        public void DefaultInstanceIsInitialized_ReturnFalse() {
            Assert.False(default(NormalizedToeplitzMatrixSingle).IsInitialized);
        }

        [Fact]
        public void CreatedInstanceIsInitialized_ReturnTrue() {
            Assert.True(NormalizedToeplitzMatrixSingle.Create(2).IsInitialized);
        }
    }
}