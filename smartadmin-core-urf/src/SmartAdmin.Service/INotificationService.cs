using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using SmartAdmin.Data.Models;
using SmartAdmin.Dto;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
    public interface INotificationService:IService<Notification>
    {

    void Subscribe(SubscribeEventData data);
		Task<Stream> ExportExcelAsync( string filterRules = "",string sort = "Id", string order = "asc");
	  Task Delete(int[] id);
    }
}
