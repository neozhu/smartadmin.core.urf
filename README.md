# 基于领域驱动设计(DDD)超轻量级快速开发架构
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/meitu_0.jpg)
**smartadmin.core.urf 这个项目是基于asp.net core 3.1(最新)基础上参照领域驱动设计（DDD）的理念，并参考目前最为了流行的abp架构开发的一套轻量级的快速开发web application 技术架构**

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
+ 域层(Domain Layer)
  * 实体(Entity,BaseEntity) 通常实体就是映射到关系数据库中的表，这里说名一下最佳做法和惯例：
  >- 在域层定义:本项目就是**(SmartAdmin.Entity.csproj)**
  >- 继承一个基类 Entity，添加必要审计类比如：创建时间，最后修改时间等
  >- 必须要有一个主键最好是GRUID(不推荐复合主键),但本项目使用递增的int类型
  >- 字段不要过多的冗余,可以通过定义关联关系
  >- 字段属性和方法尽量使用virtual关键字。有些ORM和动态代理工具需要
   * 价值对象
   * 存储库(Repositories) 封装基本数据操作方法（CRUD）,本项目通过 [URF.Core](https://github.com/urfnet/URF.Core)
   * 域服务
   * 技术指标
+ 应用层
