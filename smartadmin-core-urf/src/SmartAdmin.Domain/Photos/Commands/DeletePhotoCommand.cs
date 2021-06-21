using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Application.Shared;
using SmartAdmin.Service.Interfaces;
using URF.Core.Abstractions;

namespace SmartAdmin.Application.Photos.Commands
{
  public class DeletePhotoCommand:IRequest<Result<int>>
  {
    public int[] Id { get; set; }
  }
  internal class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, Result<int>>
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IPhotoService photoService;

    public DeletePhotoCommandHandler(IUnitOfWork unitOfWork,
      IPhotoService photoService)
    {
      this.unitOfWork = unitOfWork;
      this.photoService = photoService;
    }
    public async Task<Result<int>> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
    {
      var items =await this.photoService.Queryable().Where(x => request.Id.Contains(x.Id)).ToListAsync();
      foreach(var item in items)
      {
        this.photoService.Delete(item);
      }
      await this.unitOfWork.SaveChangesAsync();
      return await Result<int>.SuccessAsync(0, "保存成功");
    }
  }
}
