using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using FluentAssertions;
using Moq;

namespace Test.Service
{

    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Products()
        {
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10 },
            new Product { Id = 2, Name = "Product 2", Price = 20 }
        };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = await _productService.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task CreateAsync_Should_Call_AddAsync_Once()
        {
            var product = new Product { Id = 1, Name = "New Product", Price = 30 };

            await _productService.CreateAsync(product);

            _mockRepository.Verify(r => r.AddAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_DeleteAsync_Once()
        {
            var productId = 1;

            await _productService.DeleteAsync(productId);

            _mockRepository.Verify(r => r.DeleteAsync(productId), Times.Once);
        }
    }
}