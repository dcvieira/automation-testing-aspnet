﻿using CarvedRock.InnerLoop.WebApp.Tests.Utilities;
using System.Net;

namespace CarvedRock.InnerLoop.WebApp.Tests;

[Collection(nameof(InnerLoopCollection))]
public class ThankYouPageTests(CustomWebAppFactory factory)
    : IClassFixture<CustomWebAppFactory>
{
    [Fact]
    public async Task GetThankYouPage()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Authorization", "Erik Smith");
        client.DefaultRequestHeaders.Add("X-Test-idp", "Microsoft");

        var thankYouPageResponse = await client.GetAsync("/thankyou");
        var thankYouPage = await HtmlHelpers.GetDocumentAsync(thankYouPageResponse);

        Assert.Equal(HttpStatusCode.OK, thankYouPageResponse.StatusCode);

        var actualHeading = thankYouPage.QuerySelectorAll("h1").Select(e => e.TextContent);
        Assert.Equal("Thanks for your (fake) order!", actualHeading.First());
    }
}