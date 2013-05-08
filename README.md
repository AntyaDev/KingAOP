#KingAOP

It's an AOP framework which is essentially a free alternative of PostSharp. If you familiar with PostSharp, then you can notice that KingAOP has even the same interfaces to interract with you:)

The concept of aspect-oriented programming (AOP) offers an interesting alternative for the specification of non-functional component properties (such as fault-tolerance properties or timing behaviour), as well as other crosscutting concerns. These are implemented as so-called aspects and at some point "weaved" to the other functional parts of the software.

##How it works?
It‘s use a dynamic opportunities of C# 4.0 instead of IL rewriting technique which the PostSharp use. And that brings us to a predictable and clean model. You can look through an AOP weaving stuff in very easy way.

##Basic example:
* Implement a hello world aspect.


```csharp
class HelloWorldAspect : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Console.WriteLine("OnEntry: Hello KingAOP");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Console.WriteLine("OnExit: Hello KingAOP");
    }
}
```
* Add the hello aspect to some class.

```csharp
class HelloWorld : IDynamicMetaObjectProvider
{
    [HelloWorldAspect]
    public void HelloWorldCall()
    {
        Console.WriteLine("Hello World");
    }

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
        return new AspectWeaver(parameter, this);
    }
}
```
* Enjoy.

```csharp
dynamic helloWorld = new HelloWorld();
helloWorld.HelloWorldCall();
```
##Logging example:
Using the above studied concept, we will now attempt to develop a simple logger which will use AOP to log information. It’s canonical example of AOP, as without it:). Like with PostSharp we have to inherit from the OnMethoBoundaryAspect and override the OnEntry and OnExit methods.  
