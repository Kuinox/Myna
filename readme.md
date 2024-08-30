<h1 align="center">
	Myna
</h1>
<p align="center">
Myna: A drop-in alternative to Moq®️, capable of mocking interfaces and <i>any</i> classes.
</p>

<img src="assets/myna.jpg" />
<p align="center" style="font-size: 0.85em; color: gray;">
The Myna bird is known for its ability to mimic a wide range of sounds.
</p>

![NuGet Version](https://img.shields.io/nuget/v/Myna.TheFatChicken)

```xml
<PackageReference Include="Myna.TheFatChicken" Version="0.1.0" />
```

In order to be able to Mock any classes, Myna weave transparently the DLLs after your project has compiled.
It means it can only mock classes of DLLs that are copied, so for example, you cannot mock .NET librairies, thankfully.

## NuGet Packages
Myna was designed to support other Mocking libraries, right now, only Moq®️ is implemented.


| Package Name         | Description                                                                                   |
|----------------------|-----------------------------------------------------------------------------------------------|
| **Myna.API**         | Contains the class proxy that gets injected into your classes.      |
| **Myna.Build**       | An MSBuild task that runs the weaver on the assemblies.  |
| **Myna.TheFatChicken** | A Moq®️ fork that uses the class proxy from Myna.API. |

## Documentation

There is no special API to know, everything should work transparently! (if not, feel free to open an issue)  
You can just create a Mock of a class and it should work!  
For the API documentation itself, you can refer to [Moq®️ docs](https://github.com/devlooped/moq/wiki)

## The Fat Chicken ?!?
https://www.youtube.com/watch?v=bFIR2XIMQfQ


