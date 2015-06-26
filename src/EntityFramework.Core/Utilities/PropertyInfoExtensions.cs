// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

// ReSharper disable once CheckNamespace

namespace System.Reflection
{
    [DebuggerStepThrough]
    internal static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo property)
        {
            return (property.GetMethod ?? property.SetMethod).IsStatic;
        }
    }
}
