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
            return GetSimpleTypeName(type);
        }

        private static string GetSimpleTypeName(Type t)
        {
            StringBuilder sb = new StringBuilder(64);
            ProcessTypeName(t, delegate (TypeWrapper writerItem)
            {
                if (writerItem.Type == null)
                {
                    sb.Append(writerItem.Literal);
                    return;
                }
                sb.Append(GetSimpleGenericTypeName(writerItem.Type));
            });
            return sb.ToString();
        }

        private static void AppendGenericArguments(Type[] args, Action<TypeWrapper> wrapperAction)
        {
            if (args.Length > 0)
            {
                wrapperAction(new TypeWrapper("<"));
                for (int i = 0; i < args.Length; i++)
                {
                    ProcessTypeName(args[i], wrapperAction);
                    if (i + 1 < args.Length)
                    {
                        wrapperAction(new TypeWrapper(", "));
                    }
                }
                wrapperAction(new TypeWrapper(">"));
            }
        }

        private static Type GetMostGenericTypeDefinition(Type t)
        {
            while (t.GetTypeInfo().IsGenericType)
            {
                Type genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == null || t == genericTypeDefinition)
                {
                    return t;
                }
                t = genericTypeDefinition;
            }
            return t;
        }

        private static void ProcessTypeName(Type t, Action<TypeWrapper> wrapperAction)
        {
            if (t.IsGenericParameter)
            {
                wrapperAction(new TypeWrapper(t.Name));
                return;
            }
            if (t.GetTypeInfo().IsGenericType)
            {
                Type mostGenericTypeDefinition = GetMostGenericTypeDefinition(t);
                wrapperAction(new TypeWrapper(mostGenericTypeDefinition));
                AppendGenericArguments(t.GetTypeInfo().GenericTypeArguments, wrapperAction);
                return;
            }
            wrapperAction(new TypeWrapper(t));
        }

        private static string GetSimpleGenericTypeName(Type t)
        {
            string text = t.FullName;
            if (text == null)
            {
                text = t.Name;
            }
            int num = text.IndexOf('`');
            if (num != -1)
            {
                return text.Substring(0, num);
            }
            return text;
        }
    }

    internal class TypeWrapper
    {
        public Type Type { get; set; }

        public string Literal { get; set; }

        public TypeWrapper(string literal)
        {
            Literal = literal;
        }

        public TypeWrapper(Type type)
        {
            Type = type;
        }
    }
}