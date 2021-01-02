using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAdmin.Dto
{
  public class AccountRegistrationModel
  {
    public string Username { get; set; }
    public string Email { get; set; }
    public string EmailConfirm { get; set; }
    public string Password { get; set; }
     public string PasswordConfirm { get; set; }
    public string GivenName { get; set; }

    public string PhoneNumber { get; set; }
    public bool EnabledChat { get; set; }
    public string Avatar { get; set; }
    public string TenantCode { get; set; }
    public string TenantName { get; set; }
    public int TenantId { get; set; }

    

  }
  public class AccountUpdateModel {
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string Avatar { get; set; }
    public string PhoneNumber { get; set; }
    public int TenantId { get; set; }
  }
}
