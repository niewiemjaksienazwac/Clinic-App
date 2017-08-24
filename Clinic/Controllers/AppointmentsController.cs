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
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        static string[] hoursArray = new string[] { "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30" };

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
            
        }
               
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
           
            var applicationDbContext = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient);
            return View(await applicationDbContext.AsNoTracking().ToListAsync());
        }

                
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
                
        [Authorize]
        public IActionResult Create()
        {
            ViewData["Hours"] = new SelectList(hoursArray);
            ViewData["DoctorID"] = new SelectList(_context.Doctors.OrderBy(d => d.LastName), "DoctorID", "FullName");
            ViewData["PatientID"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "PatientID", "FullNamePesel");

            Appointment defaultDate = new Appointment();
            defaultDate.DateOfAppointment = DateTime.Today;
            return View(defaultDate);
        }

        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentID,PatientID,DoctorID,DateOfAppointment,TimeOfAppointment,Note")] Appointment appointment)
        {
            ViewData["Hours"] = new SelectList(hoursArray);
            ViewData["DoctorID"] = new SelectList(_context.Doctors.OrderBy(d=>d.LastName), "DoctorID", "FullName", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "PatientID", "FullNamePesel", appointment.PatientID);

            if (ModelState.IsValid)
            {
                if(appointment.DateOfAppointment < DateTime.Today || (appointment.DateOfAppointment == DateTime.Today && appointment.TimeOfAppointment<DateTime.Now)) 
                {
                    ModelState.AddModelError(string.Empty, "Wybierz odpowiedni¹ datê i godzinê");                  

                    return View(appointment);
                }



                Appointment doctorOtherAppointment = _context.Appointments.Where(a => a.DoctorID == appointment.DoctorID)
                    .Where(a => a.DateOfAppointment.Date == appointment.DateOfAppointment)
                    .Where(a => a.TimeOfAppointment == appointment.TimeOfAppointment)
                    .FirstOrDefault();

                if (doctorOtherAppointment != null)
                {
                    ModelState.AddModelError(string.Empty, "Lekarz ma w tym czasie inn¹ wizytê!");

                    return View(appointment);

                }

                Appointment patientOtherAppointment = _context.Appointments.Where(a => a.PatientID == appointment.PatientID)
                    .Where(a => a.DateOfAppointment.Date == appointment.DateOfAppointment)
                    .Where(a => a.TimeOfAppointment == appointment.TimeOfAppointment)
                    .FirstOrDefault();

                if (patientOtherAppointment != null)
                {
                    ModelState.AddModelError(string.Empty, "Pacjent ma w tym czasie inn¹ wizytê!");

                    return View(appointment);
                }

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(appointment);
        }
        
        
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.SingleOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["Hours"] = new SelectList(hoursArray);
            ViewData["DoctorID"] = new SelectList(_context.Doctors.OrderBy(d => d.LastName), "DoctorID", "FullName", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "PatientID", "FullNamePesel", appointment.PatientID);
            return View(appointment);
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentID,PatientID,DoctorID,DateOfAppointment,TimeOfAppointment,Note")] Appointment appointment)
        {
            if (id != appointment.AppointmentID)
            {
                return NotFound();
            }

            ViewData["Hours"] = new SelectList(hoursArray);
            ViewData["DoctorID"] = new SelectList(_context.Doctors.OrderBy(d => d.LastName), "DoctorID", "FullName", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "PatientID", "FullNamePesel", appointment.PatientID);

            if (ModelState.IsValid)
            {
                if (appointment.DateOfAppointment < DateTime.Today || (appointment.DateOfAppointment == DateTime.Today && appointment.TimeOfAppointment < DateTime.Now))
                {
                    ModelState.AddModelError(string.Empty, "Wybierz odpowiedni¹ datê i godzinê");

                    return View(appointment);
                }


                Appointment doctorOtherAppointment = _context.Appointments.Where(a => a.DoctorID == appointment.DoctorID)
                    .Where(a => a.DateOfAppointment.Date == appointment.DateOfAppointment)
                    .Where(a => a.TimeOfAppointment == appointment.TimeOfAppointment)
                    .Where(a => a.AppointmentID != appointment.AppointmentID)
                    .FirstOrDefault();

                if (doctorOtherAppointment != null)
                {
                    ModelState.AddModelError(string.Empty, "lekarz ma w tym czasie inn¹ wizytê");

                    return View(appointment);

                }

                Appointment patientOtherAppointment = _context.Appointments.Where(a => a.PatientID == appointment.PatientID)
                    .Where(a => a.DateOfAppointment.Date == appointment.DateOfAppointment)
                    .Where(a => a.TimeOfAppointment == appointment.TimeOfAppointment)
                    .Where(a => a.AppointmentID != appointment.AppointmentID)
                    .FirstOrDefault();

                if (patientOtherAppointment != null)
                {
                    ModelState.AddModelError(string.Empty, "pacjent ma w tym czasie inn¹ wizytê");

                    return View(appointment);

                }


                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentID))
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
            return View(appointment);
        }

        
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.SingleOrDefaultAsync(m => m.AppointmentID == id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentID == id);
        }
    }
}
