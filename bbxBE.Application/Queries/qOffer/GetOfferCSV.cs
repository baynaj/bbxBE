﻿using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.Application.Commands.cmdOffer
{
    public class GetOfferCSV : IRequest<FileStreamResult>
    {
        public long ID { get; set; }
     
    }

    public class GetOfferCSVHandler : IRequestHandler<GetOfferCSV, FileStreamResult>
    {
        private readonly IOfferRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;

        public GetOfferCSVHandler(IOfferRepositoryAsync offerRepository, IMapper mapper)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(GetOfferCSV request, CancellationToken cancellationToken)
        {

            var offer = await _offerRepository.GetOfferRecordAsync(request.ID, true);
            if (offer == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_OFFERNOTFOUND, request.ID));
            }

            string csv = String.Join(Environment.NewLine, offer.OfferLines.Select(x => x.ProductCode + ";" + x.UnitPrice.ToString().Replace(",", ".")).ToArray());
            Stream stream = Utils.StringToStream(csv);
            string fileName = $"Offer{offer.OfferNumber.Replace("/", "-")}.csv";


            var fsr = new FileStreamResult(stream, $"application/csv") { FileDownloadName = fileName };

            return fsr;
        }


        protected string loadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
