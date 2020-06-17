using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;
using System.Text;

namespace SmartAdmin.Data.Models
{
    public partial class Company : URF.Core.EF.Trackable.Entity
    {


        [Display(Name = "企业名称", Description = "归属企业名称")]
        [MaxLength(50)]
        [Required]
        //[Index(IsUnique = true)]
        public string Name { get; set; }
        [Display(Name = "组织代码", Description = "组织代码")]
        [MaxLength(12)]
        //[Index(IsUnique = true)]
        [Required]
        public string Code { get; set; }
        [Display(Name = "地址", Description = "地址")]
        [MaxLength(128)]
        [DefaultValue("-")]
        public string Address { get; set; }
        [Display(Name = "联系人", Description = "联系人")]
        [MaxLength(12)]
        public string Contect { get; set; }
        [Display(Name = "联系电话", Description = "联系电话")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [Display(Name = "注册日期", Description = "注册日期")]
        [DefaultValue("now")]
        public DateTime RegisterDate { get; set; }

    }
}
