using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Vita_Track_Server.Models
{
    public class DoctorModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }
        [JsonProperty("lastName")]
        public string? LastName { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }
        [JsonProperty("phoneNumber")]
        public string? Phone { get; set; }
        [JsonProperty("clinicAddress")]
        public string? ClinicAddress { get; set; }
        [JsonProperty("specialization")]
        public string? Specialization { get; set; }
        [JsonProperty("dateOfBirth")]
        public string? DateOfBirth { get; set; }
        [JsonProperty("experience")]
        public string? Experience { get; set; }
        [JsonProperty("licenseNumber")]
        public string? LicenseNumber { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }
        [JsonProperty("associatedPatients")]
        public List<string>? AssociatedPatients { get; set; }

    }
}