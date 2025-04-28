using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace EnvironmentControlinator.Infra
{
    public class Call
    {
        public async Task<String> Execute(String uri)
        {
            var client = new HttpClient();

            // Create the HttpContent for the form to be posted.
            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("text", "This is a block of text"),
            });

            // Get the response.
            HttpResponseMessage response = await client.PostAsync(
                uri,
                requestContent);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                return await reader.ReadToEndAsync();
            }
        }
    }
}
