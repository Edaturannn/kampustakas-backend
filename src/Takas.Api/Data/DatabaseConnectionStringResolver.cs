using Npgsql;

namespace Takas.Api.Data;

public static class DatabaseConnectionStringResolver
{
    public static void ApplyDatabaseUrlOverride(ConfigurationManager configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            configuration["ConnectionStrings:DefaultConnection"] = ConvertDatabaseUrlToConnectionString(databaseUrl);
        }
    }

    public static string GetRequiredConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return ConvertDatabaseUrlToConnectionString(databaseUrl);
        }

        throw new InvalidOperationException("Veritabanı bağlantısı için ConnectionStrings:DefaultConnection veya DATABASE_URL tanımlanmalıdır.");
    }

    private static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var separatorIndex = uri.UserInfo.IndexOf(':');
        var username = separatorIndex >= 0
            ? Uri.UnescapeDataString(uri.UserInfo[..separatorIndex])
            : Uri.UnescapeDataString(uri.UserInfo);
        var password = separatorIndex >= 0
            ? Uri.UnescapeDataString(uri.UserInfo[(separatorIndex + 1)..])
            : string.Empty;

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = username,
            Password = password,
            SslMode = SslMode.Require
        };

        return $"{builder.ConnectionString};Trust Server Certificate=true";
    }
}
