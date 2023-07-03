

using BigBangAssessment2.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBangAssessment2.Repository
{
    public interface IDoctor_Repository
    {
        /*  IEnumerable<Doctor> GetDoctors();
          void AddDoctor(Doctor doctor);

          Doctor GetDoctor(int id);*/
        Task<IEnumerable<Doctor>> GetDoctors();
        Task AddDoctor(Doctor doctor);
        Task<Doctor> GetDoctor(int id);
     //   Task UpdateDoctor(Doctor doctor);
        Task DeleteDoctor(Doctor doctor);
       Task ChangeStatus(int id);
    }
}
