using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Tests.Mapping.Dynamics;
using VitalChoice.Tests.Mapping.Entities;

namespace VitalChoice.Tests.Mapping
{
    public class TestMapper : DynamicObjectMapper<TestDynamic, TestEntity, TestEntityOptionType, TestEntityOptionValue>
    {
        public TestMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> converters, IReadRepositoryAsync<TestEntityOptionType> optionTypeRepositoryAsync) : base(mappers, converters, optionTypeRepositoryAsync)
        {
        }

        public override IQueryObject<TestEntityOptionType> GetOptionTypeQuery(int? idType)
        {
            throw new NotImplementedException();
        }

        protected override void FromEntityInternal(TestDynamic dynamic, TestEntity entity, bool withDefaults = false)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntityInternal(TestDynamic dynamic, TestEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override void ToEntityInternal(TestDynamic dynamic, TestEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
