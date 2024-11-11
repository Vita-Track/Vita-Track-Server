using Microsoft.AspNetCore.Mvc;
using Vita_Track_Server.Data;
using Vita_Track_Server.Models;

namespace Vita_Track_Server.Controllers
{
    public class LoginPayload
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class AssociationPayload
    {
        public string? DoctorId { get; set; }
        public string? PatientId { get; set; }
    }
    [Route("api/[controller]")]
    public class MainController(MongoDBServices mongoDBServices, IJWTAuth iJWTAuth) : Controller
    {
        private readonly MongoDBServices _mongoDBServices = mongoDBServices;
        private readonly IJWTAuth _iJWTAuth = iJWTAuth;
        [HttpGet("home")]
        public IActionResult Home()
        {
            Console.WriteLine("Home");
            return Ok(new { message = "Welcome to Vita Track Server" });
        }
        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] DoctorModel doctor)
        {
            try
            {
                await _mongoDBServices.RegisterDoctor(doctor);
                return Ok(new { message = "Doctor registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientModel patient)
        {
            try
            {
                await _mongoDBServices.RegisterPatient(patient);
                return Ok(new { message = "Patient registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("doctor-login")]
        public async Task<IActionResult> DoctorLogin([FromBody] LoginPayload loginPayload)
        {
            try
            {
                Console.WriteLine("Doctor Login");
                if (loginPayload.Email == null || loginPayload.Password == null)
                {
                    throw new Exception("Email and password are required");
                }
                Console.WriteLine(loginPayload.Email);
                var retrievedUser = await _mongoDBServices.DoctorLogin(loginPayload.Email, loginPayload.Password);
                // Console.WriteLine(retrievedUser.FirstName);
                // Console.WriteLine(retrievedUser.LastName);
                // Console.WriteLine(retrievedUser.Email);
                // Console.WriteLine(retrievedUser.Phone);
                // Console.WriteLine(retrievedUser.ClinicAddress);
                // Console.WriteLine(retrievedUser.Specialization);
                // Console.WriteLine(retrievedUser.DateOfBirth);
                // Console.WriteLine(retrievedUser.Experience);
                // Console.WriteLine(retrievedUser.LicenseNumber);

                DoctorDataForJWT doctorDataForJWT = new()
                {
                    Id = retrievedUser.Id,
                    FirstName = retrievedUser.FirstName,
                    LastName = retrievedUser.LastName,
                    Email = retrievedUser.Email,
                    Phone = retrievedUser.Phone,
                    ClinicAddress = retrievedUser.ClinicAddress,
                    Specialization = retrievedUser.Specialization,
                    DateOfBirth = retrievedUser.DateOfBirth,
                    Experience = retrievedUser.Experience,
                    LicenseNumber = retrievedUser.LicenseNumber
                };

                // var tk = _iJWTAuth.JWTTokenAuth(doctorDataForJWT);
                // Console.WriteLine(tk);
                return Ok(new
                {
                    message = "Doctor logged in successfully",
                    token = _iJWTAuth.JWTTokenAuth(doctorDataForJWT)
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("patient-login")]
        public async Task<IActionResult> PatientLogin([FromBody] LoginPayload loginPayload)
        {
            try
            {
                Console.WriteLine("Patient Login");
                if (loginPayload.Email == null || loginPayload.Password == null)
                {
                    throw new Exception("Email and password are required");
                }
                var retrievedUser = await _mongoDBServices.PatientLogin(loginPayload.Email, loginPayload.Password);
                // Console.WriteLine(retrievedUser.FirstName);
                // Console.WriteLine(retrievedUser.LastName);
                // Console.WriteLine(retrievedUser.Email);
                // Console.WriteLine(retrievedUser.Phone);
                // Console.WriteLine(retrievedUser.DateOfBirth);
                PatientDataForJWT patientDataForJWT = new()
                {
                    Id = retrievedUser.Id,
                    FirstName = retrievedUser.FirstName,
                    LastName = retrievedUser.LastName,
                    Email = retrievedUser.Email,
                    Phone = retrievedUser.Phone,
                    DateOfBirth = retrievedUser.DateOfBirth
                };
                return Ok(new
                {
                    message = "Patient logged in successfully",
                    token = _iJWTAuth.JWTTokenAuthPatient(patientDataForJWT)
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("create-association")]
        public async Task<IActionResult> AssociateDoctor([FromBody] AssociationPayload associationPayload)
        {
            try
            {
                await _mongoDBServices.AssociateDoctor(associationPayload.DoctorId, associationPayload.PatientId);
                return Ok(new { message = "Doctor and Patient associated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}