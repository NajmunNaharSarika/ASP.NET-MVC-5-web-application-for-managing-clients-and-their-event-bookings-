using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Event_Management_System.Models;

namespace Event_Management_System.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private EventMangementDbContex db = new EventMangementDbContex();
        [AllowAnonymous]
        // GET: Events
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventId,EventName")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventId,EventName")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Event @event = db.Events.Find(id);
        //    db.Events.Remove(@event);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        ////
        public ActionResult DeleteConfirmed(int id)
        {
            // 1. Find the product
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            // 2. Find any order entries that are using this product
            var relatedOrders = db.EventServices.Where(x => x.EventId == id).ToList();

            // 3. Delete the child records FIRST to clear the foreign key constraint
            if (relatedOrders.Any())
            {
                db.EventServices.RemoveRange(relatedOrders);
            }

            // 4. Now it is safe to delete the product
            db.Events.Remove(@event);

            // 5. Save all changes to the database
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
