using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Community.Data;

namespace Community.Controllers
{
    public class AnswersController : Controller
    {
        private CommunityEntities db = new CommunityEntities();

        public ActionResult Index(int reqId)
        {
            //var answers = db.Answers.Include(a => a.Request).Include(a => a.User);
            //return View(answers.ToList());
            List<Answer> answers = db.Answers.Include(a => a.Request).Include(a => a.User).ToList();

            if (reqId != 0)
            {
                answers = answers.Where(a => a.RequestId == reqId).ToList();
            }

            return View(answers.ToList());
        }


        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            
            
            db.SaveChanges();
            return View(answer);

            
            
        }

        // GET: Answers/Create
        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(db.Requests, "ID", "Titlu");
            ViewBag.ServiceId = new SelectList(db.Users, "ID", "FirstName");
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RequestId,ServiceId,Pret,Negociabil,Phone,Message,Gasit")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestId = new SelectList(db.Requests, "ID", "Titlu", answer.RequestId);
            ViewBag.ServiceId = new SelectList(db.Users, "ID", "FirstName", answer.ServiceId);
            return View(answer);
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id,Request request)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestId = new SelectList(db.Requests, "ID", "Titlu", answer.RequestId);
            ViewBag.ServiceId = new SelectList(db.Users, "ID", "FirstName", answer.ServiceId);
            answer.Gasit = true;
            request.Solved = true;

            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RequestId,ServiceId,Pret,Negociabil,Phone,Message,Gasit")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestId = new SelectList(db.Requests, "ID", "Titlu", answer.RequestId);
            ViewBag.ServiceId = new SelectList(db.Users, "ID", "FirstName", answer.ServiceId);
            
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();
            return RedirectToAction("Index","Requests");
        }
        public ActionResult AcceptaOferta()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AcceptaOferta(Request request)
        {
            request.Solved = true;
            db.SaveChanges();
            //return View();
            return RedirectToAction("Index", "Requests");
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
