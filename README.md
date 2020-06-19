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
//在 SmartAdmin.Data.csproj 项目 SmartDbContext.cs 添加
public virtual DbSet<Company> Companies { get; set; }
```
+ 添加服务对象 Service
> 在项目 SmartAdmin.Service.csproj 中添加**ICompanyService.cs**,**CompanyService.cs** 就是用来实现业务需求 用例的地方
```javascript
//ICompany.cs
//根据实际业务用例来创建方法，默认的CRUD,增删改查不需要再定义
namespace SmartAdmin.Service
{
  // Example: extending IService<TEntity> and/or ITrackableRepository<TEntity>, scope: ICustomerService
  public interface ICompanyService : IService<Company>
  {
    // Example: adding synchronous Single method, scope: ICustomerService
    Company Single(Expression<Func<Company, bool>> predicate);
    Task ImportDataTableAsync(DataTable datatable);
    Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc");
  }
}
```
```javascript
// 具体实现接口的方法
namespace SmartAdmin.Service
{
  public class CompanyService : Service<Company>, ICompanyService
  {
    private readonly IDataTableImportMappingService mappingservice;
    private readonly ILogger<CompanyService> logger;
    public CompanyService(
      IDataTableImportMappingService mappingservice,
      ILogger<CompanyService> logger,
      ITrackableRepository<Company> repository) : base(repository)
    {
      this.mappingservice = mappingservice;
      this.logger = logger;
    }

    public async Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = PredicateBuilder.FromFilter<Company>(filterRules);
      var expcolopts = await this.mappingservice.Queryable()
             .Where(x => x.EntitySetName == "Company")
             .Select(x => new ExpColumnOpts()
             {
               EntitySetName = x.EntitySetName,
               FieldName = x.FieldName,
               IgnoredColumn = x.IgnoredColumn,
               SourceFieldName = x.SourceFieldName
             }).ToArrayAsync();

      var works = (await this.Query(filters).OrderBy(n => n.OrderBy(sort, order)).SelectAsync()).ToList();
      var datarows = works.Select(n => new
      {
        Id = n.Id,
        Name = n.Name,
        Code = n.Code,
        Address = n.Address,
        Contect = n.Contect,
        PhoneNumber = n.PhoneNumber,
        RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss")
      }).ToList();
      return await NPOIHelper.ExportExcelAsync("Company", datarows, expcolopts);
    }

    public async Task ImportDataTableAsync(DataTable datatable)
    {
      var mapping = await this.mappingservice.Queryable()
                        .Where(x => x.EntitySetName == "Company" &&
                           (x.IsEnabled == true || (x.IsEnabled == false && x.DefaultValue != null))
                           ).ToListAsync();
      if (mapping.Count == 0)
      {
        throw new  NullReferenceException("没有找到Work对象的Excel导入配置信息，请执行[系统管理/Excel导入配置]");
      }
      foreach (DataRow row in datatable.Rows)
      {

        var requiredfield = mapping.Where(x => x.IsRequired == true && x.IsEnabled == true && x.DefaultValue == null).FirstOrDefault()?.SourceFieldName;
        if (requiredfield != null || !row.IsNull(requiredfield))
        {
          var item = new Company();
          foreach (var field in mapping)
          {
            var defval = field.DefaultValue;
            var contain = datatable.Columns.Contains(field.SourceFieldName ?? "");
            if (contain && !row.IsNull(field.SourceFieldName))
            {
              var worktype = item.GetType();
              var propertyInfo = worktype.GetProperty(field.FieldName);
              var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
              var safeValue = (row[field.SourceFieldName] == null) ? null : Convert.ChangeType(row[field.SourceFieldName], safetype);
              propertyInfo.SetValue(item, safeValue, null);
            }
            else if (!string.IsNullOrEmpty(defval))
            {
              var worktype = item.GetType();
              var propertyInfo = worktype.GetProperty(field.FieldName);
              if (string.Equals(defval, "now", StringComparison.OrdinalIgnoreCase) && (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(Nullable<DateTime>)))
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(DateTime.Now, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
              else if (string.Equals(defval, "guid", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, Guid.NewGuid().ToString(), null);
              }
              else if (string.Equals(defval, "user", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, "", null);
              }
              else
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(defval, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
            }
          }
          this.Insert(item);
        }
      }
    }

    // Example, adding synchronous Single method
    public Company Single(Expression<Func<Company, bool>> predicate)
    {
      
      return this.Repository.Queryable().Single(predicate);

    }
  }
}
```
+ 添加Controller
> MVC Controller
```javascript
namespace SmartAdmin.WebUI.Controllers
{
  public class CompaniesController : Controller
  {
    private  readonly ICompanyService companyService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CompaniesController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CompaniesController(ICompanyService companyService,
          IUnitOfWork unitOfWork,
          IWebHostEnvironment webHostEnvironment,
          ILogger<CompaniesController> logger)
    {
      this.companyService = companyService;
      this.unitOfWork = unitOfWork;
      this._logger = logger;
      this._webHostEnvironment = webHostEnvironment;
    }

