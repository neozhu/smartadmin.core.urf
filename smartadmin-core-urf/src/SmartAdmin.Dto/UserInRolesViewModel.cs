using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAdmin.Dto
{
  public class UserInRolesViewModel
  {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string GivenName { get; set; }
    public string[] Roles { get; set; }
  }
}
