using Bogus;
using CarvedRock.Core;
using CarvedRock.InnerLoop.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.Tests
{
    public class ProductControllerTests(
        CustomApiFactory factory,
        ITestOutputHelper outputHelper
        ) : IClassFixture<CustomApiFactory>
    {

        [Fact]
        public async Task GetProduct_Success()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var products = await client.GetJsonResultAsync<IEnumerable<ProductModel>>
                ("/product?category=all", HttpStatusCode.OK, outputHelper);

            // Assert
            Assert.Equal(6, products.Count());

        }

        [Fact]
        public async Task GetProduct_TroubleGivesProblemDetail()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var problemDetails = await client.GetJsonResultAsync<ProblemDetails>
                ("/product?category=trouble", HttpStatusCode.InternalServerError, outputHelper);

            // Assert
            Assert.NotNull(problemDetails.Title);
            Assert.NotNull(problemDetails.Detail);

            Assert.Contains("traceId", problemDetails.Extensions.Keys);

        }

        [Fact]
        public async Task PostProductValidationFailure()
        {
            // Arrange
            var newProduct = NewProductFaker.Generate();
            newProduct.Name = "";

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Authorization", "Bob Smith");

            // Act
            var problem = await client.PostForJsonResultAsync<ProblemDetails>
                ("/product", newProduct, HttpStatusCode.BadRequest, outputHelper);

            // Assert
            Assert.NotNull(problem);
            Assert.Equal("One or more validation errors occurred.", problem.Detail);
            Assert.Contains("Name", problem.Extensions.Keys);
            Assert.Contains("Name is required.", problem.Extensions["Name"]!.ToString());
        }


        private readonly Faker<NewProductModel> NewProductFaker = new Faker<NewProductModel>()
                .UseSeed(2001)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Category, f => f.PickRandom("boots", "equip", "kayak"))
                .RuleFor(p => p.Price, (f, p) =>
                        p.Category == "boots" ? f.Random.Double(50, 300) :
                        p.Category == "equip" ? f.Random.Double(20, 150) :
                        p.Category == "kayak" ? f.Random.Double(100, 500) : 0)
                .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());
    }
}