    // GET: Companies
    public IActionResult Index()=> View();
    //datagrid 数据源
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      try
      {
        var filters = PredicateBuilder.FromFilter<Company>(filterRules);
        var total = await this.companyService
                             .Query(filters)
                             .AsNoTracking()
                             .CountAsync()
                              ;
        var pagerows = (await this.companyService
                             .Query(filters)
                              .AsNoTracking()
                           .OrderBy(n => n.OrderBy(sort, order))
                           .Skip(page - 1).Take(rows)
                           .SelectAsync())
                           .Select(n => new
                           {
                             Id = n.Id,
                             Name = n.Name,
                             Code = n.Code,
                             Address = n.Address,
                             Contect = n.Contect,
                             PhoneNumber = n.PhoneNumber,
                             RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss")
                           }).ToList();
        var pagelist = new { total = total, rows = pagerows };
        return Json(pagelist);
      }
      catch(Exception e) {
        throw e;
        }

    }
    //编辑 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> Edit(Company company)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.companyService.Update(company);

          var result = await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true, result = result });
        }
         catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
        //DisplayErrorMessage(modelStateErrors);
      }
      //return View(work);
    }
    //新建
    [HttpPost]
    [ValidateAntiForgeryToken]
   
    public async Task<JsonResult> Create([Bind("Name,Code,Address,Contect,PhoneNumber,RegisterDate")] Company company)
    {
      if (ModelState.IsValid)
      {
        try
        {
          this.companyService.Insert(company);
       await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true});
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }

        //DisplaySuccessMessage("Has update a Work record");
        //return RedirectToAction("Index");
      }
      else
       {
        var modelStateErrors = string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
        //DisplayErrorMessage(modelStateErrors);
      }
      //return View(work);
    }
    //删除当前记录
    //GET: Companies/Delete/:id
    [HttpGet]
    public async Task<JsonResult> Delete(int id)
    {
      try
      {
        await this.companyService.DeleteAsync(id);
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
     
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }
    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {
      try
      {
        foreach (var key in id)
        {
          await this.companyService.DeleteAsync(key);
        }
        await this.unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetBaseException().Message });
      }
    }
    //保存datagrid编辑的数据
    [HttpPost]
    public async Task<JsonResult> AcceptChanges(Company[] companies)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in companies)
          {
            this.companyService.ApplyChanges(item);
          }
          var result = await this.unitOfWork.SaveChangesAsync();
          return Json(new { success = true, result });
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetBaseException().Message });
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors });
      }

    }
    //导出Excel
    [HttpPost]
    public async Task<IActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "compnay" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await this.companyService.ExportExcelAsync(filterRules, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);
    }
    //导入excel
    [HttpPost]
    public async Task<IActionResult> ImportExcel()
    {
      try
      {
        var watch = new Stopwatch();
        watch.Start();
        var total = 0;
        if (Request.Form.Files.Count > 0)
        {
          for (var i = 0; i < this.Request.Form.Files.Count; i++)
          {
            var model = Request.Form["model"].FirstOrDefault() ?? "company";
            var folder = Request.Form["folder"].FirstOrDefault() ?? "company";
            var autosave = Convert.ToBoolean(Request.Form["autosave"].FirstOrDefault());
            var properties = (Request.Form["properties"].FirstOrDefault()?.Split(','));
            var file = Request.Form.Files[i];
            var filename = file.FileName;
            var contenttype = file.ContentType;
            var size = file.Length;
            var ext = Path.GetExtension(filename);
            var path = Path.Combine(this._webHostEnvironment.ContentRootPath, "UploadFiles", folder);
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }
            var datatable = await NPOIHelper.GetDataTableFromExcelAsync(file.OpenReadStream(), ext);
            await this.companyService.ImportDataTableAsync(datatable);
            await this.unitOfWork.SaveChangesAsync();
            total = datatable.Rows.Count;
            if (autosave)
            {
              var filepath = Path.Combine(path, filename);
              file.OpenReadStream().Position = 0;

              using (var stream = System.IO.File.Create(filepath))
              {
                await file.CopyToAsync(stream);
              }
            }

          }
        }
        watch.Stop();
        return Json(new { success = true, total = total, elapsedTime = watch.ElapsedMilliseconds });
      }
      catch (Exception e) {
        this._logger.LogError(e, "Excel导入失败");
        return this.Json(new { success = false,  err = e.GetBaseException().Message });
      }
        }
    //下载模板
    public async Task<IActionResult> Download(string file) {
      
      this.Response.Cookies.Append("fileDownload", "true");
      var path = Path.Combine(this._webHostEnvironment.ContentRootPath, file);
      var downloadFile = new FileInfo(path);
      if (downloadFile.Exists)
      {
       var fileName = downloadFile.Name;
       var mimeType = MimeTypeConvert.FromExtension(downloadFile.Extension);
       var fileContent = new byte[Convert.ToInt32(downloadFile.Length)];
        using (var fs = downloadFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          await fs.ReadAsync(fileContent, 0, Convert.ToInt32(downloadFile.Length));
        }
        return this.File(fileContent, mimeType, fileName);
      }
      else
      {
        throw new FileNotFoundException($"文件 {file} 不存在!");
      }
    }

    }
}
```
+ 新建 View
> MVC Views\Companies\Index
```html
@model SmartAdmin.Data.Models.Customer
@{
  ViewData["Title"] = "客户信息";
  ViewData["PageName"] = "customers_index";
  ViewData["Heading"] = "<i class='fal fa-window text-primary'></i> 客户信息";
  ViewData["Category1"] = "组织架构";
  ViewData["PageDescription"] = "";
}
<div class="row">
  <div class="col-lg-12 col-xl-12">
    <div id="panel-1" class="panel">
      <div class="panel-hdr">
        <h2>
          客户信息
        </h2>
        <div class="panel-toolbar">
          <button class="btn btn-panel bg-transparent fs-xl w-auto h-auto rounded-0" data-action="panel-collapse" data-toggle="tooltip" data-offset="0,10" data-original-title="Collapse"><i class="fal fa-window-minimize"></i></button>
          <button class="btn btn-panel bg-transparent fs-xl w-auto h-auto rounded-0" data-action="panel-fullscreen" data-toggle="tooltip" data-offset="0,10" data-original-title="Fullscreen"><i class="fal fa-expand"></i></button>
        </div>

      </div>
      <div class="panel-container show">
        <div class="panel-content py-2 rounded-bottom border-faded border-left-0 border-right-0  text-muted bg-subtlelight-fade ">
          <div class="row no-gutters align-items-center">
            <div class="col">
              <!-- 开启授权控制请参考 @@if (Html.IsAuthorize("Create") -->
              <div class="btn-group btn-group-sm">
                <button onclick="appendItem()" class="btn btn-default">
                  <span class="fal fa-plus mr-1"></span> 新增
                </button>
              </div>
              <div class="btn-group btn-group-sm">
                <button name="deletebutton" disabled onclick="removeItem()" class="btn btn-default">
                  <span class="fal fa-times mr-1"></span> 删除
                </button>
              </div>
              <div class="btn-group btn-group-sm">
                <button name="savebutton" disabled onclick="acceptChanges()" class="btn btn-default">
                  <span class="fal fa-save mr-1"></span> 保存
                </button>
              </div>
              <div class="btn-group btn-group-sm">
                <button name="cancelbutton" disabled onclick="rejectChanges()" class="btn btn-default">
                  <span class="fal fa-ban mr-1"></span> 取消
                </button>
              </div>
              <div class="btn-group btn-group-sm">
                <button onclick="reload()" class="btn btn-default"> <span class="fal fa-search mr-1"></span> 查询 </button>
                <button type="button" class="btn btn-default dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                  <span class="sr-only">Toggle Dropdown</span>
                </button>
                <div class="dropdown-menu dropdown-menu-animated">
                  <a class="dropdown-item js-waves-on" href="javascript:void()"> 我的记录 </a>
                  <div class="dropdown-divider"></div>
                  <a class="dropdown-item js-waves-on" href="javascript:void()"> 自定义查询 </a>
                </div>
              </div>
              <div class="btn-group btn-group-sm hidden-xs">
                <button type="button" onclick="importExcel.upload()" class="btn btn-default"><span class="fal fa-cloud-upload mr-1"></span> 导入 </button>
                <button type="button" class="btn btn-default  dropdown-toggle dropdown-toggle-split waves-effect waves-themed" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                  <span class="sr-only">Toggle Dropdown</span>
                </button>
                <div class="dropdown-menu dropdown-menu-animated">
                  <a class="dropdown-item js-waves-on" href="javascript:importExcel.downloadtemplate()">
                    <span class="fal fa-download"></span> 下载模板
                  </a>
                </div>
              </div>
              <div class="btn-group btn-group-sm hidden-xs">
                <button onclick="exportExcel()" class="btn btn-default">
                  <span class="fal fa-file-export mr-1"></span>  导出
                </button>
              </div>

            </div>

          </div>

        </div>
        <div class="panel-content">
          <div class="table-responsive">
            <table id="companies_datagrid">
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
<!-- 弹出窗体form表单 -->
<div id="customerdetailwindow" class="easyui-window"
     title="明细数据"
     data-options="modal:true,
                    closed:true,
                    minimizable:false,
                    collapsible:false,
                    maximized:false,
                    iconCls:'fal fa-window',
                    onBeforeClose:function(){
                      var that = $(this);
                      if(customerhasmodified()){
                        $.messager.confirm('确认','你确定要放弃保存修改的记录?',function(r){
                        if (r){
                          var opts = that.panel('options');
                          var onBeforeClose = opts.onBeforeClose;
                          opts.onBeforeClose = function(){};
                          that.panel('close');
                          opts.onBeforeClose = onBeforeClose;
                          hook = false;
                        }
                        });
                        return false;
                      }
                    },
                    onOpen:function(){
                       $(this).window('vcenter');
                       $(this).window('hcenter');
                    },
                    onRestore:function(){
                    },
                    onMaximize:function(){
                    }
                    " style="width:820px;height:420px;display:none">
  <!-- toolbar -->
  <div class="panel-content py-2 rounded-bottom border-faded border-left-0 border-right-0  text-muted bg-subtlelight-fade sticky-top">
    <div class="d-flex flex-row-reverse pr-4">
      <div class="btn-group btn-group-sm mr-1">
        <button name="saveitembutton" onclick="savecustomeritem()" class="btn btn-default">
          <i class="fal fa-save"></i> 保存
        </button>
      </div>
      <div class="btn-group btn-group-sm mr-1" id="deleteitem-btn-group">
        <button onclick="deletecustomeritem()" class="btn btn-danger">
          <i class="fal fa-trash-alt"></i> 删除
        </button>
      </div>
    </div>
  </div>
  <div class="panel-container show">
    <div class="container">
      <div class="panel-content">
        <form id="customer_form"
              class="easyui-form form-horizontal p-1"
              method="post"
              data-options="novalidate:true,
                            onChange: function(target){
                              hook = true;
                              $('button[name*=\'saveitembutton\']').prop('disabled', false);
                             },
                             onLoadSuccess:function(data){
                               hook = false;
                               $('button[name*=\'saveitembutton\']').prop('disabled', true);
                             }">
          @Html.AntiForgeryToken()
          <!--Primary Key-->
          @Html.HiddenFor(model => model.Id)
          <fieldset class="form-group">
            <!-- begin row -->
            <!--名称-->
            <div class="row h-100 justify-content-center align-items-center">
              <label class="col-md-2 pr-1 form-label text-right"><span class="text-danger">*</span> @Html.DisplayNameFor(model => model.Name)</label>
              <div class="col-md-4 mb-1 pl-1">
                <input id="@Html.IdFor(model => model.Name)"
                       name="@Html.NameFor(model => model.Name)"
                       value="@Html.ValueFor(model => model.Name)"
                       tabindex="0" required
                       class="easyui-textbox"
                       style="width:100%"
                       type="text"
                       data-options="prompt:'@Html.DescriptionFor(model => model.Name)',
								 required:true,
                                 validType: 'length[0,50]'
                                 " />
              </div>
              <label class="col-md-2 pr-1 form-label text-right ">@Html.DisplayNameFor(model => model.Contect)</label>
              <div class="col-md-4 mb-1 pl-1">
                <input id="@Html.IdFor(model => model.Contect)"
                       name="@Html.NameFor(model => model.Contect)"
                       value="@Html.ValueFor(model => model.Contect)"
                       tabindex="1" required
                       class="easyui-textbox"
                       style="width:100%"
                       type="text"
                       data-options="prompt:'@Html.DescriptionFor(model => model.Contect)',
								 required:true,validType: 'length[0,12]'
                                 " />
              </div>
              <label class="col-md-2 pr-1 form-label text-right">@Html.DisplayNameFor(model => model.PhoneNumber)</label>
              <div class="col-md-4 mb-1 pl-1">
                <input id="@Html.IdFor(model => model.PhoneNumber)"
                       name="@Html.NameFor(model => model.PhoneNumber)"
                       value="@Html.ValueFor(model => model.PhoneNumber)"
                       tabindex="2"
                       class="easyui-textbox"
                       style="width:100%"
                       type="text"
                       data-options="prompt:'@Html.DescriptionFor(model => model.PhoneNumber)',
								 required:false,validType: 'length[0,20]'
                                 
                                 " />
              </div>
              <label class="col-md-2 pr-1 form-label text-right">@Html.DisplayNameFor(model => model.Address)</label>
              <div class="col-md-4 mb-1 pl-1">
                <input id="@Html.IdFor(model => model.Address)"
                       name="@Html.NameFor(model => model.Address)"
                       value="@Html.ValueFor(model => model.Address)"
                       tabindex="3"
                       class="easyui-textbox"
                       style="width:100%"
                       type="text"
                       data-options="prompt:'@Html.DescriptionFor(model => model.Address)',
								        required:true, validType: 'length[0,50]'
                                 " />
              </div>

            </div>
          </fieldset>
        </form>
      </div>
    </div>
  </div>
</div>


@await Component.InvokeAsync("ImportExcel", new ImportExcelOptions
{
  entity = "Customer",
  folder = "Customers",
  url = "/Customers/ImportExcel",
  tpl = "/Customers/Download"


})

@section HeadBlock {
  <link href="~/css/notifications/toastr/toastr.css" rel="stylesheet" asp-append-version="true" />
  <link href="~/css/formplugins/bootstrap-daterangepicker/bootstrap-daterangepicker.css" rel="stylesheet" asp-append-version="true" />
  <link href="~/js/easyui/themes/insdep/easyui.css" rel="stylesheet" asp-append-version="true" />
}
@section ScriptsBlock {
  <script src="~/js/dependency/numeral/numeral.min.js" asp-append-version="true"></script>
  <script src="~/js/dependency/moment/moment.js" asp-append-version="true"></script>
  <script src="~/js/notifications/toastr/toastr.js"></script>
  <script src="~/js/formplugins/bootstrap-daterangepicker/bootstrap-daterangepicker.js" asp-append-version="true"></script>
  <script src="~/js/easyui/jquery.easyui.min.js" asp-append-version="true"></script>
  <script src="~/js/easyui/plugins/datagrid-filter.js" asp-append-version="true"></script>
  <script src="~/js/easyui/plugins/columns-ext.js" asp-append-version="true"></script>
  <script src="~/js/easyui/plugins/columns-reset.js" asp-append-version="true"></script>
  <script src="~/js/easyui/locale/easyui-lang-zh_CN.js" asp-append-version="true"></script>
  <script src="~/js/easyui/jquery.easyui.component.js" asp-append-version="true"></script>
  <script src="~/js/plugin/filesaver/FileSaver.js" asp-append-version="true"></script>
  <script src="~/js/plugin/jquery.serializejson/jquery.serializejson.js" asp-append-version="true"></script>
  <script src="~/js/jquery.custom.extend.js" asp-append-version="true"></script>
  <script src="~/js/jquery.extend.formatter.js" asp-append-version="true"></script>
  <script>
        var $dg = $('#companies_datagrid');
        var EDITINLINE = true;
        var customer = null;
    var editIndex = undefined;
    //下载Excel导入模板

    //执行导出下载Excel
    function exportExcel() {
      const filterRules = JSON.stringify($dg.datagrid('options').filterRules);
      console.log(filterRules);
      $.messager.progress({ title: '请等待', msg: '正在执行导出...' });
      let formData = new FormData();
      formData.append('filterRules', filterRules);
      formData.append('sort', 'Id');
      formData.append('order', 'asc');
      $.postDownload('/Customers/ExportExcel', formData).then(res => {
        $.messager.progress('close');
        toastr.success('导出成功!');
      }).catch(err => {
        //console.log(err);
        $.messager.progress('close');
        $.messager.alert('导出失败', err.statusText, 'error');
      });

    }
            //弹出明细信息
    function showDetailsWindow(id, index) {
      const customer = $dg.datagrid('getRows')[index];
      openCustomerDetailWindow(customer, 'Modified');
    }
    function reload() {
      $dg.datagrid('uncheckAll');
      $dg.datagrid('reload');
    }
            //新增记录
    function appendItem() {
      customer = {
        Address: '-',
        RegisterDate: moment().format('YYYY-MM-DD HH:mm:ss'),
      };
      if (!EDITINLINE) {
        //弹出新增窗口
        openCustomerDetailWindow(customer, 'Added');
      } else {
        if (endEditing()) {
          //对必填字段进行默认值初始化
          $dg.datagrid('insertRow',
            {
              index: 0,
              row: customer
            });
          editIndex = 0;
          $dg.datagrid('selectRow', editIndex)
            .datagrid('beginEdit', editIndex);
          hook = true;
        }
      }
    }
            //删除编辑的行
    function removeItem() {
      if (this.$dg.datagrid('getChecked').length <= 0 && EDITINLINE) {
        if (editIndex !== undefined) {
          const delindex = editIndex;
          $dg.datagrid('cancelEdit', delindex)
            .datagrid('deleteRow', delindex);
          hook = true;
          $("button[name*='savebutton']").prop('disabled', false);
          $("button[name*='cancelbutton']").prop('disabled', false);
        } else {
          const rows = $dg.datagrid('getChecked');
          rows.slice().reverse().forEach(row => {
            const rowindex = $dg.datagrid('getRowIndex', row);
            $dg.datagrid('deleteRow', rowindex);
            hook = true;
          });
        }
      } else {
        deletechecked();
      }
    }
    //删除选中的行
    function deleteChecked() {
      const checked = $dg.datagrid('getChecked').filter(item => item.Id != null && item.Id > 0).map(item => {
        return item.Id;
      });;
      if (checked.length > 0) {
        deleteRows(checked);
      } else {
        $.messager.alert('提示', '请先选择要删除的记录!', 'question');
      }
    }
    //执行删除
    function deleteRows(selected) {
      $.messager.confirm('确认', `你确定要删除这 <span class='badge badge-icon position-relative'>${selected.length} </span> 行记录?`, result => {
        if (result) {
          $.post('/Customers/DeleteChecked', { id: selected })
            .done(response => {
              if (response.success) {
                toastr.error(`成功删除 [${selected.length}] 行记录`);
                reload();
              } else {
                $.messager.alert('错误', response.err, 'error');
              }
            })
            .fail((jqXHR, textStatus, errorThrown) => {
              $.messager.alert('异常', `${jqXHR.status}: ${jqXHR.statusText} `, 'error');
            });
        }
      });
    }
            //开启编辑状态
    function onClickCell(index, field) {

      customer = $dg.datagrid('getRows')[index];
      const _actions = ['action', 'ck'];
      if (!EDITINLINE || $.inArray(field, _actions) >= 0) {
        return;
      }

      if (editIndex !== index) {
        if (endEditing()) {
          $dg.datagrid('selectRow', index)
            .datagrid('beginEdit', index);
          hook = true;
          editIndex = index;
          const ed = $dg.datagrid('getEditor', { index: index, field: field });
          if (ed) {
            ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target)).focus();
          }
        } else {
          $dg.datagrid('selectRow', editIndex);
        }
      }
    }
            //关闭编辑状态
    function endEditing() {

      if (editIndex === undefined) {
        return true;
      }
      if (this.$dg.datagrid('validateRow', editIndex)) {
        $dg.datagrid('endEdit', editIndex);
        return true;
      } else {
        const invalidinput = $('input.validatebox-invalid', $dg.datagrid('getPanel'));
        const fieldnames = invalidinput.map((index, item) => {
          return $(item).attr('placeholder') || $(item).attr('id');
        });
        $.messager.alert('提示', `${Array.from(fieldnames)} 输入有误.`, 'error');
        return false;
      }
    }
            //提交保存后台数据库
    function acceptChanges() {
      if (endEditing()) {
        if ($dg.datagrid('getChanges').length > 0) {
          const inserted = $dg.datagrid('getChanges', 'inserted').map(item => {
            item.TrackingState = 1;
            return item;
          });
          const updated = $dg.datagrid('getChanges', 'updated').map(item => {
            item.TrackingState = 2
            return item;
          });
          const deleted = $dg.datagrid('getChanges', 'deleted').map(item => {
            item.TrackingState = 3
            return item;
          });
          //过滤已删除的重复项
          const changed = inserted.concat(updated.filter(item => {
            return !deleted.includes(item);
          })).concat(deleted);
          //$.messager.progress({ title: '请等待', msg: '正在保存数据...', interval: 200 });
          $.post('/Customers/AcceptChanges', { customers: changed })
            .done(response => {
              //$.messager.progress('close');
              //console.log(response);
              if (response.success) {
                toastr.success('保存成功');
                $dg.datagrid('acceptChanges');
                reload();
                hook = false;
              } else {
                $.messager.alert('错误', response.err, 'error');
              }
            })
            .fail((jqXHR, textStatus, errorThrown) => {
              //$.messager.progress('close');
              $.messager.alert('异常', `${jqXHR.status}: ${jqXHR.statusText} `, 'error');
            });
        }
      }
    }
    function rejectChanges() {
      $dg.datagrid('rejectChanges');
      editIndex = undefined;
      hook = false;
    }
    $(document).ready(function () {
      //定义datagrid结构
      $dg.datagrid({
        rownumbers: true,
        checkOnSelect: false,
        selectOnCheck: false,
        idField: 'Id',
        sortName: 'Id',
        sortOrder: 'desc',
        remoteFilter: true,
        singleSelect: true,
        method: 'get',
        onClickCell: onClickCell,
        clientPaging: false,
        pagination: true,
        striped: true,
        filterRules: [],
        onHeaderContextMenu: function (e, field) {
          e.preventDefault();
          $(this).datagrid('columnMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
          });
        },
        onBeforeLoad: function () {
          const that = $(this);
          document.addEventListener('panel.onfullscreen', () => {
            setTimeout(() => {
              that.datagrid('resize');
            }, 200)
          })
        },
        onLoadSuccess: function (data) {
          editIndex = undefined;
          $("button[name*='deletebutton']").prop('disabled', true);
          $("button[name*='savebutton']").prop('disabled', true);
          $("button[name*='cancelbutton']").prop('disabled', true);
        },
        onCheck: function () {
          $("button[name*='deletebutton']").prop('disabled', false);
        },
        onUncheck: function () {
          const checked = $(this).datagrid('getChecked').length > 0;
          $("button[name*='deletebutton']").prop('disabled', !checked);
        },
        onSelect: function (index, row) {
          customer = row;
        },
        onBeginEdit: function (index, row) {
          //const editors = $(this).datagrid('getEditors', index);

        },
        onEndEdit: function (index, row) {
          editIndex = undefined;
        },
        onBeforeEdit: function (index, row) {
          editIndex = index;
          row.editing = true;
          $("button[name*='deletebutton']").prop('disabled', false);
          $("button[name*='cancelbutton']").prop('disabled', false);
          $("button[name*='savebutton']").prop('disabled', false);
          $(this).datagrid('refreshRow', index);
        },
        onAfterEdit: function (index, row) {
          row.editing = false;
          editIndex = undefined;
          $(this).datagrid('refreshRow', index);
        },
        onCancelEdit: function (index, row) {
          row.editing = false;
          editIndex = undefined;
          $("button[name*='deletebutton']").prop('disabled', true);
          $("button[name*='savebutton']").prop('disabled', true);
          $("button[name*='cancelbutton']").prop('disabled', true);
          $(this).datagrid('refreshRow', index);
        },
        frozenColumns: [[
          /*开启CheckBox选择功能*/
          { field: 'ck', checkbox: true },
          {
            field: 'action',
            title: '操作',
            width: 85,
            sortable: false,
            resizable: true,
            formatter: function showdetailsformatter(value, row, index) {
              if (!row.editing) {
                return `<div class="btn-group">\
                                                         <button onclick="showDetailsWindow('${row.Id}',  ${index})" class="btn btn-primary btn-sm btn-icon waves-effect waves-themed" title="查看明细" ><i class="fal fa-edit"></i> </button>\
                                                         <button onclick="deleteRows(['${row.Id}'],${index})" class="btn btn-primary btn-sm btn-icon waves-effect waves-themed" title="删除记录" ><i class="fal fa-times"></i> </button>\
                                                    </div>`;
              } else {
                return `<button class="btn btn-primary btn-sm btn-icon waves-effect waves-themed" disabled title="查看明细"  ><i class="fal fa-edit"></i> </button>`;
              }
            }
          }
        ]],
        columns: [[
          {    /*名称*/
            field: 'Name',
            title: '@Html.DisplayNameFor(model => model.Name)',
            width: 200,
            hidden: false,
            editor: {
              type: 'textbox',
              options: { prompt: '@Html.DescriptionFor(model => model.Name)', required: true, validType: 'length[0,128]' }
            },
            sortable: true,
            resizable: true
          },
          {    /*联系人*/
            field: 'Contect',
            title: '@Html.DisplayNameFor(model => model.Contect)',
            width: 120,
            hidden: false,
            editor: {
              type: 'textbox',
              options: { prompt: '@Html.DescriptionFor(model => model.Contect)', required: true, validType: 'length[0,12]' }
            },
            sortable: true,
            resizable: true
          },
          {    /*电话*/
            field: 'PhoneNumber',
            title: '@Html.DisplayNameFor(model => model.PhoneNumber)',
            width: 200,
            hidden: false,
            editor: {
              type: 'textbox',
              options: { prompt: '@Html.DescriptionFor(model => model.PhoneNumber)', required: false, validType: 'length[0,20]' }
            },
            sortable: true,
            resizable: true
          },
          {    /*地址*/
            field: 'Address',
            title: '@Html.DisplayNameFor(model => model.Address)',
            width: 120,
            hidden: false,
            editor: {
              type: 'textbox',
              options: { prompt: '@Html.DescriptionFor(model => model.Address)', required: false, validType: 'length[0,50]'}
            },
            sortable: true,
            resizable: true,
      
          }
        ]]
      }).datagrid('columnMoving')
        .datagrid('resetColumns')
        .datagrid('enableFilter', [
        ])
        .datagrid('load', '/Customers/GetData');
    }
    );

  </script>
  <script type="text/javascript">
    //判断新增编辑状态
    var MODELSTATE = 'Added';
    var customerid = null;
    function openCustomerDetailWindow(data, state) {
      MODELSTATE = state;
      initcustomerdetailview();
      customerid = (data.Id || 0);
      $("#customerdetailwindow").window("open");
      $('#customer_form').form('reset');
      $('#customer_form').form('load', data);
    }
    //删除当前记录
    function deletecustomeritem() {
      $.messager.confirm('确认', '你确定要删除该记录?', result => {
        if (result) {
          const url = `/Customers/Delete/${customerid}`;
          $.get(url).done(res => {
            if (res.success) {
              toastr.success("删除成功");
              $("#customerdetailwindow").window("close");
              reload();
            } else {
              $.messager.alert("错误", res.err, "error");
            }
          });
        }
      });
    }
    //async 保存数据
    async function savecustomeritem() {
      const $customerform = $('#customer_form');
      if ($customerform.form('enableValidation').form('validate')) {
        let customer = $customerform.serializeJSON();
        let url = '/Customers/Edit';
        //判断是新增或是修改方法
        if (MODELSTATE === 'Added') {
          url = '/Customers/Create';
        }
        var token = $('input[name="__RequestVerificationToken"]', $customerform).val();
        //$.messager.progress({ title: '请等待', msg: '正在保存数据...', interval: 200 });
        $.ajax({
          type: "POST",
          url: url,
          data: {
            __RequestVerificationToken: token,
            customer: customer
          },
          dataType: 'json',
          contentType: 'application/x-www-form-urlencoded; charset=utf-8'
        })
          .done(response => {
            //$.messager.progress('close');
            if (response.success) {
              hook = false;
              $customerform.form('disableValidation');
              $dg.datagrid('reload');
              $('#customerdetailwindow').window("close");
              toastr.success("保存成功");
            } else {
              $.messager.alert("错误", response.err, "error");
            }
          })
          .fail((jqXHR, textStatus, errorThrown) => {
            //$.messager.progress('close');
            $.messager.alert('异常', `${jqXHR.status}: ${jqXHR.statusText} `, 'error');
          });
      }
    }
    //关闭窗口
    function closecustomerdetailwindow() {
      $('#customerdetailwindow').window('close');
    }

    //判断是否有没有保存的记录
    function customerhasmodified() {
      return hook;
    }


    function initcustomerdetailview() {
      //判断是否显示功能按钮

    }
  </script>
}

```
+ 配置依赖注入(DI),注册服务
> 打开 startup.cs 在 public void ConfigureServices(IServiceCollection services)
> 注册服务 
services.AddScoped<IRepositoryX<Customer>, RepositoryX<Customer>>(); \
services.AddScoped<ICustomerService, CustomerService>();
+ 更新数据库
> EF Core Code-First 同步更新数据库 \
> 在 Visual Studio.Net \
> Package Manager Controle 运行 \
>PM>:add-migration create_Company \
>PM>:update-database \
>PM>:更新完成
+ Debug 运行项目



