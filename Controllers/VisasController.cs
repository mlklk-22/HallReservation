﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstProject.Models;
using Microsoft.AspNetCore.Http;

namespace FirstProject.Controllers
{
    public class VisasController : Controller
    {
        private readonly ModelContext _context;

        public VisasController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Agreement(decimal id)
        {

            #region Session For Username and hallNum
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.hallNum = HttpContext.Session.GetString("hallNum");
            #endregion

            #region Reservation By UserName and Id
            var ReservationAdmin = (from Reservation in _context.Reservations
                                    where Reservation.Username == "mlklk-22" && Reservation.Hallnumber == id
                                    select Reservation).ToList(); 
            #endregion

            #region Update Status and Owner 
            foreach (var item in ReservationAdmin)
            {
                item.Username = ViewBag.UserName;
                item.Status = "Pending";
            } 
            #endregion

            _context.SaveChanges();
            return View(ReservationAdmin);
        }
        public IActionResult AgreementAcc(decimal id)
        {

            #region Session Of Username and hall number
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = HttpContext.Session.GetString("UserName"); 
            var Username = ViewBag.UserName;
            #endregion          
           
            #region Reservation by Status and Hall Number
            var ReservationsWhenPending = (from Reservation in _context.Reservations
                                           where Reservation.Status == "Pending" && Reservation.Hallnumber == id
                                           select Reservation).ToList();
            #endregion

            #region Get visa and Reservation of User to Update Status and Pocket
            var UpdatePocket = 0;
            var price = 0;
            foreach (var Reservationcol in ReservationsWhenPending)
            {
                price = (int)Reservationcol.Price;
                Reservationcol.Username = ViewBag.UserName;
                Reservationcol.Status = "Full";
                foreach (var Visacol in _context.Visas.Where(x => x.Username == Reservationcol.Username))
                {
                    UpdatePocket = Convert.ToInt32(Visacol.Pocket) - price;
                    Visacol.Pocket = Convert.ToString(UpdatePocket);
                }
            } 
            #endregion

            _context.SaveChanges();
            return RedirectToAction("ReqRes", "Visas");
        }
        public IActionResult DisAgreement(decimal id)
        {

            #region Session Of Username And Hall Num
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.hallNum = HttpContext.Session.GetString("hallNum");
            #endregion

            #region Reservation By Id
            var ReservationById = (from Reservation in _context.Reservations
                                   where Reservation.Hallnumber == id
                                   select Reservation).ToList(); 
            #endregion

            #region Get visa and Reservation of User to Update Status and Pocket
            var UpdatePocket = 0;
            var Price = 0;
           
            foreach (var ReservationCol in ReservationById)
            {
                Price = (int)ReservationCol.Price;

                ReservationCol.Status = "Available";
                foreach (var VisaCol in _context.Visas.Where(col => col.Username == ReservationCol.Username))
                {
                    UpdatePocket = Convert.ToInt32(VisaCol.Pocket) + Price;
                    VisaCol.Pocket = Convert.ToString(UpdatePocket);
                    ReservationCol.Username = "mlklk-22";
                }

            } 
            #endregion

            _context.SaveChanges();
            return View(ReservationById);
        }
        public IActionResult DisAgreeAdmin(decimal id)
        {

            #region Session Of Username
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            #endregion

            #region Reservatoin By Id
            var ReservationById = (from Reservation in _context.Reservations
                                   where Reservation.Hallnumber == id
                                   select Reservation).ToList();
            #endregion

            #region Get visa and Reservation of User to Update Status and Pocket
            var UpdatePocket = 0;
            var price = 0;
            foreach (var item in ReservationById)
            {
                price = (int)item.Price;
                if (item.Status == "Pending")
                {
                    item.Status = "Available";
                    foreach (var item2 in _context.Visas.Where(x => x.Username == item.Username))
                    {
                        item.Username = "mlklk-22";
                    }
                }
                else
                {
                    item.Status = "Available";
                    foreach (var item2 in _context.Visas.Where(x => x.Username == item.Username))
                    {
                        UpdatePocket = Convert.ToInt32(item2.Pocket) + price;
                        item2.Pocket = Convert.ToString(UpdatePocket);
                        item.Username = "mlklk-22";
                    }
                }
            } 
            #endregion

            _context.SaveChanges();
            return RedirectToAction("ReqRes", "Visas");
        }
        public IActionResult ReqRes()
        {
            #region Session For AdminName, FirstName, LastName and UserName
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            #endregion

            #region List Of Users
            var Users = _context.Reservations.Where(x => x.Username != null && x.Username != "mlklk-22");

            #endregion           

            return View(Users);
        }
        public IActionResult MyVisa()
        {
            #region Session For User
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            #endregion

            #region All Visas
            var AllVisas = _context.Visas;

            #endregion

            return View(AllVisas);
        }
        public IActionResult ReqSub()
        {
            #region Session For CardNumber, And UserName
            ViewBag.cardNum = HttpContext.Session.GetString("cardNum");
            ViewBag.UserName = HttpContext.Session.GetString("UserName"); 
            #endregion
            return View();
        }
        public IActionResult MyReservations()
        {
            #region Session For AdminName, FirstName, LastName and UserName
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            #endregion

            #region AllUsers
            var Users = _context.Reservations.Where(x => x.Username != null && x.Username != "mlklk-22");
            #endregion

            return View(Users);              
        }
        public async Task<IActionResult> Index()    
        {
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            var modelContext = _context.Visas.Include(v => v.Role).Include(v => v.UsernameNavigation);
            return View(await modelContext.ToListAsync());
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas
                .Include(v => v.Role)
                .Include(v => v.UsernameNavigation)
                .FirstOrDefaultAsync(m => m.Cardnumber == id);
            if (visa == null)
            {
                return NotFound();
            }

            return View(visa);
        }
        public IActionResult Create()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename");
            ViewData["Username"] = new SelectList(_context.Logins, "Username", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Firstname,Lastname,Cardnumber,Exp,Thrnum,Pocket,Roleid,Username")] Visa visa)
        {
            if (ModelState.IsValid)
            {
                    HttpContext.Session.SetString("cardNum", visa.Cardnumber);
                    visa.Roleid = 2;
                     visa.Pocket = "1000";
                    visa.Username = HttpContext.Session.GetString("UserName");
                    _context.Add(visa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("HomeUser", "Homef");
            }
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename", visa.Roleid = 2);
            ViewData["Username"] = new SelectList(_context.Logins, "Username", "Username", HttpContext.Session.GetString("UserName"));
            return View(visa);
        }
       public IActionResult Visa()
        {
            ViewBag.cardNum = HttpContext.Session.GetString("cardNum");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }
        public IActionResult ER()
        {
            ViewBag.cardNum = HttpContext.Session.GetString("cardNum");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas.FindAsync(id);
            if (visa == null)
            {
                return NotFound();
            }
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename", visa.Roleid);
            ViewData["Username"] = new SelectList(_context.Logins, "Username", "Username", visa.Username);
            return View(visa);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Firstname,Lastname,Cardnumber,Exp,Thrnum,Pocket,Roleid,Username")] Visa visa)
        {
            if (id != visa.Cardnumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisaExists(visa.Cardnumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename", visa.Roleid);
            ViewData["Username"] = new SelectList(_context.Logins, "Username", "Username", visa.Username);
            return View(visa);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas
                .Include(v => v.Role)
                .Include(v => v.UsernameNavigation)
                .FirstOrDefaultAsync(m => m.Cardnumber == id);
            if (visa == null)
            {
                return NotFound();
            }

            return View(visa);
        }
     

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var visa = await _context.Visas.FindAsync(id);
            _context.Visas.Remove(visa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VisaExists(string id)
        {
            return _context.Visas.Any(e => e.Cardnumber == id);
        }
    }
}
