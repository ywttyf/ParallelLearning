using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallel线程
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //调用线程个数不同
            int[] data = new int[20];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }

            Console.WriteLine("普通 for 循环：");
            for (int i = 0; i < data.Length; i++)
            {
                Console.WriteLine($"for: 索引={i}, 线程ID={Thread.CurrentThread.ManagedThreadId}");
            }


            Console.WriteLine("\nParallel.For 并行循环：");
            Parallel.For(0, data.Length, i =>
            {
                Console.WriteLine($"Parallel.For: 索引={i}, 线程ID={Thread.CurrentThread.ManagedThreadId}");
            });



            Console.WriteLine("-----------------------------");

            Int32 dataSize = 100000000;
            Int32[] data1 = new Int32[dataSize];
            for (Int32 i = 0; i < data1.Length; i++)
            {
                data1[i] = i;
            }

            // for循环计时
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (Int32 i = 0; i < data1.Length; i++)
            {
                data1[i] = data1[i] * 2;
            }
            sw.Stop();
            Console.WriteLine($"for循环耗时: {sw.ElapsedMilliseconds} ms");


            // 还原数据
            for (Int32 i = 0; i < data1.Length; i++)
            {
                data1[i] = i;
            }

            // Parallel.For计时
            sw.Restart();
            Parallel.For(0, data1.Length, i =>
            {
                data1[i] = data1[i] * 2;
            });
            sw.Stop();
            Console.WriteLine($"Parallel.For耗时: {sw.ElapsedMilliseconds} ms");



            Console.WriteLine("-----------------------------");


            int dataSize10 = 1000;
            int[] data10 = new int[dataSize10];
            for (int i = 0; i < data10.Length; i++)
            {
                data10[i] = i;
            }

            // 普通LINQ
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            var linqResult = data10.Where(x => x % 4 == 0).Select(x => ComplexCalculation(x)).ToList();
            sw1.Stop();
            Console.WriteLine($"普通LINQ耗时: {sw1.ElapsedMilliseconds} ms");

            // PLINQ
            sw1.Restart();
            var plinqResult = data10.AsParallel().Where(x => x % 4 == 0).Select(x => ComplexCalculation(x)).ToList(); 
            sw1.Stop();
            Console.WriteLine($"PLINQ耗时: {sw1.ElapsedMilliseconds} ms");



            Console.WriteLine("-----------同步线程-异步线程------------------");
           
            Console.WriteLine("同步处理：");
            sw.Restart();
            for (int i = 0; i < 5; i++)
            {
                DoWork(i);
            }
            sw.Stop();
            Console.WriteLine($"同步总耗时: {sw.ElapsedMilliseconds} ms");


            Console.WriteLine("\n异步并发处理：");
            sw.Restart();
            Task[] tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                int idx = i;
                tasks[i] = Task.Run(() => DoWork(idx));
            }
            Task.WaitAll(tasks);
            sw.Stop();
            Console.WriteLine($"异步总耗时: {sw.ElapsedMilliseconds} ms");


            Console.WriteLine("\n异步方法+await示例：");
            sw.Restart();
            RunAsyncDemo().Wait();
            sw.Stop();
            Console.WriteLine($"async/await总耗时: {sw.ElapsedMilliseconds} ms");








            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        static void DoWork(int i)
        {
            Console.WriteLine($"任务{i}开始，线程ID={Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000); // 模拟耗时操作
            Console.WriteLine($"任务{i}结束，线程ID={Thread.CurrentThread.ManagedThreadId}");
        }


        static async Task RunAsyncDemo()
        {
            Task[] tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                int idx = i;
                tasks[i] = Task.Run(async () =>
                {
                    Console.WriteLine($"async任务{idx}开始，线程ID={Thread.CurrentThread.ManagedThreadId}");
                    await Task.Delay(1000); // 异步等待
                    Console.WriteLine($"async任务{idx}结束，线程ID={Thread.CurrentThread.ManagedThreadId}");
                });
            }
            await Task.WhenAll(tasks);
        }








        // 复杂计算方法
        static double ComplexCalculation(int x)
        {
            double result = x;
            for (int i = 0; i < 100; i++)
            {
                result = Math.Sqrt(result * 1.3501 + 3.141556711);
                //Thread.Sleep(5);
            }
            return result;
        }
    }
}
