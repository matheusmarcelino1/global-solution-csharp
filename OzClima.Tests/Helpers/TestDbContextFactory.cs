
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OzClima.Api.Data;

namespace OzClima.Tests.Helpers;

public static class TestDbContextFactory
{
    public static (AppDbContext Context, SqliteConnection Connection)
        CriarContexto()
    {
        var connection = new SqliteConnection("Data Source=:memory:");

        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureCreated();

        return (context, connection);
    }
}
