using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Domain.Models
{
  public partial class Photo:Entity
  {
    [Required]
    [Display(Name = "名称", Description = "名称", Prompt = "名称")]
    [MaxLength(128)]
    public string Name { get; set; }
    [Required]
    [Display(Name = "路径", Description = "路径", Prompt = "路径")]
    [MaxLength(512)]
    public string Path { get; set;}
    [Display(Name = "尺寸", Description = "尺寸", Prompt = "尺寸")]
    public decimal Size { get; set; }
    [Display(Name = "扫描结果", Description = "扫描结果", Prompt = "扫描结果")]
    public string Landmarks { get; set; }
  }
}
