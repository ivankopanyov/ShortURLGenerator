global using System.Text.RegularExpressions;
global using Microsoft.AspNetCore.Mvc;
global using Grpc.Core;
global using Grpc.Dto;
global using Grpc.Net.Client;
global using RabbitMQ.Client;
global using Telegram.Bot;
global using Telegram.Bot.Types;
global using Telegram.Bot.Exceptions;
global using Telegram.Bot.Types.ReplyMarkups;
global using ShortURLGenerator.EventBus;
global using ShortURLGenerator.EventBus.Abstraction;
global using ShortURLGenerator.EventBus.Events;
global using ShortURLGenerator.EventBus.Handling;
global using ShortURLGenerator.TelegramBot.Services.EventBus;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Abstract;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;
global using ShortURLGenerator.TelegramBot.Services.Telegram;
global using ShortURLGenerator.TelegramBot.Services.Url;
global using ShortURLGenerator.TelegramBot.Services.Identity;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Base;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;
global using ShortURLGenerator.TelegramBot.IntegrationEventHandling;
global using ShortURLGenerator.TelegramBot.Extensions;



