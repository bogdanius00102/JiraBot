
using Microsoft.AspNetCore.Connections;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;
using User = KernelHelpBot.Models.People_Information.User;
namespace KernelHelpBot.Models.JiraRequest
{
    public static class Jira
    {
      static  string jiraBaseUrl = "https://sd.kernel.ua";
        static string username = "t-bot_sd@kernel.ua";
        static string password = "TB0tforJSD16102024";
        public static async Task<ResponseOnCreateJiraTask> CreateNewTask(long telegram_id,string tema,string text,string avtor,string project)
        {
            string escapedText = text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");

            string issueJson = @"{
    ""fields"": {
        ""project"": { ""key"": """+ project + @""" },
        ""summary"":""" + tema + @""",
        ""description"": """ + escapedText + @""",
        ""issuetype"": { ""name"": ""Service Request"" },
         ""customfield_17000"": """ + telegram_id + @""",
        ""labels"": [""TELEGRAM_BOT"",""KD_ITSD_bot""],
        ""reporter"": { ""name"": """ + avtor + @""" }
    }
}";
//            string issueJson = @"{
//    ""fields"": {
//        ""project"": { ""key"": ""ITSD"" },
//        ""summary"":""" + tema + @""",
//        ""description"": """ + escapedText + @""",
//        ""issuetype"": { ""name"": ""Service Request"" },
//        ""customfield_10300"": [{""name"":""2nd Line Research And Development""}],
//        ""labels"": [""TELEGRAM_BOT"",""KD_ITSD_bot""],
//        ""reporter"": { ""name"": """ + avtor + @""" }
//    }
//}";


