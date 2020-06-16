using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAdmin.Dto
{
  public class RoleView
  {
    public string RoleName { get; set; }
    public int Count { get; set; }

  }
  public class RoleMenusView
  {
    public string RoleName { get; set; }
    public int MenuId { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Import { get; set; }
    public bool Export { get; set; }
    public bool FunctionPoint1 { get; set; }
    public bool FunctionPoint2 { get; set; }
    public bool FunctionPoint3 { get; set; }
  }
}
