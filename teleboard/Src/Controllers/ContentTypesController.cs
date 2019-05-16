using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Teleboard.Models;
using Teleboard.DomainModel.Core;
using Teleboard.DataAccess.Context;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host")]
    public class ContentTypesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.ContentTypes.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = await db.ContentTypes.FindAsync(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        public ActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ContentType contentType)
        {
            if (ModelState.IsValid)
            {
                db.ContentTypes.Add(contentType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(contentType);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = await db.ContentTypes.FindAsync(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ContentType contentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contentType);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = await db.ContentTypes.FindAsync(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ContentType contentType = await db.ContentTypes.FindAsync(id);
            db.ContentTypes.Remove(contentType);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
