# Domain Driven Design (DDD) ultra-lightweight rapid development architecture(support .net 5.0)
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/meitu_0.jpg)
**smartadmin.core.urf offers a complete, modular and layered software architecture based on Domain Driven Design principles and patterns. It also provides the necessary infrastructure to implement this architecture, focusing on the core of the business Demand, reduce duplication coding,  allow junior programmers to develop professional and beautiful Web applications**

>Domain-driven design (DDD) is the concept that the structure and language of software code (class names, class methods, class variables) should match the business domain. For example, if a software processes loan applications, it might have classes such as LoanApplication and Customer, and methods such as AcceptOffer and Withdraw.
DDD connects the implementation to an evolving model.
Domain-driven design is predicated on the following goals:
> - placing the project's primary focus on the core domain and domain logic;
> - basing complex designs on a model of the domain;
> - initiating a creative collaboration between technical and domain experts to iteratively refine a conceptual model that addresses particular domain problems.。
## Demo Site
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/login.png)

[Demo (http://139.196.107.159:1060)](http://139.196.107.159:1060/Identity/Account/Login) 

UserName:**demo** Password:**123456** 

[Demo (http://106.52.105.140:6200)](http://106.52.105.140:6200/)
> Please give me **Star** if you like it. Every Star is a motivation to encourage me to continue updating.
> 
## Layers
**smartadmin.core.urf follow the DDD design pattern to implement the four-layer model of the application**
- Presentation Layer：This layer is the part where interaction with external systems happens. This layer is the gateway to the effects that a human, an application or a message will have on the domain. Requests will be accepted from this layer and the response will be shaped in this layer and displayed to the user.the project use [SmartAdmin - Responsive WebApp](https://www.gotbootstrap.com/themes/smartadmin/4.5.1/intel_analytics_dashboard.html) and [Jquery EasyUI](https://www.jeasyui.com/)
- Application Layer：It is the layer where business process flows are handled. The capabilities of the application can be observed in this layer. Domain entities are created and subject to update here. Depending on the usage scenarios, topics such as transaction management are also resolved here. In this layer, execution of work commands and reactions to domain events are coded. The code snippet for handling the CreateUser work command is given below as an example. In this example, by creating an object of User that comes from the Domain Layer and storing this object in the data storage, request for user creation is resolved. the project：**StartAdmin.Service.csproj**
- Domain Layer：This will be the core of the application. It is the layer where all business rules related to the problem to be solved are included. In this layer; entities, value objects, aggregates, factories and interfaces will take place. This layer should be kept away from dependencies as much as possible. Third party libraries should not be added as much as possible, as it should not take other layers as a reference.the project use EntityFrmework Core Code-first and Repository Implement. the project：**StartAdmin.Domain.csproj**
- Infrastructure Layer：This layer will be the layer that accesses external services such as database, messaging systems and email services. The IUserRepository interface designed in the domain layer and used in the application layer will be implemented in this layer and gain an identity.the project use:**Nlog**,service discovery：**Swagger UI**,EventBus:**[dotnetcore/CAP](https://github.com/dotnetcore/CAP)**,Authentication and Authorization:**Microsoft.AspNetCore.Identity**,etc.

## Project
![](https://github.com/neozhu/smartadmin.core.urf/blob/master/img/project_tree.png?raw=true)
+ Domain Layer
  * Entities are one of the core concepts of DDD (Domain Driven Design). Eric Evans describe it as "An object that is not fundamentally defined by its attributes, but rather by a thread of continuity and identity，it still follows some good practices：
  >- Entity:**(SmartAdmin.Entity.csproj)**
  >- Inherit a base class "Entity",Add necessary audit classes such as: creation time, last modification time, etc.
  >- There must be a primary key, preferably GRUID (composite primary key is not recommended), but this project uses an incremental int type
  >- The field should not be too redundant, you can define the association relationship
  >- Use virtual keywords as much as possible for field properties and methods. Some ORM and dynamic proxy tools require
   * Repositories: wrapper basic data operation method (CRUD),the Project [URF.Core](https://github.com/urfnet/URF.Core)

+ Application Layer
  * Application Services：Application services are used to implement the use cases of an application. They are used to expose domain logic to the presentation layer.the project:SmartAdmin.Service.csproj
  * Data Transfer Objects：Data Transfer Objects (DTO) are used to transfer data between the Application Layer and the Presentation Layer or other type of clients.Typically, an application service is called from the presentation layer (optionally) with a DTO as the parameter. It uses domain objects to perform some specific business logic and (optionally) returns a DTO back to the presentation layer. Thus, the presentation layer is completely isolated from domain layer.the project：(SmartAdmin.Dto.csproj)
  * Unit of work:Unit Of Work (UOW) implementation provides an abstraction and control on a database connection and transaction scope in an application.the project [URF.Core](https://github.com/urfnet/URF.Core)

+ Infrastructure
  * UI Layerout config:Choose a variety of page display modes according to user preferences
  * Multi-Tenant： Global Filter of EntityFrmework Core
  * Account: Maintenance of login system account, registration, logout, lock, unlock, reset password, import, export and other functions
  * Role：Microsoft Identity server
  * Navigation menu：System navigation bar configuration
  * Role authorization：Configure the menu that the role displays
  * Key-value configuration：Common data dictionary maintenance
  * Import & export configuration：Excel import and export to make a configurable feature
  * Logging：asp.net core logging and Nlog
  * Message publish-subscribe：CAP distributed event bus
  * WebApi: Swagger UI Api
  * CAP： CAP

## get started
+ development environment 
  >- Visual Studio .Net 2019
  >- .Net  5.0.1
  >- Sql Server(LocalDb)
+ dependence packages 
  >-  Include="AutoMapper" Version="10.1.1" 
  >-  Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="8.1.1" 
  >-  Include="DotNetCore.CAP" Version="3.1.2" 
  >-  Include="DotNetCore.CAP.Dashboard" Version="3.1.2" 
  >-  Include="DotNetCore.CAP.RabbitMQ" Version="3.1.2" 
  >-  Include="DotNetCore.CAP.SqlServer" Version="3.1.2" 
  >-  Include="DotNetCore.NPOI" Version="1.2.3" 
  >-  Include="Mapster" Version="7.1.5"
  >-  Include="MediatR" Version="9.0.0" 
  >-  Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="9.0.0" 
  >-  Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.3" 
  >-  Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="5.0.3"
  >-  Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="5.0.3" 
  >-  Include="Microsoft.AspNetCore.Identity.UI" Version="5.0.3" 
  >-  Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="5.0.3" 
  >-  Include="Microsoft.AspNetCore.Mvc.ViewFeatures" Version="2.2.0" 
  >-  Include="Microsoft.AspNetCore.SignalR" Version="1.1.0" 
  >-  Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.3"
  >-  Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.3" 
  >-  Include="Microsoft.EntityFrameworkCore.Tools" Version="5.0.3">
  >-  Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="5.0.2" 
  >-  Include="NLog" Version="4.7.8"
  >-  Include="NLog.Extensions.Logging" Version="1.7.1" 
  >-  Include="NLog.Web.AspNetCore" Version="4.11.0" 
  >-  Include="Swashbuckle.AspNetCore" Version="6.1.0" 
  >-  Include="System.Data.SqlClient" Version="4.8.2"
  >-  Include="System.Linq.Dynamic.Core" Version="1.2.8"
+ Open the solution

> **Start with the simple requirement** \
> develop CRUD, import and export function with Company object

+ Create Company Entity 
> SmartAdmin.Entity.csproj>Models new Company.cs class
```javascript
//Note: define the best practices of entity objects, inherit the base class, use the virtual keyword, and define every attribute, name, type, length, verification rule, index, default value, etc. as much as possible
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
        public virtual string Contact { get; set; }
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
+ Add Service Layer
> the project SmartAdmin.Service.csproj add **ICompanyService.cs**,**CompanyService.cs** implement business requirements and use cases
```javascript
//ICompany.cs
//Create methods based on actual business use cases, the default CRUD, additions, deletions, and changes do not need to be defined
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
// implementation of the interface method
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
+ new Controller
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
> The code of the View layer above is very complicated, but they are all in a fixed format and can be quickly generated with scaffold
+ Configure dependency injection (DI), register services
> Open startup.cs 在 public void ConfigureServices(IServiceCollection services)
> register services 
services.AddScoped<IRepositoryX<Customer>, RepositoryX<Customer>>(); \
services.AddScoped<ICustomerService, CustomerService>();
+ migration database
> EF Core Code-First migration \
> in Visual Studio.Net \
> Package Manager Controle run \
>PM>:add-migration create_Company \
>PM>:update-database \
>PM>:migration
+ Debug project
![](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/meitu_1.jpg)

## 高级应用
> CAP The solution and application scenario of distributed transaction \
> nuget install \
>PM> Install-Package DotNetCore.CAP \
>PM> Install-Package DotNetCore.CAP.RabbitMQ \
>PM> Install-Package DotNetCore.CAP.SqlServer \
+ config Startup.cs
```javascript
public void ConfigureServices(IServiceCollection services)
    {
      services.AddCap(x =>
      {
        x.UseEntityFramework<SmartDbContext>();
        x.UseRabbitMQ("127.0.0.1");
        x.UseDashboard();
        x.FailedRetryCount = 5;
        x.FailedThresholdCallback = failed =>
        {
          var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
          logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
        };
      });
    }
```
+ Publish messages
+ Subscribe messages


## contact information
[qq群](https://raw.githubusercontent.com/neozhu/smartadmin.core.urf/master/img/qqcode.png)!

