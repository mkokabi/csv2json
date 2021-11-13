namespace csv2jsontests
{
  public partial class TimesheetResponse
  {
    [Newtonsoft.Json.JsonProperty("correlationId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid? CorrelationId { get; set; }

    [Newtonsoft.Json.JsonProperty("timesheetEntries", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.List<TimesheetEntry> TimesheetEntries { get; set; } = new System.Collections.Generic.List<TimesheetEntry>();

    [Newtonsoft.Json.JsonProperty("sessionNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public long? SessionNumber { get; set; }

  }

  public partial class TimesheetEntry
  {

    [Newtonsoft.Json.JsonProperty("lineNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int LineNumber { get; set; }

    [Newtonsoft.Json.JsonProperty("employeeId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string EmployeeId { get; set; }

    [Newtonsoft.Json.JsonProperty("employeeRecord", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? EmployeeRecord { get; set; }

    [Newtonsoft.Json.JsonProperty("punchDateTime", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? PunchDateTime { get; set; }

    [Newtonsoft.Json.JsonProperty("punchType", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public TimesheetEntryPunchType? PunchType { get; set; } = TimesheetEntryPunchType._0;

    [Newtonsoft.Json.JsonProperty("fields", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.IDictionary<string, object> Fields { get; set; } = new System.Collections.Generic.Dictionary<string, object>();
  }

  public enum TimesheetEntryPunchType
  {
    _0 = 0,

    _1 = 1,

    _2 = 2,

    _3 = 3,

    _4 = 4,

    _5 = 5,

  }
}