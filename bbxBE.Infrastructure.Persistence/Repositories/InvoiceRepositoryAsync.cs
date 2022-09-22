﻿using LinqKit;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System;
using AutoMapper;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using static Dapper.SqlMapper;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepositoryAsync : GenericRepositoryAsync<Invoice>, IInvoiceRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Invoice> _dataShaperInvoice;
        private IDataShapeHelper<GetInvoiceViewModel> _dataShaperGetInvoiceViewModel;
        private IDataShapeHelper<GetPendigDeliveryNotesSummaryModel> _dataShaperGetPendigDeliveryNotesSummaryModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockRepositoryAsync _StockRepository;
        public InvoiceRepositoryAsync(ApplicationDbContext dbContext,


        IDataShapeHelper<Invoice> dataShaperInvoice,
              IDataShapeHelper<GetInvoiceViewModel> dataShaperGetInvoiceViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IStockRepositoryAsync StockRepository
            ) : base(dbContext)
        {
            _dbContext = dbContext;

            _dataShaperInvoice = dataShaperInvoice;
            _dataShaperGetInvoiceViewModel = dataShaperGetInvoiceViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _StockRepository = StockRepository;
        }


        public async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null)
        {
            return !await _dbContext.Invoice.AnyAsync(p => p.InvoiceNumber == InvoiceNumber && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Invoice> AddInvoiceAsync(Invoice p_invoice)
        {

            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var stockList = await _StockRepository.MaintainStockByInvoiceAsync(p_invoice);

                    //c# how to disable save related entity in EF ???
                    //TODO: ideiglenes megoldás, relációban álló objektumok Detach-olása hogy ne akarja menteni azokat az EF 
                    if (p_invoice.Customer != null)
                        p_invoice.Customer = null;

                    if (p_invoice.Supplier != null)
                        p_invoice.Supplier = null;

                    foreach (var il in p_invoice.InvoiceLines)
                    {
                        il.Product = null;
                        il.VatRate = null;
                         

                    }

                    await _dbContext.Invoice.AddAsync(p_invoice);
                    await _dbContext.SaveChangesAsync();

                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return p_invoice;
        }

        public async Task<Invoice> InitInvoiceAsync(Invoice p_invoice)
        {
            throw new NotImplementedException("InitInvoiceAsync");
        }


        public async Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    _dbContext.Invoice.Update(p_invoice);
                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
                return p_invoice;
            }
        }

        public async Task<Entity> GetInvoiceAsync(long ID, bool FullData)
        {

            Invoice item = await GetInvoiceRecordAsync(ID, FullData);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Invoice, GetInvoiceViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<Invoice> GetInvoiceRecordAsync(long ID, bool FullData = true)
        {


            Invoice item;

            if (FullData)
            {
                item = await _dbContext.Invoice.AsNoTracking()
                  .Include(w => w.Warehouse).AsNoTracking()
                  .Include(s => s.Supplier).AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                  .Include(i => i.InvoiceLines).ThenInclude(t => t.VatRate).AsNoTracking()
                  .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking()
                  .Where(x => x.ID == ID).FirstOrDefaultAsync();
            }
            else
            {
                item = await _dbContext.Invoice.AsNoTracking()
                  .Include(w => w.Warehouse).AsNoTracking()
                  .Include(s => s.Supplier).AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                  .Where(x => x.ID == ID).FirstOrDefaultAsync();
            }
            return item;
        }

        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummareAsync(bool incoming, long warehouseID, string currencyCode)
        {


            var lstEntities = new List<GetPendigDeliveryNotesSummaryModel>();

            if(incoming)
            {
                lstEntities = await _dbContext.InvoiceLine.AsNoTracking()
                 .Include(i => i.Invoice).ThenInclude(t => t.Supplier)
                 .Where(w => w.PendingDNQuantity > 0 && w.Invoice.Incoming == incoming && w.Invoice.WarehouseID == warehouseID && w.Invoice.CurrencyCode == currencyCode)
                .GroupBy(g => g.Invoice.SupplierID)
                       .Select(g => new GetPendigDeliveryNotesSummaryModel()
                       {

                           WarehouseID = warehouseID,
                           CustomerID = g.FirstOrDefault().Invoice.Supplier.ID,
                           Customer = g.FirstOrDefault().Invoice.Supplier.CustomerName,
                           SumNetAmount = g.Sum(s => s.LineNetAmount)
                       }
                       ).ToListAsync();
            }
            else
            {
                lstEntities = await _dbContext.InvoiceLine.AsNoTracking()
                 .Include(i => i.Invoice).ThenInclude(t => t.Customer)
                 .Where(w => w.PendingDNQuantity > 0 && w.Invoice.Incoming == incoming && w.Invoice.WarehouseID == warehouseID && w.Invoice.CurrencyCode == currencyCode)
                .GroupBy(g => g.Invoice.CustomerID)
                       .Select(g => new GetPendigDeliveryNotesSummaryModel()
                       {

                           WarehouseID = warehouseID,
                           CustomerID = g.FirstOrDefault().Invoice.CustomerID,
                           Customer = g.FirstOrDefault().Invoice.Customer.CustomerName,
                           SumNetAmount = g.Sum(s => s.LineNetAmount)
                       }
                       ).ToListAsync();
            }
            /* ????
            var queryModel = await _dbContext.InvoiceLine.AsNoTracking()
              .Include(i => i.Invoice).ThenInclude(t => (incoming ? t.Supplier : t.Customer)).AsNoTracking()
              .Where(w => w.PendingDNQuantity > 0 && w.Invoice.Incoming == incoming && w.Invoice.WarehouseID == warehouseID && w.Invoice.CurrencyCode == currencyCode)
              .GroupBy(g => (incoming ? g.Invoice.SupplierID : g.Invoice.CustomerID))
                        .Select(g => new GetPendigDeliveryNotesSummaryModel()
                        {

                            WarehouseID = warehouseID,
                            CustomerID = g.Key,
                            Customer = (incoming ? g.FirstOrDefault().Invoice.Supplier.CustomerName : g.FirstOrDefault().Invoice.Customer.CustomerName),
                            SumNetAmount = g.Sum(s => s.LineNetAmount)
                        }
                        ).ToListAsync();
            */

            var shapeData = _dataShaperGetPendigDeliveryNotesSummaryModel.ShapeData(lstEntities, "");

            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameter)
        {


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            //var query = _dbContext.Invoice//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();

            IQueryable<Invoice> query;
            if (requestParameter.FullData)
            {
                query = _dbContext.Invoice.AsNoTracking()
                 .Include(w => w.Warehouse).AsNoTracking()
                 .Include(s => s.Supplier).AsNoTracking()
                 .Include(c => c.Customer).AsNoTracking()
                 .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                 .Include(i => i.InvoiceLines).ThenInclude(t => t.VatRate).AsNoTracking()
                 .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking();
            }
            else
            {
                query = _dbContext.Invoice.AsNoTracking()
                 .Include(w => w.Warehouse).AsNoTracking()
                 .Include(s => s.Supplier).AsNoTracking()
                 .Include(c => c.Customer).AsNoTracking()
                 .Include(a => a.AdditionalInvoiceData).AsNoTracking();

            }


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            
            FilterBy(ref query, requestParameter.Incoming, requestParameter.WarehouseCode, requestParameter.InvoiceNumber,
                    requestParameter.InvoiceIssueDateFrom, requestParameter.InvoiceIssueDateTo,
                    requestParameter.InvoiceDeliveryDateFrom, requestParameter.InvoiceDeliveryDateTo);
            

            // Count records after filter
            recordsFiltered = await query.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = query.OrderBy(orderBy);
            }

            // select columns
            /*
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<Invoice>("new(" + fields + ")");
            }
            */

            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await query.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvoiceViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Invoice, GetInvoiceViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBy(ref IQueryable<Invoice> p_items, bool Incoming, string WarehouseCode, string InvoiceNumber,
                                DateTime? InvoiceIssueDateFrom, DateTime? InvoiceIssueDateTo,
                                DateTime? InvoiceDeliveryDateFrom, DateTime? InvoiceDeliveryDateTo)
        {
            if (!p_items.Any())
                return;

            /*
            if (string.IsNullOrWhiteSpace(WarehouseCode) && string.IsNullOrWhiteSpace(InvoiceNumber) &&
                        !InvoiceIssueDateFrom.HasValue && !InvoiceIssueDateTo.HasValue &&
                        !InvoiceDeliveryDateFrom.HasValue && !InvoiceDeliveryDateTo.HasValue)
                return;
            */
            var predicate = PredicateBuilder.New<Invoice>();

            predicate = predicate.And(p => p.Incoming == Incoming
                            && (WarehouseCode == null || p.Warehouse.WarehouseCode.ToUpper().Contains(WarehouseCode))
                            && (InvoiceNumber == null || p.InvoiceNumber.Contains(InvoiceNumber))
                            && ( !InvoiceIssueDateFrom.HasValue || p.InvoiceIssueDate >= InvoiceIssueDateFrom.Value)
                            && ( !InvoiceIssueDateTo.HasValue || p.InvoiceIssueDate <= InvoiceIssueDateTo.Value)
                            && ( !InvoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= InvoiceDeliveryDateFrom.Value)
                            && (!InvoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= InvoiceDeliveryDateTo.Value)
                           );

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}