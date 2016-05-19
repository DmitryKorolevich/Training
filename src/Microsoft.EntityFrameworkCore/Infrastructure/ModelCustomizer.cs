// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class ModelCustomizer : IModelCustomizer
    {
        public virtual void Customize(ModelBuilder modelBuilder, DbContext dbContext) => dbContext.OnModelCreating(modelBuilder);
    }
}
