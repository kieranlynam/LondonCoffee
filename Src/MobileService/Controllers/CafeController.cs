﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using londoncoffeeService.DataObjects;
using londoncoffeeService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace londoncoffeeService.Controllers
{
    public class CafeController : TableController<CafeData>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new DataContext();
            this.DomainManager = new EntityDomainManager<CafeData>(context, this.Request, this.Services);
        }

        public IQueryable<CafeData> GetAllCafes()
        {
            return this.Query();
        }

        public SingleResult<CafeData> GetCafe(string id)
        {
            return this.Lookup(id);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        public Task<CafeData> PatchCafe(string id, Delta<CafeData> patch)
        {
            return this.UpdateAsync(id, patch);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        public async Task<IHttpActionResult> PostCafe(CafeData item)
        {
            CafeData current = await this.InsertAsync(item);
            return this.CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        public Task DeleteCafe(string id)
        {
            return this.DeleteAsync(id);
        }
    }
}