using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TestTwitterAPI.API;
using Tweetinvi;

namespace TestTwitterAPI
{
    class Program
    {
        static ulong MinTime;
        static ulong MaxTime;
        static decimal Average;
        static List<ulong> Times;
        static int FailedBlock;


        static void Main(string[] args)
        {
            Log(".NET Core Tweetinvi and HTTPClient benchmarking");
            Log("===============================================");

            Log("> Tweetinvi testing");
            TweetinviTest();

            Log("");
            Log("- - - - - - - - - - - - - - - - - - - - - - - -");
            Log("");
            Log("> Simple HttpClient requests");
            SimpleHttpClientTest();

            Log("");
            Log("- - - - - - - - - - - - - - - - - - - - - - - -");
            Log("");
            Log("> Done. See results in \"result.txt\" file in the current working directory.");
            Log("Press ENTER to exit.");
            Console.ReadLine();
        }

        static void TweetinviTest()
        {
            ResetValues();

            // Configure Tweetinvi
            Auth.SetUserCredentials(Constants.ConsumerKey, Constants.ConsumerSecret,
                Constants.UserAccessToken, Constants.UserAccessSecret);

            TweetinviConfig.ApplicationSettings.HttpRequestTimeout = 120000; // 2 minutes max

            for (var i = 0; i < Constants.BlockAttempts; i++)
            {
                var timer = new Stopwatch();

                try
                {
                    Log($"- ({i}/{Constants.BlockAttempts}) Block started...");
                    timer.Start();

                    var blockSuccess = User.BlockUser(Constants.AccountIdToBlock);

                    if (!blockSuccess)
                    {
                        FailedBlock++;

                        var lastException = ExceptionHandler.GetLastException();
                        Log($"    - Failed to block. {(lastException == null ? "Couldn't get the last Tweetinvi exception." : $"Twitter description: {lastException.TwitterDescription}.")}");
                    }
                    else
                    {
                        Log("    - Success.");
                    }
                }
                catch (Exception e)
                {
                    Log($"    - Failed. Exception: {e.Message}");
                    FailedBlock++;
                }
                finally
                {
                    timer.Stop();
                }

                var timeElapsed = (ulong) timer.ElapsedMilliseconds;
                Log($"    - Time elapsed: {timeElapsed}ms");

                // Update stats
                MinTime = timeElapsed < MinTime || MinTime == 0 ? timeElapsed : MinTime;
                MaxTime = timeElapsed > MaxTime ? timeElapsed : MaxTime;
                Times.Add(timeElapsed);
                Average = i == 0 ? timeElapsed : (Average + timeElapsed) / 2;
            }

            // Print stats
            Log("");
            PrintStats();
        }

        static void SimpleHttpClientTest()
        {
            ResetValues();

            var client = new APIClient();

            for (var i = 0; i < Constants.BlockAttempts; i++)
            {
                var timer = new Stopwatch();

                try
                {
                    Log($"- ({i}/{Constants.BlockAttempts}) Block started...");
                    timer.Start();

                    var response = client.BlockTest(
                        Constants.ConsumerKey,
                        Constants.ConsumerSecret,
                        Constants.UserAccessToken,
                        Constants.UserAccessSecret).Result;

                    Log("    - Done.");
                }
                catch (Exception e)
                {
                    Log($"    - Failed. Exception: {e.Message}");
                    FailedBlock++;
                }
                finally
                {
                    timer.Stop();
                }

                var timeElapsed = (ulong) timer.ElapsedMilliseconds;
                Log($"    - Time elapsed: {timeElapsed}ms");

                // Update stats
                MinTime = timeElapsed < MinTime || MinTime == 0 ? timeElapsed : MinTime;
                MaxTime = timeElapsed > MaxTime ? timeElapsed : MaxTime;
                Times.Add(timeElapsed);
                Average = i == 0 ? timeElapsed : (Average + timeElapsed) / 2;
            }

            Log("");
            PrintStats();
        }

        static void ResetValues()
        {
            MinTime = 0;
            MaxTime = 0;
            Average = 0;
            Times = new List<ulong>();
            FailedBlock = 0;
        }

        static void PrintStats()
        {
            ulong sum = 0;

            foreach (var t in Times)
            {
                sum += t;
            }

            decimal average = sum / (ulong) Times.Count;

            Log($"======= STATISTICS ======");
            Log($"    - Tries: {Times.Count} attempts");
            Log($"    - minTime: {MinTime}ms");
            Log($"    - maxTime: {MaxTime}ms");
            Log($"    - average: {average}ms");
            Log($"    - failed: {FailedBlock}");
        }

        static void Log(string message)
        {
            using (var logFile = File.AppendText("result.txt"))
            {
                logFile.WriteLine(message);
            }

            Console.WriteLine(message);
        }
    }
}
