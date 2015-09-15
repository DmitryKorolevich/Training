namespace VitalChoice.Domain.Entities.Options
{
    public struct AzureStorage
	{
        public string CustomerContainerName { get; set; }

        public string StorageConnectionString { get; set; }

        public string BugTicketFilesContainerName { get; set; }

        public string BugTicketCommentFilesContainerName { get; set; }
    }
}