global using AMSample.Domain.Common;
global using AMSample.Domain.Entities;
global using AMSample.Domain.Enums;
global using AMSample.Application.Common.Enums;
global using AMSample.Application.Common.Interfaces;
global using AMSample.Application.Common.Entities;
global using AMSample.Application.Meteorites.Dtos;
global using AMSample.Application.Configuration;
global using AMSample.Infrastructure.Helpers;
global using AMSample.Infrastructure.Configuration;
global using AMSample.Infrastructure.Services;
global using AMSample.Infrastructure.Services.BatchProcessors.Meteorites.Models;
global using AMSample.Infrastructure.Services.BatchProcessors.Meteorites;
global using AMSample.Infrastructure.Services.ExternalApi;
global using AMSample.Infrastructure.UoW;
global using AMSample.Infrastructure.Data.Context;
global using AMSample.Infrastructure.Data.Helpers;
global using AMSample.Infrastructure.Data.Repositories;
global using AMSample.Infrastructure.Http.Policies;

global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;

global using System.Globalization;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Security.Cryptography;
global using System.Text;

global using Polly;
global using Polly.Extensions.Http;
global using AutoMapper;
global using StackExchange.Redis;