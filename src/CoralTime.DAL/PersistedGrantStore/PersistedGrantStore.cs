﻿using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoralTime.DAL.PersistedGrantStore
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly ILogger _logger;
        private readonly AppDbContext _appContext;

        public PersistedGrantStore(IPersistedGrantDbContext context, AppDbContext appContext, ILogger<PersistedGrantStore> logger)
        {
            _context = context;
            _logger = logger;
            _appContext = appContext;
        }

        public Task StoreAsync(PersistedGrant token)
        {
            var existing = _context.PersistedGrants.SingleOrDefault(x => x.Key == token.Key);
            if (existing == null)
            {
                _logger.LogDebug("{persistedGrantKey} not found in database", token.Key);

                var persistedGrant = token.ToEntity();
                _context.PersistedGrants.Add(persistedGrant);
            }
            else
            {
                _logger.LogDebug("{persistedGrantKey} found in database", token.Key);

                token.UpdateEntity(existing);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning("exception updating {persistedGrantKey} persisted grant in database: {error}", token.Key, ex.Message);
            }

            return Task.FromResult(0);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = _context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            var model = persistedGrant?.ToModel();

            if (model != null)
            {
                var user = _appContext.Users.FirstOrDefault(x => x.Id == model.SubjectId);
                if (!user.IsActive)
                {
                    RemoveAllAsync(model.SubjectId, model.ClientId);
                    _logger.LogDebug($"{key} found in database: {model != null}. User is deactivated and all his keys have been cleared.");
                    return Task.FromResult<PersistedGrant>(null);
                }
            }

            _logger.LogDebug($"{key} found in database: {model != null}");

            return Task.FromResult(model);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrants = _context.PersistedGrants.Where(x => x.SubjectId == subjectId).ToList();
            var model = persistedGrants.Select(x => x.ToModel());

            _logger.LogDebug("{persistedGrantCount} persisted grants found for {subjectId}", persistedGrants.Count, subjectId);

            return Task.FromResult(model);
        }

        public Task RemoveAsync(string key)
        {
            var persistedGrant = _context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            if (persistedGrant != null)
            {
                _logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);

                _context.PersistedGrants.Remove(persistedGrant);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogInformation("exception removing {persistedGrantKey} persisted grant from database: {error}", key, ex.Message);
                }
            }
            else
            {
                _logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
            }

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = _context.PersistedGrants.Where(x => x.SubjectId == subjectId && x.ClientId == clientId).ToList();

            _logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}", persistedGrants.Count, subjectId, clientId);

            _context.PersistedGrants.RemoveRange(persistedGrants);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogInformation("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}: {error}", persistedGrants.Count, subjectId, clientId, ex.Message);
            }

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = _context.PersistedGrants.Where(x =>
                x.SubjectId == subjectId &&
                x.ClientId == clientId &&
                x.Type == type).ToList();

            _logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}", persistedGrants.Count, subjectId, clientId, type);

            _context.PersistedGrants.RemoveRange(persistedGrants);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogInformation("exception removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}: {error}", persistedGrants.Count, subjectId, clientId, type, ex.Message);
            }

            return Task.FromResult(0);
        }
    }
}