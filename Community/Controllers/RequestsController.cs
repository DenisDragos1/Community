using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Community.Data;
using Community.Models;

namespace Community.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private CommunityEntities db = new CommunityEntities();

        // GET: Requests

        //var requests = db.Requests.Include(r => r.User);
        //return View(requests.ToList());
        public ActionResult Rez ()
        { 
            return View();

        }
        public ActionResult Rez(Request request )
        {
            request.Solved = true;
            db.SaveChanges();

            return View(request);
        }
        public ActionResult Index(string searching, bool myRequestsOnly = false)
        {
            //  var requests = db.Requests.Include(r => r.User);

            //return View(requests.ToList());
            //return View(db.Requests.Where(r => r.Title.Contains(searching) || searching == null).ToList());
            List<Request> result = db.Requests.Where(r => r.Titlu.Contains(searching) || searching == null).ToList();
            Request request = new Request();
            Answer answer = new Answer();
            if(request.ID==answer.RequestId && answer.Gasit==false)
            {
                request.Solved = false;
            }
            if (myRequestsOnly == true)
            {

                result = result.Where(r => r.OwnerId == (int)Session["ID"]).ToList();
            }
            
           

            return View(result);
        }
      
        // GET: Requests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            /////
            //ViewBag.OwnerId = new SelectList(db.Users, "ID", "FirstName");
            // var userDetail = db.Users.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();

            //Session["ID"] = userDetail.ID.ToString();
            //Session["FirstName"] = userDetail.ToString();
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Titlu,Imagine,Imagine1,Descriere,OwnerId,Data,Solved,Greutate,AdresaExpeditor,AdresaDestinatar,CategorieProdus,Judet,Oras,Email,Phone")] Request request,HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                request.OwnerId = (int)Session["ID"];
                // request.OwnerId = user.ID;

                request.Data = DateTime.Now;
                request.Solved = false;
                if (file != null)
                {
                    
                    string ImageName = System.IO.Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath("~/DataImages/images/" + ImageName);

                    // save image in folder
                    file.SaveAs(physicalPath);

                    request.Imagine = ImageName;
                   
                }
                    db.Requests.Add(request);

                db.SaveChanges();
                return RedirectToAction("Index", "Requests");

            }

            ViewBag.OwnerId = new SelectList(db.Users, "ID", "FirstName", request.OwnerId);
            return View(request);
        }
        
       
        // GET: Requests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "ID", "FirstName", request.OwnerId);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Titlu,Imagine,Imagine1,Descriere,OwnerId,Data,Solved,Greutate,AdresaExpeditor,AdresaDestinatar,CategorieProdus,Judet,Oras,Email,Phone")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "ID", "FirstName", request.OwnerId);
            return View(request);
        }

        // GET: Requests/Delete/5

        // GET: Requests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
          
            Request request = db.Requests.Find(id);
            
            db.Requests.Remove(request);
           
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Answers(int id)
        {
            var answers = db.Answers.Where(ans => ans.RequestId == id).ToList();
            return View("AnswersDetails", answers);
        }

        [HttpGet]
        public ActionResult AnswerRequest(int requestId)
        {
            AnswersDto model = new AnswersDto();
            Request request = db.Requests.Find(requestId);

            model.RequestId = requestId;
            model.Titlu = request.Titlu;
            model.Descriere = request.Descriere;

            return View("AnswerRequest", model);
        }
        
 
        [HttpPost]
        public ActionResult AnswerRequest(AnswersDto answerDto)
        {
            Answer ans = new Answer();
            
            ans.RequestId = answerDto.RequestId;
            ans.ServiceId = (int)Session["ID"];
            ans.Message = answerDto.Message;
            ans.Pret = answerDto.Pret;
            ans.Phone = answerDto.Telefon;

            db.Answers.Add(ans);
            db.SaveChanges();
            return RedirectToAction("Index", "Requests");
            //return View();
        }
        public ActionResult AcceptaOferta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("AcceptaOferta")]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptaOferta(int id)
        {

            Request request = db.Requests.Find(id);

            db.Requests.Remove(request);

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
