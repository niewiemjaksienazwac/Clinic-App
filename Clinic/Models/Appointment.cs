using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        public int PatientID { get; set; }

        public int DoctorID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data wizyty")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfAppointment { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Godzina wizyty")]
        public DateTime TimeOfAppointment { get; set; }

        [Display(Name = "Notatka")]
        [DisplayFormat(NullDisplayText = "Brak notatki")]
        public string Note { get; set; }

        [Display(Name = "Pacjent")]
        public Patient Patient { get; set; }

        [Display(Name = "Lekarz")]
        public Doctor Doctor { get; set; }
    }
}
