using System;
using System.Collections.Generic;

namespace MedicalApp.Models
{
    public class Patient
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public byte[] RowVersion { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
