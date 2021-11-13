using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Newtonsoft.Json;
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
      using (var stringReader = new StringReader(csv))
      using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
      {
        var result = new List<List<string>>();
        while (csvReader.Read())
        {
          var list = new List<String>();
          for (int i = 0; i < csvReader.Parser.Count; i++)
          {
              list.Add(csvReader[i]);
          }
          result.Add(list);
        }
        return result;
      }
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

      return JsonConvert.DeserializeObject<T>(jo.ToString());
    }
  }
}
