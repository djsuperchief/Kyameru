using System;

namespace Kyameru
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Kyameru.Core.Clob.From(null).Process(null).Process(null).Process(null).To(null).Build();
            
        }
    }
}
