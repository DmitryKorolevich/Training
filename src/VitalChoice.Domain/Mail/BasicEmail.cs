﻿using System;

namespace VitalChoice.Domain.Mail
{
    public class BasicEmail
    {
	    public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
	    public string Body { get; set; }
        public bool IsHTML { get; set; }
    }
}