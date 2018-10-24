using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using KLog;
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

        public static Log DetailedLog;
        public static Log SummaryLog;


        static void Main(string[] args)
        {
            // Initialise logging
            FileLog fileDetailedLog = new FileLog("logs/detailed.log", LogLevel.All);
            FileLog fileSummaryLog = new FileLog("logs/summary.log", LogLevel.All);
            ColouredConsoleLog consoleLog = new ColouredConsoleLog(LogLevel.All);
            
            DetailedLog = new CompoundLog(fileDetailedLog, consoleLog);
            SummaryLog = new CompoundLog(fileSummaryLog, consoleLog);
            
            DetailedLog.Info(".NET Core Tweetinvi and HTTPClient benchmarking");
            DetailedLog.Info("===============================================");

            DetailedLog.Info("> Tweetinvi testing");
            TweetinviTest();

            DetailedLog.Info("");
            DetailedLog.Info("- - - - - - - - - - - - - - - - - - - - - - - -");
            DetailedLog.Info("");
            DetailedLog.Info("> Simple HttpClient requests");
            SimpleHttpClientTest();

            DetailedLog.Info("");
            DetailedLog.Info("- - - - - - - - - - - - - - - - - - - - - - - -");
            DetailedLog.Info("");
            DetailedLog.Info("> Done. See results in \"logs\" directory under the current working directory.");
            
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        static void TweetinviTest()
        {
            ResetValues();

            // Configure Tweetinvi
            Auth.SetUserCredentials(Constants.ConsumerKey, Constants.ConsumerSecret,
                Constants.UserAccessToken, Constants.UserAccessSecret);
            
            TweetinviConfig.CurrentThreadSettings.HttpRequestTimeout = 120000; // 2 minutes max

            for (var i = 0; i < Constants.BlockAttempts; i++)
            {
                var timer = new Stopwatch();

                try
                {
                    DetailedLog.Info($"- ({i}/{Constants.BlockAttempts}) Block started...");
                    timer.Start();

                    var blockSuccess = User.BlockUser(Constants.AccountIdToBlock);

                    if (!blockSuccess)
                    {
                        FailedBlock++;

                        var lastException = ExceptionHandler.GetLastException();
                        DetailedLog.Error($"    - Failed to block. {(lastException == null ? "Couldn't get the last Tweetinvi exception." : $"Twitter description: {lastException.TwitterDescription}.")}");
                    }
                    else
                    {
                        DetailedLog.Info("    - Success.");
                    }
                }
                catch (Exception e)
                {
                    DetailedLog.Error($"    - Failed. Exception: {e.Message}");
                    FailedBlock++;
                }
                finally
                {
                    timer.Stop();
                }

                var timeElapsed = (ulong) timer.ElapsedMilliseconds;
                DetailedLog.Info($"    - Time elapsed: {timeElapsed}ms");

                // Update stats
                MinTime = timeElapsed < MinTime || MinTime == 0 ? timeElapsed : MinTime;
                MaxTime = timeElapsed > MaxTime ? timeElapsed : MaxTime;
                Times.Add(timeElapsed);
                Average = i == 0 ? timeElapsed : (Average + timeElapsed) / 2;
            }

            // Print stats
            DetailedLog.Info("");
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
                    DetailedLog.Info($"- ({i}/{Constants.BlockAttempts}) Block started...");
                    timer.Start();

                    var response = client.BlockTest(
                        Constants.ConsumerKey,
                        Constants.ConsumerSecret,
                        Constants.UserAccessToken,
                        Constants.UserAccessSecret).Result;

                    DetailedLog.Info("    - Done.");
                }
                catch (Exception e)
                {
                    DetailedLog.Error($"    - Failed. Exception: {e.Message}");
                    FailedBlock++;
                }
                finally
                {
                    timer.Stop();
                }

                var timeElapsed = (ulong) timer.ElapsedMilliseconds;
                DetailedLog.Info($"    - Time elapsed: {timeElapsed}ms");

                // Update stats
                MinTime = timeElapsed < MinTime || MinTime == 0 ? timeElapsed : MinTime;
                MaxTime = timeElapsed > MaxTime ? timeElapsed : MaxTime;
                Times.Add(timeElapsed);
                Average = i == 0 ? timeElapsed : (Average + timeElapsed) / 2;
            }

            DetailedLog.Info("");
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

            SummaryLog.Info($"======= STATISTICS ======");
            SummaryLog.Info($"    - Tries: {Times.Count} attempts");
            SummaryLog.Info($"    - minTime: {MinTime}ms");
            SummaryLog.Info($"    - maxTime: {MaxTime}ms");
            SummaryLog.Info($"    - average: {average}ms");
            SummaryLog.Info($"    - failed: {FailedBlock}");
        }
    }
}
