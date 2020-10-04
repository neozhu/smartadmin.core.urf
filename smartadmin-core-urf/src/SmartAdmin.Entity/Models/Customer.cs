using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;
using System.Text;

namespace SmartAdmin.Data.Models
{
  public partial class Customer : URF.Core.EF.Trackable.Entity
  {


    [Display(Name = "客户名称", Description = "客户名称")]
    [MaxLength(50)]
    [Required]
    public virtual string Name { get; set; }

    [Display(Name = "联系人", Description = "联系人")]
    [MaxLength(12)]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,12}$", ErrorMessage = "Characters are not allowed.")]
    public virtual string Contect { get; set; }
    [Display(Name = "联系电话", Description = "联系电话")]
    [MaxLength(20)]
    [RegularExpression(@"^1(3|4|5|6|7|8|9)\d{9}$", ErrorMessage = "输入正确的手机号.")]
    public virtual string PhoneNumber { get; set; }
    [Display(Name = "地址", Description = "地址")]
    [MaxLength(50)]
    [DefaultValue("-")]
    public virtual string Address { get; set; }


  }
}
