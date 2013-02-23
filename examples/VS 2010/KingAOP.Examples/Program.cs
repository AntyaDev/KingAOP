using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingAOP.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic helloWorld = new HelloWorld.HelloWorld();
            helloWorld.HelloWorldCall();
            Console.Read();
        }
    }
}
