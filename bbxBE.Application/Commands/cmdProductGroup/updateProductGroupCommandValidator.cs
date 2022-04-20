﻿using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdProductGroup;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProductGroup
{

    public class UpdateProductGroupCommandValidator : AbstractValidator<UpdateProductGroupCommand>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;

        public UpdateProductGroupCommandValidator(IProductGroupRepositoryAsync ProductGroupRepository)
        {
            this._ProductGroupRepository = ProductGroupRepository;


            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.ProductGroupCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .Must(
                        (model, Name) =>
                        {
                            return  IsUniqueProductGroupCodeAsync(Name, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.ProductGroupDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.FV_MAXLEN);
        }

        private bool IsUniqueProductGroupCodeAsync(string ProductGroupCode, long ID)
        {
            if (ProductGroupCode.Length != 0)
            {
                return _ProductGroupRepository.IsUniqueProductGroupCode(ProductGroupCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
