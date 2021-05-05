using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Domain.Models
{
  public partial class DataTableImportMapping : Entity
  {

    [Required]
    [MaxLength(50)]
    [Display(Name = "实体名称", Description = "实体名称")]
    public virtual string EntitySetName { get; set; }
    [Required]
    [MaxLength(50)]
    [Display(Name = "字段名", Description = "字段名")]
    public virtual string FieldName { get; set; }
    [MaxLength(50)]
    [Display(Name = "默认值", Description = "默认值")]
    public virtual string DefaultValue { get; set; }
    [Display(Name = "类型", Description = "类型")]
    [MaxLength(30)]
    public virtual string TypeName { get; set; }
    [Display(Name = "是否必填", Description = "是否必填")]
    [DefaultValue(false)]
    public virtual  bool IsRequired { get; set; }
    [MaxLength(50)]
    [Display(Name = "Excel列名", Description = "Excel列名")]
    public virtual string SourceFieldName { get; set; }
    [Display(Name = "是否导入", Description = "是否导入")]
    [DefaultValue(true)]
    public virtual  bool IsEnabled { get; set; }
    [Display(Name = "是否导出", Description = "是否导出")]
    [DefaultValue(false)]
    public virtual  bool Exportable { get; set; }

    [Display(Name = "验证表达式", Description = "验证表达式")]
    [MaxLength(100)]
    public virtual string RegularExpression { get; set; }
    [Display(Name = "序号", Description = "序号")]
    public virtual int LineNo { get; set; }

  }
  
  }
   
