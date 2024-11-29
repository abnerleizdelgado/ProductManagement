using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Infrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Test.Repository
{
    public class RepositoryTests
    {
        private DbContextOptions<AppDbContext> CreateSqliteDbContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }

        private async Task SeedDatabaseAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity_And_Commit_Transaction()
        {
            var options = CreateSqliteDbContextOptions();
            await using var context = new AppDbContext(options);

            // Assegure que o banco de dados e a tabela "Products" sejam criados
            await context.Database.OpenConnectionAsync();
            await context.Database.EnsureCreatedAsync();

            var repository = new RepositoryBase<Product>(context);
            var entity = new Product { Id = 1, Name = "Test Entity" };

            await repository.AddAsync(entity);  // Agora o AddAsync deve funcionar corretamente

            var result = await context.Set<Product>().FindAsync(entity.Id);
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Entity");
            await context.Database.CloseConnectionAsync();


        }


        [Fact]
        public async Task AddAsync_Should_Rollback_On_Error()
        {
            var options = CreateSqliteDbContextOptions();
            await using var context = new AppDbContext(options);

            await context.Database.OpenConnectionAsync();
            await SeedDatabaseAsync(context);

            var repository = new RepositoryBase<Product>(context);
            var entity = new Product { Id = 1, Name = "Test Entity" };

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await repository.AddAsync(entity);

                throw new Exception("Simulated failure");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }

            var result = await context.Set<Product>().FindAsync(entity.Id);
            result.Should().BeNull();
            await context.Database.CloseConnectionAsync();

        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Entities()
        {
            var options = CreateSqliteDbContextOptions();
            await using var context = new AppDbContext(options);

            await context.Database.OpenConnectionAsync();
            await SeedDatabaseAsync(context);

            var repository = new RepositoryBase<Product>(context);

            await repository.AddAsync(new Product { Id = 1, Name = "Entity 1" });
            await repository.AddAsync(new Product { Id = 2, Name = "Entity 2" });

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Select(e => e.Name).Should().Contain(new[] { "Entity 1", "Entity 2" });
            await context.Database.CloseConnectionAsync();

        }
    }
}
