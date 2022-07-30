﻿using bbxBE.Application.Commands.cmdInvCtrl;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qInvCtrl;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvCtrl;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
 //   [Authorize]
    public class InvCtrlController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public InvCtrlController( IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetInvCtrl filter)
        {
            return Ok(await Mediator.Send(filter));
        }

    

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryInvCtrl filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvCtrlCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

  /*      
        [HttpPut]
 //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateInvCtrlCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteInvCtrlCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPatch]
        //       [ValidateAntiForgeryToken]
        [HttpGet("close")]
        public async Task<IActionResult> Close([FromQuery] CloseInvCtrlCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
  */
    }
}
