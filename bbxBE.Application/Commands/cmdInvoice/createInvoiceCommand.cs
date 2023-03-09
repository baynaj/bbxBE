﻿using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
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
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.NAV;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;

namespace bxBE.Application.Commands.cmdInvoice
{
	public class CreateInvoiceCommand : IRequest<Response<Invoice>>
	{

		[Description("Számlasor")]
		public class InvoiceLine
		{

			[ColumnLabel("#")]
			[Description("Sor száma")]
			public short LineNumber { get; set; }

			[ColumnLabel("Termékkód")]
			[Description("Termékkód")]
			public string ProductCode { get; set; }

			[ColumnLabel("Áfakód")]
			[Description("Áfakód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Mennyiség")]
			[Description("Mennyiség")]
			public decimal Quantity { get; set; }

			[ColumnLabel("Me.e.")]
			[Description("Mennyiségi egység kód")]
			public string UnitOfMeasure { get; set; }
			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

			//Gyűjtőszámla - szállítólvél kapcsolat

			[ColumnLabel("Szállítólevél sor")]
			[Description("Kapcsolt szállítólevél sor")]
			public long? RelDeliveryNoteInvoiceLineID { get; set; }


		}

		[ColumnLabel("B/K")]
		[Description("Bejővő/Kimenő")]
		public bool Incoming { get; set; }

		[ColumnLabel("Típus")]
		[Description("Típus")]
		public string InvoiceType { get; set; }

		[ColumnLabel("Raktár")]
		[Description("Raktár")]
		public string WarehouseCode { get; set; }

		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime InvoiceIssueDate { get; set; }

		[ColumnLabel("Teljesítés")]
		[Description("Teljesítés dátuma")]
		public DateTime InvoiceDeliveryDate { get; set; }

		[ColumnLabel("Fiz.hat")]
		[Description("Fizetési határidő dátuma")]
		public DateTime PaymentDate { get; set; }


		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }

		[ColumnLabel("Kapcsolódó számla")]
		[Description("Bevételhez kapcsolódó számla")]
		public string CustomerInvoiceNumber { get; set; }

		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód")]
		public string PaymentMethod { get; set; }

		[ColumnLabel("Pénznem")]
		[Description("Pénznem")]
		public string CurrencyCode { get; set; }

		[ColumnLabel("Árfolyam")]
		[Description("Árfolyam")]
		public decimal ExchangeRate { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }  //AdditionalInvoiceData-ban tároljuk!

		[ColumnLabel("Kedvezmény%")]
		[Description("A számlára adott teljes kedvezmény %")]
		public decimal InvoiceDiscountPercent { get; set; }

		[ColumnLabel("Felhasználó ID")]
		[Description("Felhasználó ID")]
		public long? UserID { get; set; } = 0;

        [ColumnLabel("Munkaszám")]
        [Description("Munkaszám")]
        public string WorkNumber { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat jelölése")]
        public bool? PriceReview { get; set; } = false;

		[ColumnLabel("Módosító bizonylat?")]
		[Description("Módosító bizonylat jelölése")]
		public bool? Correction { get; set; } = false;

		[ColumnLabel("Típus")]
		[Description("Típus")]
		public string InvoiceCategory { get; set; } = enInvoiceCategory.NORMAL.ToString();


		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

	}

	public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<Invoice>>
	{
		private readonly IInvoiceRepositoryAsync _InvoiceRepository;
		private readonly IInvoiceLineRepositoryAsync _InvoiceLineRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly IWarehouseRepositoryAsync _WarehouseRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IVatRateRepositoryAsync _VatRateRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public CreateInvoiceCommandHandler(
			IInvoiceRepositoryAsync InvoiceRepository,
			IInvoiceLineRepositoryAsync InvoiceLineRepository,
			ICounterRepositoryAsync CounterRepository,
			IWarehouseRepositoryAsync WarehouseRepository,
			ICustomerRepositoryAsync CustomerRepository,
			IProductRepositoryAsync ProductRepository,
			IVatRateRepositoryAsync VatRateRepository,
			IMapper mapper, IConfiguration configuration)
		{
			_InvoiceRepository = InvoiceRepository;
			_InvoiceLineRepository = InvoiceLineRepository;
			_CounterRepository = CounterRepository;
			_WarehouseRepository = WarehouseRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_VatRateRepository = VatRateRepository;
			_mapper = mapper;
			_configuration = configuration;
		}
        /*
{
  "customerID": 206568,
  "invoiceDeliveryDate": "2023-01-30",
  "invoiceIssueDate": "2023-01-30",
  "invoiceLines": [
    {
      "lineNetAmount": 23296,
      "lineNumber": 1,
      "quantity": 1,
      "productCode": "SCH-004600100",
      "productDescription": "Fali modul STR100",
      "unitOfMeasure": "PIECE",
      "unitPrice": 23296,
      "vatRate": 0.27,
      "vatRateCode": "27%"
    },
    {
      "lineNetAmount": 440,
      "lineNumber": 2,
      "quantity": 10,
      "productCode": "001-TESZTÚJ",
      "productDescription": "Új tesztadat neki nincs DÜW engedménye",
      "unitOfMeasure": "PIECE",
      "unitPrice": 44,
      "vatRate": 0.27,
      "vatRateCode": "27%"
    }
  ],
  "notice": "megjegyzés szöveg",
  "paymentDate": "2023-01-30",
  "paymentMethod": "OTHER",
  "warehouseCode": "001",
  "currencyCode": "HUF",
  "exchangeRate": 1,
  "incoming": false,
  "invoiceType": "DNO",
  "invoiceDiscountPercent": 10,
  "workNumber": "workNumber #1",
  "priceReview": true
}st
		 */


        public async Task<Response<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
		{

			var inv = await bllInvoice.CreateInvoiceAsynch(request, _mapper,
									_InvoiceRepository,
									_InvoiceLineRepository,
									_CounterRepository,
									_WarehouseRepository,
									_CustomerRepository,
									_ProductRepository,
									_VatRateRepository,
									cancellationToken);
			return new Response<Invoice>(inv);
		}


	}
}