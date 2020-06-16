using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class Notification : Entity
  {

    [Display(Name = "主题", Description = "主题")]
    [Required]
    [MaxLength(128)]
    public string Title { get; set; }
    [Display(Name = "消息内容", Description = "消息内容")]
    public string Content { get; set; }
    [Display(Name = "链接", Description = "链接")]
    [MaxLength(256)]
    public string Link { get; set; }
    [Display(Name = "已读", Description = "已读")]
    [DefaultValue(false)]
    public bool Read { get; set; }
    [Display(Name = "From", Description = "From")]
    [DefaultValue("user")]
    public string From { get; set; }
    [Display(Name = "To", Description = "From")]
    [DefaultValue("user")]
    public string To { get; set; }
    [Display(Name = "分组", Description = "分组")]
    [MaxLength(20)]
    public string Group { get; set; }
    [Display(Name = "发出时间", Description = "发出时间")]
    [DefaultValue("now")]
    public DateTime PublishDate { get; set; }



  }
}
