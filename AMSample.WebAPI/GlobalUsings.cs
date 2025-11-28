global using AMSample.Application;
global using AMSample.Application.Common.Enums;
global using AMSample.Application.Common.Exceptions;
global using AMSample.Application.Meteorites.Queries;
global using AMSample.Application.Meteorites.Commands;
global using AMSample.Application.Meteorites.Dtos;
global using AMSample.Infrastructure;
global using AMSample.WebAPI.BackgroundJobs;
global using AMSample.WebAPI.Filters;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.AspNetCore.Mvc.Filters;

global using System.Reflection;

global using MediatR;
global using Quartz;
global using FluentValidation;
global using Quartz;