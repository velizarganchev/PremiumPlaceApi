using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PremiumPlace_API.Data;

namespace PremiumPlace_API.Tests.Helpers;

/// <summary>
/// Stripped-down DbContext that replaces SQL-Server–only defaults
/// (GETUTCDATE()) so the schema can be created on SQLite.
/// </summary>
internal sealed class SqliteTestDbContext : ApplicationDbContext
{
    public SqliteTestDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Remove HasDefaultValueSql("GETUTCDATE()") — incompatible with SQLite.
        // Tests always supply explicit values, so no default is needed.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var prop in entity.GetProperties())
            {
                if (prop.GetDefaultValueSql() is not null)
                    prop.SetDefaultValueSql(null);
            }
        }
    }
}

/// <summary>
/// Creates a SQLite in-memory database that lives for the lifetime of this
/// factory instance. Call <see cref="CreateContext"/> to get a fresh
/// DbContext that shares the same underlying connection (and data).
/// </summary>
internal sealed class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Create schema once
        using var ctx = CreateContext();
        ctx.Database.EnsureCreated();
    }

    public SqliteTestDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
