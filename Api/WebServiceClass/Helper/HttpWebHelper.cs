using System.Net.Http.Headers;

namespace WebServiceClass.Helper
{
    /// <summary>
    /// 请求外部接口
    /// </summary>
    public class HttpWebHelper:IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpWebHelper(string baseUrl, string defaultContentType = "application/json")
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(defaultContentType));
        }
        //请求样例
        //using (var HttpWebHelper = new HttpWebHelper("https://api.example.com/"))
        //{
        //    // GET 请求
        //    var getResponse = await apiHelper.GetAsync("endpoint");
        //    var getContent = await getResponse.Content.ReadAsStringAsync();
        //    Console.WriteLine($"GET Response: {getContent}");

        //    // POST 请求
        //    var postContent = new StringContent("{\"key\":\"value\"}", Encoding.UTF8, "application/json");
        //    var postResponse = await apiHelper.PostAsync("endpoint", postContent);
        //    var postResponseContent = await postResponse.Content.ReadAsStringAsync();
        //    Console.WriteLine($"POST Response: {postResponseContent}");

        //    // PUT 请求
        //    var putContent = new StringContent("{\"key\":\"new_value\"}", Encoding.UTF8, "application/json");
        //    var putResponse = await apiHelper.PutAsync("endpoint/1", putContent);
        //    var putResponseContent = await putResponse.Content.ReadAsStringAsync();
        //    Console.WriteLine($"PUT Response: {putResponseContent}");

        //    // DELETE 请求
        //    var deleteResponse = await apiHelper.DeleteAsync("endpoint/1");
        //    var deleteResponseContent = await deleteResponse.Content.ReadAsStringAsync();
        //    Console.WriteLine($"DELETE Response: {deleteResponseContent}");
        //}
        public async Task<HttpResponseMessage> GetAsync(string endpoint, string queryParams = null)
        {
            var url = string.IsNullOrEmpty(queryParams) ? endpoint : $"{endpoint}?{queryParams}";
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse(response);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse(response);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse(response);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return await HandleResponse(response);
        }

        private async Task<HttpResponseMessage> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
