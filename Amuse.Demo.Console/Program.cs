﻿using Amuse.Demo.Interfaces;
using System;
using System.Diagnostics;

namespace Amuse.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Container container = null;
            for (int i = 1; i <= 9; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                container = Container.Create();
                Console.WriteLine(string.Format("第 {0} 次创建容器: {1}ms", i, watch.ElapsedMilliseconds));
                watch.Stop();
            }

            IA a = null;
            for (int i = 1; i <= 9; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                a = container.Get<IA>("a");
                Console.WriteLine(string.Format("第 {0} 查找并装配对象: {1}ms", i, watch.ElapsedMilliseconds));
                watch.Stop();
            }

            Console.Read();
        }
    }
}