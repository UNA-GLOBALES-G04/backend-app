
using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model;

using webapp.util;

namespace webapp.service
{

    public class ServiceService
    {

        private WebAppContext context;
        public ServiceService(WebAppContext context)
        {
            this.context = context;
        }

        public Service? getService(string serviceID, bool filterDeleted)
        {
            // check if the id is a valid GUID
            if (Guid.TryParse(serviceID, out Guid serviceGuid))
            {
                if (filterDeleted)
                {
                    return context.Services.Where(s => s.Id == serviceGuid && s.isDeleted == false).FirstOrDefault();
                }
                return context.Services.Find(serviceGuid);
            }
            return null;
        }

        public IEnumerable<Service> getServicesByUserProfileID(string userProfileID, bool filterDeleted)
        {
            if (filterDeleted)
            {
                return context.Services.Where(s => s.UserProfileId == userProfileID && s.isDeleted == false);
            }
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

        public IEnumerable<Service> getServicesByFilter(string? name, string[]? tags, bool union)
        {
            name = name ?? "";
            // check if the name is like the name of the service
            // and if it contains one of the tags
            // if only name is not null or empty, return all services that contain the name
            var matcher = new FuzzyMatcher(name);
            if (name != "" && (tags == null || tags.Length == 0))
            {
                return context.Services
                    .Where(s => matcher.MatchFuzzy(s.serviceName)).ToList();
            }
            else if (name == "" && tags != null && tags.Length != 0)
            {
                return context.Services.Where(s => s.tags.Any(t => tags.Contains(t))).ToList();
            }

            if (union)
            {
                // match both name and tags (union)
                return context.Services
                                .Where(s => matcher.MatchFuzzy(s.serviceName)).Union(context.Services
                                .Where(s => tags != null &&
                                        s.tags.Any(t => tags.Contains(t))))
                                    .ToList();
            }
            else
            {
                // match both name and tags (intersection)
                return context.Services
                                .Where(s => matcher.MatchFuzzy(s.serviceName)).Intersect(context.Services
                                .Where(s => tags != null &&
                                        s.tags.Any(t => tags.Contains(t))))
                                    .ToList();
            }


        }

        public int? getServiceRating(string serviceID)
        {
            if (Guid.TryParse(serviceID, out Guid serviceGuid))
            {
                // check if the service exists
                if (existsService(serviceGuid))
                {
                    int? rating = ((int?)context.Orders
                        .Where(o =>
                            o.ServiceId == serviceGuid &&
                            o.current_status == Order.OrderStatus.COMPLETED)
                        .Select(o => o.rating).Average());
                    return rating;
                }
            }
            return null;
        }

        public Service? createService(Service service)
        {
            // the service will have a random GUID
            service.Id = Guid.NewGuid();
            // the service is not deleted by default
            // by default cannot be deleted
            service.isDeleted = false;
            context.Services.Add(service);
            if (context.SaveChanges() > 0)
            {
                return service;
            }
            return null;
        }

        public Service? updateService(Service service, bool isUser)
        {
            // the following fields cannot be updated by the user
            // Id, UserProfileId, UserProfile, isDeleted
            var originalService = context.Services.Find(service.Id);
            if (originalService != null)
            {
                if (isUser)
                {
                    // get the original isDeleted
                    if (originalService != null)
                    {
                        service.Id = originalService.Id;
                        service.UserProfileId = originalService.UserProfileId;
                        service.isDeleted = originalService.isDeleted;
                    }
                }
                if (originalService != null)
                {
                    context.Entry(originalService).CurrentValues.SetValues(service);
                    if (context.SaveChanges() > 0)
                    {
                        return service;
                    }
                }

            }
            return null;
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