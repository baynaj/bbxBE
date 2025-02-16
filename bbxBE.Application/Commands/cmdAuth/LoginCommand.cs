﻿using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.ResultModels;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdAuth
{
    public class LoginCommand : IRequest<Response<LoginInfo>>
    {

        [ColumnLabel("Login név")]
        [Description("Login név")]
        public string LoginName { get; set; }

        [ColumnLabel("Jelszó")]
        [Description("Jelszó")]
        public string Password { get; set; }


    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<LoginInfo>>
    {
        private readonly IUserRepositoryAsync _userRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LoginCommandHandler(IUserRepositoryAsync userRepositoryAsync, IMapper mapper, IConfiguration configuration, ILogger logger)
        {
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Response<LoginInfo>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"Logging:{request.LoginName}");

            var usr = await _userRepositoryAsync.GetUserRecordByLoginNameAsync(request.LoginName);
            if (usr == null || !usr.Active)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND2, request.LoginName));
            }

            var apiKey = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_SENDGRID_API_KEY);
            var salt = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_PWDSALT);

            if (BllAuth.GetPwdHash(request.Password, salt) != usr.PasswordHash)
            {
                throw new UnauthorizedAccessException();
            }

            var JWTSettings = _configuration.GetSection(bbxBEConsts.CONF_JWTSettings);
            var JWTKey = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTKey);
            var JWTIssuer = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTIssuer);
            var JWTAudience = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTAudience);
            var JWTDurationInMinutes = JWTSettings.GetValue<double>(bbxBEConsts.CONF_JWTDurationInMinutes);

            var token = BllAuth.GenerateJSONWebToken(usr, JWTKey, JWTIssuer, JWTAudience, JWTDurationInMinutes);

            return new Response<LoginInfo>(new LoginInfo() { Token = token, User = usr });
        }


    }
}
