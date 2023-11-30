
using Newtonsoft.Json;
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
        public static async Task<ResponseOnCreateJiraTask> CreateNewTask(string tema,string text,string avtor,string groops)
        {
            string escapedText = text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");

            string issueJson = @"{
    ""fields"": {
        ""project"": { ""key"": ""ITSD"" },
        ""summary"":""" + tema + @""",
        ""description"": """ + escapedText + @""",
        ""issuetype"": { ""name"": ""Service Request"" },
        ""customfield_10300"": [{""name"":""2nd Line Research And Development""}],
        ""labels"": [""TELEGRAM_BOT"",""KD_ITSD_bot""],
        ""reporter"": { ""name"": """ + avtor + @""" }
    }
}";


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



                string jqlQuery = "project = ITSD AND reporter = '" + u.email+ "' AND status in (Open, \"In Progress\", Reopened, \"Waiting for support\", Pending, Escalated, \"Waiting for approval\", \"Work in progress\", \"Awaiting CAB approval\", Planning, Implementing, Assigned, \"Assigned to group\", \"Ожидание выполнения\", \"In Progress contractor\", \"Transferred to contractor\", \"Awaiting fin CAB\", \"Cmdb owner approval\") AND issuetype != 'Epic (Проект)'";






                // Формирование URL для выполнения запроса к Jira REST API
                string apiEndpoint = "/rest/api/2/search";
                string requestUrl = $"{apiEndpoint}?jql={Uri.EscapeDataString(jqlQuery)}";

                // Выполнение HTTP GET-запроса к Jira
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {// Чтение и десериализация JSON-ответа
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var issues = System.Text.Json.JsonSerializer.Deserialize<JiraIssues>(jsonContent);
                    return issues;
                    //foreach (var issue in issues.issues)
                    //{
                        //string issueKey = issue.key;
                        //string summary = issue.fields.summary;
                        //string description = issue.fields.description;

                        // Для каждой задачи выполните дополнительный запрос, чтобы получить комментарии
                       // string comments = await GetCommentsForIssue( issueKey);

                       // Console.WriteLine($"Key: {issueKey}, Summary: {summary}, Description: {description}");
                       // Console.WriteLine("Comments:");
                        //Console.WriteLine(comments);
                   // }
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    return null;
                }
            }
        }
        private static async Task<string> GetCommentsForIssue( string issueKey)
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
                    var comments = System.Text.Json.JsonSerializer.Deserialize<JiraComments>(jsonContent);

                    // Соберите комментарии в строку
                    string commentText = string.Join("\n", comments.comments.Select(comment => comment.body));

                    return commentText;
                }
                else
                {
                    return "Ошибка при получении комментариев.";
                }
            }
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

    public class JiraIssue
    {
        public string key { get; set; }
        public JiraIssueFields fields { get; set; }
    }
    public class JiraIssueFields
    {
        public string summary { get; set; } // Название задачи
        public string description { get; set; } // Описание задачи
                                                // Другие поля задачи, если они присутствуют
    }

    public class JiraComments
    {
        public List<JiraComment> comments { get; set; }
    }

    public class JiraComment
    {
        public string body { get; set; }
        // Дополнительные поля комментария, если необходимо
    }

}
