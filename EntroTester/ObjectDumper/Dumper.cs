using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EntroTester.Tests")]
[assembly: InternalsVisibleTo("EntroTester.Tests.Core")]

namespace EntroTester.ObjectDumper
{
    internal class DumpOptions
    {
        public static DumpOptions Default = new DumpOptions();

        public bool NoFields { get; set; }
    }
    internal sealed class DebugWriter : TextWriter
    {
        public DebugWriter()
            : base(CultureInfo.InvariantCulture)
        {
            // Do nothing here
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            Debug.Write(value);
        }

        public override void Write(string value)
        {
            Debug.Write(value);
        }
    }
    internal static class StringEx
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.Trim().Length == 0;
        }
    }
    internal static class Dumper
    {
        public static void Dump(object value, string name, TextWriter writer)
        {
            Dump(value, name, writer, DumpOptions.Default);
        }

        public static void Dump(object value, string name, TextWriter writer, DumpOptions options)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (writer == null)
                throw new ArgumentNullException("writer");

            InternalDump(0, name, value, writer);
        }

        private static void InternalDump(int indentationLevel, string name, object value, TextWriter writer)
        {
            var indentation = new string(' ', indentationLevel * 3);
            var prefix = string.IsNullOrWhiteSpace(name) ? "" : $"{name} = ";

            if (value == null || indentationLevel > 18)
            {
                writer.Write("{0}{1}null", indentation, prefix);
                return;
            }

            Type type = value.GetType();

            if (new []
                {
                    typeof (int), typeof(short), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(bool), typeof (byte),
                    typeof (int?), typeof(short?), typeof(long?), typeof(float?), typeof(double?), typeof(decimal?), typeof(bool?), typeof (byte?),
                }.Contains(type))
            {
                writer.Write("{0}{1}{2}", indentation, prefix, value);
                return;
            }
            if (value is string)
            {
                writer.Write("{0}{1}\"{2}\"", indentation, prefix, value);
                return;
            }
            if (typeof (DateTime) == type || typeof (DateTime?) == type)
            {
                var date = (DateTime?) value;
                writer.Write("{0}{1}new DateTime({2})", indentation, prefix, date.Value.Ticks);
                return;
            }
            if (typeof (Guid) == type || typeof (Guid?) == type)
            {
                var guid = (Guid?) value;
                writer.Write("{0}{1}new Guid(\"{2}\")", indentation, prefix, guid.Value);
                return;
            }
            //if (!type.IsValueType)
            //{
            //    bool firstTime;
            //    idGenerator.GetId(value, out firstTime);
            //    if (!firstTime)
            //    {
            //        writer.Write("{0}{1}null", indentation, prefix);
            //        return;
            //    }
            //}
            if (value is Exception)
            {
                var exception = value as Exception;
                writer.Write("{0}{1}new {2}() {{ Message = \"{3}\" }}", indentation, prefix, type.FullName.Replace("+", "."), exception.Message);
                return;
            }

            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                var collectionTypeName = GetCollectionType(type);
                if (!collectionTypeName.EndsWith("[]"))
                {
                    collectionTypeName += "()";
                }

                writer.WriteLine("{0}{1}new {2}", indentation, prefix, collectionTypeName);
                writer.WriteLine("{0}{{", indentation);
                int i = 0;
                foreach (var item in enumerable)
                {
                    InternalDump(indentationLevel + 2, $"", item, writer);
                    writer.WriteLine(",");
                    i++;
                }
                writer.Write("{0}}}", indentation);
                return;
            }

            writer.Write("{0}{1}new {2}()", indentation, prefix, value.GetType().FullName.Replace("+", "."));

            PropertyInfo[] properties =
                (from property in type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where property.GetIndexParameters().Length == 0
                       && property.CanRead
                 select property).ToArray();

            if (!properties.Any())
                return;
           
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "\n{0}{{", indentation));
            if (properties.Any())
            {
                foreach (PropertyInfo pi in properties)
                {
                    try
                    {
                        object propertyValue = pi.GetValue(value, null);
                        InternalDump(indentationLevel + 2, pi.Name, propertyValue, writer);
                    }
                    catch (TargetInvocationException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer);
                    }
                    catch (ArgumentException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer);
                    }
                    writer.WriteLine(",");
                }
            }
            writer.Write(string.Format(CultureInfo.InvariantCulture, "{0}}}", indentation));
        }

        static string GetCollectionType(Type type)
        {
            var types = new Stack<string>();
            if (type.GetTypeInfo().IsGenericType)
            {
                var nonGenericType = type.GetGenericTypeDefinition().Name.TrimEnd('`','1');
                if (nonGenericType == "IEnumerable")
                {
                    nonGenericType = "List";
                }
                types.Push(nonGenericType);
                return GetCollectionType(types, type.GetTypeInfo().GetGenericArguments().Single());
            }
            return type.Name;
        }

        static string GetCollectionType(Stack<string> types, Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                var nonGenericType = type.GetGenericTypeDefinition().Name.TrimEnd('`', '1');
                types.Push(nonGenericType);
                return GetCollectionType(types, type.GetTypeInfo().GetGenericArguments().Single());
            }
            return types.Aggregate(type.FullName.Replace("+", "."), (s, x) => $"{x}<{s}>");
        }
    }
    internal static class ObjectDumperExtensions
    {
        public static T Dump<T>(this T value, string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            using (var writer = new DebugWriter())
            {
                return Dump(value, name, writer);
            }
        }

        public static T Dump<T>(this T value, string name, string filename)
        {
            // Error-checking in called method

            return Dump(value, filename, name, Encoding.UTF8);
        }

        public static T Dump<T>(this T value, string name, string filename, Encoding encoding)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (StringEx.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            
            using (var fileStream = new FileStream(filename, FileMode.CreateNew))
            using (var writer = new StreamWriter(fileStream, encoding))
            {
                return Dump(value, name, writer);
            }
        }

        public static T Dump<T>(this T value, string name, TextWriter writer)
        {
            return Dump(value, name, writer, DumpOptions.Default);
        }

        public static T Dump<T>(this T value, string name, TextWriter writer, DumpOptions options)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (options == null)
                throw new ArgumentNullException("options");

            Dumper.Dump(value, name, writer, options);
            writer.WriteLine(";");

            return value;
        }

        public static string DumpToString<T>(this T value, string name)
        {
            return DumpToString(value, name, new DumpOptions { NoFields = true });
        }

        public static string DumpToString<T>(this T value, string name, DumpOptions options) 
        {
            if (value == null)
            {
                return $"{name} = null";
            }
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Dump(value, name, writer, options);
                return writer.ToString();
            }
        }
    }
}
