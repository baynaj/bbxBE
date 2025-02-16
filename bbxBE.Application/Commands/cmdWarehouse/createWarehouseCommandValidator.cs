﻿using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdWarehouse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdWarehouse
{

    public class createWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;

        public createWarehouseCommandValidator(IWarehouseRepositoryAsync WarehouseRepository)
        {
            this._WarehouseRepository = WarehouseRepository;
            RuleFor(p => p.WarehouseCode)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueWarehouseCodeAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.ERR_EXISTS)
                 .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.WarehouseDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

        }

        private async Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, CancellationToken cancellationToken)
        {
            if (WarehouseCode.Length != 0)
            {
                return await _WarehouseRepository.IsUniqueWarehouseCodeAsync(WarehouseCode);
            }
            else
            {
                return true;
            }

        }

    }
}