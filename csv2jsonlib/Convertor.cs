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
      var errors = new List<string>();
      for (var i = 0; i < tbl.Count; i++)
      {
        var row = tbl[i];
        var properties = new List<JProperty>();
        JArray jArray = new JArray();
        for (int j = 0; j < row.Length; j++)
        {
          var cell = row[j];
          var sourceColumn = inst.Instructions[j].SourceColumn;
          var destField = inst.Instructions[j].DestinationField;
          var destLen = inst.Instructions[j].DestinationLen;
          var destPath = inst.Instructions[j].DestinationPath;
          jArray = (jo.SelectToken(destPath) as JArray);
          if (destLen.HasValue && row[sourceColumn - 1].Length > destLen)
          {
            errors.Add($"Length validation in row {i}");
            continue;
          }
          properties.Add(new JProperty(destField, row[sourceColumn - 1]));
        }
        jArray.Add(new JObject(properties.ToArray()));
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
