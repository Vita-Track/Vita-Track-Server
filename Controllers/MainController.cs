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
                Console.WriteLine(retrievedUser.FirstName);
                Console.WriteLine(retrievedUser.LastName);
                Console.WriteLine(retrievedUser.Email);
                Console.WriteLine(retrievedUser.Phone);
                Console.WriteLine(retrievedUser.ClinicAddress);
                Console.WriteLine(retrievedUser.Specialization);
                Console.WriteLine(retrievedUser.DateOfBirth);
                Console.WriteLine(retrievedUser.Experience);
                Console.WriteLine(retrievedUser.LicenseNumber);

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

                var tk = _iJWTAuth.JWTTokenAuth(doctorDataForJWT);
                Console.WriteLine(tk);
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
    }
}