using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Helpers
{
    public class DynamicExpressionVisitor : ExpressionVisitor
    {
        private readonly IModelConverterService _converterService;
        private readonly ITypeConverter _typeConverter;
        private readonly IIndex<GenericTypePair, IOptionTypeQueryProvider> _optionTypeQueryProviderIndex;

        public DynamicExpressionVisitor(IModelConverterService converterService, ITypeConverter typeConverter,
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
                object filterModel = Expression.Lambda(m.Arguments[1]).Compile().DynamicInvoke();
                bool objectTypeExists = false;
                int? idObjectType = null;
                if (m.Arguments.Count > 2)
                {
                    objectTypeExists = true;
                    idObjectType = (int?) Expression.Lambda(m.Arguments[2]).Compile().DynamicInvoke();
                }
                var filterModelType = m.Arguments[1].Type;
                var entityType = m.Arguments[0].Type;
                var itemType = entityType.TryGetElementType(typeof (ICollection<>));
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
                            ? queryBuilder.Filter(filterModel, filterModelType, m.Arguments[0], idObjectType)
                            : queryBuilder.Filter(filterModel, filterModelType, m.Arguments[0]);
                        if (resultExpression == null)
                            return Expression.Constant(true);
                        return resultExpression;
                    case "WhenValuesAny":
                        if (!collectionMethods)
                            throw new ApiException("You cannot use WhenAny with non-collection properties, see When");

                        resultExpression = objectTypeExists
                            ? queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], false, idObjectType)
                            : queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], false);

                        return resultExpression;
                    case "WhenValuesAll":
                        if (!collectionMethods)
                            throw new ApiException("You cannot use WhenAll with non-collection properties, see When");

                        resultExpression = objectTypeExists
                            ? queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], true, idObjectType)
                            : queryBuilder.FilterCollection(filterModel, filterModelType, m.Arguments[0], true);

                        return resultExpression;
                }
            }
            return base.VisitMethodCall(m);
        }
    }
}