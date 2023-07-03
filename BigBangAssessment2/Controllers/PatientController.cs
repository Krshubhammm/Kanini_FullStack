using BigBangAssessment2.Models;
using BigBangAssessment2.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigBangAssessment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _context;

        public PatientController(IPatientRepository context)
        {
            _context = context;
        }

        /*   // GET api/patients
           [HttpGet]
           public ActionResult<IEnumerable<Patient>> Get()
           {
               return _context.Patients.ToList();
           }*/

        /* // GET api/patients/5
         [HttpGet("{id}")]
         public ActionResult<Patient> Get(int id)
         {
             var patient = _context.Patients.Find(id);
             if (patient == null)
             {
                 return NotFound();
             }
             return patient;
         }*/

        [HttpPost]
        public ActionResult<ICollection<Patient>> PostCour(Patient d)
        {
            _context.AddPatient(d);
            return Ok(d);
        }

        [HttpGet]

        public ActionResult<ICollection<Patient>> GetCour()
        {
            var res = _context.GetPatients();
            return Ok(res);
        }

        [HttpGet("{id}")]

        public ActionResult<ICollection<Patient>> GetCour(int id)
        {
            var res = _context.GetPatient(id);
            return Ok(res);
        }


       /* [HttpDelete("{id}")]

        public void Delete(int id)
        {
            _context.DeletePatient(id);
        }*/

        [HttpPut]

        public void Put(Patient d)
        {
            _context.AddPatient(d);
        }
    }
}
