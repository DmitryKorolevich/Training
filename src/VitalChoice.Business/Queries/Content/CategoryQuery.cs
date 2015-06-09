﻿using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class CategoryQuery : QueryObject<ContentCategory>
    {
        public CategoryQuery WithType(ContentType? type=null)
        {
            if (type.HasValue)
            {
                Add(x => x.Type == type);
            }

            return this;
        }

        public CategoryQuery WithId(int id)
        {
            Add(x => x.Id ==id);

            return this;
        }

        public CategoryQuery WithParentId(int id)
        {
            Add(x => x.ParentId ==id);

            return this;
        }

        public CategoryQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public CategoryQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode ==status);
            }

            return this;
        }
    }
}