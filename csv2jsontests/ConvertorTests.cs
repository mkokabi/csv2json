using System.Collections.Generic;
using csv2jsonlib;
using Newtonsoft.Json;
using Xunit;

namespace csv2jsontests
{
  public class C
  {
    public int P1 { get; set; }
    public int P2 { get; set; }
  }

  public class J
  {
    public List<C> C { get; set; } = new List<C>();
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
      Convertor c = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'DestinationPath':'$.C',
              'DestinationField':'P1'
            },
            {
              'SourceColumn':2,
              'DestinationPath':'$.C',
              'DestinationField':'P2'
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var j = c.Convert<J>(instr, csv);
      Assert.Equal(2, j.C.Count);
      Assert.Equal(1, j.C[0].P1);
      Assert.Equal(2, j.C[0].P2);
      Assert.Equal(3, j.C[1].P1);
      Assert.Equal(4, j.C[1].P2);
    }

    [Fact]
    public void TwoLevelObject()
    {
      Convertor c = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'DestinationPath':'$.J.C',
              'DestinationField':'P1'
            },
            {
              'SourceColumn':2,
              'DestinationPath':'$.J.C',
              'DestinationField':'P2'
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var oj = c.Convert<OJ>(instr, csv);
      Assert.Equal(1, oj.J.C[0].P1);
      Assert.Equal(2, oj.J.C[0].P2);
      Assert.Equal(3, oj.J.C[1].P1);
      Assert.Equal(4, oj.J.C[1].P2);
      Assert.Equal(2, oj.J.C.Count);
    }
  }
}
