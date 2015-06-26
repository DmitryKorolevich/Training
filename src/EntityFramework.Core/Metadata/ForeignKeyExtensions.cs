// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata
{
    public static class ForeignKeyExtensions
    {
        public static Navigation GetNavigationToPrincipal([NotNull] this ForeignKey foreignKey)
        {
            return (Navigation)((IForeignKey)foreignKey).GetNavigationToPrincipal();
        }

        public static INavigation GetNavigationToPrincipal([NotNull] this IForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return foreignKey.EntityType.GetNavigations().SingleOrDefault(
                navigation => navigation.ForeignKey == foreignKey && navigation.PointsToPrincipal);
        }

        public static Navigation GetNavigationToDependent([NotNull] this ForeignKey foreignKey)
        {
            return (Navigation)((IForeignKey)foreignKey).GetNavigationToDependent();
        }

        public static INavigation GetNavigationToDependent([NotNull] this IForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return foreignKey.PrincipalEntityType.GetNavigations().SingleOrDefault(
                navigation => navigation.ForeignKey == foreignKey && !navigation.PointsToPrincipal);
        }

        public static bool IsSelfReferencing(
            [NotNull] this ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return foreignKey.EntityType == foreignKey.PrincipalEntityType;
        }

        public static bool IsCompatible(
            [NotNull] this ForeignKey foreignKey,
            [NotNull] EntityType principalType,
            [NotNull] EntityType dependentType,
            bool? isUnique)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));
            Check.NotNull(principalType, nameof(principalType));
            Check.NotNull(dependentType, nameof(dependentType));

            return (isUnique == null || ((IForeignKey)foreignKey).IsUnique == isUnique)
                   && foreignKey.PrincipalEntityType == principalType
                   && foreignKey.EntityType == dependentType;
        }

        public static bool IsCompatible(
            [NotNull] this ForeignKey foreignKey,
            [NotNull] EntityType principalType,
            [NotNull] EntityType dependentType,
            [CanBeNull] IReadOnlyList<Property> foreignKeyProperties,
            [CanBeNull] IReadOnlyList<Property> principalProperties,
            bool? isUnique)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));
            Check.NotNull(principalType, nameof(principalType));
            Check.NotNull(dependentType, nameof(dependentType));

            return foreignKey.IsCompatible(principalType, dependentType, isUnique)
                   && (foreignKeyProperties == null
                       || !foreignKeyProperties.Any()
                       || foreignKey.Properties.SequenceEqual(foreignKeyProperties))
                   && (principalProperties == null
                       || !principalProperties.Any()
                       || foreignKey.PrincipalKey.Properties.SequenceEqual(principalProperties));
        }

        public static bool IsCompatible(
            [NotNull] this ForeignKey foreignKey,
            [NotNull] EntityType principalType,
            [NotNull] EntityType dependentType,
            [CanBeNull] string navigationToPrincipal,
            [CanBeNull] string navigationToDependent,
            [CanBeNull] IReadOnlyList<Property> foreignKeyProperties,
            [CanBeNull] IReadOnlyList<Property> principalProperties,
            bool? isUnique)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));
            Check.NotNull(principalType, nameof(principalType));
            Check.NotNull(dependentType, nameof(dependentType));

            var existingNavigationToPrincipal = foreignKey.GetNavigationToPrincipal();
            var existingNavigationToDependent = foreignKey.GetNavigationToDependent();
            return foreignKey.IsCompatible(principalType, dependentType, foreignKeyProperties, principalProperties, isUnique)
                   && (existingNavigationToPrincipal == null || existingNavigationToPrincipal.Name == navigationToPrincipal)
                   && (existingNavigationToDependent == null || existingNavigationToDependent.Name == navigationToDependent);
        }
    }
}