            using (var client = new HttpClient())
            {
                // Устанавливаем базовый URL для REST API Jira.
                client.BaseAddress = new Uri(jiraBaseUrl);

                // Создаем заголовок для Basic Authentication.
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

                // Создаем запрос POST для создания комментария.
                var content = new StringContent(issueJson, Encoding.UTF8, "application/json");
                //var response = await client.PostAsync($"/rest/api/2/issue/{issueKey}/comment", content);
                var response = await client.PostAsync("/rest/api/2/issue", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    ResponseOnCreateJiraTask result=JsonConvert.DeserializeObject<ResponseOnCreateJiraTask>(responseContent);
                    Console.WriteLine("Заявка создана");
                    return result;
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }

            return null;
        }
        public static async Task<JiraIssues> GetRequestOfuser(User u)
        {
            using (var httpClient = new HttpClient())
            {
                // Установка базового URL Jira и заголовков для аутентификации
                httpClient.BaseAddress = new Uri(jiraBaseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Создание запроса JQL для получения задач пользователя b.doroshkov



                //string jqlQuery = "project = ITSD AND reporter = '" + u.email+ "' AND status in (Open, \"In Progress\", Reopened, \"Waiting for support\", Pending, Escalated, \"Waiting for approval\", \"Work in progress\", \"Awaiting CAB approval\", Planning, Implementing, Assigned, \"Assigned to group\", \"Ожидание выполнения\", \"In Progress contractor\", \"Transferred to contractor\", \"Awaiting fin CAB\", \"Cmdb owner approval\") AND issuetype != 'Epic (Проект)'";
                //string jqlQuery = "project in (ITSD, SDTES) AND reporter = '" + u.email + "' AND status in (Open, \"In Progress\", Reopened, \"Waiting for support\", Pending, Escalated, \"Waiting for approval\", \"Work in progress\", \"Awaiting CAB approval\", Planning, Implementing, Assigned, \"Assigned to group\", \"Ожидание выполнения\", \"In Progress contractor\", \"Transferred to contractor\", \"Awaiting fin CAB\", \"Cmdb owner approval\") AND issuetype != 'Epic (Проект)'";
                string jqlQuery = $"project in (ITSD, SD1C, SDNAV) AND issuetype = \"Service Request\" AND status NOT in (Resolved, Closed, Canceled) AND (reporter = \"{u.email}\" OR TelegramID  ~ \"{u.telegram_data.telegram_id}\")";





                // Формирование URL для выполнения запроса к Jira REST API
                string apiEndpoint = "/rest/api/2/search";
                string expandParams = "expand=renderedFields,transitions,editmeta,changelog";
                //string requestUrl = $"{apiEndpoint}?jql={Uri.EscapeDataString(jqlQuery)}";
                string requestUrl = $"{apiEndpoint}?jql={Uri.EscapeDataString(jqlQuery)}&{expandParams}";


                // Выполнение HTTP GET-запроса к Jira
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {// Чтение и десериализация JSON-ответа
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var issues = System.Text.Json.JsonSerializer.Deserialize<JiraIssues>(jsonContent);
                    return issues;
                       
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    return null;
                }
            }
        }
        public static async Task<Comments> GetCommentsForIssue( string issueKey)
        {
            using (var httpClient = new HttpClient())
            {
                // Установка базового URL Jira и заголовков для аутентификации
                httpClient.BaseAddress = new Uri(jiraBaseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Формирование URL для выполнения запроса к Jira REST API для комментариев
                string apiEndpoint = $"/rest/api/2/issue/{issueKey}/comment";

                // Выполнение HTTP GET-запроса к Jira для комментариев задачи
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Чтение и десериализация JSON-ответа
                    string jsonContent = await response.Content.ReadAsStringAsync();
                   

                    // Обработка ответа для извлечения комментариев
                   Comments  myDeserializedClass = JsonConvert.DeserializeObject<Comments>(jsonContent);
                    return myDeserializedClass;
                }
                else
                {
                    Console.WriteLine("Ошибка при получении комментариев.");
                    return null;
                }
            }
        }
        public static async Task<JiraIssue> GetIssueByKey(string issueKey)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Set the base URL for Jira and authentication headers
                    httpClient.BaseAddress = new Uri(jiraBaseUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

                    // Build the URL for the Jira REST API to get information about a specific issue
                    string apiEndpoint = $"/rest/api/2/issue/{issueKey}";

                    // Perform an HTTP GET request to Jira
                    HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read and deserialize the JSON response
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var issue = System.Text.Json.JsonSerializer.Deserialize<JiraIssue>(jsonContent);
                        return issue;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }
        public static async Task<JiraIssueDetails> GetIssueDetailsWithComments(string issueKey)
        {
            using (var httpClient = new HttpClient())
            {
                // Set the base URL for Jira and authentication headers
                httpClient.BaseAddress = new Uri(jiraBaseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Build the URL for the Jira REST API to get issue details along with comments
                string apiEndpoint = $"/rest/api/2/issue/{issueKey}?expand=renderedFields,transitions,editmeta,changelog,comments";

                // Perform an HTTP GET request to Jira
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the JSON response
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var issueDetails = System.Text.Json.JsonSerializer.Deserialize<JiraIssueDetails>(jsonContent);
                    return issueDetails;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return null;
                }
            }
        }
        public static async Task<bool> AddCommentToIssue(string issueKey, string comment)
        {
            using (var httpClient = new HttpClient())
            {
                // Установка базового URL Jira и заголовков для аутентификации
                httpClient.BaseAddress = new Uri(jiraBaseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Формирование URL для выполнения запроса к Jira REST API для комментариев
                string apiEndpoint = $"/rest/api/2/issue/{issueKey}/comment";

                // Создание JSON объекта с данными комментария
                var content = new StringContent(JsonConvert.SerializeObject(new { body = comment }), Encoding.UTF8, "application/json");

                // Выполнение HTTP POST-запроса к Jira для добавления комментария к задаче
                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Комментарий успешно добавлен!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении комментария.");
                }
            }
            return false;
        }
        public static async Task<byte[]> GetFileInJiraComments(string path)
        {
            try
            {
                
                using (var httpClient = new HttpClient())
                {
                    // Set the base URL for Jira and authentication headers
                    httpClient.BaseAddress = new Uri(jiraBaseUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
                    HttpResponseMessage response = await httpClient.GetAsync(path);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                        return fileBytes;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static async Task<ResponseOnCreateJiraTask> CreateNewTaskWithOneImage(long telegram_id, string tema, string text, string avtor, Stream imageStream,string filename)
        {
            using (var client = new HttpClient())
            {
                // Устанавливаем базовый URL для REST API Jira.
                client.BaseAddress = new Uri(jiraBaseUrl);

                // Создаем заголовок для Basic Authentication.
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

                // Создаем объект MultipartFormDataContent для передачи файла
                using (var formData = new MultipartFormDataContent())
                {
                    // Добавляем остальные параметры в JSON
                    var jsonContent = new StringContent(GetJsonWithImage(tema, avtor), Encoding.UTF8, "application/json");
                    formData.Add(jsonContent, "data");

                    // Добавляем файл изображения
                    //var imageContent = new StreamContent(imageStream);
                    //formData.Add(imageContent, "file", "image.jpg");
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();

                        var imageContent = new ByteArrayContent(imageBytes);
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Используйте соответствующий тип контента для вашего изображения
                        formData.Add(imageContent, "file", filename);
                    }




                    // Создаем запрос POST для создания задачи с изображением
                    var response = await client.PostAsync("/rest/api/2/issue", formData);



                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        ResponseOnCreateJiraTask result = JsonConvert.DeserializeObject<ResponseOnCreateJiraTask>(responseContent);
                        Console.WriteLine("Заявка создана");
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode}");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                }
            }

            return null;
        }
        public static async Task<bool> AddPhotoCommentToIssue(string issueKey, byte[]bytes_file, string fileName)
        {
            try
            {
                string url = $"{jiraBaseUrl}/rest/api/2/issue/{issueKey}/attachments";
                var client = new HttpClient();
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

                var header = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = header;
                client.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");

                MultipartFormDataContent multiPartContent = new MultipartFormDataContent("-data-");

                ByteArrayContent byteArrayContent = new ByteArrayContent(bytes_file);

                multiPartContent.Add(byteArrayContent, "file", fileName);

                var response = await client.PostAsync(url, multiPartContent);

                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception(result);
            }
            catch (Exception e)
            {
                // Обработка ошибки
            }
            return false;
        }
      
        private static string GetJsonWithImage(string tema, string avtor)
        {
            // Формируем JSON для создания задачи в Jira без изображения
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.AppendLine("{");
            jsonBuilder.AppendLine("  \"fields\": {");
            jsonBuilder.AppendLine($"    \"project\": {{ \"key\": \"SDTES\" }},");
            jsonBuilder.AppendLine($"    \"summary\": \"{tema}\",");
            jsonBuilder.AppendLine($"    \"issuetype\": {{ \"name\": \"Service Request\" }},");
            jsonBuilder.AppendLine($"    \"labels\": [\"TELEGRAM_BOT\",\"KD_ITSD_bot\"],");
            jsonBuilder.AppendLine($"    \"reporter\": {{ \"name\": \"{avtor}\" }}");
            jsonBuilder.AppendLine("  }");
            jsonBuilder.AppendLine("}");

            return jsonBuilder.ToString();
        }

        private static string GetFileUrl(string fileId)
        {
            return $"{jiraBaseUrl}/rest/api/2/issue/{fileId}/attachments/{fileId}";
        }
    }
   
    public class ResponseOnCreateJiraTask
    {
         public string id { get;set; }
        public string key { get; set; }
        public string self { get; set; }
      

    }

    public class JiraIssues
    {
        public JiraIssue[] issues { get; set; }
    }
    public class JiraIssueDetails
    {
        public JiraIssue Issue { get; set; }
        public Comments Comments { get; set; }
    }
    public class JiraIssue
    {
        public string key { get; set; }
        public JiraIssueFields fields { get; set; }
    }
    public class JiraIssueFields
    {
        public Comments comment { get; set; }
        public List<Attachmentt> attachment { get; set; }
        public string summary { get; set; } // Название задачи
        public string description { get; set; } // Описание задачи
        public JiraStatus status { get; set; } // Статус задачи
        public JiraAssignee assignee { get; set; } // Исполнитель задачи (если есть)
                                                                                  // Другие поля задачи, если они присутствуют
    }
    public class Attachmentt
    {
        public string self { get; set; }
        public string id { get; set; }
        public string filename { get; set; }
        public Author author { get; set; }
        public string created { get; set; }
        public int size { get; set; }
        public string mimeType { get; set; }
        public string content { get; set; }
        public string thumbnail { get; set; }
    }

    public class JiraStatus
    {
        public string name { get; set; } // Название статуса
                                         // Другие свойства статуса, если нужны
    }

    public class JiraAssignee
    {
        public string name { get; set; }
        public string displayName { get; set; } // Имя исполнителя
                                         // Другие свойства исполнителя, если нужны
    }
    public class Comments
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<Comment> comments { get; set; }
    }
    public class Comment
    {
        public string self { get; set; }
        public string id { get; set; }
        public Author author { get; set; }
        public string body { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }
    public class UpdateAuthor
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class Author
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class AvatarUrls
    {
        [JsonProperty("48x48")]
        public string _48x48 { get; set; }

        [JsonProperty("24x24")]
        public string _24x24 { get; set; }

        [JsonProperty("16x16")]
        public string _16x16 { get; set; }

        [JsonProperty("32x32")]
        public string _32x32 { get; set; }
    }
}
