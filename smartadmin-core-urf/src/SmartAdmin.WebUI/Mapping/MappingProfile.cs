using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SmartAdmin.Data.Models;
using SmartAdmin.Dto;
using SmartAdmin.WebUI.Data.Models;

namespace SmartAdmin.WebUI.Mapping
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
      DestinationMemberNamingConvention = new PascalCaseNamingConvention();

      //CreateMap<TrackPath, SubscribeChangeLocationEventData>();
      CreateMap<AccountRegistrationModel, ApplicationUser>();
    }
  }
 
}
