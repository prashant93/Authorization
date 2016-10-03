using Authorization.Models;
using Authorization.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Authorization.Controllers
{
    /// <summary>
    /// Get All ApplicatonRoles
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApplicationRoleController : ApiController
    {
        private IGenericRepository<ApplicationRole> _appRoleRepo;

        public ApplicationRoleController()
        {
            _appRoleRepo = new GenericRepository<ApplicationRole>();
        }

        [HttpGet]
        public IEnumerable<ApplicationRole> GetAppplicationRoles(int applicationId)
        {
            var applications = _appRoleRepo.Get().Select(x => new ApplicationRole()
            {
                ApplicationId = x.ApplicationId,
                ApplicationName = x.ApplicationName,
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).Where(y => y.ApplicationId == applicationId).ToList();

            return applications;
        }
    }
}