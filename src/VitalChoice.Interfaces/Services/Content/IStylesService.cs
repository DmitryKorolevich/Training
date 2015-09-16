using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services.Content
{
    public interface IStylesService
	{
	    string GetStyles();

	    Task<string> UpdateStylesAsync(string css);
    }
}
