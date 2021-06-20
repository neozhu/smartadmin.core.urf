using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartAdmin.Domain.Models;
using SmartAdmin.Application.Customers.Commands;
using SmartAdmin.Service;
using URF.Core.Abstractions;
using Mapster;
namespace SmartAdmin.Application.Customers.Handlers
{
  public class CreateOrEditCustomerHandler : IRequestHandler<CreateOrEditCustomerCommand, Customer>
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly ICustomerService customerService;

    public CreateOrEditCustomerHandler(
      IUnitOfWork unitOfWork,
      ICustomerService customerService
      )
    {
      this.unitOfWork = unitOfWork;
      this.customerService = customerService;
    }
    public async Task<Customer> Handle(CreateOrEditCustomerCommand request, CancellationToken cancellationToken) {
      var customer = request.Adapt<Customer>();
      var response =await this.customerService.CreateOrEdit(customer);
      await this.unitOfWork.SaveChangesAsync();
      return customer;

      }
  }
}
