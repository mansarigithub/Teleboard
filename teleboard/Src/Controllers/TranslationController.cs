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
    public class TranslationController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Translates.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Translate translate = await db.Translates.FindAsync(id);
            if (translate == null)
            {
                return HttpNotFound();
            }
            return View(translate);
        }

        public ActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Culture,Name,Value")] Translate translate)
        {
            if (ModelState.IsValid)
            {
                db.Translates.Add(translate);
                await db.SaveChangesAsync();
                Translator.Reset();
                return RedirectToAction("Index");
            }

            return View(translate);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Translate translate = await db.Translates.FindAsync(id);
            if (translate == null)
            {
                return HttpNotFound();
            }
            return View(translate);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Culture,Name,Value")] Translate translate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(translate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Translator.Reset();
                return RedirectToAction("Index");
            }
            return View(translate);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Translate translate = await db.Translates.FindAsync(id);
            if (translate == null)
            {
                return HttpNotFound();
            }
            return View(translate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Translate translate = await db.Translates.FindAsync(id);
            db.Translates.Remove(translate);
            await db.SaveChangesAsync();
            Translator.Reset();
            return RedirectToAction("Index");
        }
    }
}
