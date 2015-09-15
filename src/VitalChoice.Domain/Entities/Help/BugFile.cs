﻿using System;

namespace VitalChoice.Domain.Entities.Help
{
    public class BugFile : Entity
	{
	    public int? IdBugTicket { get; set; }

        public int? IdBugTicketComment { get; set; }

        public DateTime UploadDate { get; set; }

	    public string FileName { get; set; }

	    public string Description { get; set; }
	}
}
