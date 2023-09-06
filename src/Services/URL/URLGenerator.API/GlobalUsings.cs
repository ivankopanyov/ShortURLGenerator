global using System.Net;
global using System.Text.Json;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Caching.Distributed;
global using Grpc.Core;
global using Grpc.Dto;
global using ShortURLGenerator.Logger.Extansions;
global using ShortURLGenerator.URLGenerator.API.Models;
global using ShortURLGenerator.URLGenerator.API.Models.Abstract;
global using ShortURLGenerator.URLGenerator.API.Infrastructure;
global using ShortURLGenerator.URLGenerator.API.Infrastructure.EntityConfigurations;
global using ShortURLGenerator.URLGenerator.API.Repositories.URL;
global using ShortURLGenerator.URLGenerator.API.Services.Generation;
global using ShortURLGenerator.URLGenerator.API.Extensions;
global using ShortURLGenerator.URLGenerator.API.Services.URL;

