<h1 align="center">
	Myna
</h1>
<p align="center">
Myna: A drop-in alternative to Moq®️, capable of mocking interfaces and <i>any</i> classes.
</p>

<div align="center">
<img align="center" src="assets/myna.jpg" height="400" />
</div>
<p align="center" style="color: gray">
$\textcolor{gray}{\textsf{The Myna bird is known for its ability to mimic a wide range of sounds.}}$
</p>

![NuGet Version](https://img.shields.io/nuget/v/Myna.TheFatChicken)

In order to use it, you can replace your Moq®️ package reference with:

```xml
<PackageReference Include="Myna.TheFatChicken" Version="0.1.0" />
```

Without Myna, using only Moq®️:
```csharp

var mock = new Mock<IFoo>();
mock.Setup(x => x.Mocked).Returns(true);
Assert.IsTrue(mock.Object.Mocked);

interface IFoo
{
    public bool Mocked { get; }
}

class Foo : IFoo
{
   public bool Mocked => false;
}
```

With Myna:
```csharp

var mock = new Mock<Foo>();
mock.Setup(x => x.Mocked).Returns(true);
Assert.IsTrue(mock.Object.Mocked);

class Foo : IFoo
{
   public bool Mocked => false;
}
```
You don't need to pollute your codebase anymore with useless interfaces !

## How does it work ?

In order to be able to Mock any class, Myna transparently weave the DLLs after your project has compiled.
It means it can only mock classes of DLLs that are copied to the output directory, so for example, you cannot mock .NET librairies, thankfully.

## NuGet Packages
Myna was designed to support other Mocking libraries, right now, only Moq®️ is implemented.


| Package Name         | Description                                                                                   |
|----------------------|-----------------------------------------------------------------------------------------------|
| **Myna.API**         | Contains the class proxy that gets injected into your classes.      |
| **Myna.Build**       | An MSBuild task that runs the weaver on the assemblies.  |
| **Myna.TheFatChicken** | A Moq®️ fork that uses the class proxy from Myna.API. It also depends on Myna.Build so you only need to depends on this. |

## Documentation

There is no special API to know, everything should work transparently! (if not, feel free to open an issue)  
You can just create a Mock of a class and it should work!  
For the API documentation itself, you can refer to [Moq®️ docs](https://github.com/devlooped/moq/wiki)

## The Fat Chicken ?!?
https://www.youtube.com/watch?v=bFIR2XIMQfQ


