global using System.Text.RegularExpressions;
global using Microsoft.AspNetCore.Mvc;
global using RabbitMQ.Client;
global using Telegram.Bot;
global using Telegram.Bot.Types;
global using Telegram.Bot.Exceptions;
global using Telegram.Bot.Types.ReplyMarkups;
global using ShortURLGenerator.Grpc.Services;
global using ShortURLGenerator.EventBus;
global using ShortURLGenerator.EventBus.Abstraction;
global using ShortURLGenerator.EventBus.Events;
global using ShortURLGenerator.EventBus.Handling;
global using ShortURLGenerator.GrpcHelper.Abstraction;
global using ShortURLGenerator.GrpcHelper.Extensions;
global using ShortURLGenerator.TelegramBot.Services.FixURL;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Abstract;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;
global using ShortURLGenerator.TelegramBot.Services.Telegram;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Base;
global using ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;
global using ShortURLGenerator.TelegramBot.IntegrationEventHandling;
global using ShortURLGenerator.TelegramBot.Extensions;



