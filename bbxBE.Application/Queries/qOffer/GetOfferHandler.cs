﻿using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Application.Queries.qOffer
{
    public class GetOffer:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetOfferHandler : IRequestHandler<GetOffer, Entity>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public   GetOfferHandler(IOfferRepositoryAsync OfferRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _OfferRepository = OfferRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetOffer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entityOffers = await _OfferRepository.GetOfferAsync(validFilter);
            var data = entityOffers.MapItemFieldsByMapToAnnotation<GetOfferViewModel>();

            // response wrapper
            return data;
        }
    }
}