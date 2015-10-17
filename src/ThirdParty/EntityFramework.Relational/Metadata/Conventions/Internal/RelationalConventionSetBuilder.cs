// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Metadata.Conventions.Internal
{
    public abstract class RelationalConventionSetBuilder : IConventionSetBuilder
    {
        public virtual ConventionSet AddConventions(ConventionSet conventionSet)
        {
            conventionSet.PropertyAddedConventions.Add(new RelationalColumnAttributeConvention());

            conventionSet.EntityTypeAddedConventions.Add(new RelationalTableAttributeConvention());

            conventionSet.BaseEntityTypeSetConventions.Add(new DiscriminatorConvention());

            return conventionSet;
        }
    }
}
