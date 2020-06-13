using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace URF.Core.EF.Trackable
{
    public abstract class Entity : ITrackable, IMergeable, IAuditable, IMustHaveTenant
    {
       
        [Key]
        [Display(Name = "主键", Description = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [NotMapped]
        public virtual TrackingState TrackingState { get; set; }

        [NotMapped]
        public virtual ICollection<string> ModifiedProperties { get; set; }

        [NotMapped]
        public Guid EntityIdentifier { get; set; }
        [Display(Name = "创建时间", Description = "创建时间")]
        [ScaffoldColumn(false)]
        public virtual DateTime? CreatedDate { get; set; }
        [Display(Name = "创建用户", Description = "创建用户")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public virtual string CreatedBy { get; set; }
        [Display(Name = "最后更新时间", Description = "最后更新时间")]
        [ScaffoldColumn(false)]
        public virtual DateTime? LastModifiedDate { get; set; }
        [Display(Name = "最后更新用户", Description = "最后更新用户")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public virtual string LastModifiedBy { get; set; }
        [Display(Name = "租户", Description = "租户")]
        [ScaffoldColumn(false)]
        public virtual int TenantId { get; set; }
    }



    public interface IAuditable
    {
        DateTime? CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? LastModifiedDate { get; set; }
        string LastModifiedBy { get; set; }
    }

    public interface IMustHaveTenant
    {
        //
        // Summary:
        //     TenantId of this entity.
        int TenantId { get; set; }
    }
}   
