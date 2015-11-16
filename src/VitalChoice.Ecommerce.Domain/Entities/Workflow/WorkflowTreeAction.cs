namespace VitalChoice.Ecommerce.Domain.Entities.Workflow
{
    public class WorkflowTreeAction :Entity
    {
        public int IdTree { get; set; }

        public int IdExecutor { get; set; }

        public WorkflowExecutor Executor { get; set; }

        public WorkflowTree Tree { get; set; }
    }
}
