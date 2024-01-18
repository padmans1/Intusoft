using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using INTUSOFT.Data.NewDbModel;
using INTUSOFT.Data.Repository;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
namespace INTUSOFT.Desktop.Forms
{
    public partial class MedicalHistoryViewModel : ObservableObject
    {
        public delegate void ShowMessageBox(string message, MedicalHistory history);
        public event ShowMessageBox _ShowMessageBox;
        private MedicalHistory history;
        public MedicalHistoryViewModel(MedicalHistory _medicalHistory)
        {
            history = medicalHistory = _medicalHistory;
            if (Screen.PrimaryScreen.Bounds.Width == 1920)
            {
                fontSize = 28f;
                margin = new Thickness( 10, 5, 5,0 );

            }
            else if (Screen.PrimaryScreen.Bounds.Width == 1366)
            {
                fontSize = 22f;
                margin = new Thickness( 10,5,15,10);

            }
            else if (Screen.PrimaryScreen.Bounds.Width == 1280)
            {
                fontSize = 18f;
            }
        }
        [ObservableProperty]
        private MedicalHistory medicalHistory;

        [ObservableProperty]
        private float fontSize;
        [ObservableProperty]
        private Thickness margin;

        [RelayCommand]
        private void Save()
        {
                _ShowMessageBox("Changes would overwrite the existing history, Do really want to save?", medicalHistory);
                //MedicalHistoryCloseEvent();
           

        }
        [RelayCommand]
        private void Cancel()
        {
            _ShowMessageBox("There are unsaved changes,Cancelling would discard all change Do really want to cancel?", medicalHistory);

            //MedicalHistoryCloseEvent();
        }
        public MedicalHistory getMedicalHistory()
        {
            return medicalHistory;
        }


        //public string MajorComplaints => MedicalHistoryModel.MajorComplaints;
    }
}
