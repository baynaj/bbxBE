﻿using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvCtrl
{


	[Description("Leltári tétel")]
	public class CreateInvCtrlCommand : IRequest<Response<InvCtrl>>
	{

		public class InvCtrlItem
		{


			[ColumnLabel("Raktár ID")]
			[Description("Raktár ID")]
			public long WarehouseID { get; set; }

			[ColumnLabel("Leltáridőszak ID")]
			[Description("Leltáridőszak ID")]
			public long? InvCtlPeriodID { get; set; }       //Opcionális, hogy a folyamatos leltárat is kezelni lehessen

			[ColumnLabel("Termék ID")]
			[Description("Termék ID")]
			public long ProductID { get; set; }

			[ColumnLabel("Leltározás dátuma")]
			[Description("Leltározás dátuma")]
			public DateTime InvCtrlDate { get; set; }

			[ColumnLabel("Új valós")]
			[Description("Új valós mennyiség")]
			public decimal NRealQty { get; set; }

			[ColumnLabel("Felhasználó ID")]
			[Description("Felhasználó ID")]
			public long? UserID { get; set; } = 0;

		}

		public List<InvCtrlItem> Items { get; set; } = new List<InvCtrlItem>();

	}

    public class CreateInvCtrlCommandHandler : IRequestHandler<CreateInvCtrlCommand, Response<InvCtrl>>
	{
		private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public CreateInvCtrlCommandHandler(IInvCtrlRepositoryAsync InvCtrlRepository,
						IMapper mapper, IConfiguration configuration)
		{
			_InvCtrlRepository = InvCtrlRepository;


			_mapper = mapper;
			_configuration = configuration;
		}

		public async Task<Response<InvCtrl>> Handle(CreateInvCtrlCommand request, CancellationToken cancellationToken)
		{
			var InvCtrlItems = new List<InvCtrl>();
			var tasks = request.Items.Select(async  i =>
				{
					var InvCtrl = _mapper.Map<InvCtrl>(i);
					var InvCtlOri = _InvCtrlRepository.AddInvCtrlAsync(InvCtrl)

					InvCtrlItems.Add(InvCtrl);
				}
			);

			var tasks = someList.Select(async item =>
			{
				item.someValue = await asdf.Where(() => SomeMethod(item)).FirstOrDefaultAsync();
			});
			await Task.WhenAll(tasks);

			await _InvCtrlRepository.AddInvCtrlAsync(InvCtrl);

			return new Response<InvCtrl>(InvCtrl);

		}
	}
}