// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class PropertyDiscoveryConvention : IEntityTypeConvention, IBaseTypeConvention
    {
        public virtual InternalEntityTypeBuilder Apply(InternalEntityTypeBuilder entityTypeBuilder)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            var entityType = entityTypeBuilder.Metadata;

            if (entityType.HasClrType())
            {
                var primitiveProperties = entityType.ClrType.GetRuntimeProperties().Where(IsCandidatePrimitiveProperty);
                foreach (var propertyInfo in primitiveProperties)
                {
                    entityTypeBuilder.Property(propertyInfo, ConfigurationSource.Convention);
                }
            }

            return entityTypeBuilder;
        }

        protected virtual bool IsCandidatePrimitiveProperty([NotNull] PropertyInfo propertyInfo)
        {
            Check.NotNull(propertyInfo, nameof(propertyInfo));

            return propertyInfo.IsCandidateProperty() && propertyInfo.PropertyType.IsPrimitive();
        }

        public virtual bool Apply(InternalEntityTypeBuilder entityTypeBuilder, EntityType oldBaseType)
            => Apply(entityTypeBuilder) != null;
    }
}
