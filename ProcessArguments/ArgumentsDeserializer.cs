using ProcessArguments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ProcessArguments
{
    public static class ArgumentsDeserializer
    {
        public static void Deserialize<T>(IEnumerable<string> args, Action<T> callback) where T : class
        {
            T obj;

            try
            {
                obj = Deserialize<T>(args);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is InvalidProcessArgumentException)
                {
                    PrintExceptions(ex);
                    return;
                }

                throw;
            }

            callback(obj);
        }

        public static void Deserialize<T>(IEnumerable<string> args, Func<T, Task> callback) where T : class
        {
            T obj;

            try
            {
                obj = Deserialize<T>(args);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is InvalidProcessArgumentException)
                {
                    PrintExceptions(ex);
                    return;
                }

                throw;
            }

            callback(obj).GetAwaiter().GetResult();
        }

        private static void PrintExceptions(AggregateException exception)
        {
            foreach (var ex in exception.InnerExceptions)
                Console.WriteLine(ex.Message);
        }

        public static T Deserialize<T>(IEnumerable<string> args) where T : class
        {
            var type = typeof(T);
            var obj = CreateObject(type);
            var dict = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var delimiterIndex = arg.IndexOf('=');

                if (!arg.StartsWith("--"))
                    throw new IncorrectProcessArgumentException($"Incorrect argument '{arg}'");

                if (delimiterIndex == -1)
                    dict[arg[2..]] = null;
                else
                    dict[arg[2..delimiterIndex]] = arg[(delimiterIndex + 1)..];
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite);

            foreach (var property in properties)
            {
                var name = GetArgumentName(property.Name);
                var hasValue = dict.TryGetValue(name, out string value);
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

                if (hasValue)
                    property.SetValue(obj, ConvertValue(value, hasValue, property.PropertyType));
            }

            var exceptions = new List<Exception>(properties.Count());

            foreach (var property in properties)
            {
                var name = GetArgumentName(property.Name);
                var hasValue = dict.TryGetValue(name, out string value);

                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute != null && (!hasValue || (property.PropertyType != typeof(bool) && value == null)))
                    exceptions.Add(new RequiredProcessArgumentException($"Required argument '{name}'"));

                var requiredIfAttribute = property.GetCustomAttribute<RequiredIfAttribute>();

                if (requiredIfAttribute != null)
                {
                    var method = type.GetMethod(requiredIfAttribute.ConditionMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if ((bool)method.Invoke(obj, Array.Empty<object>()) && (!hasValue || (property.PropertyType != typeof(bool) && value == null)))
                        exceptions.Add(new RequiredProcessArgumentException($"Required argument '{name}'"));
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            return (T)obj;
        }

        private static object ConvertValue(object value, bool hasValue, Type conversionType)
        {
            if (conversionType == typeof(bool))
                return hasValue;

            if (value == null)
                return null;

            return Convert.ChangeType(value, conversionType);
        }

        private static string GetArgumentName(string propertyName)
        {
            return char.ToLower(propertyName[0]) + propertyName[1..];
        }

        private static object CreateObject(Type type)
        {
            var cctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);

            return cctor.Invoke(Array.Empty<object>());
        }
    }
}