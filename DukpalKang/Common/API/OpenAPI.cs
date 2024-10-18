using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RasingDeokPal.Common.API
{
    internal class OpenAPI
    {
        public static async Task<T> CallAPI<T>(string text, List<GPTRole> messageList)
        {
            string url = "https://api.openai.com/v1/chat/completions";
            string token = "sk-proj-vC1VIK6PVgVuTYgEnvEAT3BlbkFJvzWACxV8XAWL2eMXwnu6";

            using (HttpClient client = new HttpClient())
            {
                // Request Data
                GPTRequestDTO dto = new GPTRequestDTO
                {
                    messages = messageList
                };

                // Request
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(dto);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                try
                {
                    // POST 요청 보내기
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // 응답 확인
                    response.EnsureSuccessStatusCode();

                    // 응답 내용 읽기
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    T result = JsonSerializer.Deserialize<T>(responseBody, options);
                    
                    if(result != null)
                    {
                        return result;
                    }
                    return default(T);
                }
                catch (HttpRequestException e)
                {
                    return default(T);
                }
            }
        }
    }

    /// <summary>
    /// GPT Request DTO
    /// </summary>
    internal class GPTRequestDTO
    {
        //public string model { get; set; } = "gpt-3.5-turbo-1106";
        public string model { get; set; } = "ft:gpt-3.5-turbo-1106:personal::9qNJUaHv";
        public List<GPTRole> messages { get; set; } = 
            new List<GPTRole>
            {
                new GPTRole{role ="system", content = "너는 친절한 말투를 사용해, 그리고 너의 존재에 대한 질문이나, 너는 누구냐는 질문에는 덕팔이라고만 대답해." },
            };
        public int max_tokens = 150;
    }

    internal class GPTRole
    {
        public string role { get; set; } = "";
        public string content { get; set; } = "";
    }

    /// <summary>
    /// GPT Response DTO
    /// </summary>
    public class GPTResponseDTO
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
        public string SystemFingerprint { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}

