﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HC.WeChat.Products
{
    /// <summary>
    /// 产品表
    /// </summary>
    [Table("Products")]
    public class Product : Entity<Guid>, IMayHaveTenant, IHasCreationTime, ICreationAudited
    {

        /// <summary>
        /// 产品规格
        /// </summary>
        [Required]
        [StringLength(200)]
        public virtual string Specification { get; set; }

        /// <summary>
        /// 产品类型（枚举：卷烟类、特产类）
        /// </summary>
        public virtual int? Type { get; set; }

        /// <summary>
        /// 指导零售价
        /// </summary>
        public virtual decimal? Price { get; set; }

        /// <summary>
        /// 是否是特色商品
        /// </summary>
        public virtual bool? IsRare { get; set; }

        /// <summary>
        /// 包码
        /// </summary>
        [StringLength(50)]
        public virtual string PackageCode { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [StringLength(50)]
        public virtual string BarCode { get; set; }

        /// <summary>
        /// 搜索次数
        /// </summary>
        public virtual int? SearchCount { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool? IsAction { get; set; }

        /// <summary>
        /// 申请创建时间
        /// </summary>
        [Required]
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual long? CreatorUserId { get; set; }
    }
}
