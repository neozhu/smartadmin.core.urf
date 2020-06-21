using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartAdmin.Dto
{
  public class SubscribeEventData
  {
    public virtual string title { get; set; }
    public virtual string content { get; set; }
    public virtual string url { get; set; }
    public virtual string from { get; set; }
    public virtual string to { get; set; }
    public virtual string group { get; set; }
    public virtual string publisher { get; set; }
  }
}
