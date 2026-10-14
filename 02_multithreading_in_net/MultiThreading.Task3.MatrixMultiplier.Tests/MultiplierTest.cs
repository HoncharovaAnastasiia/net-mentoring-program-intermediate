using MultiThreading.Task3.MatrixMultiplier.Matrices;
using MultiThreading.Task3.MatrixMultiplier.Multipliers;
using System.Diagnostics;

namespace MultiThreading.Task3.MatrixMultiplier.Tests
{
    [TestClass]
    public class MultiplierTest
    {
        private const int WarmupIterations = 1;
        private const int TestIterations = 2;
        private static readonly int[] TestSizes = { 10, 25, 50, 75, 100, 150, 200 };

        [TestMethod]
        public void MultiplyMatrix3On3Test()
        {
            TestMatrix3On3(new MatricesMultiplier());
            TestMatrix3On3(new MatricesMultiplierParallel());
        }

        [TestMethod]
        [Timeout(60000, CooperativeCancellation = true)]
        public void ParallelEfficiencyTest()
        {
            var sequentialMultiplier = new MatricesMultiplier();
            var parallelMultiplier = new MatricesMultiplierParallel();

            int? thresholdSize = null;

            PrintHeader();

            foreach (var size in TestSizes)
            {
                var (sequentialAvg, parallelAvg) = MeasureMultipliers(sequentialMultiplier, parallelMultiplier, size);

                bool isParallelFaster = parallelAvg < sequentialAvg;
                PrintResult(size, sequentialAvg, parallelAvg, isParallelFaster);

                if (thresholdSize == null && isParallelFaster)
                {
                    thresholdSize = size;
                }
            }

            PrintFooter(thresholdSize);
            AssertParallelEfficiency(sequentialMultiplier, parallelMultiplier);
        }

        #region Performance Measurement

        private (long sequential, long parallel) MeasureMultipliers(IMatricesMultiplier sequential, IMatricesMultiplier parallel, int size)
        {
            var m1 = new Matrix(size, size);
            var m2 = new Matrix(size, size);

            for (int i = 0; i < WarmupIterations; i++)
            {
                sequential.Multiply(m1, m2);
                parallel.Multiply(m1, m2);
            }

            long sequentialAvg = MeasureAverageTime(sequential, m1, m2);
            long parallelAvg = MeasureAverageTime(parallel, m1, m2);

            return (sequentialAvg, parallelAvg);
        }

        private static long MeasureAverageTime(IMatricesMultiplier multiplier, IMatrix m1, IMatrix m2)
        {
            long total = 0;

            for (int i = 0; i < TestIterations; i++)
            {
                var sw = Stopwatch.StartNew();
                multiplier.Multiply(m1, m2);
                sw.Stop();
                total += sw.ElapsedMilliseconds;
            }

            return total / TestIterations;
        }

        private static void AssertParallelEfficiency(IMatricesMultiplier sequential, IMatricesMultiplier parallel)
        {
            const int assertionMatrixSize = 200;

            var m1 = new Matrix(assertionMatrixSize, assertionMatrixSize);
            var m2 = new Matrix(assertionMatrixSize, assertionMatrixSize);

            var swSequential = Stopwatch.StartNew();
            sequential.Multiply(m1, m2);
            swSequential.Stop();

            var swParallel = Stopwatch.StartNew();
            parallel.Multiply(m1, m2);
            swParallel.Stop();

            Console.WriteLine($"\nAssertion test ({assertionMatrixSize}x{assertionMatrixSize}):");
            Console.WriteLine($"Sequential: {swSequential.ElapsedMilliseconds}ms, Parallel: {swParallel.ElapsedMilliseconds}ms");

            Assert.IsLessThanOrEqualTo(
                swSequential.ElapsedMilliseconds,
                swParallel.ElapsedMilliseconds,
                $"For {assertionMatrixSize}x{assertionMatrixSize} matrix, parallel ({swParallel.ElapsedMilliseconds}ms) " +
                $"should be faster or equal to sequential ({swSequential.ElapsedMilliseconds}ms)");
        }

        #endregion

        #region Output Formatting

        private static void PrintHeader()
        {
            Console.WriteLine("Matrix Size\tSequential(ms)\tParallel(ms)\tWinner");
            Console.WriteLine(new string('-', 65));
        }

        private static void PrintResult(int size, long sequentialMs, long parallelMs, bool isParallelFaster)
        {
            string winner = isParallelFaster ? "Parallel" : "Sequential";
            Console.WriteLine($"{size}x{size}\t\t{sequentialMs}\t\t{parallelMs}\t\t{winner}");
        }

        private static void PrintFooter(int? thresholdSize)
        {
            Console.WriteLine(new string('-', 65));

            string message = thresholdSize.HasValue
                ? $"Parallel becomes more effective at {thresholdSize}x{thresholdSize} matrices"
                : "Parallel was not faster for any tested size.";

            Console.WriteLine(message);
        }

        #endregion

        #region Matrix 3x3 Test

        void TestMatrix3On3(IMatricesMultiplier matrixMultiplier)
        {
            if (matrixMultiplier == null)
            {
                throw new ArgumentNullException(nameof(matrixMultiplier));
            }

            var m1 = new Matrix(3, 3);
            m1.SetElement(0, 0, 34);
            m1.SetElement(0, 1, 2);
            m1.SetElement(0, 2, 6);

            m1.SetElement(1, 0, 5);
            m1.SetElement(1, 1, 4);
            m1.SetElement(1, 2, 54);

            m1.SetElement(2, 0, 2);
            m1.SetElement(2, 1, 9);
            m1.SetElement(2, 2, 8);

            var m2 = new Matrix(3, 3);
            m2.SetElement(0, 0, 12);
            m2.SetElement(0, 1, 52);
            m2.SetElement(0, 2, 85);

            m2.SetElement(1, 0, 5);
            m2.SetElement(1, 1, 5);
            m2.SetElement(1, 2, 54);

            m2.SetElement(2, 0, 5);
            m2.SetElement(2, 1, 8);
            m2.SetElement(2, 2, 9);

            var multiplied = matrixMultiplier.Multiply(m1, m2);
            Assert.AreEqual(448, multiplied.GetElement(0, 0));
            Assert.AreEqual(1826, multiplied.GetElement(0, 1));
            Assert.AreEqual(3052, multiplied.GetElement(0, 2));

            Assert.AreEqual(350, multiplied.GetElement(1, 0));
            Assert.AreEqual(712, multiplied.GetElement(1, 1));
            Assert.AreEqual(1127, multiplied.GetElement(1, 2));

            Assert.AreEqual(109, multiplied.GetElement(2, 0));
            Assert.AreEqual(213, multiplied.GetElement(2, 1));
            Assert.AreEqual(728, multiplied.GetElement(2, 2));
        }

        #endregion
    }
}