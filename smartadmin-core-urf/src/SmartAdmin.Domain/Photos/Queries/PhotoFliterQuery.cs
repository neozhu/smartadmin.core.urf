using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartAdmin.Application.Shared;
using SmartAdmin.Domain.Models;
using SmartAdmin.Service.Interfaces;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace SmartAdmin.Application.Photos.Queries
{
   public  class PhotoFliterQuery : IRequest<PageResponse<Photo>>
    {
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 10;
    public string Sort { get; set; } = "Id";
    public string Order { get; set; } = "dsc";
    public string FilterRules { get; set; } = "";
    public PhotoFliterQuery()
    {

    }
    public PhotoFliterQuery(int page,
                         int rows,
                         string sort,
                         string order,
                         string filterRules)
    {
      this.Page = page;
      this.Rows = rows;
      this.Sort = sort;
      this.Order = order;
      this.FilterRules = filterRules;
    }
    }

  public class GetPhotoById : IRequest<Photo>
  {
    public int Id { get; set; }
    public GetPhotoById()
    {

    }
  }
  public class GetPhotosQuery : IRequest<IEnumerable<Photo>>
  {

  }


  public class PhotoFilterQueryHandel : IRequestHandler<PhotoFliterQuery, PageResponse<Photo>>,
    IRequestHandler<GetPhotoById, Photo>,
    IRequestHandler<GetPhotosQuery, IEnumerable<Photo>>
  {
    private readonly IPhotoService photoService;

    public PhotoFilterQueryHandel(IPhotoService photoService)
    {
      this.photoService = photoService;
    }
    public async Task<PageResponse<Photo>> Handle(PhotoFliterQuery request, CancellationToken cancellationToken) {

      var filters = PredicateBuilder.FromFilter<Photo>(request.FilterRules);
      var total = await this.photoService
                          .Query(filters).CountAsync();
      var pagerows = (await this.photoService
                           .Query(filters)
                         .OrderBy(n => n.OrderBy($"{request.Sort} {request.Order}"))
                         .Skip(request.Page - 1).Take(request.Rows).SelectAsync())
                         .ToList();
      var pagelist = new PageResponse<Photo> { total = total, rows = pagerows };
      return pagelist;
    }

    public async Task<Photo> Handle(GetPhotoById request, CancellationToken cancellationToken) {
      return await this.photoService.FindAsync(request.Id);
      }

    public async Task<IEnumerable<Photo>> Handle(GetPhotosQuery request, CancellationToken cancellationToken) {
      return await this.photoService.Queryable().ToListAsync();

      }
  }
}
