using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartAdmin.Data.Models
{
  public partial class Category : URF.Core.EF.Trackable.Entity
  {


    [Required]
    [Display(Name = "产品目录", Description = "产品目录", Prompt = "产品目录")]
    [MaxLength(30)]
    public string Name { get; set; }

  }
}
