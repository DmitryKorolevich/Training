namespace VitalChoice.Ecommerce.Domain.Entities.Workflow
{
    public class WorkflowActionAggregation : Entity
    {
        public int IdParent { get; set; }

        public WorkflowExecutor Parent { get; set; }

        public int IdAggregate { get; set; }

        public WorkflowExecutor ToAggregate { get; set; }
    }

    public class WorkflowActionDependency : Entity
    {
        public int IdParent { get; set; }

        public WorkflowExecutor Parent { get; set; }

        public int IdDependent { get; set; }

        public WorkflowExecutor Dependent { get; set; }
    }
}
