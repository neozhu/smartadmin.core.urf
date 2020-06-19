# 基于领域驱动设计(DDD)超轻量级快速开发架构
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/meitu_0.jpg)
**smartadmin.core.urf 这个项目是基于asp.net core 3.1(最新)基础上参照领域驱动设计（DDD）的理念，并参考目前最为了流行的abp架构开发的一套轻量级的快速开发web application 技术架构,专注业务核心需求，减少重复代码，开始构建和发布，让初级程序员也能开发出专业并且漂亮的Web应用程序**

>域驱动设计（DDD）是一种通过将实现与不断发展的模型相连接来满足复杂需求的软件开发方法。域驱动设计的前提如下：
> - 将项目的主要重点放在核心领域和领域逻辑上；
> - 将复杂的设计基于领域模型；
> - 启动技术专家和领域专家之间的创造性合作，以迭代方式完善解决特定领域问题的概念模型。

## 分层
**smartadmin.core.urf遵行DDD设计模式来实现应用程序的四层模型**
- 表示层(Presentation Layer)：用户操作展示界面，使用[SmartAdmin - Responsive WebApp](https://www.gotbootstrap.com/themes/smartadmin/4.5.1/intel_analytics_dashboard.html)模板+[Jquery EasyUI](https://www.jeasyui.com/)
- 应用层(Application Layer)：在表示层与域层之间，实现具体应用程序逻辑,业务用例,Project：**StartAdmin.Service.csproj**
- 域层(Domain Layer)：包括业务对象(**Entity**)和核心(域)业务规则，应用程序的核心,使用EntityFrmework Core Code-first + Repository实现
- 基础结构层(Infrastructure Layer)：提供通用技术功能，这些功能主要有第三方库来支持，比如日志:**Nlog**,服务发现：**Swagger UI**,事件总线(EventBus):**[dotnetcore/CAP](https://github.com/dotnetcore/CAP)**,认证与授权:**Microsoft.AspNetCore.Identity**,后面会具体介绍

## 内容
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/project.png)
+ 域层(Domain Layer)
  * 实体(Entity,BaseEntity) 通常实体就是映射到关系数据库中的表，这里说名一下最佳做法和惯例：
  >- 在域层定义:本项目就是**(SmartAdmin.Entity.csproj)**
  >- 继承一个基类 Entity，添加必要审计类比如：创建时间，最后修改时间等
  >- 必须要有一个主键最好是GRUID(不推荐复合主键),但本项目使用递增的int类型
  >- 字段不要过多的冗余,可以通过定义关联关系
  >- 字段属性和方法尽量使用virtual关键字。有些ORM和动态代理工具需要
   * 价值对象
   * 存储库(Repositories) 封装基本数据操作方法（CRUD）,本项目应用 [URF.Core](https://github.com/urfnet/URF.Core)实现
   * 域服务
   * 技术指标
+ 应用层
  * 应用服务：用于实现应用程序的用例。它们用于将域逻辑公开给表示层，从表示层（可选）使用DTO（数据传输对象）作为参数调用应用程序服务。它使用域对象执行某些特定的业务逻辑，并（可选）将DTO返回到表示层。因此，表示层与域层完全隔离。对应本项目：(SmartAdmin.Service.csproj)
  * 数据传输对象(DTO)：用于在应用程序层和表示层或其他类型的客户端之间传输数据,通常，使用DTO作为参数从表示层（可选）调用应用程序服务。它使用域对象执行某些特定的业务逻辑，并（可选）将DTO返回到表示层。因此，表示层与域层完全隔离.对应本项目：(SmartAdmin.Dto.csproj)
  * Unit of work:管理和控制应用程序中操作数据库连接和事务 ，本项目使用 [URF.Core](https://github.com/urfnet/URF.Core)实现

+ 基础服务层
  * 租户管理：使用EntityFrmework Core提供的Global Filter实现简单多租户应用
  * 账号管理: 对登录系统账号维护，注册，注销，锁定，解锁，重置密码，导入、导出等功能
  * 角色管理：采用RoleManage管理用户的授权
  * 导航菜单：系统主导航栏配置
  * 角色授权：配置角色显示的菜单
  * 键值对配置：常用的数据字典维护，如何正确使用和想法后面会介绍
  * 导入&导出配置：使用Excel导入导出做一个可配置的功能
  * 系统日志：asp.net core 自带的日志+Nlog把所有日志保存到数据库方便查询和分析
  * 消息订阅：集中订阅CAP分布式事件总线的消息
  * WebApi: Swagger UI Api服务发现和在线调试工具
  * CAP： CAP看板查看发布和订阅的消息

## 快速上手开发
+ 开发环境
  >- Visual Studio .Net 2019
  >- .Net Core 3.1
  >- Sql Server(LocalDb)
+ 附加数据库
  > 使用SQL Server Management Studio 附加.\src\SmartAdmin.Data\db\smartadmindb.mdf 数据库(如果是localdb,那么不需要修改数据库连接配置)
+ 打开解决方案

> **第一个简单的需求开始** \
> 新增 Company 企业信息 完成CRUD 导入导出功能

+ 新建实体对象(Entity)
> 在SmartAdmin.Entity.csproj项目的Models目录下新增一个Company.cs类
```javascript
//记住：定义实体对象最佳做法，继承基类，使用virtual关键字,尽可能的定义每个属性，名称，类型，长度，校验规则，索引，默认值等
namespace SmartAdmin.Data.Models
{
    public partial class Company : URF.Core.EF.Trackable.Entity
    {
        [Display(Name = "企业名称", Description = "归属企业名称")]
        [MaxLength(50)]
        [Required]
        //[Index(IsUnique = true)]
        public virtual string Name { get; set; }
        [Display(Name = "组织代码", Description = "组织代码")]
        [MaxLength(12)]
        //[Index(IsUnique = true)]
        [Required]
        public virtual string Code { get; set; }
        [Display(Name = "地址", Description = "地址")]
        [MaxLength(128)]
        [DefaultValue("-")]
        public virtual string Address { get; set; }
        [Display(Name = "联系人", Description = "联系人")]
        [MaxLength(12)]
        public virtual string Contect { get; set; }
        [Display(Name = "联系电话", Description = "联系电话")]
        [MaxLength(20)]
        public virtual string PhoneNumber { get; set; }
        [Display(Name = "注册日期", Description = "注册日期")]
        [DefaultValue("now")]
        public virtual  DateTime RegisterDate { get; set; }
    }
}
```

