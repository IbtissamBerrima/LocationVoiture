﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Car_LoactionV6.Models;

namespace Car_LoactionV6.Controllers
{
    public class RentalsController : Controller
    {
        private Context db = new Context();

        public ActionResult Success()
        {            return View();

        }
        // GET: Rentals
        public ActionResult Index()
        {
            var rentals = db.Rentals.Include(r => r.Cars).Include(r => r.Users);
            return View(rentals.ToList());
        }

        // GET: Rentals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Rentals/Create
        public ActionResult Create()
        {
            ViewBag.IdCar = new SelectList(db.Cars, "IdCar", "Matriculation");
            ViewBag.UID = new SelectList(db.users, "UID", "Nom");
            return View();
        }

        // POST: Rentals/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRental,TypeLocation,DateDebut,DateFin,UID,IdCar" )] Rental rental, int id)
        {
            if (!ModelState.IsValid)
                return View();
            int duree = (rental.DateFin - rental.DateDebut).Days;

            if (duree >= 15)
            {
                rental.TypeLocation = "Longue duree";
            }
            else
            {
                rental.TypeLocation = "Courte duree";
            }
            int v = (int)Session["Iduser"];
            rental.Status = "En Attente";
            rental.IdCar = id;
            rental.UID = v;
            db.Rentals.Add(rental);
            db.SaveChanges();
                return RedirectToAction("Index","Cars");
            
            
        }

        // GET: Rentals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCar = new SelectList(db.Cars, "IdCar", "Matriculation", rental.IdCar);
            ViewBag.UID = new SelectList(db.users, "UID", "Nom", rental.UID);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRental,TypeLocation,DateDebut,DateFin,UID,IdCar")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCar = new SelectList(db.Cars, "IdCar", "Matriculation", rental.IdCar);
            ViewBag.UID = new SelectList(db.users, "UID", "Nom", rental.UID);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);

            db.Rentals.Remove(rental);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AccepterConfirm(int id)
        {
            Rental rental = db.Rentals.Find(id);
            rental.Status = "Confirmed";
            db.SaveChanges();
            return RedirectToAction("Index", "Rentals");
        }

        public ActionResult RefuserConfirm(int id)
        {
            Rental rental = db.Rentals.Find(id);
            rental.Status = "Declined";
            db.SaveChanges();
            return RedirectToAction("Index", "Rentals");
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
