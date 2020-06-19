using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class RoleMenu : Entity
  {
  
    [Display(Name ="授权角色",Description = "授权角色")]
    [MaxLength(128)]
    [Required]
    public virtual string RoleName { get; set; }

    [Display(Name = "导航栏", Description = "导航栏")]
    public virtual  int MenuId { get; set; }
    [Display(Name = "导航栏", Description = "导航栏")]
    [ForeignKey("MenuId")]
    public virtual MenuItem MenuItem { get; set; }
    [Display(Name = "是否开启", Description = "是否开启")]
    [DefaultValue(true)]
    public virtual  bool IsEnabled { get; set; }
    [Display(Name = "新增", Description = "新增")]
    [DefaultValue(true)]
    public virtual  bool Create { get; set; }
    [Display(Name = "编辑", Description = "编辑")]
    [DefaultValue(true)]
    public virtual  bool Edit { get; set; }
    [Display(Name = "删除", Description = "删除")]
    [DefaultValue(true)]
    public virtual  bool Delete { get; set; }
    [Display(Name = "导入", Description = "导入")]
    [DefaultValue(true)]
    public virtual  bool Import { get; set; }
    [Display(Name = "导出", Description = "导出")]
    [DefaultValue(true)]
    public virtual  bool Export { get; set; }
    [Display(Name = "功能点1", Description = "功能点1")]
    [DefaultValue(true)]
    public virtual  bool FunctionPoint1 { get; set; }
    [Display(Name = "功能点2", Description = "功能点2")]
    [DefaultValue(true)]
    public virtual  bool FunctionPoint2 { get; set; }
    [Display(Name = "功能点3", Description = "功能点3")]
    [DefaultValue(true)]
    public virtual  bool FunctionPoint3 { get; set; }
  }
}

