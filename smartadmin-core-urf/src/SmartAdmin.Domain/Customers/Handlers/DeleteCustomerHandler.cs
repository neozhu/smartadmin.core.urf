using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Application.Customers.Commands;
using SmartAdmin.Service;
using URF.Core.Abstractions;

namespace SmartAdmin.Application.Customers.Handlers
{
  public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand>
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly ICustomerService customerService;

    public DeleteCustomerHandler(
       IUnitOfWork unitOfWork,
      ICustomerService customerService
      )
    {
      this.unitOfWork = unitOfWork;
      this.customerService = customerService;
    }
    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken) {
      var items = await this.customerService.Queryable().Where(x => request.Id.Contains(x.Id)
        ).ToListAsync();
      foreach(var item in items)
      {
        this.customerService.Delete(item);
      }
      await this.unitOfWork.SaveChangesAsync();
      return await Task.FromResult(Unit.Value);
      }
  }
}
