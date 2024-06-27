using Bogus;
using CarvedRock.Core;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

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


    // SMTP4DEV Email Server ---------------------------
    public string EmailServerUrl => $"http://localhost:{_emailContainer.GetMappedPublicPort(80)}";
    public ushort EmailPort => _emailContainer.GetMappedPublicPort(25);

    private readonly IContainer _emailContainer = new ContainerBuilder()
        .WithImage("rnwood/smtp4dev")
        .WithPortBinding(25, assignRandomHostPort: true)
        .WithPortBinding(80, assignRandomHostPort: true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on:"))
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        // Initialize the shared fixture
        await _emailContainer.StartAsync();

        OriginalProducts = ProductFaker.Generate(10);
    }

    public  Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

