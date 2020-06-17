using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class Product:Entity
  {
    [Required]
    [Display(Name = "品名", Description = "品名", Prompt  = "品名")]
    [MaxLength(128)]
    public string Name { get; set; }
    [Display(Name = "规格型号", Description = "规格型号", Prompt = "规格型号")]
    [MaxLength(60)]
    public string Model { get; set; }
    [Display(Name = "单位", Description = "单位", Prompt = "单位")]
    [MaxLength(10)]
    public string Unit { get; set; }
    [Display(Name = "单价", Description = "单价", Prompt = "单价")]
    public decimal UnitPrice { get; set; }
  }
}
