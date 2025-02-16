﻿using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvoice;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{

    public enum invoiceQueryTypes
    {
        small = 0,
        full = 1,
        NAV = 2
    }

    public interface IInvoiceRepositoryAsync : IGenericRepositoryAsync<Invoice>
    {
        Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Invoice> AddInvoiceAsync(Invoice p_invoice, Dictionary<long, InvoiceLine> p_RelDNInvoiceLines);
        Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice, ICollection<SummaryByVatRate> p_delSummaryByVatRates);
        Task<Entity> GetInvoiceAsync(long ID, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full);
        Task<Entity> GetInvoiceByInvoiceNumberAsync(string invoiceNumber, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full);
        Task<Entity> GetAggregateInvoiceAsync(long ID);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummaryAsync(bool incoming, long warehouseID);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesItemsAsync(bool incoming, long warehouseID, long customerID, string currencyCode);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesAsync(bool incoming, long warehouseID, string currencyCode);
        Task<decimal> GetUnPaidAmountAsyn(long customerID);
        Task<Invoice> GetInvoiceRecordAsync(long ID, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full);
        Task<Invoice> GetInvoiceRecordByInvoiceNumberAsync(string invoiceNumner, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full);

        Task<Dictionary<long, Invoice>> GetInvoiceRecordsByInvoiceLinesAsync(List<long> LstInvoiceLineID);
        Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceNumber(string invoiceNumber);
        Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceID(long invoiceID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount, decimal sumInvoiceNetAmountHUF, decimal sumInvoiceGrossAmountHUF)> QueryPagedInvoiceAsync(QueryInvoice requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount, decimal sumInvoiceNetAmountHUF, decimal sumInvoiceGrossAmountHUF)> QueryPagedUnpaidInvoiceAsync(QueryUnpaidInvoice requestParameter);
        Task<(List<Invoice> pagedItems, decimal sumInvoiceNetAmountHUF, decimal sumInvoiceGrossAmountHUF)> GetPagedUnpaidInvoiceRecordsAsync(QueryUnpaidInvoice requestParameter);

        Task<(IList<GetCustomerInvoiceSummary> data, RecordsCount recordsCount, decimal sumInvoiceNetAmountHUF, decimal sumInvoiceGrossAmountHUF)> QueryPagedCustomerInvoiceSummaryAsync(QueryCustomerInvoiceSummary requestParameters);
        Task<IList<GetInvoiceViewModel>> QueryForCSVInvoiceAsync(CSVInvoice requestParameter);

        Task<Invoice> CreateInvoiceAsynch(CreateInvoiceCommand request, CancellationToken cancellationToken);
        Task<Invoice> UpdatePricePreviewAsynch(UpdatePricePreviewCommand request, CancellationToken cancellationToken);

        public IList<string> Import(string CSVContent, string warehouseCode);
        Task<List<Invoice>> GetUnsentInvoiceRecodsAsync();
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUnsentInvoiceAsync(QueryUnsentInvoices requestParameter);



    }
}