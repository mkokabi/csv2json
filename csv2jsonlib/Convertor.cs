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
    public Destination Destination { get ;set; }
  }

  public class Destination
  {
    public string Path { get; set; }
    public string Property { get; set; }
    public string Object { get; set; }
    public int? Len { get; set; }
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
          var sourceColumn = inst.Instructions[j].SourceColumn;
          var dest = inst.Instructions[j].Destination;
          jArray = (jo.SelectToken(dest.Path) as JArray);
          if (dest.Len.HasValue && row[sourceColumn - 1].Length > dest.Len)
          {
            errors.Add($"Length validation in row {i}");
            continue;
          }
          if (dest.Object == null)
          {
            properties.Add(new JProperty(dest.Property, row[sourceColumn - 1]));
          }
          else
          {
            properties.Add(new JProperty(dest.Object, 
              new JObject(new JProperty(dest.Property, row[sourceColumn - 1]))));
          }
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
