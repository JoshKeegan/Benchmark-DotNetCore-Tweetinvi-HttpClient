using System;
using System.Collections.Generic;
using System.Text;

namespace TestTwitterAPI
{
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
}
