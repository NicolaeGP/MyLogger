using System;

namespace LogUsers
{
    using LogComponent;
    using System.Threading;


    class Program
    {
        static void Main(string[] args)
        {
            AsyncFileLogger logger = new AsyncFileLogger();

            for (int i = 0; i < 15; i++)
            {
                logger.Log(LogLevel.Information, "Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.StopAndFlush();
            AsyncFileLogger logger2 = new AsyncFileLogger();

            for (int i = 50; i > 0; i--)
            {
                logger2.Log(LogLevel.Information, "Number with No flush: " + i.ToString());
                Thread.Sleep(20);
            }

            logger2.Stop();

            Console.ReadLine();
        }
    }
}
