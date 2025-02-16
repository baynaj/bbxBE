﻿using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdCustomer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
#if (!DEBUG)
    [Authorize]
#else
    [AllowAnonymous]
#endif
    public class CustomerController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public CustomerController(IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCustomer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryCustomer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPut]
        //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }



        [HttpGet("countrycode")]
        public async Task<IActionResult> GetCountryCode()
        {
            var req = new GetEnum() { type = typeof(enCountries) };
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("unitpricetype")]
        public async Task<IActionResult> GetUnitPriceType()
        {
            var req = new GetEnum() { type = typeof(enUnitPriceType) };
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("querytaxpayer")]
        public async Task<IActionResult> QueryTaxpayer([FromQuery] QueryTaxPayer request)
        {
            return Ok(await Mediator.Send(request));
        }


        [HttpPost("import")]
        public async Task<IActionResult> Import(List<IFormFile> customerFiles, string fieldSeparator)
        {
            if (customerFiles.Count.Equals(2))
            {
                var customerRequest = new ImportCustomerCommand() { CustomerFiles = customerFiles, FieldSeparator = fieldSeparator };
                return Ok(await Mediator.Send(customerRequest));
            }
            else
            {
                return BadRequest("Wrong parameters!");
            }
        }


        [HttpPost("lock")]
        public async Task<IActionResult> Lock(LockCustomerCommand command)
        {


            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                command.SessionID = identity.FindFirst(ClaimTypes.Thumbprint)?.Value;
            }
            var resp = await Mediator.Send(command);
            if (resp.Succeeded)
            {
                HttpContext.Session.SetString(bbxBEConsts.DEF_CUSTLOCK, resp.Data);
            }

            return Ok(resp);

        }

        [HttpPost("unlock")]
        public async Task<IActionResult> Unlock(UnlockCustomerCommand command)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                command.SessionID = identity.FindFirst(ClaimTypes.Thumbprint)?.Value;
            }
            var resp = await Mediator.Send(command);
            if (resp.Succeeded)
            {
                var custlock = HttpContext.Session.GetString(bbxBEConsts.DEF_CUSTLOCK);
                if (custlock != null)
                {
                    HttpContext.Session.Remove(bbxBEConsts.DEF_CUSTLOCK);
                }
            }
            return Ok(resp);


        }

        [HttpPatch("updatelatestdiscountpercent")]
        public async Task<IActionResult> UpdateCustomerLatestDiscountPercent([FromQuery] updateCustomerLatestDiscountPercentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

    }
}
