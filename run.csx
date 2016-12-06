#r "System.Web"
using System.Net;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public static async Task<HttpResponseMessage> Run(byte[] req, TraceWriter log)
{
    string appId = "";
    string luisKey = "";
    string speechToTextToken = "";
    var token = await GetNewToken(speechToTextToken);
    //string fileName = "whatstheweatherlike.wav";
    //byte[] buffer = File.ReadAllBytes(fileName);
    var result = await GetTextFromAudioAsync(token, req);
    var queryLuis = await QueryLuis(appId, luisKey, result);
    HttpResponseMessage r = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
    r.Content = new StringContent(queryLuis);
    return r;

    
}
private static async Task<string>  GetNewToken(string ApiKey)
{
    using (var client = new HttpClient())
    {   
        HttpContent content = new StringContent("");
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken"),
            Method = HttpMethod.Post,
            Content = content
        };
        request.Headers.Add("Ocp-Apim-Subscription-Key", ApiKey);
        var response = client.SendAsync(request).Result;
        var responseString = response.Content.ReadAsStringAsync().Result;
        return responseString;
    }
}
public static async Task<string> GetTextFromAudioAsync(string token, byte[] audiostream)
{
    var requestUri = @"https://speech.platform.bing.com/recognize?scenarios=smd&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale=en-US&device.os=bot&version=3.0&format=json&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3&requestid=" + Guid.NewGuid();
    using (var client = new HttpClient())
    {
        
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        using (var binaryContent = new ByteArrayContent(audiostream))
        {
            binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

            var response = await client.PostAsync(requestUri, binaryContent);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseString);
            return data.header.name;
        }
    }
}


private static async Task<string> QueryLuis(string appId, string LuisKey, string spokentext)
{
    var encodedText = HttpUtility.HtmlEncode(spokentext);
    string fullUrl = $"https://api.projectoxford.ai/luis/v1/application?subscription-key={LuisKey}&id={appId}&q={encodedText}";
    
    using (var client = new HttpClient())
    {   
        var request = new HttpRequestMessage(){RequestUri = new Uri(fullUrl),Method = HttpMethod.Get };
        var response = client.SendAsync(request).Result;
        var responseString = response.Content.ReadAsStringAsync().Result;
        return responseString;
    }

}