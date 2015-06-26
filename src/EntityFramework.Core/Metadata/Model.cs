// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata
{
    public class Model : Annotatable, IModel
    {
        // TODO: Perf: use a mutable structure before the model is made readonly
        // Issue #868
        private ImmutableSortedSet<EntityType> _entities
            = ImmutableSortedSet<EntityType>.Empty.WithComparer(new EntityTypeNameComparer());

        public virtual EntityType AddEntityType([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

            return AddEntityType(new EntityType(type, this));
        }

        public virtual EntityType AddEntityType([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            return AddEntityType(new EntityType(name, this));
        }

        private EntityType AddEntityType(EntityType entityType)
        {
            var previousLength = _entities.Count;
            _entities = _entities.Add(entityType);

            if (previousLength == _entities.Count)
            {
                throw new InvalidOperationException(Strings.DuplicateEntityType(entityType.Name));
            }

            return entityType;
        }

        public virtual EntityType GetOrAddEntityType([NotNull] Type type)
        {
            return FindEntityType(type) ?? AddEntityType(type);
        }

        public virtual EntityType GetOrAddEntityType([NotNull] string name)
        {
            return FindEntityType(name) ?? AddEntityType(name);
        }

        [CanBeNull]
        public virtual EntityType FindEntityType([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsClass ? FindEntityType(new EntityType(type, this)) : null;
        }

        [CanBeNull]
        public virtual EntityType FindEntityType([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            return FindEntityType(new EntityType(name, this));
        }

        private EntityType FindEntityType(EntityType entityType)
        {
            return _entities.TryGetValue(entityType, out entityType)
                ? entityType
                : null;
        }

        public virtual EntityType GetEntityType([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

            var entityType = FindEntityType(type);
            if (entityType == null)
            {
                throw new ModelItemNotFoundException(Strings.EntityTypeNotFound(type.Name));
            }

            return entityType;
        }

        public virtual EntityType GetEntityType([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            var entityType = FindEntityType(name);
            if (entityType == null)
            {
                throw new ModelItemNotFoundException(Strings.EntityTypeNotFound(name));
            }

            return entityType;
        }

        public virtual EntityType RemoveEntityType([NotNull] EntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            if (GetReferencingForeignKeys(entityType).Any())
            {
                throw new InvalidOperationException(Strings.EntityTypeInUse(entityType.Name));
            }

            var previousEntities = _entities;
            _entities = _entities.Remove(entityType);

            EntityType removedEntityType = null;
            if (previousEntities.Count != _entities.Count)
            {
                previousEntities.TryGetValue(entityType, out removedEntityType);
            }

            return removedEntityType;
        }

        public virtual IReadOnlyList<EntityType> EntityTypes
        {
            get { return _entities; }
        }

        public virtual IReadOnlyList<ForeignKey> GetReferencingForeignKeys([NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            // TODO: Perf: Add additional indexes so that this isn't a linear lookup
            // Issue #1179
            return EntityTypes.SelectMany(et => et.ForeignKeys).Where(fk => fk.PrincipalEntityType == entityType).ToList();
        }

        public virtual IReadOnlyList<ForeignKey> GetReferencingForeignKeys([NotNull] IKey key)
        {
            Check.NotNull(key, nameof(key));

            // TODO: Perf: Add additional indexes so that this isn't a linear lookup
            // Issue #1179
            return EntityTypes.SelectMany(e => e.ForeignKeys).Where(fk => fk.PrincipalKey == key).ToList();
        }

        public virtual IReadOnlyList<ForeignKey> GetReferencingForeignKeys([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            // TODO: Perf: Add additional indexes so that this isn't a linear lookup
            // Issue #1179
            return EntityTypes
                .SelectMany(e => e.ForeignKeys
                    .Where(f => f.PrincipalKey.Properties.Contains(property)))
                .ToList();
        }

        public virtual string StorageName { get; [param: CanBeNull] set; }

        IEntityType IModel.FindEntityType(Type type)
        {
            return FindEntityType(type);
        }

        IEntityType IModel.GetEntityType(Type type)
        {
            return GetEntityType(type);
        }

        IEntityType IModel.FindEntityType(string name)
        {
            return FindEntityType(name);
        }

        IEntityType IModel.GetEntityType(string name)
        {
            return GetEntityType(name);
        }

        IReadOnlyList<IEntityType> IModel.EntityTypes
        {
            get { return EntityTypes; }
        }

        IEnumerable<IForeignKey> IModel.GetReferencingForeignKeys(IEntityType entityType)
        {
            return GetReferencingForeignKeys(entityType);
        }

        IEnumerable<IForeignKey> IModel.GetReferencingForeignKeys(IKey key)
        {
            return GetReferencingForeignKeys(key);
        }

        IEnumerable<IForeignKey> IModel.GetReferencingForeignKeys(IProperty property)
        {
            return GetReferencingForeignKeys(property);
        }
    }
}
