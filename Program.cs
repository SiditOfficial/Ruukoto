using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuukotoBot
{
    internal class Program
    {
        private static readonly CancellationTokenSource CancellationSource = new CancellationTokenSource();

        private static void Main()
        {
            new Ruukoto().Init();
            try { Task.Delay(-1, CancellationSource.Token).GetAwaiter().GetResult(); }
            catch (TaskCanceledException) { Console.WriteLine("Бот ушёл спать"); Thread.EndThreadAffinity(); }
        }

        internal static void Close()
        {
            CancellationSource.Cancel();
        }
    }
}
