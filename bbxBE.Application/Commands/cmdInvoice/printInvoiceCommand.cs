﻿using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Commands.cmdImport;
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.Application.Commands.cmdInvoice
{
    public class PrintInvoiceCommand : IRequest<FileStreamResult>
    {
        public long ID { get; set; }

        public string baseURL;
    }

    public class PrintInvoiceCommandHandler : IRequestHandler<PrintInvoiceCommand, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;

        public PrintInvoiceCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintInvoiceCommand request, CancellationToken cancellationToken)
        {

            var reportTRDX = loadEmbeddedResource("bbxBE.Application.Reports.Invoice.trdx");

            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep = null;

            

            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (System.Xml.XmlReader xmlReader = XmlReader.Create(new StringReader(reportTRDX), settings))
            {
                ReportXmlSerializer xmlSerializer = new ReportXmlSerializer();
                rep = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                reportSource = new Telerik.Reporting.InstanceReportSource();

                reportSource.ReportDocument = rep;
            }

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("ID", request.ID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception("Invoice report result is null!");


            Stream stream = new MemoryStream(result.DocumentBytes);
            string fileName = "Invoice";

             
            var fsr = new FileStreamResult(stream, $"application/pdf") { FileDownloadName = fileName };

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
