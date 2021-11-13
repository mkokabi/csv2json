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

    public int? DestinationLen { get ;set; }
  }
  public class InstructionSet
  {
    public Instruction[] Instructions { get; set; }
  }

  public class Convertor
  {
    private List<string[]> CsvToTable(string csv)
    {
      using (var stringReader = new StringReader(csv))
      using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
      {
        var result = new List<string[]>();
        while (csvReader.Read())
        {
          var row = new string[csvReader.Parser.Count];
          for (int i = 0; i < csvReader.Parser.Count; i++)
          {
              row[i] = csvReader[i];
          }
          result.Add(row);
        }
        return result;
      }
    }

    public T Convert<T>(InstructionSet inst, string csv) where T : class, new()
    {
      var tbl = CsvToTable(csv);
      T result = new T();

      JObject jo = JObject.FromObject(result);
      // int i = 0;
      var dp = inst.Instructions[0].DestinationPath;
      var errors = new List<string>();
      for (var i = 0; i < tbl.Count; i++)
      // foreach (var row in tbl)
      {
        var row = tbl[i];
        var lo = new List<JProperty>();
        int j = 0;
        JToken jt = jo.SelectToken(dp);
        JArray ja = (jt as JArray);
        foreach (var cell in row)
        {
          var s = inst.Instructions[j].SourceColumn;
          var df = inst.Instructions[j].DestinationField;
          var dl = inst.Instructions[j++].DestinationLen;
          if (dl.HasValue && row[s - 1].Length > dl)
          {
            errors.Add($"Length validation in row {i}");
            continue;
          }
          JProperty jp = new JProperty(df, row[s - 1]);
          lo.Add(jp);
        }
        ja.Add(new JObject(lo.ToArray()));
      }
      if (errors.Count > 0)
      {
        throw new ConvertException { Errors = errors };
      }

      return JsonConvert.DeserializeObject<T>(jo.ToString());
    }
  }

  public class ConvertException : Exception
  {
    public List<string> Errors { get; set; } = new List<string>();
  }
}
