using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class MenuItem : Entity
  {
    public MenuItem()
    {
      Children = new HashSet<MenuItem>();
    }

    [Display(Name ="标题栏",Description = "标题栏")]
    [MaxLength(38)]
    [Required]
    public virtual string Title { get; set; }
    [Display(Name = "描述", Description = "描述")]
    [MaxLength(128)]
    public virtual string Description { get; set; }
    [Display(Name = "序号", Description = "序号")]
    [MaxLength(5)]
    [Required]
    public virtual string LineNum { get; set; }
    [Display(Name = "url", Description = "url")]
    [MaxLength(256)]
    [Required]
    public virtual string Url { get; set; }
    [Display(Name = "controller", Description = "controller")]
    [MaxLength(128)]
    public virtual string Controller { get; set; }
    [Display(Name = "action", Description = "action")]
    [MaxLength(128)]
    public virtual string Action { get; set; }
    [Display(Name = "icon", Description = "icon")]
    [StringLength(30)]
    public virtual string Icon { get; set; }
  
    [Display(Name = "target", Description = "target")]
    [MaxLength(128)]
    [DefaultValue("_self")]
    public virtual string Target { get; set; }
    [Display(Name = "授权", Description = "授权")]
    [MaxLength(256)]
    public virtual string Roles { get; set; }
    [Display(Name = "是否启用", Description = "是否启用")]
    [DefaultValue(true)]
    public virtual  bool IsEnabled { get; set; }
    [Display(Name = "下级菜单", Description = "下级菜单")]
    public virtual ICollection<MenuItem> Children { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    public virtual  int? ParentId { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    [ForeignKey("ParentId")]
    public virtual MenuItem Parent { get; set; }
  }
}
