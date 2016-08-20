using AutoMapper;
using GitClientVS.Contracts;

namespace GitClientVS.VisualStudio.UI
{
    public class VisualMappingsProfile : Profile
    {
        public VisualMappingsProfile()
        {
            CreateMap<NotificationFlags, Microsoft.TeamFoundation.Controls.NotificationFlags>();
            CreateMap<NotificationType, Microsoft.TeamFoundation.Controls.NotificationType>();
        }
    }
}
