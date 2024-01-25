using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTUSOFT.Data.NewDbModel
{
    public class Person : IBaseModel
    {
        #region Creation

        public static Person CreateNewPerson()
        {
            return new Person();
        }

        public Person()
        {
            #region this is done in order to handle null datetime appearing when there is no datetime value sriram 16th may 2016
            this.voidedDate = DateTime.Now;
            this.createdDate = DateTime.Now;
            this.lastModifiedDate = DateTime.Now;
            this.lastAccessedDate = DateTime.Now;
            #endregion
        }

        public static Person CreatePerson(
            int id,
            string middelName,
            string firstName,
            string lastName,
             string phoneno,
            string emilId,
            DateTime dob,
            DateTime modified_datetime,
            DateTime touched_dateTime,
            DateTime dateVoided,
            users voidedBy,
            bool birthdateEstimated,
            bool isPatient,
            ISet<person_attribute> attribute,
            ISet<PersonAddressModel> address,
            users Creator,
            users accessedBy,
            string voidReason,
            users changedBy,
            byte[] profileimagename,
            string uuid,
            char Gender,
            bool hideShowRow,
            DateTime createdTime
            )
        {
            return new Person
            {
                personId = id,
                firstName = firstName,
                middleName = middelName,
                lastName = lastName,
                profileImageName = profileimagename,
                primaryPhoneNumber=phoneno,
                primaryEmailId=emilId,
                gender = Gender,
                birthdate = dob,
                birthdateEstimated = birthdateEstimated,
                createdBy = Creator,
                lastModifiedBy = changedBy,
                voidedDate = dateVoided,
                lastAccessedBy = accessedBy,
                attributes = attribute,
                addresses = address,
                lastAccessedDate = touched_dateTime,
                lastModifiedDate = modified_datetime,
                voidedBy = voidedBy,
                voidedReason = voidReason,
                voided = hideShowRow,
                createdDate = createdTime,
                //uuid=uuid,
            };
        }

        #endregion // Creation
        
        #region State Properties

        public virtual int personId { get; set; }

        public virtual string firstName { get; set; }

        public virtual string middleName { get; set; }

        public virtual byte[] profileImageName { get; set; }

        public virtual string primaryPhoneNumber { get; set; }

        public virtual string primaryEmailId { get; set; }

        public virtual ISet<person_attribute> attributes { get; set; }

        public virtual ISet<PersonAddressModel> addresses { get; set; }

        //public virtual string uuid { get; set; }

        public virtual users createdBy { get; set; }

        public virtual users lastModifiedBy { get; set; }

        public virtual users voidedBy { get; set; }

        public virtual char gender { get; set; }

        public virtual string lastName { get; set; }

        public virtual DateTime birthdate { get; set; }

        public virtual string voidedReason { get; set; }

        public virtual DateTime createdDate { get; set; }

        public virtual DateTime lastModifiedDate { get; set; }

        public virtual DateTime lastAccessedDate { get; set; }

        public virtual DateTime voidedDate { get; set; }

        public virtual string PatientPhoto { get; set; }

        public virtual users lastAccessedBy { get; set; }

        public virtual bool voided { get; set; }

        public virtual bool birthdateEstimated { get; set; }

        #endregion // State Properties
    }
    public static class MedicalHistoryHelper
    {
        public static MedicalHistory GetMedicalHistory(this string medicalHistory)
        {
            if (!string.IsNullOrEmpty(medicalHistory))
            {
                return JsonConvert.DeserializeObject<MedicalHistory>(medicalHistory);
            }
            return new MedicalHistory();
        }
        public static string SetMedicalHistory(this MedicalHistory medicalHistory)
        {
           return JsonConvert.SerializeObject(medicalHistory);

        }
    }
    public class MedicalHistory : IEquatable<MedicalHistory>
    {
        public MedicalHistory()
        {
            FamilyHistory = string.Empty;
            MajorComplaints = string.Empty;
            DiseaseHistory = new DiseaseHistory();
            VisualAcuity = new VisualAcuityModel();
            OcularHistory = new OcularHistory();
            Refraction = new RefractionModel();
            BloodPressure = new BloodPressureDetails();
            Findings = new FindingsModel();
            IOP = new IOP();
        }
        public string FamilyHistory { get; set; }
        public string MajorComplaints { get; set; }
        public DiseaseHistory  DiseaseHistory{ get; set; }
        public VisualAcuityModel VisualAcuity{ get; set; }
        public OcularHistory OcularHistory{ get; set; }
        public RefractionModel Refraction { get; set; }
        public BloodPressureDetails BloodPressure { get; set; }
        public FindingsModel Findings { get; set; }
        public IOP IOP { get; set; }

        public bool Equals(MedicalHistory other)
        {
            return Equals(this, other);
        }
    }

    public class IOP
    {
        public IOP()
        {
            LeftEye = string.Empty;
            RightEye = string.Empty;
        }
        public string LeftEye { get; set; }
        public string RightEye { get; set; }
    }
    public class BloodPressureDetails
    {
        public BloodPressureDetails()
        {
            DBP = "0";
            SBP = "0";
        }
        public string DBP { get; set; }
        public string SBP { get; set; }
    }
    public class FindingsModel
    {
        public FindingsModel()
        {
            LeftEye = new FindingsDetails();
            RightEye = new FindingsDetails();
        }
        public FindingsDetails RightEye { get; set; }
        public FindingsDetails LeftEye { get; set; }
    }
    public class FindingsDetails
    {
        public FindingsDetails()
        {
            ASValue = string.Empty;
            PSValue = string.Empty;
        }
        public string ASValue { get; set; }
        public string PSValue { get; set; }
    }

    public class RefrationDetails
    {
        public RefrationDetails()
        {
            Spherical = "0";
            Cylindrical = "0";
            Axis = "0";
            VisualAcuity = new VisualAcuityValue();
        }
        public string Spherical{ get; set; }
        public string Cylindrical{ get; set; }
        public string Axis{ get; set; }
        public VisualAcuityValue VisualAcuity { get; set; }
    }
    public class RefractionModel
    {
        public RefractionModel()
        {
            LeftEye = new RefrationDetails();
            RightEye = new RefrationDetails();
            AddLeftEye = new RefrationDetails();
            AddRightEye = new RefrationDetails();
        }
        public RefrationDetails LeftEye{ get; set; }
        public RefrationDetails RightEye{ get; set; }
        public RefrationDetails AddLeftEye{ get; set; }
        public RefrationDetails AddRightEye{ get; set; }
    }

    public class DiseaseHistory
    {
        public DiseaseHistory()
        {
            Asthma = new DiseaseDetails();
            DM = new DiseaseDetails();
            HTN = new DiseaseDetails();
            Others = new DiseaseDetails();
        }
        public DiseaseDetails Asthma { get; set; }
        public DiseaseDetails DM { get; set; }
        public DiseaseDetails HTN { get; set; }
        public DiseaseDetails Others { get; set; }
    }
    public class DiseaseDetails
    {
        public DiseaseDetails()
        {
            IsPresent = false;
            Years = "0";
            Details = string.Empty;
        }
        public bool IsPresent { get; set; } 
        public string Years{ get; set; } 
        public string Details { get; set; }
    }
    public class VisualAcuityModel
    {
        public VisualAcuityModel()
        {
            UnAided = new VisualAcuityDetails();
            PinHole = new VisualAcuityDetails();
        }
        public VisualAcuityDetails UnAided { get; set; }
        public VisualAcuityDetails PinHole { get; set; }
    }
    public class VisualAcuityDetails
    {
        public VisualAcuityDetails()
        {
            LeftEyeDetails = new VisualAcuityValue();
            RightEyeDetails = new VisualAcuityValue();
        }
        public VisualAcuityValue LeftEyeDetails { get; set; }
        public VisualAcuityValue RightEyeDetails { get; set; }
    }
    public class VisualAcuityValue
    {
        public VisualAcuityValue()
        {
            Numerator = "0";
            Denominator = "0";
        }
        public string Numerator { get; set; }
        public string Denominator { get; set; }
    }
    public class OcularHistory
    {
        public OcularHistory()
        {
            Eyelids = new OcularDetails();
            ExtraOcularMovements = new OcularDetails();
            Cornea = new OcularDetails();
            AnteriorChamber = new OcularDetails();
            Conjuctiva = new OcularDetails();
            Sclera = new OcularDetails();
            Iris = new OcularDetails();
            Pupil = new OcularDetails();
            Lens = new OcularDetails();
        }
        public OcularDetails Eyelids { get; set; }
        public OcularDetails ExtraOcularMovements { get; set; }
        public OcularDetails Cornea { get; set; }
        public OcularDetails AnteriorChamber { get; set; }
        public OcularDetails Conjuctiva { get; set; }
        public OcularDetails Sclera { get; set; }
        public OcularDetails Iris { get; set; }
        public OcularDetails Pupil { get; set; }
        public OcularDetails Lens { get; set; }
    }
    public class OcularDetails
    {
        public OcularDetails()
        {
            RightEye = string.Empty;
            LeftEye = string.Empty;
        }
        public string RightEye { get; set; }
        public string LeftEye { get; set; }
    }
}
