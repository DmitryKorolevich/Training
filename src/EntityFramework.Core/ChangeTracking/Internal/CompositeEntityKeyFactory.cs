// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;

namespace Microsoft.Data.Entity.ChangeTracking.Internal
{
    public class CompositeEntityKeyFactory : EntityKeyFactory
    {
        private readonly IReadOnlyList<object> _sentinels;
        private readonly IReadOnlyList<IBoxedValueReader> _boxedValueReaders;

        public CompositeEntityKeyFactory(
            [NotNull] IReadOnlyList<object> sentinels,
            [NotNull] IReadOnlyList<IBoxedValueReader> boxedValueReaders)
        {
            _sentinels = sentinels;
            _boxedValueReaders = boxedValueReaders;
        }

        public override EntityKey Create(
            IEntityType entityType, IReadOnlyList<IProperty> properties, IValueReader valueReader)
        {
            var components = new object[properties.Count];

            for (var i = 0; i < properties.Count; i++)
            {
                var index = properties[i].Index;

                if (valueReader.IsNull(index))
                {
                    return EntityKey.InvalidEntityKey;
                }

                var value = _boxedValueReaders[i].ReadValue(valueReader, index);

                if (Equals(value, _sentinels[i]))
                {
                    return EntityKey.InvalidEntityKey;
                }

                components[i] = value;
            }

            return new CompositeEntityKey(entityType, components);
        }

        public override EntityKey Create(
            IEntityType entityType, IReadOnlyList<IProperty> properties, IPropertyAccessor propertyAccessor)
        {
            var components = new object[properties.Count];

            for (var i = 0; i < properties.Count; i++)
            {
                var value = propertyAccessor[properties[i]];

                if (value == null
                    || Equals(value, _sentinels[i]))
                {
                    return EntityKey.InvalidEntityKey;
                }

                components[i] = value;
            }

            return new CompositeEntityKey(entityType, components);
        }
    }
}
