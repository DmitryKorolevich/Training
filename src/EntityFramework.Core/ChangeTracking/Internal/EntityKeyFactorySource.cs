// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.ChangeTracking.Internal
{
    public class EntityKeyFactorySource : IEntityKeyFactorySource
    {
        private readonly IBoxedValueReaderSource _boxedValueReaderSource;

        private readonly ThreadSafeDictionaryCache<IReadOnlyList<IProperty>, EntityKeyFactory> _cache
            = new ThreadSafeDictionaryCache<IReadOnlyList<IProperty>, EntityKeyFactory>(
                new ReferenceEnumerableEqualityComparer<IReadOnlyList<IProperty>, IProperty>());

        public EntityKeyFactorySource([NotNull] IBoxedValueReaderSource boxedValueReaderSource)
        {
            _boxedValueReaderSource = boxedValueReaderSource;
        }

        public virtual EntityKeyFactory GetKeyFactory([NotNull] IReadOnlyList<IProperty> keyProperties)
            => _cache.GetOrAdd(
                keyProperties,
                k =>
                    {
                        if (k.Count == 1)
                        {
                            var keyProperty = k[0];
                            var keyType = keyProperty.ClrType;

                            // Use composite key for anything with structural (e.g. byte[]) properties even if they are
                            // not composite because it is setup to do structural comparisons and the generic typing
                            // advantages of the simple key don't really apply anyway.
                            if (!typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(keyType.GetTypeInfo()))
                            {
                                var sentinel = keyProperty.SentinelValue;

                                return (EntityKeyFactory)(keyType.IsNullableType()
                                    ? Activator.CreateInstance(typeof(SimpleEntityKeyFactory<>).MakeGenericType(keyType.UnwrapNullableType()), sentinel)
                                    : Activator.CreateInstance(typeof(SimpleEntityKeyFactory<>).MakeGenericType(keyType), sentinel));
                            }
                        }

                        return new CompositeEntityKeyFactory(
                            k.Select(p => p.SentinelValue).ToList(),
                            k.Select(p => _boxedValueReaderSource.GetReader(p)).ToList());
                    });
    }
}
