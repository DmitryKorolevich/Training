﻿using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class MasterContentItemQuery : QueryObject<MasterContentItem>
    {
        public MasterContentItemQuery WithType(ContentType? type=null)
        {
            if (type.HasValue)
            {
                Add(x => x.TypeId == (int)type);
            }

            return this;
        }

        public MasterContentItemQuery WithId(int id)
        {
            Add(x => x.Id==id);

            return this;
        }

        public MasterContentItemQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public MasterContentItemQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode == status);
            }

            return this;
        }
    }
}