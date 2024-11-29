using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services
{

    public class ProductService : ServiceBase<Product, ProductDTO>, IProductService
    {

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository, IMapper mapper)
            : base(productRepository, mapper)
        {
            _productRepository = productRepository;
        }

    }
}