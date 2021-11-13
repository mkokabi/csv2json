using System.Collections.Generic;
using csv2jsonlib;
using Newtonsoft.Json;
using Xunit;

namespace csv2jsontests
{
  public class convertor
  {
    public int P1 { get; set; }
    public string P2 { get; set; }
  }

  public class J
  {
    public List<convertor> convertor { get; set; } = new List<convertor>();
  }

  public class OJ
  {
    public J J { get; set; } = new J();
  }

  public class UnitTest1
  {
    [Fact]
    public void OneLevelObject()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'DestinationPath':'$.convertor',
              'DestinationField':'P1'
            },
            {
              'SourceColumn':2,
              'DestinationPath':'$.convertor',
              'DestinationField':'P2'
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var j = convertor.Convert<J>(instr, csv);
      Assert.Equal(2, j.convertor.Count);
      Assert.Equal(1, j.convertor[0].P1);
      Assert.Equal("2", j.convertor[0].P2);
      Assert.Equal(3, j.convertor[1].P1);
      Assert.Equal("4", j.convertor[1].P2);
    }

    [Fact]
    public void TwoLevelObject()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'DestinationPath':'$.J.convertor',
              'DestinationField':'P1'
            },
            {
              'SourceColumn':2,
              'DestinationPath':'$.J.convertor',
              'DestinationField':'P2'
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var oj = convertor.Convert<OJ>(instr, csv);
      Assert.Equal(2, oj.J.convertor.Count);
      Assert.Equal(1, oj.J.convertor[0].P1);
      Assert.Equal("2", oj.J.convertor[0].P2);
      Assert.Equal(3, oj.J.convertor[1].P1);
      Assert.Equal("4", oj.J.convertor[1].P2);
    }

    [Fact]
    public void TwoLevelObjectWithLenValidation()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'DestinationPath':'$.J.convertor',
              'DestinationField':'P1',
              'DestinationLen': 2
            },
            {
              'SourceColumn':2,
              'DestinationPath':'$.J.convertor',
              'DestinationField':'P2'
            }
        ]
      }");
      var csv = @"1AA,2
3,4";

      var exception = Assert.Throws<ConvertException>(() => convertor.Convert<OJ>(instr, csv));
      Assert.Equal("Length validation in row 0", exception.Errors[0]);
    }
  }
}
