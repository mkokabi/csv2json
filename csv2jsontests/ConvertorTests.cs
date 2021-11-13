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
                'Property':'P1'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path': '$.C',
                'Property':'P2'
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
                'Property':'P1'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.J.C',
                'Property':'P2'
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
                'Property':'P1',
                'Len': 2
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.J.C',
                'Property':'P2'
              }
            }
        ]
      }");
      var csv = @"1AA,2
3,4";

      var exception = Assert.Throws<ConvertException>(() => convertor.Convert<OJ>(instr, csv));
      Assert.Equal("Length validation in row 0 column 0", exception.Errors[0]);
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
                'Property':'EmployeeId'
              }
            },
            {
              'SourceColumn':2,
              'Destination': {
                'Path':'$.timesheetEntries',
                'Property':'EmployeeRecord'
              }
            },
            {
              'SourceColumn':3,
              'Destination': {
                'Path':'$.timesheetEntries',
                'Property':'TL_QUANTITY',
                'Object': 'fields'
              }
            }
        ]
      }");
      var csv = @"11AA,2,5
33CC,4,6";

      var tsr = convertor.Convert<TimesheetResponse>(instr, csv);
      Assert.Equal(2, tsr.TimesheetEntries.Count);
      Assert.Equal("11AA", tsr.TimesheetEntries[0].EmployeeId);
      Assert.Equal(2, tsr.TimesheetEntries[0].EmployeeRecord);
      Assert.Equal("5", tsr.TimesheetEntries[0].Fields["TL_QUANTITY"]);
      Assert.Equal("33CC", tsr.TimesheetEntries[1].EmployeeId);
      Assert.Equal(4, tsr.TimesheetEntries[1].EmployeeRecord);
      Assert.Equal("6", tsr.TimesheetEntries[1].Fields["TL_QUANTITY"]);
    }
  }
}
