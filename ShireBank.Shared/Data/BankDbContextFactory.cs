using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShireBank.Shared.Data;

public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
{
    public BankDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();
        optionsBuilder.UseSqlite("Data Source=test.db");

        return new BankDbContext(optionsBuilder.Options);
    }
}