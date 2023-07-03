using BigBangAssessment2.Models;
using BigBangAssessment2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BigBangAssessment2.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor_Repository _Irepository;
        private ApplicationDbContext context;
        public DoctorController(IDoctor_Repository repository, ApplicationDbContext context)
        {
            _Irepository = repository;
            this.context = context;
        }



        [Authorize(Roles = "admin")]

        [HttpGet]
        public async Task<ActionResult> FetchDoctors()
        {
            var doctors = await Task.Run(() => _Irepository.GetDoctors());
            return Ok(doctors);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> AddDoctor(Doctor doctor)
        {
            await Task.Run(() => _Irepository.AddDoctor(doctor));
            return Ok(doctor);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDoctor(int id)
        {
            var doctor = await Task.Run(() => _Irepository.GetDoctor(id));
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDoctor(int id, Doctor doctor)
        {
           var doc = context.Doctors.Find(id);
            if (doc != null)
            {
                doc.FirstName= doctor.FirstName;
                doc.LastName= doctor.LastName;
                doc.Clinic_Location = doctor.Clinic_Location;
                doc.Fees = doctor.Fees; 
                doc.IsActive= doctor.IsActive;
                doc.Phone= doctor.Phone;
                context.Doctors.Add(doc); 
                context.SaveChanges();
                return Ok(doc);  
            }
            return BadRequest();    
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var doctor = await Task.Run(() => _Irepository.GetDoctor(id));
            if (doctor == null)
            {
                return NotFound();
            }

            await Task.Run(() => _Irepository.DeleteDoctor(doctor));
            return NoContent();
        }
        /*[HttpGet("patientcount")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorPatientCount()
        {
            var doctorPatientCount = await _Irepository.GetDoctors().Select(d => new
                {
                    DoctorName = d.Name,
                    PatientCount = d.Patients.Count
                })
                .ToListAsync();

            return Ok(doctorPatientCount);
        }*/

        [Authorize(Roles = "admin")]
        [Route("api/doctor/change-status/{id}")]

        [HttpPut]
        public async Task<ActionResult> ChangeStatus(int id)
        {
            return Ok( _Irepository.ChangeStatus(id));
        }

        [Authorize(Roles = "patient")]
        [HttpGet("activated")]
        public IActionResult GetActivatedDoctors()
        {
            var activatedDoctors = context.Doctors.Where(d => d.IsActive == true).ToList();
            return Ok(activatedDoctors);
        }


    }

}
