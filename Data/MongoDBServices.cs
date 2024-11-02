using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Vita_Track_Server.Models;


namespace Vita_Track_Server.Data
{
    public class MongoDBServices
    {
        private readonly IMongoCollection<DoctorModel> _doctorCollection;
        private readonly IMongoCollection<PatientModel> _patientCollection;
        public MongoDBServices(IOptions<MongoDBSettings> mongoDBSettings)
        {
            try
            {
                MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
                IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
                _doctorCollection = database.GetCollection<DoctorModel>(mongoDBSettings.Value.DoctorCollectionName);
                _patientCollection = database.GetCollection<PatientModel>(mongoDBSettings.Value.PatientCollectionName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task RegisterDoctor(DoctorModel doctor)
        {
            var doctorExists = await _doctorCollection.Find(x => x.Email == doctor.Email).FirstOrDefaultAsync();
            if (doctorExists != null)
            {
                throw new Exception("Doctor with this email already exists");
            }
            doctor.Password = BCrypt.Net.BCrypt.HashPassword(doctor.Password);
            await _doctorCollection.InsertOneAsync(doctor);
        }
        public async Task<DoctorModel> DoctorLogin(string? email, string? password)
        {
            var doctorExists = await _doctorCollection.Find(x => x.Email == email).FirstOrDefaultAsync() ?? throw new Exception("Doctor with this email does not exist");
            if (!BCrypt.Net.BCrypt.Verify(password, doctorExists.Password))
            {
                throw new Exception("Invalid password");
            }
            return doctorExists;
        }
        public async Task RegisterPatient(PatientModel patient)
        {
            var patientExists = await _patientCollection.Find(x => x.Email == patient.Email).FirstOrDefaultAsync();
            if (patientExists != null)
            {
                throw new Exception("Patient with this email already exists");
            }
            patient.Password = BCrypt.Net.BCrypt.HashPassword(patient.Password);
            await _patientCollection.InsertOneAsync(patient);
        }
        public async Task<PatientModel> PatientLogin(string? email, string? password)
        {
            var patientExists = await _patientCollection.Find(x => x.Email == email).FirstOrDefaultAsync() ?? throw new Exception("Patient with this email does not exist");
            if (!BCrypt.Net.BCrypt.Verify(password, patientExists.Password))
            {
                throw new Exception("Invalid password");
            }
            return patientExists;
        }
    }
}