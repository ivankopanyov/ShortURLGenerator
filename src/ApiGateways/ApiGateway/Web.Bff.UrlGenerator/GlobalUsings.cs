global using System.Text;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using ShortURLGenerator.GrpcHelper.Abstraction;
global using ShortURLGenerator.GrpcHelper.Services.URL;
global using ShortURLGenerator.EventBus;
global using ShortURLGenerator.EventBus.Abstraction;
global using ShortURLGenerator.EventBus.Events;
global using ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

