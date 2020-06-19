using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAdmin.Dto
{
  public class ExpColumnOpts
  {
    public virtual string EntitySetName { get; set; }
    public virtual string FieldName { get; set; }
    public virtual string SourceFieldName { get; set; }
    public virtual bool IgnoredColumn { get; set; }
  }
}
