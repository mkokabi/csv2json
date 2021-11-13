using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace csv2jsonlib
{
  public class Instruction
  {
    public int SourceColumn { get; set; }
    public string DestinationPath { get; set; }

    public string DestinationField { get; set; }
  }
  public class InstructionSet
  {
    public Instruction[] Instructions { get; set; }
  }

  public class Convertor
  {
    private List<List<string>> CsvToTable(string csv)
    {
      var result = new List<List<string>>();
      var rows = csv.Split(Environment.NewLine);
      foreach (var row in rows)
      {
        var list = new List<String>();
        var cells = row.Split(",");
        foreach (var cell in cells)
        {
          list.Add(cell);
        }
        result.Add(list);
      }
      return result;
    }

    public T Convert<T>(InstructionSet inst, string csv) where T : class, new()
    {
      var tbl = CsvToTable(csv);
      T result = new T();

      JObject jo = JObject.FromObject(result);
      int i = 0;
      var dp = inst.Instructions[i].DestinationPath;
      foreach (var row in tbl)
      {
        var lo = new List<JProperty>();
        int j = 0;
        JToken jt = jo.SelectToken(dp);
        JArray ja = (jt as JArray);
        foreach (var cell in row)
        {
          var s = inst.Instructions[j].SourceColumn;
          var df = inst.Instructions[j++].DestinationField;
          JProperty jp = new JProperty(df, row[s - 1]);
          lo.Add(jp);
        }
        ja.Add(new JObject(lo.ToArray()));
      }

      return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jo.ToString());
    }
  }
}
