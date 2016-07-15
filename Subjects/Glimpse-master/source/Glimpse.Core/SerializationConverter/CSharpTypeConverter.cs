﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;

namespace Glimpse.Core.SerializationConverter
{
    /// <summary>
    /// The <see cref="ISerializationConverter"/> implementation responsible converting <see cref="Type"/> representation's into more recognizable C# syntax.
    /// </summary>
    /// <example>
    /// With <see cref="CSharpTypeConverter"/>, <c>System.Int32</c> is converted to <c>int</c> and <c>System.Collections.Generic.IDictionary`2[System.Double, System.String[]]</c> to <c>IDictionary&lt;double, string[]&gt;</c>.
    /// </example>
    /// <remarks>
    /// Users of other languages could disable <see cref="CSharpTypeConverter"/> and create a <c>SerializationConverter&lt;Type&gt;</c> implementation for their language.
    /// </remarks>
    public class CSharpTypeConverter : SerializationConverter<Type>
    {
        private static readonly Dictionary<Type, string> PrimitiveTypes =
            new Dictionary<Type, string>
                {
                    { typeof(bool), "bool" },
                    { typeof(byte), "byte" },
                    { typeof(char), "char" },
                    { typeof(decimal), "decimal" },
                    { typeof(double), "double" },
                    { typeof(float), "float" },
                    { typeof(int), "int" },
                    { typeof(long), "long" },
                    { typeof(object), "object" },
                    { typeof(sbyte), "sbyte" },
                    { typeof(short), "short" },
                    { typeof(string), "string" },
                    { typeof(uint), "uint" },
                    { typeof(ulong), "ulong" },
                    { typeof(ushort), "ushort" },
                };

        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An string of the C# syntax which would be used to represent <paramref name="type"/>.</returns>
        public override object Convert(Type type)
        {
            return GetName(type);
        }

        private string GetName(Type type)
        {
            var typeName = new StringBuilder();
            GetName(type, typeName, new Queue<Type>(type.GetGenericArguments()));
            return typeName.ToString();
        }

        private void GetName(Type type, StringBuilder output, Queue<Type> genericArgsStack)
        {
            if (type.IsNested)
            {
                GetName(type.DeclaringType, output, genericArgsStack);
                output.Append(".");
            }

            if (type.IsArray) 
            {
                // Array
                GetName(type.GetElementType(), output, genericArgsStack);
                output.Append("[]");
            }
            else if (!type.IsGenericType)
            {
                // Non-Generics
                output.Append(PrimitiveTypes.ContainsKey(type) ? PrimitiveTypes[type] : type.Name);
            }
            else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Null types
                GetName(type.GetGenericArguments().First(), output, genericArgsStack);
                output.Append("?");
            }
            else
            {
                // Generics
                var genericBaseType = type.GetGenericTypeDefinition();
                var genericName = genericBaseType.Name;

                if (genericName.Contains("`"))
                {
                    output.Append(genericName, 0, genericName.LastIndexOf('`'));

                    output.Append("<");

                    var typeArgsCount = GetGenericArgumentCount(type);
                    var remainingArgsCount = genericArgsStack.Count;

                    for (int i = 0; i < Math.Min(typeArgsCount, remainingArgsCount); i++)
                    {
                        if (i > 0)
                        {
                            output.Append(", ");
                        }

                        var arg = genericArgsStack.Dequeue();
                        GetName(arg, output, new Queue<Type>(arg.GetGenericArguments()));
                    }

                    output.Append(">");
                }
                else
                {
                    output.Append(genericName);
                }
            }
        }

        private int GetGenericArgumentCount(Type type)
        {
            if (type.DeclaringType == null)
            {
                return type.GetGenericArguments().Length;
            }

            return type.GetGenericArguments().Length - type.DeclaringType.GetGenericArguments().Length;
        }
    }
}