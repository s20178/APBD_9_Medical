using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicalApp.Data;
using MedicalApp.DTO;
using MedicalApp.Models;

namespace MedicalApp.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly MedicalContext _context;

        public PrescriptionService(MedicalContext context)
        {
            _context = context;
        }

        public async Task<Prescription> AddPrescriptionAsync(PrescriptionDto prescriptionDto)
        {
            var patient = await _context.Patients.FindAsync(prescriptionDto.PatientId);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = prescriptionDto.PatientFirstName,
                    LastName = prescriptionDto.PatientLastName,
                    BirthDate = prescriptionDto.PatientBirthDate
                };
                _context.Patients.Add(patient);
            }

            var doctor = await _context.Doctors.FindAsync(prescriptionDto.DoctorId);
            if (doctor == null) throw new ArgumentException("Doctor not found");

            var prescription = new Prescription
            {
                Date = prescriptionDto.Date,
                DueDate = prescriptionDto.DueDate,
                Patient = patient,
                Doctor = doctor
            };

            if (prescriptionDto.Medicaments.Count > 10) throw new ArgumentException("Too many medicaments");

            foreach (var medDto in prescriptionDto.Medicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(medDto.MedicamentId);
                if (medicament == null) throw new ArgumentException("Medicament not found");

                var prescriptionMedicament = new PrescriptionMedicament
                {
                    Medicament = medicament,
                    Dose = medDto.Dose,
                    Details = medDto.Details
                };
                prescription.PrescriptionMedicaments.Add(prescriptionMedicament);
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return prescription;
        }

        public async Task<Patient> GetPatientDataAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == patientId);

            if (patient == null) throw new ArgumentException("Patient not found");

            return patient;
        }
    }
}
