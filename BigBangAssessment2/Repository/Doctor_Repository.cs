
using BigBangAssessment2.Models;
using BigBangAssessment2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigBangAssessment2.Repository
{
    public class Doctor_Repository : IDoctor_Repository
    {
        private readonly ApplicationDbContext _dbContext;
        //user repository constructor
        public Doctor_Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Doctor>> GetDoctors()
        {
            
            return await Task.Run(() => _dbContext.Doctors.Include(x =>x.Patients).ToList());
        }

        public async Task AddDoctor(Doctor doctor)
        {
            await Task.Run(() => {
                _dbContext.Doctors.Add(doctor);
                _dbContext.SaveChanges();
            });
        }

        public async Task<Doctor> GetDoctor(int id)
        {
            return await Task.Run(() => _dbContext.Doctors.Find(id));
        }

       

        public async Task DeleteDoctor(Doctor doctor)
        {
            await Task.Run(() =>
            {
                _dbContext.Doctors.Remove(doctor);
                _dbContext.SaveChanges();
            });
        }

        
        public async Task ChangeStatus(int id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            /*if (doctor == null)
            {
                return NotFound($"Doctor Not Found with id = {id}");

            }*/
            doctor.IsActive = !doctor.IsActive;
            _dbContext.SaveChanges();

        }

    }
}
