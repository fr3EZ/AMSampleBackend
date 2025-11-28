var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<ApiExceptionFilterAttribute>();
builder.Services.AddControllers(cfg => cfg.Filters.AddService<ApiExceptionFilterAttribute>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var devOrigin = "devOrigin";

builder.Services.AddCors(options =>
{
    options.AddPolicy(devOrigin,
        c =>
        {
            c.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey(nameof(MeteoritesSyncJob));
    
    q.AddJob<MeteoritesSyncJob>(options => options
            .WithIdentity(jobKey)
            .StoreDurably() // Сохранять job между перезапусками
    );

    q.AddTrigger(options => options
        .WithIdentity($"{nameof(MeteoritesSyncJob)}-trigger")
        .ForJob(jobKey)
        .WithCronSchedule("0 0 3 * * ?")
        //.WithCronSchedule("0 */2 * * * ?") // Для теста каждые 2 минуты
        .WithDescription("Meteorites sync job trigger")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(devOrigin);
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseStatusCodePages();

app.Run();