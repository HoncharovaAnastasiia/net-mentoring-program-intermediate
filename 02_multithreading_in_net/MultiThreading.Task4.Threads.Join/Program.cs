/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            CreateThread(10);

            Console.WriteLine();

            var semaphore = new SemaphoreSlim(0, 1);
            CreateThreadsWithThreadPool(10, semaphore);
            semaphore.Wait();

            Console.ReadLine();
        }

        static void CreateThread(int state)
        {
            if (state < 1) return;

            PrintState(--state);

            var thread = new Thread(() => CreateThread(state));
            thread.Start();
            thread.Join();
        }

        static void CreateThreadsWithThreadPool(int state, SemaphoreSlim semaphore)
        {
            if (state < 1)
            {
                semaphore.Release();
                return;
            }

            PrintState(--state);
            ThreadPool.QueueUserWorkItem(_ => CreateThreadsWithThreadPool(state, semaphore));
        }

        static void PrintState(int count)
        {
            Console.WriteLine($"Count: {count}");
        }
    }
}
