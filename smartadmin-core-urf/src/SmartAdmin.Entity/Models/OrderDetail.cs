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
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Display(Name = "商品", Description = "商品")]
        public Product Product { get; set; }
        [Required(ErrorMessage="必填")]
        [Range(1,9999)]
        [DefaultValue(1)]
        [Display(Name = "数量", Description = "需求数量")]
        public int Qty { get; set; }
        [Required(ErrorMessage = "必填")]
        [Range(1, 9999)]
        [Display(Name = "单价", Description = "单价")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "必填")]
        [Range(1, 9999)]
        [Display(Name = "金额", Description = "金额(数量x单价)")]
        public decimal Amount { get; set; }
        [Display(Name = "备注", Description = "备注")]
        [MaxLength(30)]
        [StringLength(20)]
        public string Remark { get; set; }
        [Display(Name = "订单", Description = "订单")]
        public int OrderId { get; set; }
        //关联订单表头
        [ForeignKey("OrderId")]
        [Display(Name = "订单号", Description = "订单号")]
        public Order Order { get; set; }

    }
}
