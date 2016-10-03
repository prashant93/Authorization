using Authorization.Models;
using Authorization.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Authorization.Controllers
{
    public class GroupController : ApiController
    {
        [HttpGet]
        public IEnumerable<Group> GetGroups()
        {
            using (var context = new AuthorizationContext())
            {
                return context.Groups.ToList();
            }
        }

        [HttpPost]
        public void AddGroup(Group group)
        {
            using (var context = new AuthorizationContext())
            {
                //todo checked id exist
                context.Groups.Add(group);
                context.SaveChanges();
            }
        }

        [HttpPut]
        public void UpdateGroup(Group group)
        {
            using (var context = new AuthorizationContext())
            {
                var result = context.Groups.Where(r => r.Id == group.Id).FirstOrDefault();
                if (result != null)
                {
                    //todo check is exist
                    result.GroupName = group.GroupName;
                    result.LastModifiedOn = DateTime.Now;

                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                        result.LastModifiedBy = HttpContext.Current.User.Identity.Name;
                    else
                        result.LastModifiedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";

                    context.Entry(result).State = System.Data.Entity.EntityState.Modified;

                    context.SaveChanges();
                }
            }
        }

        [HttpDelete]
        public void DeleteGroup(Group group)
        {
            using (var context = new AuthorizationContext())
            {
                var result = context.Groups.Find(group.Id);
                if (result != null)
                { //todo check is already deleted
                    result.IsActive = false;
                    result.LastModifiedOn = DateTime.Now;
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                        result.LastModifiedBy = HttpContext.Current.User.Identity.Name;
                    else
                        result.LastModifiedBy = Environment.UserName;

                    context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
    }
}