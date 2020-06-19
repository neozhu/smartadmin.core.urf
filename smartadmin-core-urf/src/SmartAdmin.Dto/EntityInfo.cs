using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAdmin.Dto
{
  public partial class EntityInfo
  {
    public virtual string EntitySetName { get; set; }
    public virtual string FieldName { get; set; }
    public virtual string DisplayName { get; set; }
    public virtual string FieldTypeName { get; set; }
    public virtual bool IsRequired { get; set; }
    public virtual string DefaultValue { get; set; }
    public virtual bool IgnoredColumn { get; set; }
  }
}
