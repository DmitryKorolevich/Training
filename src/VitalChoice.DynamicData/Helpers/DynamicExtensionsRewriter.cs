using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autofac.Features.Indexed;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.DynamicData.Helpers
{
    public class DynamicExtensionsRewriter : ExpressionVisitor
    {
        private readonly IModelConverterService _converterService;
        private readonly ITypeConverter _typeConverter;
        private readonly IIndex<GenericTypePair, IOptionTypeQueryProvider> _optionTypeQueryProviderIndex;

        public DynamicExtensionsRewriter(IModelConverterService converterService, ITypeConverter typeConverter,
            IIndex<GenericTypePair, IOptionTypeQueryProvider> optionTypeQueryProviderIndex)
        {
            _converterService = converterService;
            _typeConverter = typeConverter;
            _optionTypeQueryProviderIndex = optionTypeQueryProviderIndex;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof (DynamicEntityFilterExtension))
            {
                object filterModel = m.Arguments[1].GetValue();
                bool objectTypeExists = false;
                int? idObjectType = null;
                ValuesFilterType filterType;
                CompareBehaviour compareBehaviour;
                if (m.Arguments.Count > 4)
                {
                    objectTypeExists = true;
                    idObjectType = (int?) m.Arguments[2].GetValue();
                    filterType = (ValuesFilterType)m.Arguments[3].GetValue();
                    compareBehaviour = (CompareBehaviour) m.Arguments[4].GetValue();
                }
                else
                {
                    filterType = (ValuesFilterType)m.Arguments[2].GetValue();
                    compareBehaviour = (CompareBehaviour) m.Arguments[3].GetValue();
                }
                var filterModelType = m.Arguments[1].Type;
                var entityType = m.Arguments[0].Type;
                var itemType = entityType.TryGetElementType(typeof (IEnumerable<>));
                bool collectionMethods = false;
                if (itemType != null)
                {
                    entityType = itemType;
                    collectionMethods = true;
                }
                var entityTypeParameters = entityType.TryGetTypeArguments(typeof (DynamicDataEntity<,>));

                IDynamicDataEntityQueryBuilder queryBuilder =
                    (IDynamicDataEntityQueryBuilder)
                        Activator.CreateInstance(typeof (DynamicDataEntityQueryBuilder<,,>).MakeGenericType(entityType,
                            entityTypeParameters[0], entityTypeParameters[1]), _converterService, _typeConverter,
                            _optionTypeQueryProviderIndex);

                Expression resultExpression;
                switch (m.Method.Name)
                {
                    case "WhenValues":
                        if (collectionMethods)
                            throw new ApiException("You cannot use When with collection properties, see Any/All");


                        resultExpression = objectTypeExists
                            ? queryBuilder.Filter(filterModel, filterModelType, m.Arguments[0], filterType, idObjectType, compareBehaviour)
                            : queryBuilder.Filter(filterModel, filterModelType, m.Arguments[0], filterType, compareBehaviour);
                        if (resultExpression == null)
                            return Expression.Constant(true);
                        return Visit(resultExpression);
                    case "WhenValuesAny":
                        if (!collectionMethods)
                            throw new ApiException("You cannot use WhenAny with non-collection properties, see When");

                        resultExpression = objectTypeExists
                            ? queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], filterType, false, idObjectType, compareBehaviour)
                            : queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], filterType, false, compareBehaviour);

                        return Visit(resultExpression);
                    case "WhenValuesAll":
                        if (!collectionMethods)
                            throw new ApiException("You cannot use WhenAll with non-collection properties, see When");

                        resultExpression = objectTypeExists
                            ? queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], filterType, true, idObjectType, compareBehaviour)
                            : queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], filterType, true, compareBehaviour);

                        return Visit(resultExpression);
                }
            }
            return base.VisitMethodCall(m);
        }
    }
}