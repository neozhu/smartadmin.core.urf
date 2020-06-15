using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class Log : Entity
  {
    [Display(Name = "主机名", Description = "主机名")]
    [MaxLength(128)]
    public string MachineName { get; set; }
    [Display(Name = "时间", Description = "时间")]
    public DateTime? Logged { get; set; }
    [Display(Name = "级别", Description = "级别")]
    [MaxLength(5)]
    public string Level { get; set; }
    [Display(Name = "信息", Description = "信息")]
    public string Message { get; set; }
    [Display(Name = "异常信息", Description = "异常信息")]
    public string Exception { get; set; }
    [Display(Name = "请求IP", Description = "请求IP")]
    [MaxLength(32)]
    public string RequestIp { get; set; }
    [Display(Name = "事件属性", Description = "事件属性")]
    public string Properties { get; set; }
    [Display(Name = "请求表单值", Description = "请求表单值")]
    public string RequestForm { get; set; }
    [Display(Name = "账号", Description = "账号")]
    [MaxLength(128)]
    public string Identity { get; set; }
    [Display(Name = "日志记录器", Description = "日志记录器")]
    [MaxLength(256)]
    public string Logger { get; set; }
    [Display(Name = "日志来源", Description = "日志来源")]
    [MaxLength(256)]
    public string Callsite { get; set; }
    [Display(Name = "网站名称", Description = "网站名称")]
    [MaxLength(128)]
    public string SiteName { get; set; }
    [Display(Name = "已处理", Description = "已处理")]
    [DefaultValue(false)]
    public bool Resolved { get; set; }
  }
}
