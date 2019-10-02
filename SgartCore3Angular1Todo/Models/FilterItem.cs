using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SgartCoreAngular1Todo.Models
{
  public class FilterItem
  {
    public int ID { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
    public string Text { get; set; }
    public int IDCategory { get; set; }
    public int Status { get; set; }
    public string Sort { get; set; }
  }
}
