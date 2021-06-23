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
  public class UpdatePhotoCommand:IRequest<Result<int>>
  {
    public int Id { get; set; }
    public string Landmarks { get; set; }
  }
  internal class UpdatePhotoCommandHandler : IRequestHandler<UpdatePhotoCommand, Result<int>>
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IPhotoService photoService;

    public UpdatePhotoCommandHandler(IUnitOfWork unitOfWork,
      IPhotoService photoService)
    {
      this.unitOfWork = unitOfWork;
      this.photoService = photoService;
    }
    public async Task<Result<int>> Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
    {
      var item =await this.photoService.FindAsync(request.Id);
      item.Landmarks = request.Landmarks;
      await this.unitOfWork.SaveChangesAsync();
      return await Result<int>.SuccessAsync(0, "保存成功");
    }
  }
}
