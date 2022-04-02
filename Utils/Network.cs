using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MalodyInfoQuery.Utils
{
    internal class Network
    {
        public static async Task<byte[]> Download(string url, Dictionary<string, string> header = null,
            int timeout = 8000, int limitLen = 0)
        {
            // Create request
            var request = WebRequest.CreateHttp(url);
            {
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                request.AutomaticDecompression = DecompressionMethods.All;

                // Default useragent
                request.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) " +
                    "Chrome/94.0.4606.31 Safari/537.36 " +
                    "maikon/1.0.0 (Konata Project)");

                // Append request header
                if (header != null)
                {
                    foreach (var (k, v) in header)
                        request.Headers.Add(k, v);
                }
            }

            // Open response stream
            var response = await request.GetResponseAsync();
            {
                // length limitation
                if (limitLen != 0)
                {
                    // Get content length
                    var totalLen = int.Parse
                        (response.Headers["Content-Length"]!);

                    // Decline streaming transport
                    if (totalLen > limitLen || totalLen == 0) return null;
                }

                // Receive the response data
                var stream = response.GetResponseStream();
                try
                {
                    await using var memStream = new MemoryStream();
                    // Copy the stream
                    if (stream != null)
                    {
                        await stream.CopyToAsync(memStream);
                        // Close
                        response.Close();
                        return memStream.ToArray();
                    }

                    return null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public static async Task<string> HttpGetContent(string url, Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HttpResponseMessage> HttpGet(string url, Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return await client.SendAsync(request);
        }

        public static async Task<string> HttpPostContent(string url, HttpContent postContent,
            Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = postContent
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HttpResponseMessage> HttpPost(string url, HttpContent postContent,
            Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = postContent
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return await client.SendAsync(request);
        }
    }
}