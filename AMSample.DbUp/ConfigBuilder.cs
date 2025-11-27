using Microsoft.Extensions.Configuration;

namespace AMSample.DbUp;

public class ConfigBuilder
{
    public static IConfigurationRoot GetConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();


        return configuration;
    }
}