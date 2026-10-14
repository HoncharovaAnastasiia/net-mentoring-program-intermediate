/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static readonly Random random = new();

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Task.Run(CreateArray)
                .ContinueWith(t => MultiplyArray(t.Result))
                .ContinueWith(t => SortArrayByAscending(t.Result))
                .ContinueWith(t => GetAverageValue(t.Result))
                .Wait();

            Console.ReadLine();
        }

        static int[] CreateArray()
        {
            var array = Enumerable.Range(0, 10).Select(_ => GetRandomNumber()).ToArray();
            Console.WriteLine($"Created: [{string.Join(", ", array)}]");
            return array;
        }

        static int[] MultiplyArray(int[] array)
        {
            var result = array.Select(number => number * GetRandomNumber()).ToArray();
            Console.WriteLine($"Multiplied: [{string.Join(", ", result)}]");
            return result;
        }

        static int[] SortArrayByAscending(int[] array)
        {
            var result = array.Order().ToArray();
            Console.WriteLine($"Sorted: [{string.Join(", ", result)}]");
            return result;
        }

        static int GetAverageValue(int[] array)
        {
            var average = (int)array.Average();
            Console.WriteLine($"Average: {average}");
            return average;
        }

        static int GetRandomNumber() => random.Next(0, 10);
    }
}
