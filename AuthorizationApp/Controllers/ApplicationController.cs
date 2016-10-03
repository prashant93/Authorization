using Authorization.Models;
using Authorization.Repository;
using Merilent.Authorization;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace Authorization.Controllers
{
    /// <summary>
    /// Get Application from DB configured for ADFS. Get User Roles from AD and Applicaitons
    /// </summary>
    public class ApplicationController : ApiController
    {
        private IGenericRepository<Application> _appRepo;

        public ApplicationController()
        {
            _appRepo = new GenericRepository<Application>();
        }

        [HttpGet]
        public IEnumerable<Application> GetApplication()
        {
            return _appRepo.Get().Where(x => x.ApplicationName != null)
                                 .Select(x => new Application()
                                 {
                                     Id = x.Id,
                                     ApplicationName = x.ApplicationName,
                                     ApplicationURL = x.ApplicationURL,
                                     IsActive = x.IsActive
                                 })
                                 .ToList();
        }

        [HttpGet]
        public UserRole GetUserRoles(string username, int applicationId, string domain)
        {
            try
            {
                var applicationame = _appRepo.Get(x => x.Id == applicationId).Select(x => x.ApplicationName).FirstOrDefault();
                UserRolesHandler userRoles = new UserRolesHandler(ConfigurationManager.ConnectionStrings["AuthorizationConnection"].ConnectionString);
                return userRoles.GetRoles(username, applicationame, domain);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}