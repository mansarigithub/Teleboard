using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Teleboard.Models;
using Teleboard.DomainModel.Core;
using Teleboard.DataAccess.Context;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host")]
    public class ConnectionTypesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ConnectionTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ConnectionTypes.ToListAsync());
        }

        // GET: ConnectionTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionType connectionType = await db.ConnectionTypes.FindAsync(id);
            if (connectionType == null)
            {
                return HttpNotFound();
            }
            return View(connectionType);
        }

        // GET: ConnectionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConnectionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ConnectionType connectionType)
        {
            if (ModelState.IsValid)
            {
                db.ConnectionTypes.Add(connectionType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(connectionType);
        }

        // GET: ConnectionTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionType connectionType = await db.ConnectionTypes.FindAsync(id);
            if (connectionType == null)
            {
                return HttpNotFound();
            }
            return View(connectionType);
        }

        // POST: ConnectionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ConnectionType connectionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(connectionType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(connectionType);
        }

        // GET: ConnectionTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionType connectionType = await db.ConnectionTypes.FindAsync(id);
            if (connectionType == null)
            {
                return HttpNotFound();
            }
            return View(connectionType);
        }

        // POST: ConnectionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ConnectionType connectionType = await db.ConnectionTypes.FindAsync(id);
            db.ConnectionTypes.Remove(connectionType);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
