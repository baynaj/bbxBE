﻿using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class NAVXResultRepositoryAsync : GenericRepositoryAsync<NAVXResult>, INAVXResultRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public NAVXResultRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }
    }
}