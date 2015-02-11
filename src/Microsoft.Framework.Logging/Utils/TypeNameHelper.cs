// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Text;

namespace Microsoft.Framework.Logging
{
    public static class TypeNameHelper
    {
        public static string GetTypeDisplayFullName(Type type)
        {
            var sb = new StringBuilder(64);
            ProcessTypeName(type, sb);
            return sb.ToString();
        }

        private static void AppendGenericArguments(Type[] args, StringBuilder sb)
        {
            if (args.Length > 0)
            {
                ConstructorTypeString("<", sb);
                for (int i = 0; i < args.Length; i++)
                {
                    ProcessTypeName(args[i], sb);
                    if (i + 1 < args.Length)
                    {
                        ConstructorTypeString(", ", sb);
                    }
                }
                ConstructorTypeString(">", sb);
            }
        }

        private static Type GetMostGenericTypeDefinition(Type t)
        {
            while (t.GetTypeInfo().IsGenericType)
            {
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == null || t == genericTypeDefinition)
                {
                    return t;
                }
                t = genericTypeDefinition;
            }
            return t;
        }

        private static void ProcessTypeName(Type t, StringBuilder sb)
        {
            if (t.IsGenericParameter)
            {
                ConstructorTypeString(t.GetTypeInfo().Name, sb);
                return;
            }
            if (t.GetTypeInfo().IsGenericType)
            {
                var mostGenericTypeDefinition = GetMostGenericTypeDefinition(t);
                ConstructorTypeString(mostGenericTypeDefinition, sb);
                AppendGenericArguments(t.GetTypeInfo().GenericTypeArguments, sb);
                return;
            }
            ConstructorTypeString(t, sb);
        }

        private static string GetSimpleGenericTypeName(Type t)
        {
            var text = t.FullName;
            if (text == null)
            {
                text = t.Name;
            }
            var num = text.IndexOf('`');
            if (num != -1)
            {
                return text.Substring(0, num);
            }
            return text;
        }

        private static void ConstructorTypeString(object typeInfo, StringBuilder sb)
        {
            if (typeInfo is string)
            {
                sb.Append(typeInfo.ToString());
                return;
            }

            sb.Append(GetSimpleGenericTypeName((Type)typeInfo));
        }
    }
}