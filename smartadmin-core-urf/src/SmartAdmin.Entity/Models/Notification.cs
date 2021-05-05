using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Domain.Models
{
  public partial class Notification : Entity
  {

    [Display(Name = "主题", Description = "主题")]
    [Required]
    [MaxLength(128)]
    public virtual string Title { get; set; }
    [Display(Name = "消息内容", Description = "消息内容")]
    public virtual string Content { get; set; }
    [Display(Name = "链接", Description = "链接")]
    [MaxLength(256)]
    public virtual string Link { get; set; }
    [Display(Name = "已读", Description = "已读")]
    [DefaultValue(false)]
    public virtual  bool Read { get; set; }
    [Display(Name = "From", Description = "From")]
    public virtual string From { get; set; }
    [Display(Name = "To", Description = "From")]
    public virtual string To { get; set; }
    [Display(Name = "分组", Description = "分组")]
    [MaxLength(20)]
    public virtual string Group { get; set; }
    [Display(Name = "发出时间", Description = "发出时间")]
    [DefaultValue("now")]
    public virtual  DateTime PublishDate { get; set; }
    [Display(Name = "发布者", Description = "发布者")]
    [MaxLength(128)]
    public virtual string Publisher { get; set; }



  }
}
