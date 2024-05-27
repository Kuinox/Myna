using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Mocker.API;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Mocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var guid = Guid.NewGuid().ToString();
            var libPath = @"C:\dev\Mocker\Mocker.API\bin\Debug\net8.0\Mocker.API.dll";
            var dllPath = @"C:\dev\Mocker\MockerExecuting\bin\Debug\net8.0\Mocked.dll";
            var tempDllPath = dllPath + guid;

            using (var libPEStream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var libModule = ModuleDefinition.ReadModule(libPEStream))
            using (var peStream = File.Open(dllPath, FileMode.Open, FileAccess.ReadWrite))
            using (var module = ModuleDefinition.ReadModule(peStream))
            {
                if (!ShouldWeave(module)) return;

                var proxyType = libModule.GetType("Mocker.API.MockProxy");
                var proxyMethod = proxyType.Methods.Single(x => x.Name == nameof(MockProxy.Relay));
                var importedProxyType = module.ImportReference(proxyType);

                var type = module.GetType("Mocked.ClassToMock");

                WeaveType(module, proxyMethod, importedProxyType, type);

                module.Write(tempDllPath);
            }

            File.Move(tempDllPath, dllPath, true);
        }

        static void WeaveType(ModuleDefinition module, MethodDefinition proxyMethod, TypeReference importedProxyType, TypeDefinition type)
        {
            var proxyField = new FieldDefinition("mockProxy", FieldAttributes.Public, importedProxyType);
            type.Fields.Add(proxyField);

            // Capture all original methods names
            var redirectedMethods = new Dictionary<string, MethodDefinition>();
            foreach (var method in type.Methods)
            {
                if (method.IsConstructor || method.IsStatic)
                    continue;

                redirectedMethods.Add(method.Name, method);
                method.Name += "_Original";
            }

            // Create new methods to redirect the original method
            foreach (var method in redirectedMethods)
            {
                var methodInfo = method.Value;
                var newMethod = new MethodDefinition(method.Key, method.Value.Attributes, method.Value.ReturnType);
                type.Methods.Add(newMethod);
                var il = newMethod.Body.GetILProcessor();

                var elseStart = il.Create(OpCodes.Ldarg_0);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, proxyField);
                il.Emit(OpCodes.Brtrue_S, elseStart); //0x0f
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, method.Value);
                il.Emit(OpCodes.Ret);
                il.Append(elseStart);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, proxyField);
                il.Emit(OpCodes.Ldftn, method.Value);

                var types = methodInfo.Parameters.Select(x => typeof(object)).Append(methodInfo.ReturnType == module.TypeSystem.Void ? typeof(void) : typeof(object)).ToArray();
                var delegateType = Expression.GetDelegateType(types);
                if(delegateType.ContainsGenericParameters)
                {
                    delegateType = delegateType.GetGenericTypeDefinition();
                }
                var delegateConstructor = module.ImportReference(delegateType.GetConstructors()[0]);

                il.Emit(OpCodes.Newobj, delegateConstructor);
                il.Emit(OpCodes.Ldc_I4, methodInfo.Parameters.Count);
                il.Emit(OpCodes.Newarr, module.TypeSystem.Object);

                for ( var i = 0; i < methodInfo.Parameters.Count; i++ )
                {
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    if (methodInfo.Parameters[i].ParameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, methodInfo.Parameters[i].ParameterType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }

                il.Emit(OpCodes.Callvirt, module.ImportReference(proxyMethod));
                il.Emit(OpCodes.Ret);
            }
        }

        static bool ShouldWeave(ModuleDefinition module)
        {
            // Check if MockerWeavingSentinelAttribute is present on the assembly
            var sentinelAttribute = module.CustomAttributes
                .FirstOrDefault(attr => attr.AttributeType.FullName == typeof(Mocker.API.MockerWeavingSentinelAttribute).FullName);

            if (sentinelAttribute != null)
            {
                Console.WriteLine("MockerWeavingSentinelAttribute found on the assembly. Exiting.");
                return false;
            }

            // Add MockerWeavingSentinelAttribute to the assembly
            var attributeConstructor = module.ImportReference(typeof(Mocker.API.MockerWeavingSentinelAttribute).GetConstructor(Type.EmptyTypes));
            var customAttribute = new CustomAttribute(attributeConstructor);
            module.Assembly.CustomAttributes.Add(customAttribute);
            return true;
        }
    }
}
