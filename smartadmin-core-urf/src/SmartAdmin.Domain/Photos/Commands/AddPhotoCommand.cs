using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartAdmin.Application.Shared;
using SmartAdmin.Domain.Models;
using SmartAdmin.Service.Interfaces;
using URF.Core.Abstractions;

namespace SmartAdmin.Application.Photos.Commands
{
  public partial class AddPhotoCommand : IRequest<Result<int>>
  {
    public Stream Stream { get; set; }
    public string FileName { get; set; }
    public decimal Size { get; set; }
    public string Path { get; set; }

  }
  internal class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, Result<int>>
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IPhotoService photoService;

    public AddPhotoCommandHandler(IUnitOfWork unitOfWork,
      IPhotoService photoService)
    {
      this.unitOfWork = unitOfWork;
      this.photoService = photoService;
    }
    public async Task<Result<int>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
    {
      var info = new DirectoryInfo(request.Path);
      if (!info.Exists)
      {
        info.Create();
      }
      using (FileStream outputFileStream = new FileStream(Path.Combine(request.Path,request.FileName), FileMode.Create))
      {
        request.Stream.CopyTo(outputFileStream);
        outputFileStream.Close();
      }
      var photo = new Photo()
      {
        Name = Path.GetFileNameWithoutExtension(request.FileName),
        Size = request.Size,
        Path = $"/photos/{request.FileName}",
      };
      this.photoService.Insert(photo);
      await this.unitOfWork.SaveChangesAsync();
      return await Result<int>.SuccessAsync(0, "保存成功");
    }

  }
}
