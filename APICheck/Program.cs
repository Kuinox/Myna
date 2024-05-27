using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        string directory = Directory.GetCurrentDirectory();

        string moqPath = Path.Combine(directory, "Moq.dll");
        string mockerApiPath = Path.Combine(directory, "Mocker.Moq.dll");

        Assembly moqAssembly = Assembly.LoadFrom(moqPath);
        Assembly mockerApiAssembly = Assembly.LoadFrom(mockerApiPath);

        var moqTypes = moqAssembly.GetExportedTypes();
        var mockerApiTypes = mockerApiAssembly.GetExportedTypes();

        var missingTypes = new List<string>();
        var incorrectTypes = new List<string>();
        var missingMembers = new List<MemberInfo>();

        foreach (var moqType in moqTypes)
        {
            var mockerApiType = mockerApiTypes.FirstOrDefault(t => t.FullName == moqType.FullName);

            if (mockerApiType == null)
            {
                missingTypes.Add(moqType.FullName!);
            }
            else
            {
                // check if it's also a class/interface/enum/struct/delegate
                if (moqType.IsClass != mockerApiType.IsClass ||
                    moqType.IsInterface != mockerApiType.IsInterface ||
                    moqType.IsEnum != mockerApiType.IsEnum ||
                    moqType.IsValueType != mockerApiType.IsValueType ||
                    moqType.IsTypeDefinition != mockerApiType.IsTypeDefinition)
                {
                    incorrectTypes.Add(moqType.FullName!);
                    continue;
                }
                var moqMembers = moqType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                var mockerApiMembers = mockerApiType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                foreach (var moqMember in moqMembers)
                {
                    bool memberExists = mockerApiMembers.Any(m => MembersAreEqual(moqMember, m));

                    if (!memberExists)
                    {
                        missingMembers.Add(moqMember);
                    }
                }
            }
        }

        Console.WriteLine("Missing Types:");
        missingTypes.ForEach(Console.WriteLine);

        Console.WriteLine("\nMissing Members:");
        missingMembers.ForEach(x =>
        {
            if(x is MethodInfo method)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                Console.WriteLine($"{method.ReturnType} {method.DeclaringType.FullName}.{method.Name}({parameters})");
            }
            else if(x is PropertyInfo property)
                Console.WriteLine($"{property.PropertyType.FullName} {property.DeclaringType.FullName}.{property.Name}");
            else if(x is FieldInfo field)
                Console.WriteLine($"{field.FieldType.FullName} {field.DeclaringType.FullName}.{field.Name}");
            else if(x is EventInfo theEvent)
                Console.WriteLine($"{theEvent.EventHandlerType.FullName} {theEvent.DeclaringType.FullName}.{theEvent.Name}");
        });

        Console.WriteLine("\nIncorrect Types:");
        incorrectTypes.ForEach( Console.WriteLine );
    }

    static bool MembersAreEqual(MemberInfo moqMember, MemberInfo mockerApiMember)
    {
        if (moqMember.MemberType != mockerApiMember.MemberType)
            return false;

        if (moqMember.Name != mockerApiMember.Name)
            return false;

        if (moqMember is MethodInfo moqMethod && mockerApiMember is MethodInfo mockerApiMethod)
            return MethodsAreEqual(moqMethod, mockerApiMethod);

        if (moqMember is PropertyInfo moqProperty && mockerApiMember is PropertyInfo mockerApiProperty)
            return PropertiesAreEqual(moqProperty, mockerApiProperty);

        if (moqMember is FieldInfo moqField && mockerApiMember is FieldInfo mockerApiField)
            return FieldsAreEqual(moqField, mockerApiField);

        if (moqMember is EventInfo moqEvent && mockerApiMember is EventInfo mockerApiEvent)
            return EventsAreEqual(moqEvent, mockerApiEvent);

        return false;
    }

    static bool MethodsAreEqual(MethodInfo moqMethod, MethodInfo mockerApiMethod)
    {
        if (moqMethod.ReturnType != mockerApiMethod.ReturnType)
            return false;

        var moqParameters = moqMethod.GetParameters();
        var mockerApiParameters = mockerApiMethod.GetParameters();

        if (moqParameters.Length != mockerApiParameters.Length)
            return false;

        for (int i = 0; i < moqParameters.Length; i++)
        {
            if (moqParameters[i].ParameterType != mockerApiParameters[i].ParameterType)
                return false;
        }

        return true;
    }

    static bool PropertiesAreEqual(PropertyInfo moqProperty, PropertyInfo mockerApiProperty)
    {
        return moqProperty.PropertyType == mockerApiProperty.PropertyType;
    }

    static bool FieldsAreEqual(FieldInfo moqField, FieldInfo mockerApiField)
    {
        return moqField.FieldType == mockerApiField.FieldType;
    }

    static bool EventsAreEqual(EventInfo moqEvent, EventInfo mockerApiEvent)
    {
        return moqEvent.EventHandlerType == mockerApiEvent.EventHandlerType;
    }
}
