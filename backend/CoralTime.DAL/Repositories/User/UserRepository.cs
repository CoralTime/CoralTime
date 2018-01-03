using System;
using System.Collections.Generic;
using CoralTime.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using CoralTime.Common.Exceptions;

namespace CoralTime.DAL.Repositories
{
    public class UserRepository : _BaseRepository<ApplicationUser>
    {

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, IMemoryCache memoryCache, string userId) : base(context, memoryCache, userId)
        {}

        public override ApplicationUser LinkedCacheGetByName(string name)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.UserName == name);
        }
    }
}