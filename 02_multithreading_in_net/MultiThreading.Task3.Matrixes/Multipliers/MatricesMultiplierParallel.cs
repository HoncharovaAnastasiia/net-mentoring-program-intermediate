using MultiThreading.Task3.MatrixMultiplier.Matrices;
using System;
using System.Threading.Tasks;

namespace MultiThreading.Task3.MatrixMultiplier.Multipliers
{
    public class MatricesMultiplierParallel : IMatricesMultiplier
    {
        public IMatrix Multiply(IMatrix m1, IMatrix m2)
        {
            if (m1.ColCount != m2.RowCount)
            {
                throw new InvalidOperationException($"Cannot multiply: m1 columns ({m1.ColCount}) != m2 rows ({m2.RowCount})");
            }

            long rows = m1.RowCount;
            long columns = m2.ColCount;
            long innerDimension = m1.ColCount;

            var matrix = new Matrix(rows, columns);

            Parallel.For(0, rows, row => {
                for (long column = 0; column < columns; column++)
                {
                    long value = 0;
                    for (long k = 0; k < innerDimension; k++) 
                    {
                        value += m1.GetElement(row, k) * m2.GetElement(k, column);
                    }
                    matrix.SetElement(row, column, value);
                }
            });

            return matrix;
        }
    }
}
