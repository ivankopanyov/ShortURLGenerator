global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualBasic;
global using Xunit;
global using Moq;
global using MockQueryable.Moq;
global using Grpc.Core;
global using ShortURLGenerator.Grpc.Services;
global using ShortURLGenerator.StringGenerator;
global using ShortURLGenerator.URLGenerator.API.Repositories.URL;
global using ShortURLGenerator.URLGenerator.API.Infrastructure;
global using UrlService = ShortURLGenerator.URLGenerator.API.Services.URL.UrlService;
global using Url = ShortURLGenerator.URLGenerator.API.Models.Url;

