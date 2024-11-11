using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Vita_Track_Server;

public class Authentication(IAuthentication context) : IJWTAuth
{
    private readonly IAuthentication _context = context;

    public string JWTTokenAuth(DoctorDataForJWT doctorDataForJWT)
    {
        var key = _context.Key;
        var issuer = _context.Issuer;
        var tokenGenerator = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("Id", doctorDataForJWT.Id!),
                new Claim("FirstName", doctorDataForJWT.FirstName!),
                new Claim("LastName", doctorDataForJWT.LastName!),
                new Claim("Email", doctorDataForJWT.Email!),
                new Claim("Phone", doctorDataForJWT.Phone!),
                new Claim("ClinicAddress", doctorDataForJWT.ClinicAddress!),
                new Claim("Specialization", doctorDataForJWT.Specialization!),
                new Claim("DateOfBirth", doctorDataForJWT.DateOfBirth!),
                new Claim("Experience", doctorDataForJWT.Experience!),
                new Claim("LicenseNumber", doctorDataForJWT.LicenseNumber!),
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = issuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenGenerator);
        return tokenHandler.WriteToken(token);
    }
    public string JWTTokenAuthPatient(PatientDataForJWT patientDataForJWT)
    {
        var key = _context.Key;
        var issuer = _context.Issuer;
        var tokenGenerator = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("Id", patientDataForJWT.Id!),
                new Claim("FirstName", patientDataForJWT.FirstName!),
                new Claim("LastName", patientDataForJWT.LastName!),
                new Claim("Email", patientDataForJWT.Email!),
                new Claim("Phone", patientDataForJWT.Phone!),
                new Claim("DateOfBirth", patientDataForJWT.DateOfBirth!)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenGenerator);
        return tokenHandler.WriteToken(token);
    }
}
