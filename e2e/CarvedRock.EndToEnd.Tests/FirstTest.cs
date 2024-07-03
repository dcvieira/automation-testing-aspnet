using CarvedRock.EndToEnd.Tests.Utilities;
using Microsoft.Playwright;

namespace CarvedRock.EndToEnd.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class FirstTest : BaseTest
    {

        [Test]
        public async Task HomePageHasCorretContent()
        {
            await Page.GotoAsync(BaseUrl);
            await Expect(Page).ToHaveTitleAsync("Carved Rock Fitness");
            await Expect(Page.GetByText("GET A GRIP" )).ToBeVisibleAsync();
        }


        [Test]
        public async Task MyTest()
        {
            await Page.GotoAsync($"{BaseUrl}/Listing?cat=boots");
            await CheckCartItemCountAsync(0);

            await Page.Locator("#add-btn-1").ClickAsync();
            await CheckCartItemCountAsync(1);

            await Page.Locator("#add-btn-1").ClickAsync();
            await CheckCartItemCountAsync(2);
            await Page.Locator("#add-btn-3").ClickAsync();

            await CheckCartItemCountAsync(3);
        }

        private async Task CheckCartItemCountAsync(int expectedCount)
        {
            await Expect(Page.Locator("#carvedrockcart")).ToContainTextAsync($"({expectedCount})");
        }
    }
}
