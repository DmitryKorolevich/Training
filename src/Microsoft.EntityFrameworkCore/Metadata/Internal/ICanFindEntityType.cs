// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public interface ICanFindEntityType
    {
        IEntityType FindEntityType([NotNull] Type type);
        IMutableEntityType AddEntityType([NotNull] string name, [CanBeNull] Type type);
    }
}
