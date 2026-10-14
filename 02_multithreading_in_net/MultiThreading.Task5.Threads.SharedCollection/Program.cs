/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static readonly int countOfElements = 10;
        static readonly List<string> sharedCollection = new(countOfElements);

        static readonly AutoResetEvent elementAdded = new(false);
        static readonly AutoResetEvent elementPrinted = new(false);

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var addTask = Task.Run(AddElements);
            var printTask = Task.Run(PrintElements);

            Task.WaitAll(addTask, printTask);

            elementAdded.Dispose();
            elementPrinted.Dispose();

            Console.ReadLine();
        }

        static void AddElements() => IterateElements(element =>
        {
            lock (sharedCollection)
            {
                sharedCollection.Add($"Element {element}");
            }

            elementAdded.Set();
            elementPrinted.WaitOne();
        });

        static void PrintElements() => IterateElements(_ =>
        {
            elementAdded.WaitOne();

            lock (sharedCollection)
            {
                foreach (string element in sharedCollection)
                {
                    Console.WriteLine(element);
                }

                Console.WriteLine();
            }

            elementPrinted.Set();
        });

        static void IterateElements(Action<int> action)
        {
            for (int element = 1; element <= countOfElements; element++)
            {
                action(element);
            }
        }
    }
}
