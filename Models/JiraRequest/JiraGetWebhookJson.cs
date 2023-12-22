using Newtonsoft.Json;

namespace KernelHelpBot.Models.JiraRequest
{
   
    public class JiraGetWebhookJson
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public Changelog changelog { get; set; }
        public Fields fields { get; set; }
        public RenderedFields renderedFields { get; set; }
    }
   
  
    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Assignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public object accountId { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public object groups { get; set; }
        public object locale { get; set; }
    }

  

    public class BreachTime
    {
        public DateTime iso8601 { get; set; }
        public DateTime jira { get; set; }
        public string friendly { get; set; }
        public long epochMillis { get; set; }
    }

    public class Changelog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public object histories { get; set; }
    }

    public class Child
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class CompletedCycle
    {
        public StartTime startTime { get; set; }
        public StopTime stopTime { get; set; }
        public bool breached { get; set; }
        public GoalDuration goalDuration { get; set; }
        public ElapsedTime elapsedTime { get; set; }
        public RemainingTime remainingTime { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public object accountId { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public object groups { get; set; }
        public object locale { get; set; }
    }

    public class Customfield10110
    {
        public string id { get; set; }
        public string name { get; set; }
        public Links _links { get; set; }
        public List<object> completedCycles { get; set; }
        public OngoingCycle ongoingCycle { get; set; }
    }

    public class Customfield10111
    {
        public string id { get; set; }
        public string name { get; set; }
        public Links _links { get; set; }
        public List<CompletedCycle> completedCycles { get; set; }
    }

    public class Customfield10216
    {
        public string id { get; set; }
        public string name { get; set; }
        public Links _links { get; set; }
        public List<object> completedCycles { get; set; }
    }

    public class Customfield10217
    {
        public string id { get; set; }
        public string name { get; set; }
        public Links _links { get; set; }
        public List<object> completedCycles { get; set; }
    }

    public class Customfield10303
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield12900
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield13000
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14402
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14403
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14404
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14405
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14406
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield14900
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

    public class Customfield15000
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class Customfield15001
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
        public Child child { get; set; }
    }

    public class Customfield15200
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

    public class ElapsedTime
    {
        public int millis { get; set; }
        public string friendly { get; set; }
    }

    public class Fields
    {
        public Customfield15000 customfield_15000 { get; set; }
        public Customfield15001 customfield_15001 { get; set; }
        public object customfield_13100 { get; set; }
        public string customfield_15400 { get; set; }
        public List<object> fixVersions { get; set; }
        public Customfield10110 customfield_10110 { get; set; }
        public Customfield10111 customfield_10111 { get; set; }
        public object customfield_13500 { get; set; }
        public object customfield_15800 { get; set; }
        public object resolution { get; set; }
        public object customfield_10112 { get; set; }
        public object customfield_15801 { get; set; }
        public object customfield_10107 { get; set; }
        public object customfield_10108 { get; set; }
        public object customfield_10109 { get; set; }
        public string lastViewed { get; set; }
        public object customfield_16200 { get; set; }
        public object customfield_14300 { get; set; }
        public object customfield_14301 { get; set; }
        public object customfield_16201 { get; set; }
        public Priority priority { get; set; }
        public object customfield_10100 { get; set; }
        public object customfield_14700 { get; set; }
        public List<string> labels { get; set; }
        public object customfield_10214 { get; set; }
        public object customfield_15903 { get; set; }
        public object customfield_10215 { get; set; }
        public object customfield_15904 { get; set; }
        public Customfield10216 customfield_10216 { get; set; }
        public object customfield_11702 { get; set; }
        public object customfield_15901 { get; set; }
        public Customfield10217 customfield_10217 { get; set; }
        public object customfield_15902 { get; set; }
        public object customfield_15907 { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public object timeestimate { get; set; }
        public List<object> versions { get; set; }
        public object customfield_15908 { get; set; }
        public object customfield_15905 { get; set; }
        public object customfield_15906 { get; set; }
        public List<object> issuelinks { get; set; }
        public Assignee assignee { get; set; }
        public Status status { get; set; }
        public List<object> components { get; set; }
        public string customfield_17000 { get; set; }
        public object customfield_15500 { get; set; }
        public object customfield_13200 { get; set; }
        public object customfield_10210 { get; set; }
        public object customfield_10211 { get; set; }
        public object customfield_15900 { get; set; }
        public object customfield_10212 { get; set; }
        public object customfield_11301 { get; set; }
        public object customfield_10213 { get; set; }
        public object customfield_13600 { get; set; }
        public object customfield_10203 { get; set; }
        public object customfield_10204 { get; set; }
        public object customfield_10205 { get; set; }
        public Customfield14405 customfield_14405 { get; set; }
        public Customfield12900 customfield_12900 { get; set; }
        public object customfield_10206 { get; set; }
        public object customfield_10602 { get; set; }
        public Customfield14406 customfield_14406 { get; set; }
        public object customfield_10603 { get; set; }
        public object customfield_10207 { get; set; }
        public object aggregatetimeestimate { get; set; }
        public object customfield_10604 { get; set; }
        public object customfield_10208 { get; set; }
        public object customfield_10209 { get; set; }
        public Creator creator { get; set; }
        public List<object> subtasks { get; set; }
        public object customfield_12101 { get; set; }
        public Reporter reporter { get; set; }
        public object customfield_12100 { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public Customfield14403 customfield_14403 { get; set; }
        public object customfield_10200 { get; set; }
        public Customfield14404 customfield_14404 { get; set; }
        public object customfield_10201 { get; set; }
        public object customfield_16701 { get; set; }
        public object customfield_12500 { get; set; }
        public object customfield_16700 { get; set; }
        public Customfield14402 customfield_14402 { get; set; }
        public object customfield_16304 { get; set; }
        public object customfield_13702 { get; set; }
        public object customfield_13704 { get; set; }
        public string customfield_10712 { get; set; }
        public object customfield_13703 { get; set; }
        public object customfield_10713 { get; set; }
        public object customfield_11803 { get; set; }
        public object customfield_13705 { get; set; }
        public Progress progress { get; set; }
        public Votes votes { get; set; }
        public Issuetype issuetype { get; set; }
        public object timespent { get; set; }
        public Project project { get; set; }
        public List<Customfield15200> customfield_15200 { get; set; }
        public object aggregatetimespent { get; set; }
        public object customfield_15600 { get; set; }
        public Customfield10303 customfield_10303 { get; set; }
        public object customfield_10700 { get; set; }
        public object customfield_10304 { get; set; }
        public List<Customfield14900> customfield_14900 { get; set; }
        public object customfield_10703 { get; set; }
        public object customfield_10704 { get; set; }
        public object resolutiondate { get; set; }
        public object customfield_10705 { get; set; }
        public object customfield_10706 { get; set; }
        public object customfield_10707 { get; set; }
        public int workratio { get; set; }
        public object customfield_10708 { get; set; }
        public Watches watches { get; set; }
        public object customfield_16000 { get; set; }
        public long created { get; set; }
        public object customfield_16401 { get; set; }
        public object customfield_14100 { get; set; }
        public object customfield_16400 { get; set; }
        public object customfield_14101 { get; set; }
        public object customfield_10300 { get; set; }
        public object customfield_16800 { get; set; }
        public object customfield_13801 { get; set; }
        public object customfield_11501 { get; set; }
        public object customfield_13800 { get; set; }
        public object customfield_10810 { get; set; }
        public object customfield_11900 { get; set; }
        public object customfield_10811 { get; set; }
        public object customfield_10937 { get; set; }
        public long updated { get; set; }
        public object timeoriginalestimate { get; set; }
        public string description { get; set; }
        public Customfield13000 customfield_13000 { get; set; }
        public object customfield_15300 { get; set; }
        public Timetracking timetracking { get; set; }
        public object customfield_15700 { get; set; }
        public object customfield_10801 { get; set; }
        public object customfield_10802 { get; set; }
        public object customfield_10803 { get; set; }
        public object customfield_10804 { get; set; }
        public object customfield_10807 { get; set; }
        public object customfield_10808 { get; set; }
        public object customfield_10809 { get; set; }
        public string summary { get; set; }
        public string customfield_16101 { get; set; }
        public object customfield_16100 { get; set; }
        public object customfield_16501 { get; set; }
        public object customfield_16500 { get; set; }
        public List<object> customfield_10000 { get; set; }
        public object customfield_14200 { get; set; }
        public object customfield_10001 { get; set; }
        public List<object> customfield_10002 { get; set; }
        public object customfield_12300 { get; set; }
        public object customfield_16900 { get; set; }
        public object customfield_16503 { get; set; }
        public object customfield_10400 { get; set; }
        public object customfield_16502 { get; set; }
        public object customfield_13900 { get; set; }
        public object customfield_11600 { get; set; }
        public object environment { get; set; }
        public object customfield_13902 { get; set; }
        public object customfield_13901 { get; set; }
        public object customfield_13903 { get; set; }
        public object duedate { get; set; }
    }

    public class GoalDuration
    {
        public int millis { get; set; }
        public string friendly { get; set; }
    }

    public class Issuetype
    {
        public string self { get; set; }
        public int id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public object fields { get; set; }
        public List<object> statuses { get; set; }
        public string namedValue { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }

    public class OngoingCycle
    {
        public StartTime startTime { get; set; }
        public BreachTime breachTime { get; set; }
        public bool breached { get; set; }
        public bool paused { get; set; }
        public bool withinCalendarHours { get; set; }
        public GoalDuration goalDuration { get; set; }
        public ElapsedTime elapsedTime { get; set; }
        public RemainingTime remainingTime { get; set; }
    }

    public class Priority
    {
        public string self { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
        public string namedValue { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Project
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public object issuetypes { get; set; }
        public object projectCategory { get; set; }
        public object email { get; set; }
        public object lead { get; set; }
        public object components { get; set; }
        public object versions { get; set; }
        public string projectTypeKey { get; set; }
        public bool simplified { get; set; }
    }

    public class RemainingTime
    {
        public int millis { get; set; }
        public string friendly { get; set; }
    }

    public class RenderedFields
    {
    }

    public class Reporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public object accountId { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public object groups { get; set; }
        public object locale { get; set; }
    }

   

    public class StartTime
    {
        public DateTime iso8601 { get; set; }
        public DateTime jira { get; set; }
        public string friendly { get; set; }
        public long epochMillis { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class StopTime
    {
        public DateTime iso8601 { get; set; }
        public DateTime jira { get; set; }
        public string friendly { get; set; }
        public long epochMillis { get; set; }
    }

    public class Timetracking
    {
        public object originalEstimate { get; set; }
        public object remainingEstimate { get; set; }
        public object timeSpent { get; set; }
        public int originalEstimateSeconds { get; set; }
        public int remainingEstimateSeconds { get; set; }
        public int timeSpentSeconds { get; set; }
    }

    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }


}
