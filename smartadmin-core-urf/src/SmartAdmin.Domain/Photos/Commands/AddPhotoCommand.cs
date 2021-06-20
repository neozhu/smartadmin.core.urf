using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartAdmin.Application.Shared;
using SmartAdmin.Service.Interfaces;
using URF.Core.Abstractions;

namespace SmartAdmin.Application.Photos.Commands
{
  public partial class AddPhotoCommand:IRequest<Result<int>>
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

      return await Result<int>.SuccessAsync(0, "保存成功");
    }

  }
}
