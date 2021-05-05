using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Domain.Models
{
  public partial class Product:Entity
  {
    [Required]
    [Display(Name = "品名", Description = "品名", Prompt  = "品名")]
    [MaxLength(128)]
    public virtual string Name { get; set; }
    [Display(Name = "规格型号", Description = "规格型号", Prompt = "规格型号")]
    [MaxLength(60)]
    public virtual string Model { get; set; }
    [Display(Name = "单位", Description = "单位", Prompt = "单位")]
    [MaxLength(10)]
    public virtual string Unit { get; set; }
    [Display(Name = "单价", Description = "单价", Prompt = "单价")]
    [Column(TypeName = "decimal(18, 2)")]
    public virtual  decimal UnitPrice { get; set; }
  }
}
