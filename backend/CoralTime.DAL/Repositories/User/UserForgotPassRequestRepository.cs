﻿using System;
using System.Linq;
using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories.User
{
    public class UserForgotPassRequestRepository : GenericRepository<UserForgotPassRequest>
    {
        private readonly AppDbContext _context;

        public UserForgotPassRequestRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId)
        {
            _context = context;
        }

        public override IQueryable<UserForgotPassRequest> GetIncludes(IQueryable<UserForgotPassRequest> query) => query;

        public UserForgotPassRequest CreateUserForgotPassRequest(string email, int tokenExpirationHours, string token)
        {
            var requestId = Guid.NewGuid();
            var record = new UserForgotPassRequest
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(tokenExpirationHours),
                Email = email,
                UserForgotPassRequestUid = requestId,
                RefreshToken = token
            };

            Insert(record);
            _context.SaveChanges();

            return GetRequest(requestId.ToString());
        }


        public UserForgotPassRequest GetRequest(string requestId)
        {
            if (!Guid.TryParse(requestId, out Guid requestUid))
                return null;

            var currentDate = DateTime.Now;
            return GetQuery().FirstOrDefault(x =>
                x.UserForgotPassRequestUid.Equals(requestUid)
                && x.DateFrom <= currentDate
                && x.DateTo >= currentDate);
        }
    }
}