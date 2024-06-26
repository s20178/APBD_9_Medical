using System.Threading.Tasks;
using MedicalApp.DTO;
using MedicalApp.Models;

namespace MedicalApp.Services
{
    public interface IPrescriptionService
    {
        Task<Prescription> AddPrescriptionAsync(PrescriptionDto prescriptionDto);
        Task<Patient> GetPatientDataAsync(int patientId);
    }
}
