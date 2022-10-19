
using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model;

namespace webapp.service
{

    public class ServiceService
    {

        private WebAppContext context;
        public ServiceService(WebAppContext context)
        {
            this.context = context;
        }

        public Service? getService(string serviceID)
        {
            return context.Services.Find(serviceID);
        }

        public IEnumerable<Service> getServicesByUserProfileID(string userProfileID)
        {
            return context.Services.Where(s => s.UserProfileId == userProfileID).ToList();
        }

        public bool existsService(Guid serviceID)
        {
            return context.Services.AsNoTracking().Any(s => s.Id == serviceID);
        }

        public IEnumerable<Service> getAllServices()
        {
            return context.Services.ToList();
        }

        public Service? createService(Service service)
        {
            context.Services.Add(service);
            if (context.SaveChanges() > 0)
            {
                return service;
            }
            return null;
        }

        public Service updateService(Service service, bool isUser)
        {
            // the following fields cannot be updated by the user
            // Id, UserProfileId, UserProfile, isDeleted
            if (isUser)
            {
                // get the original isDeleted
                var originalService = context.Services.Find(service.Id);
                if (originalService != null)
                {
                    service.Id = originalService.Id;
                    service.UserProfileId = originalService.UserProfileId;
                    service.UserProfile = originalService.UserProfile;
                    service.isDeleted = originalService.isDeleted;
                }
            }
            context.Services.Update(service);
            context.SaveChanges();
            return service;
        }

        public bool deleteService(Guid serviceID)
        {
            var service = context.Services.Find(serviceID);
            if (service != null)
            {
                service.isDeleted = true;
                context.Services.Update(service);
                return context.SaveChanges() > 0;
            }
            return false;
        }


    }
}