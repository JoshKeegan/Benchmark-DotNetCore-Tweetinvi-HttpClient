# Benchmark-DotNetCore-Tweetinvi-HttpClient
Benchmarking a large set of requests using .NET Core 2.1 pure HttpClient and Tweetinvi.

# Requirements

- .NET Core 2.1 SDK
- Visual Studio

# How to use
First, clone the repository or download the source code as a ZIP archive then open the solution file `TestTwitterAPI.sln`.

Then, open the `Constants.cs` file and enter your Twitter App and account access credentials

```CS
public static class Constants
{
    // - Twitter App Credentials
    public static readonly string ConsumerKey    = "";
    public static readonly string ConsumerSecret = "";

    // - User Credentials
    public static readonly string UserAccessSecret = "";
    public static readonly string UserAccessToken  = "";

    // - Others constants
    public static readonly long AccountIdToBlock  = 3359397555; // This is one of my test account no worries
    public static readonly int BlockAttempts      = 300;
}
```

You can also change the `BlockAttempts` variable which is the attempts count for each loop. I would not recommand to exceed 300 but you can try if you really want and understand that you're likely to reach your API request limit.

Now you can launch the project (this is a simple .NET Core console app) and wait for the benchmark to finish.

All console outputs are logged in `result.txt` file which will be saved in the same directory of the project executable.

# Comments

I know the code can be better but this is just a benchmarking tool quickly written to try to debug and reproduce an issue I get randomly on my Debian 8 server.

See more here: https://github.com/linvi/tweetinvi/issues/700.