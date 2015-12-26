using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Expressions
{
    internal class RelationalModel<TEntity>
        where TEntity: Entity
    {
        private readonly IEntityInfoStorage _entityInfo;

        public RelationalModel(IEntityInfoStorage entityInfo)
        {
            _entityInfo = entityInfo;
        }
    }
}