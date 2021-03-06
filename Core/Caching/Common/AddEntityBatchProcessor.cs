﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public class AddEntitisToAccountDbProcessor<TCacheObject, TEntity>
        where TEntity : class 
    {
        public void Add(Guid accountId, List<TCacheObject> cacheObjects, Func<TCacheObject, TEntity> getEntity)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }
            if (accountId == Guid.Empty)
            {
                throw new Exception("accountId == Guid.Empty");
            }
            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                foreach (var cacheObject in cacheObjects)
                {
                    var dbEntity = getEntity(cacheObject);
                    if (dbEntity == null)
                    {
                        throw new Exception("dbEntity == null");
                    }
                    var entry = accountDbContext.Entry(dbEntity);
                    entry.State = EntityState.Added;
                }
                accountDbContext.SaveChanges();
            }
        }

        public void Update(
            Guid accountId, 
            List<TCacheObject> cacheObjects, 
            Func<TCacheObject, TEntity> getCurrentEntity,
            Func<TCacheObject, TEntity> getLastSavedEntity)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }
            if (accountId == Guid.Empty)
            {
                throw new Exception("accountId == Guid.Empty");
            }
            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                foreach (var cacheObject in cacheObjects)
                {
                    var lastSavedEntity = getLastSavedEntity(cacheObject);
                    if (lastSavedEntity == null)
                    {
                        throw new Exception("lastSavedEntity == null");
                    }
                    var currentEntity = getCurrentEntity(cacheObject);
                    if (currentEntity == null)
                    {
                        throw new Exception("currentEntity == null");
                    }
                    var entry = accountDbContext.Entry(lastSavedEntity);
                    entry.CurrentValues.SetValues(currentEntity);
                }
                accountDbContext.SaveChanges();
            }
        }
    }
}
