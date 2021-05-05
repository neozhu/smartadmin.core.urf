using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.Domain.Models
{
  public partial class Order : Entity
  {
    public Order()
    {
      OrderDetails = new HashSet<OrderDetail>();
    }
    [Required]
    [Display(Name = "订单号", Description = "订单号", Order = 1)]
    [MaxLength(8)]
    [MinLength(8)]
    public virtual string OrderNo { get; set; }
    [Display(Name = "状态", Description = "状态", Order = 1)]
    [MaxLength(8)]
    [DefaultValue("草稿")]
    public virtual string Status { get; set; } = "草稿";
    [Display(Name = "接单日期", Description = "接单日期", Order = 2)]
    public virtual  DateTime OrderDate { get; set; } = DateTime.Now;
    [Display(Name = "客户", Description = "客户", Order = 3)]
    public virtual  int CustomerId { get; set; }
    [Display(Name = "客户", Description = "客户", Order = 3)]
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    [Required]
    [Display(Name = "发货地址", Description = "发货地址", Order = 4)]
    [MaxLength(256)]
    public virtual string ShippingAddress { get; set; }
    [Display(Name = "联系人", Description = "联系人", Order = 5)]
    [MaxLength(20)]
    public virtual string Contect { get; set; }
    [Display(Name = "联系电话", Description = "联系电话")]
    [MaxLength(30)]
    public virtual string PhoneNumber { get; set; }
    [Display(Name = "备注", Description = "备注", Order = 6)]
    [MaxLength(100)]
    public virtual string Remark { get; set; }
    [Display(Name = "订单明细", Description = "订单明细", Order = 6)]
    [InverseProperty("Order")]
    //关联订单明细 1-*
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
  }
}
