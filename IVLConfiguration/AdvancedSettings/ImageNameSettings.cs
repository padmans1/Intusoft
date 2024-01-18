using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace INTUSOFT.Configuration.AdvanceSettings
{
    [Serializable]
    public class ImageNameSettings
    {
        private static  IVLControlProperties changeImageName = null;

        public IVLControlProperties ChangeImageName
        {
            get { return changeImageName; }
            set { changeImageName = value; }
        }

        private static  IVLControlProperties isMRNPresent = null;

        public IVLControlProperties IsMRNPresent
        {
            get { return isMRNPresent; }
            set { isMRNPresent = value; }
        }

        private static  IVLControlProperties isFirstNamePresent = null;

        public IVLControlProperties IsFirstNamePresent
        {
            get { return isFirstNamePresent; }
            set { isFirstNamePresent = value; }
        }

        private static  IVLControlProperties isLastNamePresent = null;

        public IVLControlProperties IsLastNamePresent
        {
            get { return isLastNamePresent; }
            set { isLastNamePresent = value; }
        }

        private static IVLControlProperties isAgePresent = null;
        public IVLControlProperties IsAgePresent
        {
            get { return isAgePresent; }
            set { isAgePresent = value; }
        }

        private static IVLControlProperties isGenderPresent = null;
        public IVLControlProperties IsGenderPresent
        {
            get { return isGenderPresent; }
            set { isGenderPresent = value; }
        }

        private static IVLControlProperties isWriteAllDetails = null;
        public IVLControlProperties IsWriteAllDetails
        {
            get { return isWriteAllDetails; }
            set { isWriteAllDetails = value; }
        }

        private static  IVLControlProperties isEyeSidePresent = null;

        public IVLControlProperties IsEyeSidePresent
        {
            get { return isEyeSidePresent; }
            set { isEyeSidePresent = value; }
        }


        public ImageNameSettings()
        {

            ChangeImageName = new IVLControlProperties();
            ChangeImageName.name = "ChangeImageName";
            ChangeImageName.type = "bool";
            ChangeImageName.val = false.ToString();
            ChangeImageName.control = "System.Windows.Forms.RadioButton";
            ChangeImageName.text = "Change Image Name";

            IsMRNPresent = new IVLControlProperties();
            IsMRNPresent.name = "IsMRNPresent";
            IsMRNPresent.type = "bool";
            IsMRNPresent.val = false.ToString();
            IsMRNPresent.control = "System.Windows.Forms.RadioButton";
            IsMRNPresent.text = "Is MRN Present ";

            IsFirstNamePresent = new IVLControlProperties();
            IsFirstNamePresent.name = "IsFirstNamePresent";
            IsFirstNamePresent.type = "bool";
            IsFirstNamePresent.val = false.ToString();
            IsFirstNamePresent.control = "System.Windows.Forms.RadioButton";
            IsFirstNamePresent.text = "Is First Name Present";

            IsLastNamePresent = new IVLControlProperties();
            IsLastNamePresent.name = "IsLastNamePresent";
            IsLastNamePresent.type = "bool";
            IsLastNamePresent.val = false.ToString();
            IsLastNamePresent.control = "System.Windows.Forms.RadioButton";
            IsLastNamePresent.text = "Is Last Name Present";

            IsEyeSidePresent = new IVLControlProperties();
            IsEyeSidePresent.name = "IsEyeSidePresent";
            IsEyeSidePresent.type = "bool";
            IsEyeSidePresent.val = false.ToString();
            IsEyeSidePresent.control = "System.Windows.Forms.RadioButton";
            IsEyeSidePresent.text = "Is Eye Side Present";

            IsAgePresent = new IVLControlProperties();
            IsAgePresent.name = "IsAgePresent";
            IsAgePresent.type = "bool";
            IsAgePresent.val = false.ToString();
            IsAgePresent.control = "System.Windows.Forms.RadioButton";
            IsAgePresent.text = "Is Age Present";

            IsGenderPresent = new IVLControlProperties();
            IsGenderPresent.name = "IsGenderPresent";
            IsGenderPresent.type = "bool";
            IsGenderPresent.val = false.ToString();
            IsGenderPresent.control = "System.Windows.Forms.RadioButton";
            IsGenderPresent.text = "Is Gender Present";

            IsWriteAllDetails = new IVLControlProperties();
            IsWriteAllDetails.name = "IsWriteAllDetails";
            IsWriteAllDetails.type = "bool";
            IsWriteAllDetails.val = false.ToString();
            IsWriteAllDetails.control = "System.Windows.Forms.RadioButton";
            IsWriteAllDetails.text = "Is Write All Details";
        }

    }
}
