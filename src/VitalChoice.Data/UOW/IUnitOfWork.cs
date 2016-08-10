﻿using System;
using System.Data;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Data.UOW
{
    public interface IUnitOfWork: IDisposable
    {
        int SaveChanges();
        IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted);
    }
}