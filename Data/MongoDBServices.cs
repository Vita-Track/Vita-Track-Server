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
            doctor.AssociatedPatients = [];
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
            patient.AssociatedDoctors = [];
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
        public async Task AssociateDoctor(string? doctorId, string? patientId)
        {
            var doctor = await _doctorCollection.Find(d => d.Id == doctorId).FirstOrDefaultAsync() ?? throw new Exception("Doctor not found");
            var patient = await _patientCollection.Find(p => p.Id == patientId).FirstOrDefaultAsync() ?? throw new Exception("Patient not found");

            // Check if association already exists
            if (doctor.AssociatedPatients == null) doctor.AssociatedPatients = new List<PatientModel>();
            if (patient.AssociatedDoctors == null) patient.AssociatedDoctors = new List<DoctorModel>();

            bool isDoctorAlreadyAssociated = doctor.AssociatedPatients.Exists(p => p.Id == patientId);
            bool isPatientAlreadyAssociated = patient.AssociatedDoctors.Exists(d => d.Id == doctorId);

            if (!isDoctorAlreadyAssociated)
            {
                doctor.AssociatedPatients.Add(patient);
                await _doctorCollection.ReplaceOneAsync(d => d.Id == doctorId, doctor);
            }

            if (!isPatientAlreadyAssociated)
            {
                patient.AssociatedDoctors.Add(doctor);
                await _patientCollection.ReplaceOneAsync(p => p.Id == patientId, patient);
            }
        }
    }
}