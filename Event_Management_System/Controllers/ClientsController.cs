using Event_Management_System.Models;
using Event_Management_System.Models.View_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Event_Management_System.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
       
        private readonly EventMangementDbContex db =new EventMangementDbContex ();

        //GET: Clients
        [AllowAnonymous]
         public ActionResult Index()
        
         {
            List<Client> clients = db.Clients.Include(x => x.EventServices.Select(e => e.Event)).OrderByDescending(x => x.ClientId).ToList();
            return View(clients);
        }

        public ActionResult AddNewEvent(int? id)
        {
            ViewBag.events = new SelectList(db.Events.ToList(), "EventId", "EventName", (id != null) ? id.ToString() : "");
            return PartialView("_addNewEvent");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ClientVM clientVM, int[] eventId)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Age = clientVM.Age,
                    MaritalStatus = clientVM.MaritalStatus,

                };
                //Image Process
                HttpPostedFileBase file = clientVM.PictureFile;
                if (file != null)
                {

                    string fileName = Path.Combine("/Images/", DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
                    

                    file.SaveAs(Server.MapPath(fileName));
                    client.Picture = fileName;
                }

                //Save all Spot from SpotId
                foreach (var item in eventId)
                {
                    EventService eventServices = new EventService()
                    {
                        Client = client,
                        ClientId = client.ClientId,
                        ServiceName ="Test service",
                        VendorName ="Test Vendor",
                        EventId = item
                    };
                    db.EventServices.Add(eventServices);
                }
                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_Error");
        }
        public ActionResult Edit(int? id)
        {
            Client client = db.Clients.First(x => x.ClientId == id);
            var clientEvent = db.EventServices.Where(x => x.ClientId == id).ToList();
            ClientVM clientVM = new ClientVM()
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                BirthDate = client.BirthDate,
                Age = client.Age,
                Picture = client.Picture,
                MaritalStatus = client.MaritalStatus
            };
            if (clientEvent.Count() > 0)
            {
                foreach (var item in clientEvent)
                {
                    clientVM.EventList.Add(item.EventId);
                }
            }
            return View(clientVM);
        }
        [HttpPost]
        public ActionResult Edit(ClientVM clientVM, int[] eventId)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    ClientId = clientVM.ClientId,
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Age = clientVM.Age,
                    MaritalStatus = clientVM.MaritalStatus
                };
                //image
                HttpPostedFileBase file = clientVM.PictureFile;
                if (file != null)
                {
                    string fileName = Path.Combine("/Images/", DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(fileName));
                    client.Picture = fileName;
                }
                else
                {
                    client.Picture = clientVM.Picture;
                }
                //Spot 
                var eventEntry = db.EventServices.Where(x => x.ClientId == client.ClientId).ToList();
                //db.BookingEntries.RemoveRange(spotEntry);
                foreach (var bo in eventEntry)
                {
                    db.EventServices.Remove(bo);
                }
                foreach (var item in eventId)
                {
                    EventService eventService = new EventService()
                    {
                        ClientId = client.ClientId,
                        ServiceName = "Test service",
                        VendorName = "Test Vendor",
                        EventId = item
                    };
                    db.EventServices.Add(eventService);
                }
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_Error");
        }
        public ActionResult Delete(int? id)
        {
            Client client = db.Clients.FirstOrDefault(x => x.ClientId == id);
            var clientEvent = db.EventServices.Where(x => x.ClientId == id).ToList();
            ClientVM clientVM = new ClientVM()
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                BirthDate = client.BirthDate,
                Age = client.Age,
                Picture = client.Picture,
                MaritalStatus = client.MaritalStatus
            };
            if (clientEvent.Count() > 0)
            {
                foreach (var item in clientEvent)
                {
                    clientVM.EventList.Add(item.EventId);
                }
            }
            return View(clientVM);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Client client = db.Clients.Find(id);

            if (client == null)
            {
                return HttpNotFound();
            }
            var eventEntry = db.EventServices.Where(x => x.ClientId == client.ClientId).ToList();
            db.EventServices.RemoveRange(eventEntry);
            db.Entry(client).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}