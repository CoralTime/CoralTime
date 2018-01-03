using CoralTime.DAL.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public partial class UnitOfWork
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly string _userId;

        //  private static Logger _logger = LogManager.GetCurrentClassLogger();

        public UnitOfWork(UserManager<ApplicationUser> userManager, AppDbContext context, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _memoryCache = memoryCache;
            _userId = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        //public int Save<T>()
        //{
        //    return _context.SaveChanges();
        //}

        //private bool _disposed;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            _context.Dispose();
        //        }
        //    }
        //    _disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}