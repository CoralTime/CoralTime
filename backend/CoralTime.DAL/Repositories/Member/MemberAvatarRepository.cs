using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class MemberAvatarRepository: _BaseRepository<MemberAvatar>
    {
        public MemberAvatarRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        private static readonly object LockObject = new object();

        public static Guid PicturesCacheGuid { get; set; }

        public override IQueryable<MemberAvatar> GetIncludes(IQueryable<MemberAvatar> query)
        {
            return query.Include(x => x.Member);
        }
        
        public override List<MemberAvatar> LinkedCacheGetList()
        {
            try
            {
                var key = GenerateCacheKey();
                var itemsMemberAvatar = CacheManager.CachedListGet<MemberAvatar>(key);

                if (itemsMemberAvatar != null) 
                    return itemsMemberAvatar;
                
                lock (LockObject)
                {
                    var cachedItems = CacheManager.CachedListGet<MemberAvatar>(key);
                    if (cachedItems != null)
                        return cachedItems;
                    
                    PicturesCacheGuid = Guid.NewGuid();

                    itemsMemberAvatar = GetQueryWithIncludes().ToList();
                    CacheManager.LinkedPutList(key, itemsMemberAvatar);
                }

                return itemsMemberAvatar;
            }
            catch (Exception seq)
            {
                throw new CoralTimeDangerException(seq.Message);
            }
        }
    }
}
