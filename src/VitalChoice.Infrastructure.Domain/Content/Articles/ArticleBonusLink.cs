using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Articles
{
    public class ArticleBonusLink : Entity
    {
        public string Url { get; set; }

        public DateTime StartDate { get; set; }
    }
}