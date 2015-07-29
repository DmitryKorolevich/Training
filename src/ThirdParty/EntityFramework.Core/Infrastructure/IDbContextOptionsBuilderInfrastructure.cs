// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.Data.Entity.Infrastructure
{
    public interface IDbContextOptionsBuilderInfrastructure
    {
        void AddOrUpdateExtension<TExtension>([NotNull] TExtension extension)
            where TExtension : class, IDbContextOptionsExtension;
    }
}
