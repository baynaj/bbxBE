﻿using AsyncKeyedLock;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace bbxBE.Infrastructure.Persistence.Caches
{

    public class OriginCacheService : BaseCacheService<Origin>, ICacheService<Origin>
    {
        public OriginCacheService(ILogger logger, IConfiguration p_Configuration, ApplicationDbContext dbcontext, AsyncKeyedLocker<string> asyncKeyedLocker)
            : base(logger, p_Configuration, dbcontext, asyncKeyedLocker)
        {
            _cacheQuery = dbcontext.Origin
                .AsNoTracking()
                .AsExpandable();
        }
    }
}
