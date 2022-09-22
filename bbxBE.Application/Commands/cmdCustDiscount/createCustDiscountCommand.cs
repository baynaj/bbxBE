﻿using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCustDiscount
{
    public class CreateCustDiscountCommand : IRequest<Response<List<CustDiscount>>>
    {
        public class CustDiscountItem
        {
            [ColumnLabel("Ügyfél ID")]
            [Description("Ügyfél ID")]
            public long CustomerID { get; set; }

            [ColumnLabel("Termékcsoport ID")]
            [Description("Termékcsoport ID")]
            public long ProductGroupID { get; set; }

            [ColumnLabel("Árengedmény %")]
            [Description("Árengedmény %)")]
            public decimal Discount { get; set; }
        }
        public List<CustDiscountItem> Items { get; set; } = new List<CustDiscountItem>();
    }

    public class CreateCustDiscountCommandHandler : IRequestHandler<CreateCustDiscountCommand, Response<List<CustDiscount>>>
    {
        private readonly ICustDiscountRepositoryAsync _CustDiscountRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateCustDiscountCommandHandler(ICustDiscountRepositoryAsync CustDiscountRepository, IMapper mapper, IConfiguration configuration)
        {
            _CustDiscountRepository = CustDiscountRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<List<CustDiscount>>> Handle(CreateCustDiscountCommand request, CancellationToken cancellationToken)
        {
            var CustDiscountItems = new List<CustDiscount>();
            request.Items.ForEach(i =>
            {
                var CustDiscount= _mapper.Map<CustDiscount>(i);
                CustDiscountItems.Add(CustDiscount);
            }
            );
            var res = await _CustDiscountRepository.AddCustDiscountRangeAsync(CustDiscountItems);
            return new Response<List<CustDiscount>>(CustDiscountItems);
        }


    }
}
