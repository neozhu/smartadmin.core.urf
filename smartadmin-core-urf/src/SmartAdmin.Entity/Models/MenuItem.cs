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
    public string Title { get; set; }
    [Display(Name = "描述", Description = "描述")]
    [MaxLength(128)]
    public string Description { get; set; }
    [Display(Name = "序号", Description = "序号")]
    [MaxLength(5)]
    [Required]
    public string LineNum { get; set; }
    [Display(Name = "url", Description = "url")]
    [MaxLength(256)]
    [Required]
    public string Url { get; set; }
    [Display(Name = "controller", Description = "controller")]
    [MaxLength(128)]
    public string Controller { get; set; }
    [Display(Name = "action", Description = "action")]
    [MaxLength(128)]
    public string Action { get; set; }
    [Display(Name = "icon", Description = "icon")]
    [StringLength(30)]
    public string Icon { get; set; }
  
    [Display(Name = "target", Description = "target")]
    [MaxLength(128)]
    [DefaultValue("_self")]
    public string Target { get; set; }
    [Display(Name = "授权", Description = "授权")]
    [MaxLength(256)]
    public string Roles { get; set; }
    [Display(Name = "是否启用", Description = "是否启用")]
    [DefaultValue(true)]
    public bool IsEnabled { get; set; }
    [Display(Name = "下级菜单", Description = "下级菜单")]
    public ICollection<MenuItem> Children { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    public int? ParentId { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    [ForeignKey("ParentId")]
    public MenuItem Parent { get; set; }
  }
}
