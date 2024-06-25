using Bogus;
using CarvedRock.Core;

namespace CarvedRock.InnerLoop.WebApp.Tests.Utilities;


[CollectionDefinition(nameof(InnerLoopCollection))]
public class InnerLoopCollection: ICollectionFixture<SharedFixture>
{
}
public class SharedFixture : IAsyncLifetime
{
    public readonly Faker Faker = new();
    public List<ProductModel> OriginalProducts { get; private set; } = new();
    private static readonly List<string> _categories = ["boots", "equip", "kayak"];

    public readonly Faker<ProductModel> ProductFaker = new Faker<ProductModel>()
       .RuleFor(p => p.Id, f => f.UniqueIndex + 1)
       .RuleFor(p => p.Name, f => f.Commerce.ProductName())
       .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
       .RuleFor(p => p.Price, f => f.Random.Double(10, 1000))
       .RuleFor(p => p.Category, f => f.PickRandom(_categories))
       .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());

    public Task InitializeAsync()
    {
        // Initialize the shared fixture
        OriginalProducts = ProductFaker.Generate(10);

        return Task.CompletedTask;
    }

    public  Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

