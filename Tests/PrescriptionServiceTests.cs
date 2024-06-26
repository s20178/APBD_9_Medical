using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MedicalApp.Data;
using MedicalApp.DTO;
using MedicalApp.Models;
using MedicalApp.Services;

namespace MedicalApp.Tests
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<MedicalContext> _contextMock;
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionServiceTests()
        {
            _contextMock = new Mock<MedicalContext>();
            _prescriptionService = new PrescriptionService(_contextMock.Object);
        }

        private Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> elements) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elements.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elements.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elements.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elements.GetEnumerator());

            return dbSetMock;
        }

        [Fact]
        public async Task AddPrescriptionAsync_ShouldAddPrescription_WhenValidData()
        {
            // Arrange
            var prescriptionDto = new PrescriptionDto
            {
                PatientId = 1,
                PatientFirstName = "John",
                PatientLastName = "Doe",
                PatientBirthDate = new DateTime(1980, 1, 1),
                DoctorId = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { MedicamentId = 1, Dose = 1, Details = "Take once daily" }
                }
            };

            var patients = new List<Patient>().AsQueryable();
            var patientsMock = CreateDbSetMock(patients);

            _contextMock.Setup(c => c.Patients).Returns(patientsMock.Object);

            // Act
            var result = await _prescriptionService.AddPrescriptionAsync(prescriptionDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Patient.IdPatient);
            Assert.Equal(1, result.Doctor.IdDoctor);
            Assert.Single(result.PrescriptionMedicaments);
        }

        [Fact]
        public async Task GetPatientDataAsync_ShouldReturnPatient_WhenPatientExists()
        {
            // Arrange
            var patientId = 1;
            var patient = new Patient
            {
                IdPatient = patientId,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1980, 1, 1),
                Prescriptions = new List<Prescription>
                {
                    new Prescription
                    {
                        IdPrescription = 1,
                        Date = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(7),
                        Doctor = new Doctor { IdDoctor = 1, FirstName = "Jane", LastName = "Smith", Specialization = "General" },
                        PrescriptionMedicaments = new List<PrescriptionMedicament>
                        {
                            new PrescriptionMedicament { Medicament = new Medicament { IdMedicament = 1, Name = "Med1", Description = "Desc1" }, Dose = 1, Details = "Take once daily" }
                        }
                    }
                }
            };

            var patients = new List<Patient> { patient }.AsQueryable();
            var patientsMock = CreateDbSetMock(patients);

            _contextMock.Setup(c => c.Patients).Returns(patientsMock.Object);

            // Act
            var result = await _prescriptionService.GetPatientDataAsync(patientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.IdPatient);
            Assert.Single(result.Prescriptions);
        }
    }
}
