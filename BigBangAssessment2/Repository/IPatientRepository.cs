using BigBangAssessment2.Models;

namespace BigBangAssessment2.Repository
{
    public interface IPatientRepository
    {
      
        Task<IEnumerable<Patient>> GetPatients();
        Task AddPatient(Patient Patients);
        Task<Patient> GetPatient(int id);
        Task UpdateDoctor(Patient Patient);
        Task DeleteDoctor(Patient Patient);
    }
}
