global using AMSample.Domain.Entities;
global using AMSample.Application.Common.Enums;
global using AMSample.Application.Common.Behaviours;
global using AMSample.Application.Common.Interfaces;
global using AMSample.Application.Common.Entities;
global using AMSample.Application.Meteorites.Queries;
global using AMSample.Application.Meteorites.Commands;
global using AMSample.Application.Meteorites.Dtos;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Caching.Distributed;

global using System.Net;

global using FluentAssertions;
global using FluentValidation;
global using FluentValidation.Results;
global using MediatR;
global using Moq;
global using AutoMapper;