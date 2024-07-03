using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarvedRock.EndToEnd.Tests.Utilities;
public class BaseTest : PageTest
{
    public string BaseUrl { get; private set; } = null!;

    public BaseTest()
    {
        BaseUrl = Environment.GetEnvironmentVariable("WEB_URL") ?? "https://DEFINE_AS_ENV_VAR_OR_RUNSETTINGS";
    }
}