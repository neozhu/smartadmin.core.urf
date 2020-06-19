using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.Data.Models
{
    public partial class OrderDetail:Entity
    {
        [Required(ErrorMessage = "必选")]
        [Display(Name ="商品", Description ="商品")]
        public virtual  int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Display(Name = "商品", Description = "商品")]
        public Product Product { get; set; }
        [Required(ErrorMessage="必填")]
        [Range(1,9999)]
        [DefaultValue(1)]
        [Display(Name = "数量", Description = "需求数量")]
        public virtual  int Qty { get; set; }
        [Required(ErrorMessage = "必填")]
        [Range(1, 9999)]
        [Display(Name = "单价", Description = "单价")]
        public virtual  decimal Price { get; set; }
        [Required(ErrorMessage = "必填")]
        [Range(1, 9999)]
        [Display(Name = "金额", Description = "金额(数量x单价)")]
        public virtual  decimal Amount { get; set; }
        [Display(Name = "备注", Description = "备注")]
        [MaxLength(30)]
        [StringLength(20)]
        public virtual string Remark { get; set; }
        [Display(Name = "订单", Description = "订单")]
        public virtual  int OrderId { get; set; }
        //关联订单表头
        [ForeignKey("OrderId")]
        [Display(Name = "订单号", Description = "订单号")]
        public virtual Order Order { get; set; }

    }
}
