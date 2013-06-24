using System;
using KingAOP.Examples.ExceptionHandling;
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

            string text = "test";
            helloWorld.PrintIfArgNotNull(text);
            helloWorld.PrintIfArgNotNull(null);
            
            // logging example
            var entity = new TestEntity {Name = "Jon", Number = 99};
            dynamic repository = new TestRepository();
            repository.Save(entity);

            // exception handling example
            dynamic publicService = new PublicService
            {
                ServiceName = "TestService",
                Port = 1234,
                SessionId = "123908sdfhjkgdfg"
            };
            
            try
            {
                publicService.Send(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("We catched original exception after 'exception aspect' did his job.");
            }

            Console.Read();
        }
    }
}
