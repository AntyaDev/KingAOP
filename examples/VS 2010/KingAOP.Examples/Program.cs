using System;
using KingAOP.Examples.Logging;
using KingAOP.Examples.TestData;

namespace KingAOP.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // hello world example
            dynamic helloWorld = new HelloWorld.HelloWorld();
            helloWorld.HelloWorldCall();
            
            // logging example
            var entity = new TestEntity {Name = "Jon", Number = 99};
            dynamic repository = new TestRepository();
            repository.Save(entity);

            Console.Read();
        }
    }
}
