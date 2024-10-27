namespace Vita_Track_Server.Models
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string DoctorCollectionName { get; set; } = null!;
        public string PatientCollectionName { get; set; } = null!;
    }
}
