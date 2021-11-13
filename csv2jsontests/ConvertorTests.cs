using System.Collections.Generic;
using csv2jsonlib;
using Newtonsoft.Json;
using Xunit;

namespace csv2jsontests
{
  public class C
  {
    public int P1 { get; set; }
    public string P2 { get; set; }
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
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'Destination': {
                'Path': '$.C',
                'Field':'P1'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path': '$.C',
                'Field':'P2'
              }
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var j = convertor.Convert<J>(instr, csv);
      Assert.Equal(2, j.C.Count);
      Assert.Equal(1, j.C[0].P1);
      Assert.Equal("2", j.C[0].P2);
      Assert.Equal(3, j.C[1].P1);
      Assert.Equal("4", j.C[1].P2);
    }

    [Fact]
    public void TwoLevelObject()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'Destination': {
                'Path': '$.J.C',
                'Field':'P1'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.J.C',
                'Field':'P2'
              }
            }
        ]
      }");
      var csv = @"1,2
3,4";
      var oj = convertor.Convert<OJ>(instr, csv);
      Assert.Equal(2, oj.J.C.Count);
      Assert.Equal(1, oj.J.C[0].P1);
      Assert.Equal("2", oj.J.C[0].P2);
      Assert.Equal(3, oj.J.C[1].P1);
      Assert.Equal("4", oj.J.C[1].P2);
    }

    [Fact]
    public void TwoLevelObjectWithLenValidation()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'Destination': {
                'Path':'$.J.C',
                'Field':'P1',
                'Len': 2
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.J.C',
                'Field':'P2'
              }
            }
        ]
      }");
      var csv = @"1AA,2
3,4";

      var exception = Assert.Throws<ConvertException>(() => convertor.Convert<OJ>(instr, csv));
      Assert.Equal("Length validation in row 0", exception.Errors[0]);
    }
  }

  public class RealisticUnitTest
  {
    [Fact]
    public void TimesheetLoad()
    {
      Convertor convertor = new Convertor();
      var instr = JsonConvert.DeserializeObject<InstructionSet>(@"{
        'Instructions':[
            {
              'SourceColumn':1,
              'Destination': {
                'Path':'$.timesheetEntries',
                'Field':'EmployeeId'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.timesheetEntries',
                'Field':'EmployeeRecord'
              }
            }
        ]
      }");
      var csv = @"11AA,2
33CC,4";

      var tsr = convertor.Convert<TimesheetResponse>(instr, csv);
      Assert.Equal(2, tsr.TimesheetEntries.Count);
      Assert.Equal("11AA", tsr.TimesheetEntries[0].EmployeeId);
      Assert.Equal(2, tsr.TimesheetEntries[0].EmployeeRecord);
      Assert.Equal("33CC", tsr.TimesheetEntries[1].EmployeeId);
      Assert.Equal(4, tsr.TimesheetEntries[1].EmployeeRecord);
    }
  }
}
