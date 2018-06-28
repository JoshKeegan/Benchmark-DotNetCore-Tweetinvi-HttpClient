using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestTwitterAPI.API
{
    public class APIClient
    { 
        private static HttpClient _twitterClient;

        public APIClient()
        {
            // Ensure that we only have one instance of our HttpClient
            if (APIClient._twitterClient != null) return;

            APIClient._twitterClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> BlockTest(string consumerKey, string consumerSecret, string userAccessToken, string userAccessSecret)
        {
            var fullEndpoint    = "https://api.twitter.com/1.1/blocks/create.json?user_id=18165788";
            var endpoint        = "https://api.twitter.com/1.1/blocks/create.json";
            var timestamp       = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            //Create Number Once
            var nonce      = Convert.ToBase64String(Encoding.UTF8.GetBytes(timestamp));
            var nonceRegex = new Regex("[^\\w\\s\\-]*"); // Removes non alphanumeric characters from the nonce (or the request will be invalid)
            nonce          = nonceRegex.Replace(nonce, "");

            // Oauth variables
            var oauth_consumer_key     = consumerKey;
            var oauth_nonce            = nonce;
            var oauth_signature_method = "HMAC-SHA1";
            var oauth_timestamp        = timestamp;
            var oauth_token            = userAccessToken;
            var oauth_version          = "1.0";
            var user_id                = "18165788";

            // Signature values
            var signingKeyBase = $"{consumerSecret}&{userAccessSecret}";

            var signingValues  = "";
            signingValues      += $"{Uri.EscapeDataString($"oauth_consumer_key={oauth_consumer_key}")}";
            signingValues      += $"{Uri.EscapeDataString($"&oauth_nonce={oauth_nonce}")}";
            signingValues      += $"{Uri.EscapeDataString($"&oauth_signature_method={oauth_signature_method}")}";
            signingValues      += $"{Uri.EscapeDataString($"&oauth_timestamp={oauth_timestamp}")}";
            signingValues      += $"{Uri.EscapeDataString($"&oauth_token={oauth_token}")}";
            signingValues      += $"{Uri.EscapeDataString($"&oauth_version={oauth_version}")}";
            signingValues      += $"{Uri.EscapeDataString($"&user_id={user_id}")}";

            // Signature base string
            var signature = $"POST&{Uri.EscapeDataString(endpoint)}&{signingValues}";

            // Generate HMAC-SHA1 key
            var hmac            = new HMACSHA1(Encoding.UTF8.GetBytes(signingKeyBase));
            var signatureHashed = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signature)));

            // Construct the header
            var header = "OAuth ";
            header += $"{Uri.EscapeDataString("oauth_consumer_key")}={Uri.EscapeDataString(oauth_consumer_key)}, ";
            header += $"{Uri.EscapeDataString("oauth_nonce")}={Uri.EscapeDataString(oauth_nonce)}, ";
            header += $"{Uri.EscapeDataString("oauth_signature")}={Uri.EscapeDataString(signatureHashed)}, ";
            header += $"{Uri.EscapeDataString("oauth_signature_method")}={Uri.EscapeDataString(oauth_signature_method)}, ";
            header += $"{Uri.EscapeDataString("oauth_timestamp")}={Uri.EscapeDataString(oauth_timestamp)}, ";
            header += $"{Uri.EscapeDataString("oauth_token")}={Uri.EscapeDataString(oauth_token)}, ";
            header += $"{Uri.EscapeDataString("oauth_version")}={Uri.EscapeDataString(oauth_version)}";

            // Prepare the request
            var httpContent = new HttpRequestMessage()
            {
                RequestUri = new Uri(fullEndpoint),
                Method = HttpMethod.Post,
            };

            httpContent.Headers.Add("Authorization", header);
            httpContent.Version = new Version("1.1");

            // Send the request
            var response = await APIClient._twitterClient.SendAsync(httpContent);

            response.EnsureSuccessStatusCode(); // Throws an exception if not an http 2xx code
            
            return response;
        }
    }
}
