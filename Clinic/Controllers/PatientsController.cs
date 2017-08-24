using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;

namespace Clinic.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        [Authorize]
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Patients.ToListAsync());
        //}
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var patients = from s in _context.Patients
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p => p.LastName.Contains(searchString)
                                            || p.FirstName.Contains(searchString)
                                            || p.Pesel.Contains(searchString)
                                            || p.Email.Contains(searchString) 
                                            || p.Phone.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    patients = patients.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    patients = patients.OrderBy(s => s.DateOfBirth);
                    break;
                case "date_desc":
                    patients = patients.OrderByDescending(s => s.DateOfBirth);
                    break;
                default:
                    patients = patients.OrderBy(s => s.LastName);
                    break;
            }
            return View(await patients.AsNoTracking().ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .SingleOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize]
        public IActionResult Create()
        {
            var gender = new List<SelectListItem>();
            foreach (Sex eVal in Enum.GetValues(typeof(Sex)))
            {
                gender.Add(new SelectListItem { Text = Enum.GetName(typeof(Sex), eVal), Value = eVal.ToString() });
            }
            Patient defaultDate = new Patient();
            defaultDate.DateOfBirth = DateTime.Today;
            
            ViewBag.Gender = gender;
            return View(defaultDate);
        }

     
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientID,FirstName,LastName,Pesel,Email,Phone,Gender,DateOfBirth,Address,Note")] Patient patient)
        {
            var gender = new List<SelectListItem>();
            foreach (Sex eVal in Enum.GetValues(typeof(Sex)))
            {
                gender.Add(new SelectListItem { Text = Enum.GetName(typeof(Sex), eVal), Value = eVal.ToString() });
            }
            ViewBag.Gender = gender;

            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gender = new List<SelectListItem>();
            foreach (Sex eVal in Enum.GetValues(typeof(Sex)))
            {
                gender.Add(new SelectListItem { Text = Enum.GetName(typeof(Sex), eVal), Value = eVal.ToString() });
            }

            ViewBag.Gender = gender;
            var patient = await _context.Patients.SingleOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

     
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientID,FirstName,LastName,Pesel,Email,Phone,Gender,DateOfBirth,Address,Note")] Patient patient)
        {
            if (id != patient.PatientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .SingleOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.SingleOrDefaultAsync(m => m.PatientID == id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientID == id);
        }
    }
}
