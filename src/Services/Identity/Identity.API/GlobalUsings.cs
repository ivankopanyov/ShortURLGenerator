﻿global using System.Net;
global using System.Text.Json;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.Extensions.Caching.Distributed;
global using Grpc.Core;
global using Grpc.Dto;
global using ShortURLGenerator.StringGenerator;
global using ShortURLGenerator.Identity.API.Models;
global using ShortURLGenerator.Identity.API.Services.Identity;
global using ShortURLGenerator.Identity.API.Extensions;
global using ShortURLGenerator.Identity.API.Services.VerificationCodeGenerator;
global using ShortURLGenerator.Identity.API.Services.RefreshTokenGenerator;
global using ShortURLGenerator.Identity.API.Repositories.Connection;
global using ShortURLGenerator.Identity.API.Repositories.VerificationCode;

