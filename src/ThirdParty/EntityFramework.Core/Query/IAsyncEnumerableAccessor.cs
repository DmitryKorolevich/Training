// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Data.Entity.Query
{
    public interface IAsyncEnumerableAccessor<out T>
    {
        IAsyncEnumerable<T> AsyncEnumerable { get; }
    }
}
