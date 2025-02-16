﻿using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using EFCore.BulkExtensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustomerRepositoryAsync : GenericRepositoryAsync<Customer>, ICustomerRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Customer> _dataShaperCustomer;
        private IDataShapeHelper<GetCustomerViewModel> _dataShaperGetCustomerViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        private readonly ICacheService<Customer> _cacheService;

        public CustomerRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData, IExpiringData<ExpiringDataObject> expiringData,
            ICacheService<Customer> customerCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperCustomer = new DataShapeHelper<Customer>();
            _dataShaperGetCustomerViewModel = new DataShapeHelper<GetCustomerViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _expiringData = expiringData;
            _cacheService = customerCacheService;
        }


        public bool IsUniqueTaxpayerId(string TaxpayerId, long? ID = null)
        {
            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.TaxpayerId == TaxpayerId && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public bool IsUniqueIsOwnData(long? ID = null)
        {
            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.IsOwnData && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public enValidateBankAccountResult CheckCustomerBankAccount(string bankAccountNumber)
        {
            //Először megnézzük a rendes bankszámlaszám formátumot
            var res = bllCustomer.ValidateBankAccount(bankAccountNumber);
            if (res == enValidateBankAccountResult.OK || res == enValidateBankAccountResult.ERR_EMPTY)
                return res;
            //Ha az hibás, akkor talán IBAN?
            return bllCustomer.ValidateIBAN(bankAccountNumber);
        }

        public bool CheckCountryCode(string countrycode)
        {
            return bllCustomer.ValidateCountryCode(countrycode);
        }
        public bool CheckUnitPriceType(string unitPriceType)
        {
            return bllCustomer.ValidateUnitPriceType(unitPriceType);
        }

        public bool CheckTaxPayerNumber(string taxPayerNumber)
        {
            if (string.IsNullOrWhiteSpace(taxPayerNumber.Replace("-", "")))
                return true;

            return bllCustomer.ValidateTaxPayerNumber(taxPayerNumber);
        }

        public async Task<Customer> AddCustomerAsync(Customer p_customer)
        {

            await AddAsync(p_customer);
            _cacheService.AddOrUpdate(p_customer);
            return p_customer;
        }
        public async Task<int> AddCustomerRangeAsync(List<Customer> p_customerList)
        {

            _dbContext.Instance.Database.SetCommandTimeout(3600);
            await _dbContext.Instance.BulkInsertAsync(p_customerList, new BulkConfig
            {
                SetOutputIdentity = true,
                PreserveInsertOrder = true,
                BulkCopyTimeout = 0,
                WithHoldlock = false,
                BatchSize = 5000
            });
            await _dbContext.SaveChangesAsync();

            return p_customerList.Count;

        }

        public async Task<Customer> UpdateCustomerAsync(Customer p_customer)
        {
            _cacheService.AddOrUpdate(p_customer);
            await UpdateAsync(p_customer);

            //szemafr kiütések
            var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + p_customer.ID.ToString();
            await _expiringData.DeleteItemAsync(key);

            return p_customer;
        }

        public async Task<Customer> UpdateCustomerLatestDiscountPercentAsync(long ID, decimal LatestDiscountPercent)
        {

            Customer cust = null;
            if (!_cacheService.TryGetValue(ID, out cust))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTNOTFOUND, ID));

            cust.LatestDiscountPercent = LatestDiscountPercent;
            _cacheService.AddOrUpdate(cust);
            await UpdateAsync(cust);
            return cust;
        }

        public async Task<Customer> DeleteCustomerAsync(long ID)
        {

            Customer cust = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                cust = await _dbContext.Customer.Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (cust != null)
                {

                    _cacheService.TryRemove(cust);
                    await RemoveAsync(cust);
                    await dbContextTransaction.CommitAsync();
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTNOTFOUND, ID));
                }
            }
            return cust;
        }
        public Customer GetOwnData()
        {
            var query = _cacheService.QueryCache();
            return query.SingleOrDefault(s => s.IsOwnData);
        }

        public Customer GetCustomerRecord(long customerID)
        {
            Customer cust = null;
            if (!_cacheService.TryGetValue(customerID, out cust))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTNOTFOUND, customerID));
            return cust;
        }

        public Customer GetCustomerRecordByTaxpayerId(string taxpaxpayerID)
        {
            var query = _cacheService.QueryCache();
            return query.FirstOrDefault(p => !p.Deleted && p.TaxpayerId != null && p.TaxpayerId == taxpaxpayerID);
        }


        public List<Customer> GetCustomerRecordsByName(string name)
        {

            var query = _cacheService.QueryCache();
            var res = query.Where(w => !w.Deleted && w.CustomerName != null && w.CustomerName.ToUpper().Contains(name.ToUpper().Trim()));
            return res.ToList();

        }

        public Entity GetCustomer(long customerID)
        {
            var cust = GetCustomerRecord(customerID);

            var itemModel = _mapper.Map<Customer, GetCustomerViewModel>(cust);
            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            // shape data
            var shapeData = _dataShaperGetCustomerViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParameter)
        {

            var searchString = requestParameter.SearchString;
            var IsOwnData = requestParameter.IsOwnData;

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCustomerViewModel, Customer>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable

            var query = _cacheService.QueryCache();

            // Count records total
            recordsTotal = query.Count();

            // filter data
            FilterBySearchString(ref query, searchString, IsOwnData);

            // Count records after filter
            recordsFiltered = query.Count();

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
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<Customer>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);


            //TODO: szebben megoldani
            var resultDataModel = new List<GetCustomerViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Customer, GetCustomerViewModel>(i))
            ); ;


            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            var shapedData = _dataShaperGetCustomerViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Customer> p_item, string p_searchString, bool? IsOwnData = null)
        {
            if (!p_item.Any())
                return;


            var predicate = PredicateBuilder.New<Customer>();
            var srcFor = "";

            if (IsOwnData.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(p_searchString))
                {
                    srcFor = p_searchString.ToUpper().Trim();
                    predicate = predicate.And(p => (p.CustomerName != null && p.CustomerName.ToUpper().StartsWith(srcFor)) || (p.TaxpayerId != null && p.TaxpayerId.ToUpper().StartsWith(srcFor)));
                }
                predicate = predicate.And(p => p.IsOwnData == IsOwnData.Value);

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(p_searchString))
                {
                    srcFor = p_searchString.ToUpper().Trim();
                    predicate = predicate.And(p => (p.CustomerName != null && p.CustomerName.ToUpper().StartsWith(srcFor)) || (p.TaxpayerId != null && p.TaxpayerId.ToUpper().StartsWith(srcFor)));
                }
                else
                {
                    predicate = predicate.And(p => true);
                }
            }
            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}