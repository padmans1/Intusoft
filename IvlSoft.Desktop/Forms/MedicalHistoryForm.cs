using Common;
using INTUSOFT.Data.NewDbModel;
using INTUSOFT.Data.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INTUSOFT.Desktop.Forms
{
    public partial class MedicalHistoryForm : Form
    {
        MedicalHistory_UC userControl;
        MedicalHistoryViewModel viewModel;
        public MedicalHistoryForm()
        {
            InitializeComponent();
            
            var elemthost1 = new System.Windows.Forms.Integration.ElementHost();

            elemthost1.Dock = DockStyle.Fill; // change to to suit your need

            // you can add the WPF control to the form or any other desired control
            elemthost1.Parent = this;
            this.WindowState = FormWindowState.Maximized;
            

            // change to to suit your need
            var medicalHistory = NewDataVariables.Active_Visit.medicalHistory.GetMedicalHistory();
            viewModel = new MedicalHistoryViewModel(medicalHistory);
            viewModel._ShowMessageBox += MedicalHistoryVM__ShowMessageBox;
            userControl = new MedicalHistory_UC(viewModel); // Assign the WPF control
            elemthost1.Child = userControl; // Assign the WPF control

        }

        private void MedicalHistoryVM__ShowMessageBox(string message, MedicalHistory history)
        {
           
            CloseEvent(message, history);
        }

        //private void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        //{
        //    this.Close();
        //}


        private void MedicalHistoryForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {

            CloseEvent("There are unsaved changes,Cancelling would discard all change Do really want to close?", viewModel.getMedicalHistory(), true);

        }
        private void CloseEvent(string message, MedicalHistory history, bool isForm = false)
        {
            MedicalHistory medicalHistory = NewDataVariables.Active_Visit.medicalHistory.GetMedicalHistory();
            if (medicalHistory.SetMedicalHistory() != history.SetMedicalHistory())
            {
                if (CustomMessageBox.Show(message, "History Save Warning", CustomMessageBoxButtons.YesNo, CustomMessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    NewDataVariables.Active_Visit.medicalHistory = history.SetMedicalHistory();
                    var currentPat = NewDataVariables.GetCurrentPat();
                    CloudPatientInfo cloudPatientInfo = new CloudPatientInfo()
                    {
                        Age = (DateTime.Now.Year - currentPat.birthdate.Year),
                        PatientName = $"{currentPat.firstName} {currentPat.lastName}",
                        OrganaizationName = IVLVariables.CurrentSettings.UserSettings._HeaderText.val,
                        OrganaizationId = IVLVariables.CurrentSettings.ReportSettings.ImagingCenterId.val,
                        ScreeningCenter = IVLVariables.CurrentSettings.ReportSettings.ImagingCenter.val,
                        OperatorId = IVLVariables.CurrentSettings.ReportSettings.UserName.val,
                        ClinicalHistory = history.SetMedicalHistory(),
                        Gender = currentPat.gender == 'M' ? "Male" : "Female",
                        PatientId = NewDataVariables.Active_PatientIdentifier.value,
                        Phone = currentPat.primaryPhoneNumber,
                        TestDoneBy = IVLVariables.CurrentSettings.ReportSettings.UserName.val,
                        VisitDate = currentPat.patientLastModifiedDate,
                        VisitId = (currentPat.visits.ToList().IndexOf(NewDataVariables.Active_Visit) + 1).ToString()
                    };
                    CreateModifyPatient_UC.UpsertPatientInfo(cloudPatientInfo);
                    if (!isForm)
                           this.Close();
                }
                else
                {
                    if(!isForm)
                        this.Close();
                }
            }
            else
                if(!isForm)
                    this.Close();



        }
    }
}
