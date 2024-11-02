namespace Vita_Track_Server;

public class DoctorDataForJWT
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? ClinicAddress { get; set; }
    public string? Specialization { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Experience { get; set; }
    public string? LicenseNumber { get; set; }
}
public interface IJWTAuth
{
    public string JWTTokenAuth(DoctorDataForJWT doctorDataForJWT);
}