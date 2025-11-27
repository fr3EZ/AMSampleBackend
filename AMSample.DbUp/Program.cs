using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace AMSample.DbUp;

internal class Program
{
    public static int Main(string[] args)
    {
        var config = ConfigBuilder.GetConfiguration();

        var connectionString = config.GetConnectionString("AMSampleDb");

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .WithTransactionPerScript()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            Console.ReadLine();
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}