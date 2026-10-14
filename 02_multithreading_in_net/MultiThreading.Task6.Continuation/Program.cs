/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            Regardless(); // a
            NotOnSuccess(); // b
            FaultedSameThread(); // c
            CanceledOutsideThreadPool(); // d

            Console.ReadLine();
        }

        static void Regardless()
        {
            Console.WriteLine("\nCase A");
            var task = Task.Run(LogParentAndSleep).ContinueWith(LogContinuation);
            task.Wait();
        }

        static void NotOnSuccess()
        {
            Console.WriteLine("\nCase B");
            var task = Task.Run(LogParentAndThrow).ContinueWith(LogContinuation, TaskContinuationOptions.NotOnRanToCompletion);
            Try(task.Wait);
        }

        static void FaultedSameThread()
        {
            Console.WriteLine("\nCase C");
            var task = Task.Run(LogParentAndThrow).ContinueWith(
                LogContinuation,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            );

            Try(task.Wait);
        }

        static void CanceledOutsideThreadPool()
        {
            Console.WriteLine("\nCase D");
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Task.FromCanceled(cts.Token).ContinueWith(
                LogContinuation,
                TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning
            ).Wait();
        }

        static void LogParentAndSleep() 
        { 
            Log("Parent"); 
            Thread.Sleep(1000); 
        }

        static void LogParentAndThrow()
        {
            LogParentAndSleep();
            throw new Exception();
        }

        static void LogContinuation(Task task)
        {
            Log($"Continuation, IsThreadPool: {Thread.CurrentThread.IsThreadPoolThread}, parent status: {task.Status}");
        }

        static void Log(string message) => Console.WriteLine($"  [{Environment.CurrentManagedThreadId}] {message}");

        static void Try(Action action) { try { action(); } catch { } }
    }
}
