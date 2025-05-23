﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ServiceProcess;
using System.ServiceProcess.Design;
using INTUSOFT.EventHandler;
using INTUSOFT.Data.Repository;
using INTUSOFT.Data.NewDbModel;
using INTUSOFT.Desktop.Properties;
using INTUSOFT.Custom.Controls;
using System.IO;
using INTUSOFT.Imaging;
using Common;
using INTUSOFT.Data;
using Common.Validators;
using Common.ValidatorDatas;
using System.Globalization;
using System.Resources;
using INTUSOFT.ThumbnailModule;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Security.Policy;
using System.Management;
using System.Management.Instrumentation;
using Microsoft.VisualBasic.Devices;
using INTUSOFT.Configuration;
using NLog;
namespace INTUSOFT.Desktop.Forms
{
    public partial class IvlMainWindow : BaseGradientForm
    {
        #region variables and constants


        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static Logger exceptionLog = LogManager.GetLogger("ExceptionLog");
        Login_UCL loginScreen;
        //UI_Camera cameraUI;
        //Imaging_UC cameraUI;
        ThemesForm tForm;
        GradientForm gForm;
        EmrManage emr;
        IVLEventHandler _eventHandler;
        PatDetails_UC ActivePatUI;
        DataBaseServiceAndConnection dataBaseServerConnection;
        System.Diagnostics.Stopwatch stW;
        Process p;
        SplashScreen splashScreen;
        Imaging_UC imaging_UC;
        System.Timers.Timer serverTimer;// old system.timer 
        System.Timers.Timer splashScreenTimer;//
        //MicroTimer serverTimer;// micro Timer added for more precision
        //BackgroundWorker bgw;
        string selcetedLanguage = string.Empty;
        string mrn = "";
        string firstName = "";
        string LastName = "";
        string dataBasebackupPath = string.Empty;
        ServiceController mySC;
        string Gender = "";
        string Age = "";
        bool isComponentInitialized = false;
        int databaseTimerIntervel;
        Bitmap cameraConnected, cameraDisconnected, PowerConnected, PowerDisconnected, serviceAvaiable, serviceNotAvaiable, databaseConnected, databaseNotConnected, CameraError;
        string ConfigFileName = "IVLConfig.xml";
        string RestoreFieName = "IVLRestore.xml";
        bool isCameraConnected = false;
        bool isPowerConnected = false;
        ImageList imgList;
        string thumbnailFileName = "";
        string mysqlServiceName = "MySQL57";
        long timeElapsed;
        public static string LogFilePath = "";
        delegate void ShowCameraDelegate(String s,Args arg);
        ShowCameraDelegate mShowCameraDelegate;
        delegate void ShowPowerDelegate(String s, Args arg);
        ShowPowerDelegate mShowPowerDelegate;
        //This below code is added by darshan to solve defect no 0000478: Not Responding message is coming up.
        [DllImport("user32.dll")]
        public static extern void DisableProcessWindowsGhosting();
        public bool isEmr = false;

        bool vc2010Count = false;
        bool vc2012Count = false;
        bool vc2013Count = false;
        bool vc2015Count = false;
        bool frameworkVersionCount = false;

        string displayNameText = "DisplayName";
        const string displayVersionText = "DisplayVersion";
        const string csdVersionText = "CSDVersion";
        const string productNameText = "ProductName";
        const string pathText = "Path";
        const string vc10RedistributablePath = @"SOFTWARE\Classes\Installer\Products";
        const string remainingVcRedistributablePath = @"SOFTWARE\Classes\Installer\Dependencies";
        const string versionsPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        const string servicePackPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        string adobeReaderPath = @"SOFTWARE\Adobe\Adobe Reader";
        const string osBitVersionPath = @"SYSTEM\CurrentControlSet\Control\Session Manager";

        int prerequisitesCount = 0;
        Microsoft.Win32.RegistryKey key = Registry.LocalMachine;
        string registry_key = string.Empty;
        List<string> prerequisiteList;
        string uninstalledPrerquisite = string.Empty;
        IntuSoftRuntimeProperties intuSoftRuntime;

        #endregion
        public IvlMainWindow()
        {
            IVLVariables.GradientColorValues = new Desktop.GradientColor();
            
            _eventHandler = IVLEventHandler.getInstance();
            prerequisiteList = new List<string>();
            prerequisiteList.Add(Constants.adobeReaderVersion);
            prerequisiteList.Add(Constants.boardDriver);
            prerequisiteList.Add(Constants.cameraDriverVersion);
            //prerequisiteList.Add(Constants.flashProgrammerVersion); removed flash programmer check in the intusoft application to avoid not launching of the application
            prerequisiteList.Add(Constants.frameworkVersion);
            prerequisiteList.Add(Constants.mysqlVersion);
            prerequisiteList.Add(Constants.servicePackVersion);
            prerequisiteList.Add(Constants.vcredistributable2013);
            prerequisiteList.Add(Constants.vcredistributable2015);

            #region event registration for IVL event handlers

            _eventHandler.Register(_eventHandler.Consultation2Imaging, new NotificationHandler(Consultation2Imaging));
            _eventHandler.Register(_eventHandler.ShowSplashScreen, new NotificationHandler(ShowSplashScreen));
            _eventHandler.Register(_eventHandler.ShowThumbnails, new NotificationHandler(showThumbnails));
            _eventHandler.Register(_eventHandler.SetActivePatDetails, new NotificationHandler(SetActivePatientDetails));
            _eventHandler.Register(_eventHandler.Navigate2ViewImageScreen, new NotificationHandler(Navigate2ViewImageScreen));
            _eventHandler.Register(_eventHandler.UPDATE_CAMERA_STATUS, new NotificationHandler(ShowCameraConnection));
            _eventHandler.Register(_eventHandler.UPDATE_POWER_STATUS, new NotificationHandler(ShowPowerConnection));
            _eventHandler.Register(_eventHandler.ExportImageFiles, new NotificationHandler(exportFiles));
            _eventHandler.Register(_eventHandler.EnableImagingBtn, new NotificationHandler(EnableDisableImagingBtn));
            _eventHandler.Register(_eventHandler.ThumbnailSelected, new NotificationHandler(ThumbnailSelected));
            _eventHandler.Register(_eventHandler.GetImageFiles, new NotificationHandler(getImageFiles));
            _eventHandler.Register(_eventHandler.ChangeThumbnailSide, new NotificationHandler(changeThumbnailSide));
            _eventHandler.Register(_eventHandler.ThumbnailAdd, new NotificationHandler(AddThumbnailEvent));
            _eventHandler.Register(_eventHandler.EnableCapturePowerStatusTimer, new NotificationHandler(enableCapturePowerStatusTimer));
            _eventHandler.Register(_eventHandler.IsShiftAndControl, new NotificationHandler(isshiftandcontrol));
            _eventHandler.Register(_eventHandler.ConnectCamera, new NotificationHandler(Connect2Camera));
            _eventHandler.Register(_eventHandler.Navigate2LiveScreen, new NotificationHandler(NavigateFromViewToLive));
            // Defect number 0000581 has been fixed by calling the same function as in the case of space bar by sriram on August 18th 2015
            _eventHandler.Register(_eventHandler.CaptureScreenUpdate, new NotificationHandler(CaptureUIUpdate));
            _eventHandler.Register(_eventHandler.UpdateMainWindowCursor, new NotificationHandler(CursorUpdate));
            _eventHandler.Register(_eventHandler.StartStopServerDatabaseTimer, new NotificationHandler(StartStopServerDatabaseTimer));
            _eventHandler.Register(_eventHandler.UpdateCaptureRLiveUI, new NotificationHandler(updateCaptureRLiveUI));
            _eventHandler.Register(_eventHandler.GoToViewScreen, new NotificationHandler(GoToViewScreen));
            _eventHandler.Register(_eventHandler.EnableDisableEmrButton, new NotificationHandler(EnableDisableEmrButton));

            mShowCameraDelegate = new ShowCameraDelegate(ShowCameraConnection);
            mShowPowerDelegate = new ShowPowerDelegate(ShowPowerConnection);
            CustomMessageBox newMsgBox = new CustomMessageBox();
            newMsgBox.shiftControlEvent += newMsgBox_shiftControlEvent;
            #endregion
            IVLVariables.ExceptionLog = Exception2StringConverter.GetInstance();// display exceptions that are caught. By Ashutosh 11-08-2017.
            Control.CheckForIllegalCrossThreadCalls = false;
           
            this.MinimumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);//This code has been added to resolve the defect of application resizing when double clicked on the form header.
            //The below 5 lines are added by sriram sir to handle the object reference problem when resolution is set to 1024*768.
            IVLVariables.LangResourceManager = new ResourceManager("INTUSOFT.Desktop.LanguageResources.Res", typeof(IvlMainWindow).Assembly);
            IVLConfig.fileName = IVLVariables.appDirPathName + ConfigFileName;
            if (!File.Exists(IVLConfig.fileName))
            {
                if (File.Exists(ConfigFileName))
                    File.Copy(ConfigFileName, IVLConfig.fileName);
            }
            IVLVariables._ivlConfig = IVLConfig.getInstance();
            IVLVariables.ivl_Camera = IntucamHelper.GetInstance();
            //IVLVariables.ivl_Camera.ImagingMode = IVLVariables._ivlConfig.Mode = ImagingMode.Posterior_45;
            //IVLVariables.ivl_Camera.camPropsHelper.ImagingMode = (Imaging.ImagingMode)Enum.Parse(typeof(Imaging.ImagingMode), ConfigVariables._ivlConfig.Mode.ToString());
            Configuration.ImagingMode currentMode = ConfigVariables._ivlConfig.Mode;
            #region to set the prime and anterior current gain levels to default values

          if (currentMode == Configuration.ImagingMode.Posterior_Prime || currentMode == Configuration.ImagingMode.Anterior_Prime)
          {
              INTUSOFT.Configuration.ImagingMode[] modes = new Configuration.ImagingMode[] { Configuration.ImagingMode.Anterior_Prime, Configuration.ImagingMode.Posterior_Prime };
              for (int i = 0; i < modes.Length; i++)
              {
                  IVLVariables._ivlConfig.Mode = modes[i];
                  Configuration.ConfigVariables.GetCurrentSettings();
                  IVLVariables.CurrentSettings.CameraSettings.CaptureCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._DigitalGainDefault.val;
                  IVLVariables.CurrentSettings.CameraSettings.LiveCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._LiveGainDefault.val;
                  INTUSOFT.Configuration.ConfigVariables.SetCurrentSettings();
              }
              Configuration.ConfigVariables._ivlConfig.Mode = currentMode;
          }
            #endregion
            
            ConfigVariables.GetCurrentSettings();
            IVLVariables.FileFolderValidator = Common.Validators.FileNameFolderPathValidator.GetInstance();
            m_DelegateLive2ViewViaTrigger = new DelegateLive2ViewViaTrigger(NavigateFromViewToLive);
            dataBaseServerConnection = new DataBaseServiceAndConnection();
            IVLVariables.CurrentSettings.CameraSettings._LiveDigitalGain.min = 0;
            IVLVariables.CurrentSettings.CameraSettings._DigitalGain.min = 0;

            #region below block of code has been added to read the db name , user name and password for db connection string

            var runtimePath = IVLVariables.appDirPathName + "Intusoft-runtime.json";
            if (!IVLVariables.isCommandLineArgsPresent)
            {
                IntuSoftRuntimeProperties.filePath = runtimePath;
                if (File.Exists(runtimePath))
                {
                    //foreach (var row in File.ReadAllLines(runtimePath))
                    //    data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                    var data = File.ReadAllText(runtimePath);
                    intuSoftRuntime = (IntuSoftRuntimeProperties)JsonConvert.DeserializeObject(data, typeof(IntuSoftRuntimeProperties));
                }
                if (intuSoftRuntime == null)
                {
                    intuSoftRuntime = new IntuSoftRuntimeProperties();
                    var data = JsonConvert.SerializeObject(intuSoftRuntime);
                    File.WriteAllText(runtimePath, data);
                }

                NHibernateHelper_MySQL.dbName = intuSoftRuntime.dbName;
                NHibernateHelper_MySQL.userName = intuSoftRuntime.userName;
                NHibernateHelper_MySQL.password = intuSoftRuntime.password;
                NHibernateHelper_MySQL.serverPath = intuSoftRuntime.server_path;
                databaseTimerIntervel = Convert.ToInt32(intuSoftRuntime.db_interval);
                dataBasebackupPath = intuSoftRuntime.db_backup_path;
                NHibernateHelper_MySQL.WarningText = IVLVariables.LangResourceManager.GetString("DB_creation_waring_text", IVLVariables.LangResourceCultureInfo);
                NHibernateHelper_MySQL.WarningHeader = IVLVariables.LangResourceManager.GetString("Software_Name", IVLVariables.LangResourceCultureInfo);
                NewDataVariables._Repo = Repository.GetInstance();
                Process process = new Process();
                this.Cursor = Cursors.WaitCursor;
                if (File.Exists(@"CreateOrAlterDB.exe"))
                {
                    process.StartInfo = new ProcessStartInfo(@"CreateOrAlterDB.exe");
                    process.StartInfo.Arguments = string.Format("{0}", runtimePath);
                    process.Start();
                    this.Cursor = Cursors.WaitCursor;

                }
                while (!process.HasExited)
                {
                    ;
                }
                //MessageBox.Show(process.HasExited.ToString());
                this.Cursor = Cursors.Default;

                //if (!NHibernateHelper_MySQL.DbExists(NHibernateHelper_MySQL.dbName))


            }
            #endregion

            if (!Directory.Exists(IVLVariables.CurrentSettings.ImageStorageSettings._ExportImagePath.val))
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                IVLVariables.CurrentSettings.ImageStorageSettings._ExportImagePath.val = desktopPath;
            }
            if (IVLVariables.CurrentSettings.UserSettings._Language.val.ToString().Equals("English") || IVLVariables.CurrentSettings.UserSettings._Language.val.ToString().Equals("en"))
                selcetedLanguage = "en";
            else if (IVLVariables.CurrentSettings.UserSettings._Language.val.ToString().Equals("Spanish") || IVLVariables.CurrentSettings.UserSettings._Language.val.ToString().Equals("es"))
                selcetedLanguage = "es";
            IVLVariables.LangResourceCultureInfo = CultureInfo.CreateSpecificCulture(selcetedLanguage);
            serverTimer = new System.Timers.Timer();
            serverTimer.Elapsed += serverTimer_Elapsed;
            serverTimer.Interval = 60000;// Convert.ToInt32(IVLVariables.CurrentSettings.FirmwareSettings._CameraPowerTimerInterval.val);
            
           

            InitializeComponent();
            IVLVariables.IVLThemes = Themes.GetInstance();
            IVLVariables.IVLThemes.GetAllThemeNames();
            IVLVariables.GradientColorValues = IVLVariables.IVLThemes.GetCurrentTheme();

            this.Color1 = IVLVariables.GradientColorValues.Color1;
            this.Color2 = IVLVariables.GradientColorValues.Color2;
            this.ColorAngle = IVLVariables.GradientColorValues.ColorAngle;
            //this.FontForeColor = IVLVariables.GradientColorValues.FontForeColor;
            //The below static variables are added by Darshan on 02-09-2015 to maintain a single instance of json repositories throught the application.
            DisableProcessWindowsGhosting();
          
            //MessageBox.Show(stW.ElapsedMilliseconds.ToString());
            //NewDataVariables.Patients = NewDataVariables._Repo.GetPageData<INTUSOFT.Data.NewDbModel.Patient>(Convert.ToInt32(IVLVariables.CurrentSettings.UserSettings._NoOfPatientsToBeSelected.val), 1).ToList<INTUSOFT.Data.NewDbModel.Patient>();
            //Config_UC.addToGlobalProperty();
            if (IVLVariables.postprocessingHelper == null)
                IVLVariables.postprocessingHelper = PostProcessing.GetInstance();
            imgList = new ImageList();
            if (Convert.ToBoolean(IVLVariables.CurrentSettings.UserSettings._Is24clock.val))
            {
                Time_lbl.Text = DateTime.Now.ToString(" HH:mm ");
                //Date_lbl.Text = DateTime.Now.ToString("  dd-MM-yyyy ");
                Date_lbl.Text = DateTime.Today.ToShortDateString();//The date format has been changed to maintain a uniform date format.
            }
            else
            {
                Time_lbl.Text = DateTime.Now.ToString("  hh:mm tt ");
                //Date_lbl.Text = DateTime.Now.ToString("  dd-MM-yyyy ");
                Date_lbl.Text = DateTime.Today.ToShortDateString();//The date format has been changed to maintain a uniform date format.
            }
           
            # region lines added in order save the text for each buttons in the custom message box from resource files by Darshan on 22-02-2016
            Common.CustomMessageBox.yesBtnText = IVLVariables.LangResourceManager.GetString("CustomMsg_Yes_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomMessageBox.noBtnText = IVLVariables.LangResourceManager.GetString("CustomMsg_No_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomMessageBox.okBtnText = IVLVariables.LangResourceManager.GetString("Ok_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomMessageBox.cancelBtnText = IVLVariables.LangResourceManager.GetString("Cancel_Button_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomMessageBox.restartLaterBtnText = IVLVariables.LangResourceManager.GetString("CustomMsg_RestartLater_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomMessageBox.restartNowBtnText = IVLVariables.LangResourceManager.GetString("CustomMsg_RestartNow_Text", IVLVariables.LangResourceCultureInfo);
            #endregion

            # region lines added in order save the text for each buttons in the custom copyandreplace form from resource files by Darshan on 26-10-2016
            Common.CustomCopyAndReplace.copyReplaceBtn = IVLVariables.LangResourceManager.GetString("CopyAndReplace_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.copyAndKeepReplaceBtn = IVLVariables.LangResourceManager.GetString("CopyButKeepBothFiles_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.dontCopyBtnText = IVLVariables.LangResourceManager.GetString("DontCopy_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.cancelBtnText = IVLVariables.LangResourceManager.GetString("Cancel_Button_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.noOfConflitsChkbxText1 = IVLVariables.LangResourceManager.GetString("ConflictWarning_Text1", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.noOfConflitsChkbxText2 = IVLVariables.LangResourceManager.GetString("ConflictWarning_Text2", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.dialogText = IVLVariables.LangResourceManager.GetString("CopyAndReplaceDialog_Text", IVLVariables.LangResourceCultureInfo);
            Common.CustomCopyAndReplace.warningLblText = IVLVariables.LangResourceManager.GetString("CopyAndReplaceWarning_Text", IVLVariables.LangResourceCultureInfo);
            #endregion
            //this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.Text = IVLVariables.LangResourceManager.GetString("Software_Name", IVLVariables.LangResourceCultureInfo);

            this.thumbnailUI1.DeleteMsg = IVLVariables.LangResourceManager.GetString("DeleteMsg", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.DeleteSingleImageMsg = IVLVariables.LangResourceManager.GetString("DeleteMsg_SingleImage_Text", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.DeletedImageMsg = IVLVariables.LangResourceManager.GetString("Removing_Deleted_Image_Text", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.DeletedImageHeader = IVLVariables.LangResourceManager.GetString("Removing_Deleted_Image_Header", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.anotherWindowOpen += thumbnailUI1_anotherWindowOpen;
            logOut_lbl.Text = IVLVariables.LangResourceManager.GetString("logOut_label_Text", IVLVariables.LangResourceCultureInfo);
            //This below code block has been added by darshan to resolve the issue of limit no of images.
            this.thumbnailUI1.NoOfImagesToBeSelected = Convert.ToInt32(IVLVariables.CurrentSettings.UserSettings._noOfImagesForReport.val);
            this.thumbnailUI1.NoOfImagesToBeSelectedText1 = IVLVariables.LangResourceManager.GetString("NoOfImagesToBeSelected_Text1", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.NoOfImagesToBeSelectedText2 = IVLVariables.LangResourceManager.GetString("NoOfImagesToBeSelected_Text2", IVLVariables.LangResourceCultureInfo);
            this.thumbnailUI1.NoOfImagesToBeSelectedHeader = IVLVariables.LangResourceManager.GetString("NoOfImagesToBeSelected_Header", IVLVariables.LangResourceCultureInfo);
            // cameraStatus_lbl.Text = IVLVariables.LangResourceManager.GetString("Camera_Status", IVLVariables.LangResourceCultureInfo);
            //cameraText_lbl.Text = IVLVariables.LangResourceManager.GetString("Camera_Status", IVLVariables.LangResourceCultureInfo);
            //PowerStatus_lbl.Text = " " + IVLVariables.LangResourceManager.GetString("Power_Status", IVLVariables.LangResourceCultureInfo);
            //powerText_lbl.Text = " " + IVLVariables.LangResourceManager.GetString("Power_Status", IVLVariables.LangResourceCultureInfo);
            Header_lbl.Text = IVLVariables.CurrentSettings.UserSettings._HeaderText.val;

            this.SetStyle(ControlStyles.Selectable, false);
            emr_btn.Text = IVLVariables.LangResourceManager.GetString("Emr_Btn_Text", IVLVariables.LangResourceCultureInfo);
            ToolTip EmrBtn = new ToolTip();   //code to add tool tip text for the button.
            //EmrBtn.SetToolTip(emr_btn, IVLVariables.LangResourceManager.GetString("RecordButton_ToolTipText", IVLVariables.LangResourceCultureInfo));
            toolStrip4.Renderer = new INTUSOFT.Custom.Controls.FormToolStripRenderer();
            //PagePanel_p.BorderStyle = BorderStyle.FixedSingle;
            emr_btn.ToolTipText = IVLVariables.LangResourceManager.GetString("RecordButton_ToolTipText", IVLVariables.LangResourceCultureInfo);
            this.Image_btn.Text = "";// IVLVariables.LangResourceManager.GetString( "Imaging_btn_Text;
            //Settings_btn.Text = IVLVariables.LangResourceManager.GetString( "Settings_Button_Text",IVLVariables.LangResourceCultureInfo);
            ////Settings_btn.Visible = Convert.ToBoolean(IVLVariables.CurrentSettings.UISettings.EmrUI._IntuCloudButtonVisible.val);
            //ToolTip settingsbtn = new ToolTip();  //code to add tool tip text for the button.
            //settingsbtn.SetToolTip(Settings_btn, IVLVariables.LangResourceManager.GetString("IntuCloudButton_ToolTipText", IVLVariables.LangResourceCultureInfo));
            
            loginScreen = new Login_UCL();
            loginScreen._loggedIn += loginScreen__loggedIn;
            this.thumbnailUI1.deleteimgs += thumbnailUI1_deleteimgs;
            //_eventHandler.Register(_eventHandler.ImageAdded, new NotificationHandler(ImageAdded));
            this.thumbnailUI1.showImgFromThumbnail += thumbnailUI1_showImgFromThumbnail;
            this.thumbnailUI1.imageadded += thumbnailUI1_imageadded;
            this.thumbnailUI1.imageaddedThumbnailData += thumbnailUI1_imageaddedThumbnailData;

            this.thumbnailUI1.sendFocusBackToParent += thumbnailUI1_sendFocusBackToParent;
            #region Logo Paths for the application
            string appLogoFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\IntuSoft.ico";
            string hospitalLogoFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\hospitalLogo.png";
            string companyLogoFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\CompanyLogo.png";
            string recordsLogoFilePath = IVLVariables.appDirPathName + @"ImageResources\PatientDetailsImageResources\Records1.png";
            string productImageFilePath = "";
            if (IVLVariables._ivlConfig.Mode == Configuration.ImagingMode.Posterior_45)// && IVLVariables.ivl_Camera.camPropsHelper.ImagingMode != Imaging.ImagingMode.Posterior_Prime)
                productImageFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\45.jpg";
            else if (IVLVariables._ivlConfig.Mode == Configuration.ImagingMode.FFA_Plus || IVLVariables._ivlConfig.Mode == Configuration.ImagingMode.FFAColor)
                productImageFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\45+.jpg";
            else
                productImageFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\Prime.jpg";

            //if (IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.Posterior_45)// && IVLVariables.ivl_Camera.camPropsHelper.ImagingMode != Imaging.ImagingMode.Posterior_Prime)
            //    productImageFilePath = @"ImageResources\LogoImageResources\45.jpg";
            //else if (IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.FFA_Plus || IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.FFAColor)
            //    productImageFilePath = @"ImageResources\LogoImageResources\45+.jpg";
            //else
            //    productImageFilePath = @"ImageResources\LogoImageResources\Prime.jpg";

            string cameraConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Camera-ON.png";
            string cameraConnErrorFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Camera-Error2.png";
            string cameraDisConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Camera-OFF.png";
            string powerConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Power-ON.png";
            string powerDisConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Power-OFF.png";
            string mysqlServiceConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Database_Server_Running.png";
            string mysqlServiceDisConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Database_Server_Stopped.png";
            string databaseConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Database_Connected_01.png";
            string databaseDisConFilePath = IVLVariables.appDirPathName + @"ImageResources\ConnectionStatusImageResources\Database_No Connection.png";
            #endregion

            #region Apply logo from paths if the file is available
            if (File.Exists(appLogoFilePath))
                this.Icon = new System.Drawing.Icon(appLogoFilePath, 256, 256);

            if (File.Exists(hospitalLogoFilePath))
                HospitalLogo_pbx.Image = new Bitmap(hospitalLogoFilePath);

            if (File.Exists(companyLogoFilePath))
                companyLogo_pbx.Image = new Bitmap(companyLogoFilePath);

            if (File.Exists(recordsLogoFilePath))
                emr_btn.Image = Image.FromFile(recordsLogoFilePath);

            if (File.Exists(productImageFilePath))
                Image_btn.Image = Image.FromFile(productImageFilePath);
            if (File.Exists(cameraConFilePath))
                cameraConnected = new Bitmap(cameraConFilePath);
            if (File.Exists(cameraConnErrorFilePath))
                CameraError = new Bitmap(cameraConnErrorFilePath);
            if (File.Exists(cameraDisConFilePath))
                cameraDisconnected = new Bitmap(cameraDisConFilePath);

            if (File.Exists(powerConFilePath))
                PowerConnected = new Bitmap(powerConFilePath);

            if (File.Exists(powerDisConFilePath))
                PowerDisconnected = new Bitmap(powerDisConFilePath);

            if (File.Exists(mysqlServiceConFilePath))
                serviceAvaiable = new Bitmap(mysqlServiceConFilePath);

            if (File.Exists(mysqlServiceDisConFilePath))
                serviceNotAvaiable = new Bitmap(mysqlServiceDisConFilePath);

            if (File.Exists(databaseConFilePath))
                databaseConnected = new Bitmap(databaseConFilePath);

            if (File.Exists(databaseDisConFilePath))
                databaseNotConnected = new Bitmap(databaseDisConFilePath);

            #endregion
            Image_btn.Visible = true;
            //Image_btn.ImageAlign = ContentAlignment.MiddleLeft;
            patientDetails_p.Visible = false;

            //powerConnectionStatus_pbx.Image = PowerDisconnected;
            powerImage_lbl.Image = PowerDisconnected;
            powerImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("PowerOff_ToolTipText", IVLVariables.LangResourceCultureInfo);
            powerImage_lbl.Text = IVLVariables.LangResourceManager.GetString("Power_Status", IVLVariables.LangResourceCultureInfo);

            //CameraStatus_pbx.Image = cameraDisconnected;
            cameraImage_lbl.Image = cameraDisconnected;
            cameraImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("CameraOff_ToolTipText", IVLVariables.LangResourceCultureInfo);
            cameraImage_lbl.Text = IVLVariables.LangResourceManager.GetString("Camera_Status", IVLVariables.LangResourceCultureInfo);

            MysqlServiceConnection_lbl.Image = serviceAvaiable;
            MysqlServiceConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("MysqlServiceAvailableToolTip_Text", IVLVariables.LangResourceCultureInfo);
            MysqlServiceConnection_lbl.Text = IVLVariables.LangResourceManager.GetString("Server_Label_Text", IVLVariables.LangResourceCultureInfo);

            databaseConnection_lbl.Image = databaseNotConnected;
            databaseConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("DatabaseNotConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo);
            databaseConnection_lbl.Text = IVLVariables.LangResourceManager.GetString("Database_Label_Text", IVLVariables.LangResourceCultureInfo);

            if (Screen.PrimaryScreen.Bounds.Width == 1920)
            {
                MysqlServiceConnection_lbl.Size = databaseConnection_lbl.Size = new Size(70, 70);
                MysqlServiceConnection_lbl.Margin = databaseConnection_lbl.Margin = new Padding(10, 1, 0, 2);
            }
            splashScreen = new SplashScreen();//Initialize splashScreen
            cameraPower_toolstrip.Renderer = new Custom.Controls.FormToolStripRenderer();
            server_toolstrip.Renderer = new Custom.Controls.FormToolStripRenderer();
            database_toolstrip.Renderer = new Custom.Controls.FormToolStripRenderer();
            ImagingButtonToolStrip.Renderer = new Custom.Controls.FormToolStripRenderer();
            Header_lbl.Width = this.Width;
            if (imaging_UC == null)
                imaging_UC = Imaging_UC.GetInstance();

            UpdateFontForeColor();

        //IVLVariables.GradientColorValues.Color1 = this.Color1;
        //IVLVariables.GradientColorValues.Color2 = this.Color2;
        }

        void splashScreenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (IVLVariables.ivl_Camera.IsLogWritingCompleted)
            {
                splashScreen.Close();
            }
        }


        void thumbnailUI1_imageaddedThumbnailData(ThumbnailData thumbnailData)
        {
            if (!isModifiedimage)
            {
                Args arg = new Args();
                arg["ImageName"] = thumbnailData.fileName;
                arg["id"] = thumbnailData.id;
                if(!IVLVariables.ivl_Camera.IsCapturing)
                _eventHandler.Notify(_eventHandler.ImageAdded, arg);
                this.Focus();
            }
        }

        private void UpdateFontForeColor()
        {
            #region resize controls depending on the resolution
            List<Control> controls = GetControls(this).ToList();
            if (Screen.PrimaryScreen.Bounds.Width == 1920)
            {
                cameraImage_lbl.Margin = new Padding(25, 30, cameraImage_lbl.Margin.Right, cameraImage_lbl.Margin.Bottom);
                powerImage_lbl.Margin = new Padding(30, 30, powerImage_lbl.Margin.Right, powerImage_lbl.Margin.Bottom);
                emr_btn.Size = new System.Drawing.Size(222, emr_btn.Size.Height);
                MysqlServiceConnection_lbl.Margin = new Padding(22, MysqlServiceConnection_lbl.Margin.Top, MysqlServiceConnection_lbl.Margin.Right, MysqlServiceConnection_lbl.Margin.Bottom);
            }
            foreach (Control c in controls)
            {
                if (c.Name != emr_btn.Name)
                    c.ForeColor = IVLVariables.GradientColorValues.FontForeColor;
                if (c.Name == HospitalLogo_pbx.Name || c.Name == companyLogo_pbx.Name && Screen.PrimaryScreen.Bounds.Width == 1366)
                {
                    c.Size = new Size();
                }
                if (c is ToolStrip)
                {
                    ToolStrip l = c as ToolStrip;
                    if (l.Name == ImagingButtonToolStrip.Name)
                    {
                        if (Screen.PrimaryScreen.Bounds.Width == 1920)
                        {
                            l.ImageScalingSize = new Size(160, 160);
                            l.Padding = new Padding(20, 0, 0, 0);
                        }
                        else if (Screen.PrimaryScreen.Bounds.Width == 1366)
                        {
                            l.ImageScalingSize = new Size(135, 150);
                            l.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
                        }
                        else
                            l.ImageScalingSize = new Size(128, 128);
                        Image_btn.Size = l.ImageScalingSize;
                    }
                    if (l.Name == cameraPower_toolstrip.Name || l.Name == server_toolstrip.Name || l.Name == database_toolstrip.Name)
                        for (int i = 0; i < l.Items.Count; i++)
                        {
                            l.Items[i].ForeColor = IVLVariables.GradientColorValues.FontForeColor;
                        }
                }
                
            }
            #endregion
        }

        /// <summary>
        /// to draw the rounded rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="CornerRadius"></param>
        private void ResizeToRoundedRectangle(object sender, int CornerRadius)
        {
            try
            {
                if (sender is Control)
                {
                    Control c = sender as Control;

                    Rectangle Bounds = new Rectangle(0, 0, c.Bounds.Width, c.Bounds.Height);
                    //int CornerRadius = c.Bounds.Height / 2;
                    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                    path = RoundedRect(Bounds, CornerRadius);
                    if (c is PictureBox)
                    {
                        //the below code is to draw the square or rectangle image to rounded image.
                        PictureBox p = c as PictureBox;
                        Bitmap b = new Bitmap(p.Image.Width, p.Image.Height);
                        Image i = p.Image;
                        Brush brush = new TextureBrush(i);
                        Bounds = new Rectangle(0, 0, b.Width, b.Height);
                        Graphics g = Graphics.FromImage(b);

                        CornerRadius = b.Height / 2; ;
                        path = RoundedRect(Bounds, CornerRadius);

                        g.FillPath(brush, path);
                        p.Image = b;
                    }
                    else
                        c.Region = new Region(path);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogWriter.WriteLog(ex, exceptionLog);
            }
        }
        public GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        void newMsgBox_shiftControlEvent(Dictionary<string, object> dic)
        {
            Args arg = new Args();
            arg["keyArg"] = dic["keyArg"];
            arg["isKeyUp"] = dic["isKeyUp"];
            _eventHandler.Notify(_eventHandler.IsShiftAndControl, arg);
        }
        //This below function is added by darshan to update the global property.
        public static void globalProperty()
        {
            List<global_property> globalPropertyList = new List<global_property>();
            globalPropertyList = NewDataVariables._Repo.GetAll<global_property>().ToList<global_property>();
            foreach (global_property item in globalPropertyList)
            {
                if (item.key.Equals( IVLVariables.CurrentSettings.UserSettings._HeaderText.text))
                {
                    if (!item.value.Equals( IVLVariables.CurrentSettings.UserSettings._HeaderText.val))
                    {
                        if (!string.IsNullOrEmpty( IVLVariables.CurrentSettings.UserSettings._HeaderText.val))
                        {
                            item.value =  IVLVariables.CurrentSettings.UserSettings._HeaderText.val;
                            NewDataVariables._Repo.Update<global_property>(item);
                        }
                        else
                        {
                            item.value = string.Empty;
                            NewDataVariables._Repo.Update<global_property>(item);
                        }
                    }
                }
                else
                    if (item.key.Equals( IVLVariables.CurrentSettings.UserSettings._DoctorName.text))
                    {
                        if (!item.value.Equals( IVLVariables.CurrentSettings.UserSettings._DoctorName.val))
                        {
                            if (!string.IsNullOrEmpty( IVLVariables.CurrentSettings.UserSettings._DoctorName.val))
                            {
                                item.value =  IVLVariables.CurrentSettings.UserSettings._DoctorName.val;
                                NewDataVariables._Repo.Update<global_property>(item);
                            }
                            else
                            {
                                item.value = string.Empty;
                                NewDataVariables._Repo.Update<global_property>(item);
                            }
                        }
                    }
                    else if (item.key.Equals( IVLVariables.LangResourceManager.GetString("GlobalPropertyImage_Path_Key",  IVLVariables.LangResourceCultureInfo)))
                    {
                        item.value =  IVLVariables.LangResourceManager.GetString("GlobalPropertyImage_Path_Value",  IVLVariables.LangResourceCultureInfo);
                        if (!item.value.Equals( IVLVariables.LangResourceManager.GetString("GlobalPropertyImage_Path_Value",  IVLVariables.LangResourceCultureInfo)))
                        {
                            NewDataVariables._Repo.Update<global_property>(item);
                        }
                    }
                    else if (item.key.Equals( IVLVariables.LangResourceManager.GetString("ReportMainHeading_Text",  IVLVariables.LangResourceCultureInfo)))
                    {
                        if (!item.value.Equals( IVLVariables.CurrentSettings.UserSettings._HeaderText.val))
                        {
                            if (!string.IsNullOrEmpty( IVLVariables.CurrentSettings.UserSettings._HeaderText.val))
                            {
                                item.value =  IVLVariables.CurrentSettings.UserSettings._HeaderText.val;
                                NewDataVariables._Repo.Update<global_property>(item);
                            }
                            else
                            {
                                item.value = string.Empty;
                                NewDataVariables._Repo.Update<global_property>(item);
                            }
                        }
                    }
            }
        }

        /// <summary>
        /// To start and stop server and database timer
        /// </summary>
        /// <param name="arg"></param>
        void StartStopServerDatabaseTimer(string s,Args arg)
        {
            {
                    if (arg.ContainsKey("StartTimer"))
                        if ((bool)arg["StartTimer"])
                        {
                            serverTimer.Enabled = true;
                            //databaseTimer.Enabled = true;
                            serverTimer.Start();
                            //databaseTimer.Start();
                        }
                        else
                        {
                            serverTimer.Enabled = false;
                            //databaseTimer.Enabled = false;
                            serverTimer.Stop();
                            //databaseTimer.Stop();
                        }
                }
            }

        /// <summary>
        /// To enable and disable the emr button
        /// </summary>
        /// <param name="enableDisableEmrButton"></param>
        /// <param name="arg"></param>
        void EnableDisableEmrButton(string enableDisableEmrButton, Args arg)//thiis code is added by kishore on 17 august 2017.
        {
            try
            {
                if (arg.ContainsKey("EnableEmrButton"))
                     if ((bool)arg["EnableEmrButton"])
                     {
                         emr_btn.BackColor = Color.Transparent;
                         emr_btn.ForeColor = this.FontColor;
                         emr_btn.Enabled = true;
                     }
                     else
                     {
                         emr_btn.BackColor = Color.Transparent;
                         emr_btn.ForeColor = this.FontColor;
                         emr_btn.Enabled = false;
                    }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        private void CheckDatabaseConnectivity()
        {
            if (dataBaseServerConnection.GetMysqlServiceStatus())
            {
                if (dataBaseServerConnection.GetDataBaseConnectionStatus())
                {
                 // int patCount =  NewDataVariables._Repo.GetPatientCount();
                    //if (NewDataVariables.Patients == null)
                    //    NewDataVariables.Patients = NewDataVariables._Repo.GetAll<Patient>().ToList();
                    if (MysqlServiceConnection_lbl.ToolTipText != IVLVariables.LangResourceManager.GetString("MysqlServiceAvailableToolTip_Text", IVLVariables.LangResourceCultureInfo))
                    {
                        MysqlServiceConnection_lbl.Image = serviceAvaiable;
                        MysqlServiceConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("MysqlServiceAvailableToolTip_Text", IVLVariables.LangResourceCultureInfo);
                    }
                    if (databaseConnection_lbl.ToolTipText != IVLVariables.LangResourceManager.GetString("DatabaseConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo))
                    {
                        databaseConnection_lbl.Image = databaseConnected;
                        databaseConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("DatabaseConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo);
                    }
                }
                else
                {
                    if (databaseConnection_lbl.ToolTipText != IVLVariables.LangResourceManager.GetString("DatabaseNotConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo))
                    {
                        databaseConnection_lbl.Image = databaseNotConnected;
                        databaseConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("DatabaseNotConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo);
                    }
                    CustomMessageBox.Show(IVLVariables.LangResourceManager.GetString("DatabaseNotConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo), IVLVariables.LangResourceManager.GetString("DatabaseNotConnectedToolTip_Text", IVLVariables.LangResourceCultureInfo), CustomMessageBoxButtons.OK);

                    Application.Exit();
                }
            }
            else
            {
                if (MysqlServiceConnection_lbl.ToolTipText != IVLVariables.LangResourceManager.GetString("MysqlServiceNotAvailableToolTip_Text", IVLVariables.LangResourceCultureInfo))
                {
                    MysqlServiceConnection_lbl.Image = serviceNotAvaiable;
                    MysqlServiceConnection_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("MysqlServiceNotAvailableToolTip_Text", IVLVariables.LangResourceCultureInfo);
                }
                CustomMessageBox.Show(IVLVariables.LangResourceManager.GetString("OneOfTheServicesNotRunning_Text", IVLVariables.LangResourceCultureInfo), IVLVariables.LangResourceManager.GetString("SaveAs_Warning_Header", IVLVariables.LangResourceCultureInfo), CustomMessageBoxButtons.OK);
                Application.Exit();
            }
        }

        void databaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckDatabaseConnectivity();
        }

        void thumbnailUI1_anotherWindowOpen(bool isOpen)
        {
            IVLVariables.IsAnotherWindowOpen = isOpen;
            //if (isOpen)
            //    IVLVariables.ivl_Camera.TriggerOff();
            //else
            //    IVLVariables.ivl_Camera.TriggerOn();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        void serverTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // this.Focus();
            TimerUIUpdation();
        }


        //protected override void WndProc(ref Message m)
        //{
        //    IVLVariables.ivl_Camera.callWndProc(ref m);
        //    base.WndProc(ref m);
        //}

        void thumbnailUI1_sendFocusBackToParent()
        {
            this.Focus();
        }

        /// <summary>
        /// This will unselect the image when control key is pressed.And selects the the multiple images on shift or control selection.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        public void isshiftandcontrol(string s, Args arg)
        {
            KeyEventArgs e = arg["keyArg"] as KeyEventArgs;
            if (!IVLVariables.isReportWindowOpen)
            {
                if ((bool)arg["isKeyUp"])
                    IvlMainWindow_KeyUp(null, e);
                else
                    IvlMainWindow_KeyDown(null, e);
            }
            else
            {
                _eventHandler.Notify(_eventHandler.ReportImagesIsShiftControl, arg);
            }
        }

        public void ScrollControl()
        {
            this.thumbnailUI1.ScrollControlIntoView(this.thumbnailUI1);
        }

        private void enableCapturePowerStatusTimer(String s, Args arg)
        {
            this.Focus();
        }

        /// <summary>
        /// Updates the cursor in live imaging screen to waitcursor while capturing image and reset the cursor to default after image has been captured.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void CursorUpdate(String s, Args arg)
        {
            if (Convert.ToBoolean(arg["isDefault"]))
                this.Cursor = Cursors.Default;
            else
                this.Cursor = Cursors.WaitCursor;
            if (arg.ContainsKey("isImaging"))
            {
                if ((bool)arg["isImaging"])
                {
                    Image_btn.Enabled = false;
                    Image_btn.ToolTipText = "";
                }
                else
                {
                    serverTimer.Enabled = true;
                    serverTimer.Start();
                    Image_btn.Enabled = false;
                }
            }
        }

        /// <summary>
        /// To stop the timers
        /// </summary>
        private void DisposeTimers()
        {
            IVLVariables.ivl_Camera.DisposeCameraBoardTimers();
        }



        // Defect number 0000581 has been fixed by calling the same function as in the case of space bar by sriram on August 18th 2015
        private void CaptureUIUpdate(string s, Args arg)
        {
            //This below if statement has been added by Darshan on 17-08-2015 to solve Defect no 0000562: Image capture happening for previous dates.
            if (IVLVariables.ShowImagingBtn)
            {
                //This below method has been added by Darshan to solve defect no 0000319
                brightnesspopup();
                if (!isEmr && IVLVariables.ivl_Camera.CameraIsLive)
                {
                    arg["isDefault"] = false;
                    _eventHandler.Notify(_eventHandler.UpdateMainWindowCursor, arg);
                }
                else if (!isEmr && !IVLVariables.ivl_Camera.CameraIsLive)
                {
                    arg["isDefault"] = true;
                    Imaging_btn_Click(null, null);
                    Image_btn.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Function to call connect camera to open the camera and the board
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void Connect2Camera(string s, Args arg)
        {
            //IVLVariables.ivl_Camera.picBox = imaging_UC.display_pbx;
            //IVLVariables.ivl_Camera.SetPicbox(ref imaging_UC.display_pbx);
            IVLVariables.ivl_Camera.OpenCameraBoard();
            //IVLVariables.ivl_Camera.ConnectCamera();
        }
        delegate void DelegateLive2ViewViaTrigger(string s, Args arg);
        private DelegateLive2ViewViaTrigger m_DelegateLive2ViewViaTrigger;


        /// <summary>
        /// to update live or capture screen UI
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        void updateCaptureRLiveUI( String s , Args arg)
        {
               
                     if((bool)arg["CaptureSequence"])
                       //if (!IVLVariables.ivl_Camera.IsCapturing && !this.imaging_UC.isImagingDisabled)// start capture event
                        {
                            serverTimer.Enabled = false;
                            serverTimer.Stop();
                            _eventHandler.Notify(_eventHandler.CaptureEvent, new Args());
                        }
                       else
                       {
                            if (this.InvokeRequired)
                            {
                               this.Invoke(m_DelegateLive2ViewViaTrigger, "", new Args());
                            }
                            else
                                NavigateFromViewToLive("", new Args());
                        }
                       
        }
        /// <summary>
        /// This event is fired when any key is pressed.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {

                if (keyData == Keys.Space)
                {
                    if (!isEmr)
                    {
                        //Thread t1 = new Thread(() =>
                        if (!IVLVariables.isCommandLineAppLaunch)//this has been added to ensure the launch is not from command  line.
                        {
                            brightnesspopup();//this has been added to check for any changes in the images like brightness or contrast is changed.
                            if (!IVLVariables.isValueChanged)//if the modified image is not saved it wont let to live screen.
                            {
                                if (NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date)
                                {
                                    this.Cursor = Cursors.WaitCursor;
                                    IVLVariables.ivl_Camera.CaptureStartTime = DateTime.Now;
                                    IVLVariables.ivl_Camera.HowImageWasCaptured = IntucamHelper.CapturedUIMode.SpaceBar;
                                    //ThreadPool.QueueUserWorkItem(new WaitCallback(f=>
                                    //{
                                    IVLVariables.ivl_Camera.MRNValue = NewDataVariables.Active_PatientIdentifier.value;
                                    IVLVariables.ivl_Camera.VisitDate = NewDataVariables.Active_Visit.createdDate.Date.ToString().Replace('/', '_').Remove(10);
                                    if (!IVLVariables.ivl_Camera.Trigger_Or_SpacebarPressed(false))
                                    {
                                        thumbnailUI1.isCaptureSequenceInProgress = IVLVariables.isCapturing;
                                        this.Cursor = Cursors.Default;
                                    }
                                    //}));
                                }
                            }
                            //);
                            //t1.Start();
                        }
                        else
                        {
                            this.Cursor = Cursors.WaitCursor;
                            IVLVariables.ivl_Camera.CaptureStartTime = DateTime.Now;
                            IVLVariables.ivl_Camera.HowImageWasCaptured = IntucamHelper.CapturedUIMode.SpaceBar;
                            if (!IVLVariables.ivl_Camera.Trigger_Or_SpacebarPressed(false))
                            {
                                thumbnailUI1.isCaptureSequenceInProgress = IVLVariables.isCapturing;
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }




                    //IVLVariables.ivl_Camera.Trigger_Or_SpacebarPressed(IVLVariables.istrigger);
                }
                //this.Focus();
            }
            else if (keyData == (Keys.Alt | Keys.I))
            {
                if (isEmr)// display software and firmware version details only in EMR screen
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                    string version = fvi.FileVersion;
                    string firmwareVer = IVLVariables.ivl_Camera.camPropsHelper.GetFirmwareVersion();
                    //Common.CustomMessageBox.Show(IVLVariables.LangResourceManager.GetString( "Software_Name + " " + IVLVariables.LangResourceManager.GetString( "Version_Text + " : " + version + Environment.NewLine + Environment.NewLine + IVLVariables.LangResourceManager.GetString( "FirmwareVersion_Text + " : " + IntucamBoardCommHelper.returnVal, IVLVariables.LangResourceManager.GetString( "Information_Text, Common.CustomMessageBoxButtons.OK, Common.CustomMessageBoxIcon.Information);
                    string message = IVLVariables.LangResourceManager.GetString("Software_Name", IVLVariables.LangResourceCultureInfo) + " " + IVLVariables.LangResourceManager.GetString("Version_Text", IVLVariables.LangResourceCultureInfo) + " : " + version; // + Environment.NewLine + firmwareVer + Environment.NewLine + "Software Release Date : " + Constants.SoftwareReleaseDate;
                    Common.CustomMessageBox.Show(message, IVLVariables.LangResourceManager.GetString("Information_Text", IVLVariables.LangResourceCultureInfo), Common.CustomMessageBoxButtons.OK, Common.CustomMessageBoxIcon.Information, 442, 150);
                }
            }
            else if (keyData == (Keys.Alt | Keys.T))//To open the report template creator 
            {
                if (isEmr)
                {
                    {
                        Process p = new Process();
                        p.StartInfo = new ProcessStartInfo("ReportTemplateCreator.exe");
                        p.Start();
                    }
                    
                }
            }
            else if (keyData == Keys.PageUp)
            {
                //this.TopMost = !this.TopMost;//has been commented to prevent the custom message box getting background.
            }
            else if (keyData == Keys.Delete)
            {
                if (!IVLVariables.ivl_Camera.CameraIsLive)//0000588 defect is fixed by checking for live imaging and thumbnail visibility by sriram on August 20th 2015
                    if (thumbnailUI1.Visible)
                    {
                        thumbnailUI1.thumbnailDelete();
                    }
            }
            else if ((keyData == Keys.Up))
            {
                if (!IVLVariables.ivl_Camera.CameraIsLive && !IVLVariables.ivl_Camera.IsCapturing/*Added to handle to avoid selection change of thumbnail when  capturing sequence is in progress to fix defect 0001667 */) //0000588 defect is fixed by checking for live imaging and thumbnail visibility by sriram on August 20th 2015
                    if (this.thumbnailUI1.Visible)
                    {
                        thumbnailUI1.ThumbnailUpArrow();
                    }
            }
            else if (keyData == Keys.Down)
            {
                if (!IVLVariables.ivl_Camera.CameraIsLive && !IVLVariables.ivl_Camera.IsCapturing/*Added to handle to avoid selection change of thumbnail when  capturing sequence is in progress to fix defect 0001667 */) //0000588 defect is fixed by checking for live imaging and thumbnail visibility by sriram on August 20th 2015
                    if (thumbnailUI1.Visible)
                    {
                        thumbnailUI1.ThumbnailDownArrow();
                        Point p = this.thumbnailUI1.Location;
                    }
            }
            else if (keyData == (Keys.Alt | Keys.C))
            {
                if (isEmr)
                {
                    if (tForm == null)
                    {
                        tForm = new ThemesForm();
                        tForm.gradientColorChangeEvent += gForm_gradientColorChangeEvent;
                    }
                    if (!tForm.Visible)
                        tForm.ShowDialog();
                    else
                        tForm.Focus();

                    //if (gForm == null)
                    //    gForm = new GradientForm();
                    //gForm.gradientColorChangeEvent += gForm_gradientColorChangeEvent;
                    //gForm.ShowDialog();
                }
            }
            //else if (keyData == (Keys.Control | Keys.N))
            //{
            //    if (isEmr)
            //        emr.CreatePatient();
            //}
            //else if (keyData == (Keys.Control | Keys.Alt | Keys.N))
            //{
            //    if (isEmr)
            //        emr.CreateConsultation();
            //}
            //else if (keyData == (Keys.Control | Keys.D))
            //{
            //    if (isEmr)
            //        emr.DeletePatient();
            //}
            //else if (keyData == (Keys.Control | Keys.Alt | Keys.D))
            //{
            //    if (isEmr)
            //        emr.DeleteConsultation();
            //}
            //else if (keyData == (Keys.Control | Keys.U))
            //{
            //    if (isEmr)
            //        emr.UpdatePatient();
            //}
            else if (keyData == (Keys.Control | Keys.I))
            {

            }
            else if (keyData == (Keys.Control | Keys.Add))
            {

            }
            else if (keyData == (Keys.Control | Keys.S))
            {

            }
            else if (keyData == (Keys.Control | Keys.Z))
            {

            }
            else if (keyData == (Keys.Control | Keys.B | Keys.Add))
            {

            }
            else if (keyData == (Keys.Control | Keys.B | Keys.Subtract))
            {

            }
            else if (keyData == (Keys.Control | Keys.Add))
            {

            }
            else if (keyData == (Keys.Control | Keys.Subtract))
            {

            }
            else if (keyData == (Keys.Alt | Keys.Add))
            {

            }
            else if (keyData == (Keys.Alt | Keys.Subtract))
            {

            }
            else if (keyData == (Keys.Add))
            {

            }
            else if (keyData == (Keys.Subtract))
            {

            }
            else if (keyData == (Keys.Up | Keys.Shift))
            {
                if (!IVLVariables.ivl_Camera.CameraIsLive)//0000588 defect is fixed by checking for live imaging and thumbnail visibility by sriram on August 20th 2015
                    if (thumbnailUI1.Visible)
                    {
                        thumbnailUI1.ThumbnailUpArrow();
                    }
            }
            else if (keyData == (Keys.Down | Keys.Shift))
            {
                if (!IVLVariables.ivl_Camera.CameraIsLive)//0000588 defect is fixed by checking for live imaging and thumbnail visibility by sriram on August 20th 2015
                    if (thumbnailUI1.Visible)
                    {
                        thumbnailUI1.ThumbnailDownArrow();
                    }
            }
            else if (keyData == (Keys.Alt | Keys.S))// Alt + s for invoking config settings UI added by sriram on 7th august 2015
            {
                if (PagePanel_p.Contains(emr))
                {
                    settings_window();
                }
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.E))//Ctrl + Alt + e for invoking EEPROM settings UI which is editable
            {
                if (PagePanel_p.Contains(emr))
                {
                    EEPROM.Settings_UCL.IsReadOnly = false;
                    EEPROM_Window eeprom = new EEPROM_Window();
                    eeprom.ShowDialog();
                }
            }
            else if (keyData == (Keys.Alt | Keys.E))// Alt + e for invoking EEPROM settings UI which is read only
            {
                if (PagePanel_p.Contains(emr))
                {
                    EEPROM.Settings_UCL.IsReadOnly = true;

                    EEPROM_Window eeprom = new EEPROM_Window();
                    eeprom.ShowDialog();

                }
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        void gForm_gradientColorChangeEvent(Dictionary<string, object> ColorValues)
        {
            if (ColorValues.ContainsKey("Ok"))
            {
                //Color emrColor = emr_btn.ForeColor;
                IVLVariables.GradientColorValues = ColorValues["CurrentTheme"] as GradientColor;// t.GetCurrentTheme());
                this.Color1 = IVLVariables.GradientColorValues.Color1;
                this.Color2 = IVLVariables.GradientColorValues.Color2;
                this.FontColor = IVLVariables.GradientColorValues.FontForeColor;
                //emr_btn.ForeColor = emrColor;
                this.ColorAngle = IVLVariables.GradientColorValues.ColorAngle;
                this.ThemeName = IVLVariables.GradientColorValues.GradientColorName;

                UpdateFontForeColor();
                emr.UpdateGridView();
                imaging_UC.UpdateFontForeColor();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// thumbnailUI1_imageadded is a event which will save image details.
        /// </summary>
        /// <param name="s">s gives image name and id</param>
        void thumbnailUI1_imageadded(Dictionary<string, object> s)
        {
            if (!isModifiedimage)
            {
                Args arg = new Args();
                arg["ImageName"] = s["ImageName"] as string;
                arg["id"] = (int)s["id"];
                _eventHandler.Notify(_eventHandler.ImageAdded, arg);
                this.Focus();
            }
        }

        bool isModifiedimage = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void getImageFiles(string s, Args arg)
        {
            Dictionary<string, List<string>> exportimges = this.thumbnailUI1.getImageFiles();
            string[] fileNames = exportimges["FileNames"].ToList().ToArray();
            List<FileInfo> finfArr = new List<FileInfo>();
            for (int i = 0; i < fileNames.Count(); i++)
            {
                finfArr.Add(new FileInfo(fileNames[i]));
            }
            arg["fileNames"] = finfArr;
            arg["FileNameArr"] = fileNames;
            arg["ImageLabelText"] = exportimges["ImageNames"].ToArray();
            if ((bool)arg["isExport"])
                _eventHandler.Notify(_eventHandler.ExportImageFiles, arg);
            else
                _eventHandler.Notify(_eventHandler.GetImageFilesFromThumbnails, arg);
        }

        int Thumbnail_id = 0, side = 0;
        bool isImaging = false;

        /// <summary>
        /// Select the thumbnail when clicked on it in viewimaging screen.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void ThumbnailSelected(String s, Args arg)
        {
            //if (arg.ContainsKey("ThumbnailFileName"))
            //{
            //    this.thumbnailUI1.ThumbnailSelected(arg["ThumbnailFileName"] as String);
            //    thumbnailFileName = arg["ThumbnailFileName"] as String;
            //}
            if (arg.ContainsKey("ThumbnailData"))
            {
                ThumbnailData tData = arg["ThumbnailData"] as ThumbnailData;
                this.thumbnailUI1.ThumbnailSelected(tData);
                thumbnailFileName = tData.fileName;
            }
        }

        //This below function has been added by Darshan BS,this function will create backup files for all json files.
        public static IEnumerable<Control> GetControls(Control form)
        {
            foreach (Control childControl in form.Controls)
            {   // Recurse child controls.
                foreach (Control grandChild in GetControls(childControl))
                {
                    yield return grandChild;
                }
                yield return childControl;
            }
        }

        /// <summary>
        /// To update the live and view screen controls when power or camera is connected or disconnected.
        /// </summary>
        private void UpdateLiveViewScreen(Args arg)
        {
            if (isPowerConnected && isCameraConnected) // if both power and camera are connected 
            {
                if (!isEmr)//if it is not record screen.
                {
                    //if (imaging_UC.isImaging)//if it is in live screen mode.
                    //{

                    //    imaging_UC.setLiveScreen();//set live mode.
                    //    //System.Threading.Thread.Sleep(5000);//to resume the live mode

                    //}
                    if (NewDataVariables.Active_Visit != null)
                    {
                        if (NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date)//if it is in view mode and the visit'sdate is todays date
                        {
                            
                            //IVLVariables.ivl_Camera.TriggerOn();//turn on the trigger in order to navigate from view to live via trigger press.
                            Image_btn.Enabled = true;
                        }
                        else
                        {
                            Image_btn.Enabled = false;
                        }
                    }
                }
            }

            else
            {
                if (!IVLVariables.isCommandLineAppLaunch && !isEmr)//if it is not from command line launch.
                {
                    if (arg.ContainsKey("ShowPopMsg"))
                    {
                        if ((bool)arg["ShowPopMsg"])
                        {
                            CustomMessageBox.Show(IVLVariables.LangResourceManager.GetString("PowerCameraDisconnected_Text", IVLVariables.LangResourceCultureInfo), IVLVariables.LangResourceManager.GetString("FolderPath_Warning_Header", IVLVariables.LangResourceCultureInfo), CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Error);
                       
                        }
                        //_eventHandler.Notify(_eventHandler.ConnectCamera, new Args());
                       
                    
                    }
                    
                        GoToViewScreen("", new Args());
                     
                }
            }
        }

        private void GoToViewScreen(string s, Args arg)
        {
            if(splashScreen.Visible && !splashScreen.CanFocus)
            splashScreen.Close();
            if (_eventHandler.isHandlerPresent(_eventHandler.EnableDisableEmrButton))
            {
                arg["EnableEmrButton"] = true;
                _eventHandler.Notify(_eventHandler.EnableDisableEmrButton, arg);
            }

            if (NewDataVariables.Active_Obs == null || NewDataVariables.Obs.Count == 0)//if the active observation is null.
            {
                imaging_UC.DisableLiveScreen(isPowerConnected && isCameraConnected && NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date, arg);//To disable or enable the live imaging screen buttons
                this.thumbnailUI1.NoImages_Selected();//to select the no images selected label.
            
            }
            else
            {
                #region Thumbnail data populate to display if the camera or power is disconnected in live
                ThumbnailModule.ThumbnailData tData = new ThumbnailModule.ThumbnailData();
                tData.fileName = IVLVariables.CurrentSettings.ImageStorageSettings._LocalProcessedImagePath.val.ToString() + Path.DirectorySeparatorChar + NewDataVariables.Active_Obs.value;
                tData.id = NewDataVariables.Active_Obs.observationId;
                if (NewDataVariables.Active_Obs.eyeSide == 'L')
                    tData.side = 1;
                else
                    tData.side = 0;
                tData.isAnnotated = NewDataVariables.Active_Obs.annotationsAvailable;
                tData.isCDR = NewDataVariables.Active_Obs.cdrAnnotationAvailable;

                //arg["ThumbnailFileName"] = IVLVariables.CurrentSettings.ImageStorageSettings._LocalProcessedImagePath.val.ToString() + Path.DirectorySeparatorChar + NewDataVariables.Active_Obs.value;
                arg["ThumbnailData"] = tData;
                #endregion
                imaging_UC.DisableLiveScreen(isPowerConnected && isCameraConnected && NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date, arg);//To disable or enable the live imaging screen buttons
            }
        }
        /// <summary>
        /// This event will set status of the camera status.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void ShowCameraConnection(string s, Args arg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mShowCameraDelegate, s, arg);
            }

            if ((bool)arg["isCameraConnected"])
            {
                {
                    if (isCameraConnected)
                        return;
                    isCameraConnected = true;
                    //CameraStatus_pbx.Image = cameraConnected;
                    if (IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.Anterior_Prime || IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.Posterior_Prime)
                    {
                        if (!IVLVariables.ivl_Camera.CameraName.Contains("E3CMOS06300KPA(USB2.0)"))//if camera is connected to usb port 3.0
                        {
                            cameraImage_lbl.Image = cameraConnected;
                            cameraImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("CameraOn_ToolTipText", IVLVariables.LangResourceCultureInfo);

                        }
                        else
                        {
                            cameraImage_lbl.Image = CameraError;
                            cameraImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("CameraWarningUsb2_0ToolTipText", IVLVariables.LangResourceCultureInfo);

                        }
                    }
                    else
                    {
                        cameraImage_lbl.Image = cameraConnected;
                        cameraImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("CameraOn_ToolTipText", IVLVariables.LangResourceCultureInfo);
                    }
                }
            }
            else if (!(bool)arg["isCameraConnected"])
            {
                if (IVLVariables.isCommandLineAppLaunch)

                    if (IVLVariables.ivl_Camera.camPropsHelper.IsCameraConnected != Devices.CameraConnected || IVLVariables.ivl_Camera.camPropsHelper.IsPowerConnected != Devices.PowerConnected)
                    {
                        string powerOffText = IVLVariables.LangResourceManager.GetString("PowerCameraDisconnected_Text", IVLVariables.LangResourceCultureInfo);
                        CustomMessageBox.Show(String.Format("{0}", powerOffText));
                        Application.Exit();
                    }
                if (!isCameraConnected)
                    return;
                isCameraConnected = false;
                
                cameraImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("CameraOff_ToolTipText", IVLVariables.LangResourceCultureInfo);
                // CameraStatus_pbx.Image = cameraDisconnected;
                cameraImage_lbl.Image = cameraDisconnected;
               
                
            }

            if (isEmr)
            {
                emr.UpdateVisitButton();
            }
            else
                UpdateLiveViewScreen(arg);
            if (arg.ContainsKey("ShowPopMsg"))
                arg.Remove("ShowPopMsg");
        }

        /// <summary>
        /// This will set the power connection status.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void ShowPowerConnection(string s, Args arg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mShowPowerDelegate, s, arg);
            }

            if ((bool)arg["isPowerConnected"])
            {
                if (isPowerConnected)
                    return;
                //powerConnectionStatus_pbx.Image = PowerConnected;
                powerImage_lbl.Image = PowerConnected;
                isPowerConnected = true;
                powerImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("PowerOn_ToolTipText", IVLVariables.LangResourceCultureInfo);


            }
            else if (!(bool)arg["isPowerConnected"])
            {
                if (IVLVariables.isCommandLineAppLaunch)

                    if (IVLVariables.ivl_Camera.camPropsHelper.IsCameraConnected != Devices.CameraConnected || IVLVariables.ivl_Camera.camPropsHelper.IsPowerConnected != Devices.PowerConnected)
                    {
                        string powerOffText = IVLVariables.LangResourceManager.GetString("PowerCameraDisconnected_Text", IVLVariables.LangResourceCultureInfo);
                        CustomMessageBox.Show(String.Format("{0}", powerOffText));
                       
                        Application.Exit();
                    }
                if (!isPowerConnected)
                    return;
                //powerConnectionStatus_pbx.Image = PowerDisconnected;
                powerImage_lbl.Image = PowerDisconnected;
                powerImage_lbl.ToolTipText = IVLVariables.LangResourceManager.GetString("PowerOff_ToolTipText", IVLVariables.LangResourceCultureInfo);
                isPowerConnected = false;
                
              
            }
            if (isEmr)
            {
                emr.UpdateVisitButton();
            }
            else
            {
                UpdateLiveViewScreen(arg);
               if( arg.ContainsKey("ShowPopMsg"))
                    arg.Remove("ShowPopMsg");
            }
        }

        /// <summary>
        /// Enables or disables the add images button on the conslutation grid based on camera connection.
        /// </summary>
        private void TimerUIUpdation()
        {
            if (!IVLVariables.ApplicationClosing && !IVLVariables.ivl_Camera.IsCapturing)
            {
                if (Convert.ToBoolean(IVLVariables.CurrentSettings.UserSettings._Is24clock.val))
                {
                    Time_lbl.Text = DateTime.Now.ToString(" HH:mm ");
                    Date_lbl.Text = DateTime.Today.ToShortDateString();//The date format has been changed to maintain a uniform date format.
                }
                else
                {
                    Time_lbl.Text = DateTime.Now.ToString("  hh:mm tt ");
                    Date_lbl.Text = DateTime.Today.ToShortDateString();//The date format has been changed to maintain a uniform date format.
                }
               
            }
            //if (!CheckIfServiceIsRunning(mysqlServiceName))
            //{
            //    System.Diagnostics.Process proc = new System.Diagnostics.Process();

            //    proc.StartInfo.FileName = "mysqlServiceStart.bat";
            //    proc.StartInfo.Verb = "runas";
            //   proc.StartInfo.CreateNoWindow = false;
            //   proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //   proc.Start();
            //   proc.WaitForExit(10000);
            //} this.Focus();
        }
        AdobeAndOSInfo adobeOsInfo  ;
        /// <summary>
        /// IvlMainWindow_Load is a private event which will load panels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IvlMainWindow_Load(object sender, EventArgs e)
        {
            //int dayOfTheWeek = Convert.ToInt32(IVLVariables.CurrentSettings.FirmwareSettings._MotorCaptureSteps.val);
            //int month = Convert.ToInt32(IVLVariables.CurrentSettings.FirmwareSettings._MotorPlySteps.val);
            //List<Patient> pats = NewDataVariables._Repo.GetPageData<Patient>(500, 1).ToList().Where(x=>x.createdDate.Day == dayOfTheWeek && x.createdDate.Month == month ).ToList();
            //StreamWriter stW = new StreamWriter("Data_"+dayOfTheWeek.ToString()+".csv");
            //foreach (Patient item in pats)
            //{
            //    string str = item.identifiers.ToList()[0].value.ToString()+","+ item.firstName +","+ item.lastName +","+item.gender +","+(DateTime.Now.Year -  item.birthdate.Year).ToString() +",";
            //    foreach (visit visitItem in item.visits)
            //    {
            //        if (visitItem.observations.Where(x=>x.voided == false).ToList().Count > 0)
            //        {
            //            if (visitItem.reports.Count > 0)
            //            {
            //                report r = visitItem.reports.ToList()[visitItem.reports.Count - 1];
            //                IVLReport.JsonReportModel dataJson = (IVLReport.JsonReportModel)JsonConvert.DeserializeObject(r.dataJson, typeof(IVLReport.JsonReportModel));
            //                str += dataJson.reportValues.Where(x=>x.Key == "Comments").ToList()[0].Value.ToString();
            //            }
            //        }
            //    }
            //    stW.WriteLine(str);
            // }
            //stW.Flush();
            //stW.Close();
            //stW.Dispose();
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("AdobeCheckOSVersionInfo.exe");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            string adobeInfoJson = File.ReadAllText("AdobeOsInfo.json");
            adobeOsInfo = new AdobeAndOSInfo();
            if(!string.IsNullOrEmpty( adobeInfoJson))
                try
                {
                    adobeOsInfo = (AdobeAndOSInfo)JsonConvert.DeserializeObject(adobeInfoJson, typeof(AdobeAndOSInfo));

                }
                catch (Exception)
                {
                    
                }

            Constants.prerquisitesCount = prerequisiteList.Count;
            if (GetPrerequisitesInstalledCount() == Constants.prerquisitesCount)
            {
                    if (IVLVariables.isCommandLineArgsPresent)//to open directly the report window if  the batchfile is executed.
                    {
                        if (IVLVariables.isCommandLineAppLaunch)
                        {
                            IVLVariables.ivl_Camera.AppLaunched = true;

                            _eventHandler.Notify(_eventHandler.ConnectCamera, new Args());
                            if (IVLVariables.isCommandLineAppLaunch)

                                if (IVLVariables.ivl_Camera.camPropsHelper.IsCameraConnected != Devices.CameraConnected || IVLVariables.ivl_Camera.camPropsHelper.IsPowerConnected != Devices.PowerConnected)
                                {
                                    string powerOffText = IVLVariables.LangResourceManager.GetString("PowerCameraDisconnected_Text", IVLVariables.LangResourceCultureInfo);
                                    CustomMessageBox.Show(String.Format("{0}", powerOffText));

                                    Application.Exit();
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(IVLVariables.CmdImageSavePath))
                                    {
                                        IVLVariables.ivl_Camera.ProcessedImagePathFromConfig = true;
                                    }
                                    else
                                    {
                                        IVLVariables.ivl_Camera.ProcessedImagePathFromConfig = false;
                                        IVLVariables.ivl_Camera.camPropsHelper._Settings.ImageSaveSettings.ProcessedImageDirPath = IVLVariables.CmdImageSavePath;
                                    }
                                    Args arg = new Args();
                                    arg["imageCount"] = 0;
                                    arg["isImaging"] = true;

                                    _eventHandler.Notify(_eventHandler.Navigate2ViewImageScreen, arg);
                                }
                        }
                        else
                        {
                            this.Visible = false;
                            string batchFilePath = @IVLVariables.batchFilePath + Path.DirectorySeparatorChar + "PatientDetails.json";
                            if (File.Exists(batchFilePath))
                            {
                                IVLVariables.patDetails = (PatientDetailsForCommandLineArgs)JsonConvert.DeserializeObject(File.ReadAllText(batchFilePath), typeof(PatientDetailsForCommandLineArgs));
                                for (int i = 0; i < IVLVariables.patDetails.observationPaths.Count; i++)
                                {
                                    IVLVariables.patDetails.observationPaths[i] = @IVLVariables.batchFilePath + Path.DirectorySeparatorChar + IVLVariables.patDetails.observationPaths[i];
                                }
                            }


                            string observationDetailsFilePath = @IVLVariables.batchFilePath + Path.DirectorySeparatorChar + "observations.txt";
                            if (File.Exists(observationDetailsFilePath))
                            {
                                string str = File.ReadAllText(observationDetailsFilePath);
                                string[] tokens = str.Split(new[] { "-------------------------------------" }, StringSplitOptions.None);
                                List<string> listObservation = tokens.ToList<string>();
                                if (listObservation.Count == 7)
                                    listObservation.RemoveAt(listObservation.Count - 1);
                                IVLVariables.observationDic = new Dictionary<string, string>();
                                for (int i = 0; i < listObservation.Count; i = i + 2)
                                {
                                    IVLVariables.observationDic.Add(listObservation[i], listObservation[i + 1].Trim());
                                }
                            }
                            string emailJsonPath = @IVLVariables.batchFilePath + Path.DirectorySeparatorChar + "EmailData.json";
                            if (File.Exists(emailJsonPath))
                            {
                                IVLVariables.mailData = (EmailsData)JsonConvert.DeserializeObject(File.ReadAllText(emailJsonPath), typeof(EmailsData));
                                string toMail = IVLVariables.mailData.EmailReplyTo;
                                string fromMail = IVLVariables.mailData.EmailTo;
                                IVLVariables.mailData.EmailTo = toMail;
                                IVLVariables.mailData.EmailReplyTo = fromMail;
                            }
                            _eventHandler.Notify(_eventHandler.CreateReportEvent, new Args());

                        }
                    }
                    else
                    {
                        isComponentInitialized = true;
                        CheckDatabaseConnectivity();
                        dataBaseServerConnection.DatabaseBackup(dataBasebackupPath);
                        //databaseTimer.Start();
                        emr = new EmrManage();
                        SetPanels();
                        serverTimer.Start();
                        IVLVariables.ivl_Camera.AppLaunched = true;
                        _eventHandler.Notify(_eventHandler.ConnectCamera, new Args());
                    }
            }
            else
            {
                //the below for loop is to add the not installed prerequisites to a string to show it on the message box.
                for (int i = 0; i < prerequisiteList.Count; i++)
                {
                    uninstalledPrerquisite = uninstalledPrerquisite + prerequisiteList[i] + Environment.NewLine;
                }
                CustomMessageBox.Show(uninstalledPrerquisite + (IVLVariables.LangResourceManager.GetString("PrerequisitesNotInstalled_Text", IVLVariables.LangResourceCultureInfo)), IVLVariables.LangResourceManager.GetString("PrerequisitesWarning_Header_Text", IVLVariables.LangResourceCultureInfo), CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information, 500, 300);
                Application.Exit();
            }
            //Bitmap st = new Bitmap("");

        }

        private int GetPrerequisitesInstalledCount()
        {
            if (IVLVariables.isCommandLineAppLaunch)
            {
                prerequisitesCount = 1;//if it is command line app launcah mysql will not be there to compensate the total prerequisitesCount of prerequisite making the prerequisitesCount as 1.
                prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.mysqlVersion));
            }
            if (adobeOsInfo.OSInfo.Contains("10"))
            {

                prerequisitesCount++;//if it is command line app launcah mysql will not be there to compensate the total prerequisitesCount of prerequisite making the prerequisitesCount as 1.
                prerequisitesCount++;//if it is command line app launcah mysql will not be there to compensate the total prerequisitesCount of prerequisite making the prerequisitesCount as 1.
                prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.vcredistributable2015));
                prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.servicePackVersion));
            }
            else
            {
                CheckServicePackInstalled();

            }
            CheckRedistributableFilesInstalled();
            CheckDriversInstalled();
            CheckDotFrameWork();
            if ( IVLVariables.isCommandLineAppLaunch)
            {
                prerequisitesCount++;//if it is command line app launcah adobe will not be there to compensate the total prerequisitesCount of prerequisite making the prerequisitesCount as 1.
                prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.adobeReaderVersion));
            }
            else
            CheckAdobeReaderInstalled();

            return prerequisitesCount;
        }

        private void CheckServicePackInstalled()
        {
            string windowsVersion = string.Empty;

            #region Code to check whether service pack is installed
            registry_key = servicePackPath;
            key = Registry.LocalMachine;
            key = key.OpenSubKey(registry_key);
            {
                windowsVersion = (key.GetValue(productNameText)).ToString();
                if (!windowsVersion.Contains(Constants.windowsVersion10) && !windowsVersion.Contains(Constants.windowsVersion8))//checking whether the windows version is not 10, if it is 10 no need to check for service pack.
                {
                    if (Environment.OSVersion.ServicePack.Contains(Constants.servicePackVersion))//if the string contains the service pack string it will go inside and increase the prerequisite count.
                    {
                        prerequisitesCount++;
                        prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.servicePackVersion));//this is to indicate that service pack is installed.
                    }
                }
                else
                {
                    prerequisitesCount++;
                    prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.servicePackVersion));//this is to indicate that service pack is installed.
                }
            }
            #endregion
        }

        private void CheckRedistributableFilesInstalled()
        {
            #region Code to check VC2012, 2013, 2015 is installed
            registry_key = remainingVcRedistributablePath;
            key = Registry.LocalMachine;
            key = key.OpenSubKey(registry_key);
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subKeyName))
                {
                    string[] subKeys = subkey.GetValueNames();
                    for (int i = 0; i < subKeys.Length; i++)
                    {

                        if (subKeys[i].Contains(displayNameText))//to check whether the key in the registry contains display name text 
                        {
                            string version = (string)subkey.GetValue(displayNameText);
                            if (version.Contains(Constants.vcredistributable2013))//if the string contains the 2013 redistributable file
                            {
                                if (!vc2013Count)//if the bool is false
                                {
                                    vc2013Count = true;
                                    prerequisitesCount++;//increasing the prerequisite count.
                                    prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.vcredistributable2013));//this is to indicate that vcredistributable2013 is installed.
                                }
                            }
                            else if (prerequisiteList.Contains(Constants.vcredistributable2015)&& version.Contains(Constants.vcredistributable2015))//if the string contains the 2015 redistributable file
                            {
                               
                                if (!vc2015Count)//if the bool is false
                                {
                                    vc2015Count = true;
                                    prerequisitesCount++;//increasing the prerequisite count.
                                    prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.vcredistributable2015));//this is to indicate that vcredistributable2015 is installed.
                                }
                             }
                        }
                    }
                }
            }
            #endregion
        }

        private void CheckDriversInstalled()
        {
            #region Code to check all the drivers installed
            registry_key = versionsPath;
            key = Registry.LocalMachine;
            key = key.OpenSubKey(registry_key);
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        string[] subKeys = subkey.GetValueNames();
                        for (int i = 0; i < subKeys.Length; i++)
                        {
                            if (subKeys[i].Contains(displayNameText))//to check whether the key in the registry contains display name text 
                            {
                                if (!string.IsNullOrEmpty((string)subkey.GetValue(displayNameText)))
                                {
                                    string version = (string)subkey.GetValue(displayNameText);
                                    if ((version.Contains(Constants.mysqlVersion) || version.Contains(Constants.mysqlVersion1)) && !IVLVariables.isCommandLineAppLaunch)//if the string contains the mysql in the registry key
                                    {
                                        prerequisitesCount++;
                                        prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.mysqlVersion));//this is to indicate that mysqlVersion is installed.
                                    }
                                    if (version.Contains(Constants.cameraDriverVersion) && version.Contains(Constants.cameraVersionText))//if the string contains the camera driver in the registry key
                                    {
                                        prerequisitesCount++;
                                        prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.cameraDriverVersion));//this is to indicate that cameraDriverVersion is installed.
                                    }
                                    if (version.Contains(Constants.boardDriver))//if the string contains the board driver in the registry key
                                    {
                                        prerequisitesCount++;
                                        prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.boardDriver));//this is to indicate that boardDriver is installed.
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
        private void CheckDotFrameWork()
        {

            if (!frameworkVersionCount)//if the bool is false
            {
                frameworkVersionCount = DotNetUtils.IsCompatible();
                if (frameworkVersionCount)
                {
                    prerequisitesCount++;//increasing the prerequisites count
                    prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.frameworkVersion));//this is to indicate that frameworkVersion is installed.
                }
            }
        }
        private void CheckAdobeReaderInstalled()
        {
            #region Code to check adobe reader is installed
            if (adobeOsInfo.IsAdobeInstalled)
            {
                prerequisitesCount++;
                prerequisiteList.RemoveAt(prerequisiteList.IndexOf(Constants.adobeReaderVersion));//this is to indicate that adobeReaderVersion is installed.
            }
            #endregion
        }

        /// <summary>
        /// Loads the user control into the PagePanel_p panel.
        /// </summary>
        private void SetPanels()
        {
            PagePanel_p.Controls.Clear();
            emr.Dock = DockStyle.Fill;
            try
            {
                PagePanel_p.Controls.Add(emr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            isEmr = true;
            Image_btn.Enabled = false;
            emr.Show();
            #region this has to be implemented later when login screen has been added
            //loginScreen.Dock = DockStyle.Fill;
            //PagePanel_p.Controls.Add(loginScreen);
            ////isEmr = true;
            //loginScreen.Show();
            // commented to remove login screen at startup of the application by sriram on october 16th 2015
            //loginScreen.Dock = DockStyle.Fill;
            //loginScreen.Show();
            #endregion
        }

        /// <summary>
        /// Loads the image and its details into the thumbnailUI.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void showThumbnails(string s, Args arg)
        {

            //this.thumbnailUI1.isFirstThumbnail_Selected = true;
            //the below code has been added by Darshan to solve defect no 0000525: numbering of thumbnail is mismatching in view image section.
            this.thumbnailUI1.ResetThumbnailUI();
            //imgList.fileNames = arg["Thumbnails"] as List<string>;
            //imgList.ids = arg["id"] as List<int>;
            //int count = imgList.ids.Count;
            //imgList.isAnnotatedList = arg["isannotated"] as List<bool>;
            //imgList.isCDRList = arg["isCDR"] as List<bool>;
            //imgList.sides = arg["Side"] as List<int>;
           // this.thumbnailUI1.AddThumbnails(imgList.fileNames, imgList.ids, imgList.sides, imgList.isAnnotatedList, imgList.isCDRList);

            List<ThumbnailData> thumbnailDataList = arg["Thumbnails"] as List<ThumbnailData>;
            this.thumbnailUI1.AddThumbnails(thumbnailDataList);
            //The below code is commented so as not remove the corrupted images.
            //This below if statement has been added by Darshan on 18-09-2015 to remove invalid images.
            //if (this.thumbnailUI1.corrupted_images.Count > 0)
            //{
            //    imgList.ids = this.thumbnailUI1.corrupted_images;
            //    for (int i = 0; i < imgList.ids.Count; i++)
            //    {
            //        remove_corruptedImages(imgList.ids[i], i);
            //    }
            //}
        }

        void Consultation2Imaging(string a, Args e)
        {
            _eventHandler.Notify(_eventHandler.CameraUIShown, e);
            emr_btn.Visible = true;
            patientDetails_p.Visible = true;
            Imaging_btn_Click(null, null);
        }

        /// <summary>
        /// Navigates to live imaging screen from view imaging screen
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg1"></param>
        private void NavigateFromViewToLive(string s, Args arg1)
        {
            bool isStartLive = false;
            if (IVLVariables.isCommandLineAppLaunch)
                isStartLive = true;
            else
            {
              if(  INTUSOFT.Data.Repository.NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date )
                  isStartLive = true;

            }
            if (!IVLVariables.ivl_Camera.IsCapturing && isStartLive)
            {
                this.thumbnailUI1.Visible = true;
                Args arg = new Args();
                if (IVLVariables.isZoomEnabled)
                    _eventHandler.Notify(_eventHandler.EnableZoomMagnification, new Args());
                //brightnesspopup();
                //if (!IVLVariables.isValueChanged)//to check whether any changes made in the image is saved or discarded.
                {
                    arg["isImaging"] = true;
                    _eventHandler.Notify(_eventHandler.SetImagingScreen, arg);
                    Image_btn.Enabled = false;
                }
            }
            this.Focus();
        }

        /// <summary>
        /// Invokes the StartCaptureSequenceForSpaceBarAndTrigger()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Imaging_btn_Click(object sender, EventArgs e)
        {
            brightnesspopup();
            if (!IVLVariables.isValueChanged)//this has been added to check wheter the changes made in images has been saved or not.
            {
                IVLVariables.istrigger = false;
                IVLVariables.ivl_Camera.Trigger_Or_SpacebarPressed(IVLVariables.istrigger);
                this.Focus();
            }
            // NavigateFromViewToLive("", new Args());
        }

        /// <summary>
        /// This event will enable or disables the Image_btn in view imaging screen based on the camera connection.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg"></param>
        private void EnableDisableImagingBtn(string s, Args arg)
        {
            if (IVLVariables.ShowImagingBtn)
            {
                if ((bool)arg["BtnEnable"])
                {
                    Image_btn.Enabled = true;
                    Image_btn.ToolTipText = IVLVariables.LangResourceManager.GetString("ImageButton_ToolTipText", IVLVariables.LangResourceCultureInfo);
                }
                else
                {
                    Image_btn.Enabled = false;
                    Image_btn.ToolTipText = "";
                }
                this.Focus();// this line has been added in order retrieve focus to the main form after capturing of an image so that space bar failing is fixed in order to defect number 0000695 by sriram on 23 september 2015
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            PagePanel_p.Controls.Clear();
            loginScreen.Dock = DockStyle.Fill;
            PagePanel_p.Controls.Add(loginScreen);
            this.thumbnailUI1.Visible = false;
        }

        private void IvlMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (thumbnailUI1.Visible)
                    thumbnailUI1.isShiftKeyPressed = true;
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                if (thumbnailUI1.Visible)
                    thumbnailUI1.isControlKeyPressed = true;
            }
        }


        private void IvlMainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    {
                        if (thumbnailUI1.Visible)
                        {
                            thumbnailUI1.isShiftKeyPressed = false;
                        }
                        break;
                    }
                case Keys.ControlKey:
                    {
                        if (thumbnailUI1.Visible)
                            thumbnailUI1.isControlKeyPressed = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// Loads the setting window when alt+s is pressed.
        /// </summary>
        public void settings_window()
        {
            string appLogoFilePath = IVLVariables.appDirPathName + @"ImageResources\LogoImageResources\IntuSoft.ico";

            SettingsWindow settings = new SettingsWindow();
            if (File.Exists(appLogoFilePath))
                settings.Icon = new System.Drawing.Icon(appLogoFilePath, 256, 256);
            settings.FormBorderStyle = FormBorderStyle.FixedSingle;
            //This below bool variable is added by Darshan on 21-08-2015 to solve Defect no:0000585: annotation window,report window,view full info,user settings window, when trigger press capture is happening.
            IVLVariables.IsAnotherWindowOpen = true;
            settings.ShowDialog();
            //This below bool variable is added by Darshan on 21-08-2015 to solve Defect no:0000585: annotation window,report window,view full info,user settings window, when trigger press capture is happening.
            IVLVariables.IsAnotherWindowOpen = false;
        }

        /// <summary>
        /// Will open the intucloud on a default browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_btn_Click(object sender, EventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("http://ec2-54-164-57-137.compute-1.amazonaws.com:8080/intucloud2/#/login"); // older implementation for intucloud launch using web app.
            }
            catch { }
        }

        #region Public methods

        //This below method has been added by Darshan to solve defect no 0000319
        /// <summary>
        /// This method will display a pop up when brightness or contrast is changed in view image screen.
        /// </summary>
        public void brightnesspopup()
        {
            if (IVLVariables.isValueChanged)
            {
                _eventHandler.Notify(_eventHandler.SaveImgChanges, new Args());
            }
        }

        /// <summary>
        /// This function which will update the patient details when any detail related to a patient is changed.
        /// </summary>
        public void updatePatient()
        {
            //Patient p =DataVariables._patientRepo.GetById(IVLVariables.ActivePatID);
            //This below code has been Added by Darshan on 31-07-2015 to support advance searching.
            NewDataVariables.Patients.Where(x => x.personId == NewDataVariables.Active_Patient).ToList()[0].patientLastModifiedDate = DateTime.Now;
            NewIVLDataMethods.UpdatePatient();
            NewDataVariables.Active_PatientIdentifier.lastModifiedDate = DateTime.Now;
            NewIVLDataMethods.UpdatePatientIdentifier();
        }

        public bool CheckIfServiceIsRunning(string serviceName)
        {
            ServiceController mysqlSe = new ServiceController();
            mysqlSe.ServiceName = serviceName;
            if (mysqlSe.Status == ServiceControllerStatus.Running)
            {
                // Service already running
                //mysqlSe.Dispose();
                return true;
            }
            else
            {

                return false;
            }
        }
        #endregion
        #region private methods

        //This below method has been added by Darshan on 18-09-2015 to delete the corrupted images.
        /// <summary>
        /// This function will remove the corrupted images.
        /// </summary>
        /// <param name="id">id of the image</param>
        /// <param name="index">index in the list of images</param>
        public void remove_corruptedImages(int id, int index)
        {
            //foreach (int item in this.thumbnailUI1.corrupted_images)
            {
            //    if (item == id)
                {
                    imgList.ids.RemoveAt(index);
                    //VisitModel vs = DataVariables._visitViewRepo.GetById(IVLVariables.ActiveVisitID);
                    //vs.NoOfImages--;
                    //DataVariables._visitViewRepo.Update(vs);
                    obs ob = NewDataVariables._Repo.GetById<obs>(id);
                    ob.voided = true;
                    NewDataVariables._Repo.Update<obs>(ob);
                    //ImageModel im = DataVariables._imageRepo.GetById(id);
                    //im.HideShowRow = true;
                    //DataVariables._imageRepo.Update(im);
                    //imgList.sides.RemoveAt(index);
                    //imgList.fileNames.RemoveAt(index);
                    emr.corrupted_count++;
                }
            }
        }
        #endregion
        #region private events

        /// <summary>
        /// This event will help user to navigate from live imaging screen or view imaging screen to patient details screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmrManage_btn_Click(object sender, EventArgs e)
        {
            if (!IVLVariables.ivl_Camera.IsCapturing)
            {

                //This Below Line was added by Darshan on 24-08-2015 18-52 to clean the design,as discussed with sriram sir.
                PagePanel_p.BorderStyle = BorderStyle.None;
                if (!isEmr)
                {
                    if (IVLVariables.ivl_Camera != null)
                    {
                        IVLVariables.ivl_Camera.StopLive();
                    }
                    if (IVLVariables.isZoomEnabled)
                        _eventHandler.Notify(_eventHandler.EnableZoomMagnification, new Args());
                }
                if (IVLVariables.isValueChanged)
                {
                    _eventHandler.Notify(_eventHandler.SaveImgChanges, new Args());
                    //IVLVariables.isValueChanged = false;
                }
                //This below if statement is added by Darshan on 21-08-2015 to solve Defect no 0000593: The highlighting should be done on the channel display button.
                if (IVLVariables.iscolorChange)
                {
                    _eventHandler.Notify(_eventHandler.RGBColorchange, new Args());
                    IVLVariables.iscolorChange = false;
                }
                if (!IVLVariables.isValueChanged)
                {

                    if (imaging_UC.liveImagingControl.IsEnableFFATimer)
                        imaging_UC.liveImagingControl.IsEnableFFATimer = false ;
                   
                    PagePanel_p.Controls.Clear();
                    if (emr == null)//added to fix defect 0000458 by sriram 18th july 2015
                        emr = new EmrManage(); //added to fix defect 0000458 by sriram 18th july 2015
                    PagePanel_p.Controls.Add(emr);
                    isEmr = true;
                    this.thumbnailUI1.Visible = false;
                    this.thumbnailUI1.ResetThumbnailUI();
                 
                    //this.thumbnailUI1.thumbnail_FLP.Controls.Clear();
                    //logOut_lbl.Visible = true; commented by sriram to remove logout button when login screen is not present
                    Image_btn.Visible = true;
                    Image_btn.Enabled = false;
                    Image_btn.ToolTipText = "";
                    IVLVariables.ivl_Camera.TriggerOff();
                    IVLVariables.ivl_Camera.camPropsHelper.IRLightOnOff(false);
                    patientDetails_p.Visible = false;
                    IVLVariables.CanAddImages = false;// Added to fix the defect 0000582  and 0000562 by sriram on August 18th 2015
                    //This code has been added by darshan to solve defect no 0000541: Reopen:Consultation grid is not shown when returned back from the live mode.
                    thumbnailFileName = string.Empty;
                    #region Added to fix defect 0001753
                    emr_btn.AutoToolTip = false;
                    emr_btn.ToolTipText = string.Empty;
                    #endregion
                    emr_btn.Visible = false;
                    
                    emr.ShowIndividualPatDetails();
                    emr.UpdateVisitButton();
                    emr.Dock = DockStyle.Fill;
                    emr.Show();
                    emr.Refresh();

                    //dataBaseServerConnection.DatabaseBackup(dataBasebackupPath);
                }
            }
        }

        /// <summary>
        /// This event will help user to navigate to login screen from any scren in the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logOut_lbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (IVLVariables.isValueChanged)
            {
                _eventHandler.Notify(_eventHandler.SaveImgChanges, new Args());
            }
            emr_btn.Visible = false;
            logOut_lbl.Visible = false;
            thumbnailUI1.Visible = false;
            Image_btn.Visible = false;
            patientDetails_p.Controls.Clear();
            //This Below Line was added by Darshan on 24-08-2015 18-52 to clean the design,as discussed with sriram sir.
            PagePanel_p.BorderStyle = BorderStyle.FixedSingle;
            emr.UnsubscribleEvents();// Added to remove multiple image captures 
            emr.Dispose();// added to fix defect 0000458 by sriram 18th july 2015
            emr = null;//added to fix defect 0000458 by sriram 18th july 2015
            SetPanels();
        }

        /// <summary>
        /// This event will stop all operations which are currently running in the application. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IvlMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            e.Cancel = IVLVariables.ivl_Camera.IsCapturing;// added to avoid closing of application when capture of the image is in progress by sriram
            if (IVLVariables.ivl_Camera.IsCapturing)// added to avoid closing of application when capture of the image is in progress by sriram
                return;// added to avoid closing of application when capture of the image is in progress by sriram
            if (!IVLVariables.ivl_Camera.IsCapturing)// added to avoid closing of application when capture of the image is in progress by sriram
            {
                IVLVariables.ApplicationClosing = true;
                IVLVariables.ivl_Camera.camPropsHelper.IsApplicationClosing = true;
                //This below code is added by darshan on 17-08-2015 to solve defect no 0000564: In view image screen,When changes are made and clicked on close,No confirmation message is popping up.
                brightnesspopup();
                if (IVLVariables.isValueChanged)
                {
                    e.Cancel = IVLVariables.isValueChanged;// added to avoid closing of application if the brightness or contrast value changed image is not saved
                    return;
                }
                
                if (serverTimer.Enabled)
                {
                    serverTimer.Stop();
                    serverTimer.Enabled = false;
                }
                //if(!IVLVariables.isCommandLineAppLaunch && !IVLVariables.isCommandLineArgsPresent)
                //if (databaseTimer.Enabled)
                //{
                //    databaseTimer.Stop();
                //    databaseTimer.Enabled = false;
                //}
                //INTUSOFT.Configuration.ConfigVariables.SetCurrentSettings();

                //XmlConfigUtility.Serialize(IVLVariables._ivlConfig, IVLConfig.fileName);
                IVLVariables.IVLThemes.SerializeTheme();// uncommented to save the change in themes when application was running
                if (IVLVariables.ivl_Camera.isCameraOpen || IVLVariables.ivl_Camera.camPropsHelper.IsBoardOpen)
                {
                    //splashScreen = new SplashScreen();
                    //splashScreenTimer = new System.Timers.Timer();
                    //splashScreenTimer.Elapsed += splashScreenTimer_Elapsed;
                    //splashScreenTimer.Interval = 1000;
                    //splashScreenTimer.Start();
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ShowSplashScreen));
                    IVLVariables.ivl_Camera.DisconnectCameraModule();
                }
              
                DisposeTimers();


                if (p != null && !p.HasExited)
                    p.Kill();
                this.Cursor = Cursors.Default;
            }
        }


        private void ShowSplashScreen(string s, Args arg)
        {

            if ((bool)arg["ShowSplash"])
            {
                if (splashScreen == null) 
                splashScreen = new SplashScreen();
                splashScreen.SplashScreenText = IVLVariables.LangResourceManager.GetString("CameraPowerInit_Text", IVLVariables.LangResourceCultureInfo);
                if (IVLVariables.ivl_Camera.camPropsHelper.IsCameraConnected == Devices.CameraConnected && IVLVariables.ivl_Camera.camPropsHelper.IsPowerConnected == Devices.PowerConnected)
                    splashScreen.ShowDialog();
                else
                {

                    splashScreen.Close();
                }
            }
            else
            {
                splashScreen.Close();
        
            }
        }

        // added by sriram on 4th august 2015 in order to manage the problem of minimizing and maximizing of the application not responding properly in order to fix defect 0000361
        /// <summary>
        /// This event will trigger when application is maximized or minimized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IvlMainWindow_SizeChanged(object sender, EventArgs e)
        {
            if(!IVLVariables.ivl_Camera.IsCapturing)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    serverTimer.Stop();
                    serverTimer.Enabled = false;
                    if (isComponentInitialized)
                    {
                        //databaseTimer.Stop();
                        //databaseTimer.Enabled = false;

                    }
                    if (isEmr)
                    {
                        emr.RemovePatDetails();
                        //emr.patd = null;// added this to fix defect 0000501 by sriram on 4th august 2015
                    }
                    else
                    {
                        if(!IVLVariables.isCommandLineAppLaunch)
                            GoToViewScreen("", new Args());
                        IVLVariables.ivl_Camera.TriggerOff();
                    }
                    //IVLVariables.ivl_Camera.StopCameraBoardTimers();
                }
                else if (this.WindowState == FormWindowState.Maximized)
                {
                    //if (this.IsHandleCreated)
                    {
                        serverTimer.Enabled = true;
                        serverTimer.Start();
                        if (isComponentInitialized)
                        {
                            //databaseTimer.Enabled = true;
                            //databaseTimer.Start();
                        }
                        // this check is done in order to enable the trigger timer only if the serial port is open in order to avoid enabling of timer before initialization of the board by sriram on 1st september 2015
                        this.Activate();
                        try
                        {
                            if (isEmr)
                            {
                                emr.AddPatDetails();
                                //emr.ShowIndividualPatDetails();
                            }
                            else
                            {
                                if (NewDataVariables.Active_Visit != null)
                                    if (!IVLVariables.IsAnotherWindowOpen && NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date)
                                        IVLVariables.ivl_Camera.TriggerOn();
                            }
                            // this.Refresh();
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogWriter.WriteLog(ex, exceptionLog);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// This event will set the current patient details in the view image screen.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg">arg will give the patient details</param>
        private void SetActivePatientDetails(string s, Args arg)
        {
            if (ActivePatUI == null)
            {
                ActivePatUI = new PatDetails_UC();
                ActivePatUI.Dock = DockStyle.Fill;
            }
            ActivePatUI.setPatValue(arg);
            patientDetails_p.Controls.Add(ActivePatUI);
            mrn = arg["MRN"] as string;
            firstName = arg["FirstName"] as string;
            LastName = arg["LastName"] as string;
            Age = arg["Age"] as string;
            Gender = arg["Gender"] as string;
            patientDetails_p.Visible = true;
        }

        /// <summary>
        /// This event will help user to navigate from patient details screen to the view image screen.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg">arg will give details about the images</param>
        private void Navigate2ViewImageScreen(string s, Args arg)
        {
            this.thumbnailUI1.Visible = true;
            imaging_UC.Dock = DockStyle.Fill;
            if (!IVLVariables.isCommandLineAppLaunch)
            {
                emr_btn.Visible = true;
                #region Added to fix defect 0001753
                emr_btn.AutoToolTip = true;
                emr_btn.ToolTipText = IVLVariables.LangResourceManager.GetString("RecordButton_ToolTipText", IVLVariables.LangResourceCultureInfo);
                #endregion
                if (isEmr)
                {
                    PagePanel_p.Controls.Remove(emr);
                    isEmr = false;

                }
            }
            //else

                //if ((int)arg["imageCount"] > 0)
                //{
                //    //arg["ThumbnailFileName"] = thumbnailFileName;
                //    //arg["id"] = Thumbnail_id;
                //    //arg["side"] = side;
                //}
                //ThumbnailData tdata = new ThumbnailData();
                //if (arg.ContainsKey("ThumbnailFileName"))
                //    tdata.fileName = arg["ThumbnailFileName"] as string;
                //if (arg.ContainsKey("idval"))
                //    tdata.id = Convert.ToInt32(arg["idval"]);
                //if (arg.ContainsKey("side"))
                //    tdata.side = (int)arg["side"];

                //arg["ThumbnailData"] = tdata;
                // IVLVariables.ivl_Camera.TriggerOn();// enable trigger in the view screen.

                //IVLVariables._ivlConfig.Mode = ImagingMode.Posterior_Prime;
                //IVLVariables.GetCurrentSettings();

                if (Convert.ToBoolean(arg["isImaging"]))//This if statement has been added by darshan to check to which screen to navigate. 
                {
                    Image_btn.Enabled = false;
                    Image_btn.ToolTipText = "";
                    if (IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.Anterior_Prime && IVLVariables.isDefaultAnteriorGain)
                    {
                        IVLVariables.CurrentSettings.CameraSettings.CaptureCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._DigitalGainDefault.val;
                        IVLVariables.CurrentSettings.CameraSettings.LiveCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._LiveGainDefault.val;
                        IVLVariables.isDefaultAnteriorGain = false;
                    }
                    else if (IVLVariables.ivl_Camera.camPropsHelper.ImagingMode == Imaging.ImagingMode.Posterior_Prime && IVLVariables.isDefaultPrimeGain)
                    {
                        IVLVariables.CurrentSettings.CameraSettings.CaptureCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._DigitalGainDefault.val;
                        IVLVariables.CurrentSettings.CameraSettings.LiveCurrentGainLevel.val = IVLVariables.CurrentSettings.CameraSettings._LiveGainDefault.val;
                        IVLVariables.isDefaultPrimeGain = false;
                    }

                    IVLVariables.CurrentLiveGain = (GainLevels)Enum.Parse(typeof(GainLevels), IVLVariables.CurrentSettings.CameraSettings.LiveCurrentGainLevel.val);
                    IVLVariables.CurrentCaptureGain = (GainLevels)Enum.Parse(typeof(GainLevels), IVLVariables.CurrentSettings.CameraSettings.CaptureCurrentGainLevel.val);

                }

            _eventHandler.Notify(_eventHandler.ThumbnailSelected, arg);
            //IVLVariables.ivl_Camera.ImagingMode = ImagingMode.Posterior_Prime;
            _eventHandler.Notify(_eventHandler.SetImagingScreen, arg);




            PagePanel_p.Controls.Add(imaging_UC);
            this.Refresh();
            this.Focus();
        }

        /// <summary>
        ///This event will delete the selected images from the image list.
        /// </summary>
        /// <param name="imagname">imagname has list of image urls to be deleted</param>
        void thumbnailUI1_deleteimgs(Dictionary<string, object> imagname)
        {
            IVLVariables.isdelete_thumbnail = true;
            List<int> ids = imagname["ImageIds"] as List<int>;
            List<string> imageLocations = imagname["ImageLocations"] as List<string>;
            if (IVLVariables.isCommandLineAppLaunch)
            {
                foreach (string item in imageLocations)
                {
                    FileInfo finf = new FileInfo(item);
                    finf.Delete();
                    string[] str = finf.Name.Split('.');
                    finf = new FileInfo(finf.Directory.FullName + Path.DirectorySeparatorChar + str[0] + "_tb." + str[1]);
                    finf.Delete();

                }
            }
            else
            {
                foreach (int item in ids)
                {
                    //The below code has been modified for the defect no 0001791

                    //// DataVariables.Patients[ DataVariables.Patients.IndexOf(DataVariables.Active_Patient)].visits.ToList().FindIndex(x=>x.ID == IVLVariables.ActiveVisitID)
                    //NewDataVariables.Active_Obs = NewDataVariables.Obs.Find(x => x.observationId == item);
                    ////NewDataVariables.Active_EyeFundusImage = NewDataVariables.EyeFundusImage.Find(x => NewDataVariables.Active_Obs.observationId == item);
                    //NewDataVariables.Active_Obs.lastModifiedDate = DateTime.Now;
                    //NewDataVariables.Active_Obs.voidedDate = DateTime.Now;
                    //NewDataVariables.Active_Obs.voided = true;
                    eye_fundus_image ImageToDelete = NewDataVariables.Obs.Find(x => x.observationId == item);
                    ImageToDelete.lastModifiedDate = DateTime.Now;
                    ImageToDelete.voidedDate = DateTime.Now;
                    ImageToDelete.voided = true;
                    NewDataVariables.Obs[NewDataVariables.Obs.FindIndex(x => x.observationId == item)] = ImageToDelete;
                    //NewIVLDataMethods.RemoveImage();
                }
                Patient pat = NewDataVariables.Patients.Find(x => x.personId == NewDataVariables.Active_Patient);
                pat.visits.ToList()[pat.visits.ToList().FindIndex(x => x.visitId == NewDataVariables.Active_Visit.visitId)].observations = new HashSet<eye_fundus_image>(NewDataVariables.Obs);
                NewDataVariables.Patients[NewDataVariables.Patients.FindIndex(x => x.personId == NewDataVariables.Active_Patient)] = pat;
                updatePatient();
                NewDataVariables.Visits.Reverse();
                NewDataVariables.Obs = pat.visits.ToList().Where(x => x.visitId == NewDataVariables.Active_Visit.visitId).ToList()[0].observations.ToList();
                //List<obs> obs = NewDataVariables._Repo.GetByCategory<obs>("visit", NewDataVariables.Active_Visit).ToList();
                // NewDataVariables.Patients.Where(x => x.personId == NewDataVariables.Active_Patient).ToList()[0].visits.ToList()[NewDataVariables.Active_Visit.visitId].observations.Where(y => y.observationId == NewDataVariables.Active_Obs.observationId).ToList()[0] = NewDataVariables.Active_Obs;
            }
        }

        /// <summary>
        ///This event will show a tool tip of todays date and time when mouse is hovered on panel2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel2_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();
            DayOfWeek tod = DateTime.Today.DayOfWeek;
            String date = DateTime.Today.ToLongDateString() + "\n" + tod.ToString();
            tip.SetToolTip(panel2, date);
        }

        private void PagePanel_p_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void upload2CLoud_btn_Click(object sender, EventArgs e)
        {
            //this.Cursor = Cursors.WaitCursor;
            //if (!isEmr && !IVLVariables.ivl_Camera.CameraIsLive)
            //{
            //    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            //    Dictionary<string, List<string>> exportimges = this.thumbnailUI1.getImageFiles();
            //    string[] fileNames = exportimges["FileNames"].ToList().ToArray();
            //    for (int i = 0; i < fileNames.Length; i++)
            //    {
            //        var obs = NewDataVariables.Obs.Where(x => x.value == new FileInfo(fileNames[i]).Name).ToList();
            //        if (obs.Any())
            //        {
            //            if (obs[0].eyeSide == 'L' && !keyValuePairs.ContainsKey("left_image_files"))
            //                keyValuePairs.Add("left_image_files",new FileInfo( fileNames[i]));
            //            else if (!keyValuePairs.ContainsKey("right_image_files"))
            //                keyValuePairs.Add("right_image_files",new FileInfo( fileNames[i]));
            //        }

            //    }
            //    keyValuePairs.Add("MRNValue", IVLVariables.MRN);
            //    var result = await REST_Utilities.REST_Helper.PutImages(keyValuePairs);
            //    var response =(ResultResponseModel) JsonConvert.DeserializeObject(result, typeof(ResultResponseModel));
            //    CustomMessageBox.Show($"Uploaded Successfully for {response.patient_id}","Upload Status", CustomMessageBoxIcon.Information);
            //}
            //this.Cursor = Cursors.Default;
        }

        /// <summary>
        ///This event will help user to navigate from login screen to patient details screen. 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        void loginScreen__loggedIn(string s, EventArgs e)
        {
            EmrManage_btn_Click(null, null);
        }

        /// <summary>
        /// This event will add the image to the list if images present in view image screen.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg">arg gives details about the image</param>
        private void AddThumbnailEvent(string s, Args arg)
        {
            //Dictionary<string, object> val = new Dictionary<string, object>();
            //val.Add("ImageName", arg["ImageName"] as string);
            //val.Add("id", (int)arg["id"]);
            //val.Add("side", (int)arg["side"]);
            //if (arg.ContainsKey("isannotated"))
            //    val.Add("isannotated", (bool)arg["isannotated"]);
            //else
            //    val.Add("isannotated", false);
            //if (arg.ContainsKey("isCDR"))
            //    val.Add("isCDR", (bool)arg["isCDR"]);
            //else
            //    val.Add("isCDR", false);
            //isModifiedimage = (bool)arg["isModifiedimage"];
            //this.thumbnailUI1.AddThumbnailEvent(val);
            this.thumbnailUI1.AddThumbnailEvent((ThumbnailData)arg["ThumbnailData"]);
            this.Focus();
        }

        /// <summary>
        /// Method to get the patient details to draw on the  exported images.
        /// </summary>
        /// <returns></returns>
        private List<string> GetPatientDetails()
        {
            List<string> patDetails = new List<string>();
            if (Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsWriteAllDetails.val))
            {
                if (Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsMRNPresent.val))
                {
                    string mrn = "ID: " + NewDataVariables.Active_PatientIdentifier.value.ToString();
                    patDetails.Add(mrn);
                }

                if (Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsFirstNamePresent.val))
                {
                    string name = NewDataVariables.Active_PatientIdentifier.patient.firstName;
                    if (Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsLastNamePresent.val))
                        name += " " + NewDataVariables.Active_PatientIdentifier.patient.lastName;
                    patDetails.Add(name);
                }
                else if(Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsLastNamePresent.val))
                {
                    string name = NewDataVariables.Active_PatientIdentifier.patient.lastName;
                    patDetails.Add(name);
                }

                if (Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsAgePresent.val))
                {
                    string ageGender = (DateTime.Now.Year - NewDataVariables.Active_PatientIdentifier.patient.birthdate.Year).ToString();
                    if(Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsGenderPresent.val))
                        ageGender += " \\ " + NewDataVariables.Active_PatientIdentifier.patient.gender.ToString();
                    patDetails.Add(ageGender);
                }
                else if(Convert.ToBoolean(IVLVariables.CurrentSettings.ImageNameSettings.IsGenderPresent.val))
                {
                    string ageGender = NewDataVariables.Active_PatientIdentifier.patient.gender.ToString();
                    patDetails.Add(ageGender);
                }

                //patDetails[1] = NewDataVariables.Active_PatientIdentifier.patient.firstName + " " + NewDataVariables.Active_PatientIdentifier.patient.lastName;
                //patDetails[2] = (DateTime.Now.Year - NewDataVariables.Active_PatientIdentifier.patient.birthdate.Year).ToString() + " \\ " + NewDataVariables.Active_PatientIdentifier.patient.gender.ToString();
                //patDetails[3] = "Diagnosis: ";
                //if(NewDataVariables.Active_Obs.eyeSide == '0')
                //    patDetails[4] = "";
                //else
                //    patDetails[4] = "";\
                string date = "Date: " + NewDataVariables.Active_Obs.createdDate.Date.ToString("dd/MM/yyyy");
                patDetails.Add(date);
                string time = "Time: " + NewDataVariables.Active_Obs.createdDate.ToString("HH:mm:ss");
                patDetails.Add(time);
            }
            return patDetails;
        }


        /// <summary>
        /// This event will export selected images to a new folder.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg">arg will give detail about the folder path</param>
        private void exportFiles(string s, Args arg)
        {
            string folderPath = arg["folderPath"] as string;
            string imageFormat = arg["imageFormat"] as string;
            ImageSaveFormat imageSaveFormat = (ImageSaveFormat)Enum.Parse(typeof(ImageSaveFormat), imageFormat);
            int compressionRatio = (int)arg["compressionRatio"];
            // int imageCompressionRatio = Convert.ToInt64(compressionRatio);
            string folderName = mrn + "_" + firstName + "_" + LastName + "_" + Age + "_" + Gender;
            DirectoryInfo dinf = new DirectoryInfo(folderPath + Path.DirectorySeparatorChar + folderName);
            if (!Directory.Exists(dinf.FullName))
                dinf = Directory.CreateDirectory(dinf.FullName);
            List<FileInfo> files = arg["fileNames"] as List<FileInfo>;
            string[] Labels = arg["ImageLabelText"] as string[];
            List<string> imageLabels = Labels.ToList<string>();
            Common.CustomCopyAndReplace.isDoForAllConflicts = false;
            int sameFilesCount = -1;
            List<string> patDet = GetPatientDetails();
            CustomFolderBrowser.fileNames = new string[files.Count];// to handle opening of mutiples files .By Ashutosh 21-7-2017
            //if two files are selected to be opened / exported , then count will be 2.
            for (int i = 0; i < files.Count; i++)//
            {
                string[] name = files[i].Name.Split('.');
                string path = dinf.FullName + Path.DirectorySeparatorChar + imageLabels[i].Replace("  ", "_") + "_" + name[0];// +"." + imageFormat;
                
                if (File.Exists(path + "." + imageFormat))//Check for the existience of the file.
                {
                    sameFilesCount++;
                }
                CustomFolderBrowser.fileNames[i]= path + "." + imageFormat;
                //CustomFolderBrowser.fileNames= path + "." + imageFormat;fileNames is of string array type whereas path is string, therefore [i] is necessary.
            }
            
            for (int i = 0; i < files.Count; i++)
            {
                string[] name = files[i].Name.Split('.');//Splits the name of  the file path to get the name of the file.
                string path = dinf.FullName + Path.DirectorySeparatorChar + imageLabels[i].Replace("  ", "_") + "_" + name[0];// +"." + imageFormat;
                if (!File.Exists(path + "." + imageFormat))//Check for the existience of the file if not it will save the file.
                {
                    Bitmap imageToBeSaved = Image.FromFile(files[i].FullName) as Bitmap;
                    IVLVariables.ivl_Camera.camPropsHelper.SaveImage2Dir(imageToBeSaved, path, imageSaveFormat, compressionRatio, true, patDet);
                }
                else
                {
                    if (!Common.CustomCopyAndReplace.isDoForAllConflicts)//Checks weather the do it for all conflicts check box is checked or unchecked.
                    {
                        DialogResult = CustomCopyAndReplace.Show(sameFilesCount);
                    }
                    if (DialogResult == System.Windows.Forms.DialogResult.OK)//Checks weather to replace all the files in the list.
                    {
                        {
                            {
                                Bitmap imageToBeSaved = Image.FromFile(files[i].FullName) as Bitmap;
                                IVLVariables.ivl_Camera.camPropsHelper.SaveImage2Dir(imageToBeSaved, path, imageSaveFormat, compressionRatio, true, patDet);//Save the file in the path.
                                sameFilesCount--;
                            }
                        }
                    }
                    else if (DialogResult == System.Windows.Forms.DialogResult.Yes)//Checks weather to copy and keep both old and new file.
                    {
                        Bitmap imageToBeSaved = Image.FromFile(files[i].FullName) as Bitmap;
                        string modifiedFileName = GetUniqueFilePath(path + "." + imageFormat);//Will generate the new name.
                        IVLVariables.ivl_Camera.camPropsHelper.SaveImage2Dir(imageToBeSaved, modifiedFileName, imageSaveFormat, compressionRatio, true, patDet);//Save the file in the path.
                        sameFilesCount--;
                    }
                    else if (DialogResult == System.Windows.Forms.DialogResult.Ignore)//Checks if we don't want to export the file.
                    {
                        sameFilesCount--;
                    }
                    else if (DialogResult == System.Windows.Forms.DialogResult.Cancel)//Checks for the cancel operation.
                    {
                        return;
                    }
                }
            }
            if (DialogResult != System.Windows.Forms.DialogResult.Ignore)
                CustomMessageBox.Show(IVLVariables.LangResourceManager.GetString("ExportCompleted_Text", IVLVariables.LangResourceCultureInfo), IVLVariables.LangResourceManager.GetString("ImageViewer_ExportImages_Button_Text", IVLVariables.LangResourceCultureInfo), CustomMessageBoxIcon.Information);
        }

        /// <summary>
        /// This will getuniquefilepath if file already exisits in the path.
        /// </summary>
        /// <param name="filepath">Path of the file</param>
        /// <returns>unique file name</returns>
        public static string GetUniqueFilePath(string filepath)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;
                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");
                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }
                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
                filepath = Path.Combine(folder, string.Format("{0} ({1})", filename, number));
            }
            return filepath;
        }

        /// <summary>
        /// This event will change the side ofa image.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg">arg will give the details of the image</param>
        private void changeThumbnailSide(string s, Args arg)
        {
            string fileName = string.Empty;
            if (arg.ContainsKey("ImgLoc"))
                fileName = (String)arg["ImgLoc"];
            //the below code has been added by Darshan to solve defect no 0000510: Duplicate numbering in comments if pressed on control key.
            this.thumbnailUI1.ChangeThumbnailSide((int)arg["id"], (int)arg["side"], (bool)arg["isannotated"], (bool)arg["isCDR"],fileName);
        }

        /// <summary>
        /// This event will show a single image on the screen from the list of images in view image screen.
        /// </summary>
        /// <param name="s">s give details about the image</param>
        /// <param name="e"></param>
        void thumbnailUI1_showImgFromThumbnail(ThumbnailData s, EventArgs e)
        {
            //This method modified by Darshan on 16-09-2015 to stop courroupting of json file.
            // thumbnailFileName = s["$ThumbnailFileName"] as string;
            // Thumbnail_id = (int)s["$id"];
            //string thumbnailName = s["$ThumbnailName"] as string;
            // if (s.ContainsKey("$side"))
            //     side = (int)s["$side"];
            //isImaging = false;
            if (IVLVariables.isValueChanged && !IVLVariables.ivl_Camera.IsCapturing)//to check if brightness or contrast has been changed.
            {
                _eventHandler.Notify(_eventHandler.SaveImgChanges, new Args()); //to notify the event save changed images.
                if(!IVLVariables.isValueChanged) //if the value is not changed
                {
                    thumbnailUI1.ThumbnailSelected(s.fileName); //thumbnail selection has been called .
                    thumbnailUI1.isControlKeyPressed = false; //this has been added to resolve the defect no 0001853 by R.Kishore on 27th March 2018.
                    thumbnailUI1.isShiftKeyPressed = false; //this has been added to resolve the defect no 0001853 by R.Kishore on 27th March 2018.
                }

            }

            else 
            {
                Args arg = new Args();
                arg["ThumbnailData"] = s;
                if (!IVLVariables.ivl_Camera.IsCapturing && !IVLVariables.ivl_Camera.IsResuming && !IVLVariables.ivl_Camera.isResetMode && IVLVariables.ivl_Camera.IsMotorMovementDone)
                {
                    if (!IVLVariables.isCommandLineAppLaunch)
                    {
                        if (NewDataVariables.Active_Visit.createdDate.Date == DateTime.Now.Date && isPowerConnected && isCameraConnected)
                        {
                            Image_btn.Enabled = true;
                            Image_btn.ToolTipText = IVLVariables.LangResourceManager.GetString("ImageButton_ToolTipText", IVLVariables.LangResourceCultureInfo);
                        }
                        else
                        {
                            Image_btn.Enabled = false;
                            Image_btn.ToolTipText = "";
                        }
                    }
                    arg["isImaging"] = false;
                    _eventHandler.Notify(_eventHandler.SetImagingScreen, arg);
                    _eventHandler.Notify(_eventHandler.LoadImageFromFileViewingScreen, arg);

                }
            }
            this.thumbnailUI1.Focus();//This is added to fix defect 0000998 to enable mouse wheel for thumbnail scrolling
            this.Focus();
        }

        #endregion

        private void HospitalLogo_pbx_Resize(object sender, EventArgs e)
        {
            ResizeToRoundedRectangle(sender, HospitalLogo_pbx.Height / 2);
        }

        private void companyLogo_pbx_Resize(object sender, EventArgs e)
        {
            ResizeToRoundedRectangle(sender, companyLogo_pbx.Height / 2);
        }


 
    }

    public class ImageList
    {
        public List<int> ids;
        public List<int> sides;
        public List<bool> isCDRList;
        public List<bool> isAnnotatedList;
        public List<string> fileNames;

        public ImageList()
        {
            ids = new List<int>();
            sides = new List<int>();
            fileNames = new List<string>();
            isCDRList = new List<bool>();
            isAnnotatedList = new List<bool>();
        }
    }

    class DotNetUtils
    {
        public enum DotNetRelease
        {
            NOTFOUND,
            NET45,
            NET451,
            NET452,
            NET46,
            NET461,
            NET462,
            NET47,
            NET471
        }

        public static bool IsCompatible(DotNetRelease req = DotNetRelease.NET45)
        {
            DotNetRelease r = GetRelease();
            if (r < req)
            {
                Console.WriteLine(String.Format("This this application requires {0} or greater.", req.ToString()));
                return false;
            }
            else
            {
                Console.WriteLine(r.ToString());
                return true;
            }
        }

        public static DotNetRelease GetRelease(int release = default(int))
        {
            int r = release != default(int) ? release : GetVersion();
            if (r >= 461308) return DotNetRelease.NET471;
            if (r >= 460798) return DotNetRelease.NET47;
            if (r >= 394802) return DotNetRelease.NET462;
            if (r >= 394254) return DotNetRelease.NET461;
            if (r >= 393295) return DotNetRelease.NET46;
            if (r >= 379893) return DotNetRelease.NET452;
            if (r >= 378675) return DotNetRelease.NET451;
            if (r >= 378389) return DotNetRelease.NET45;
            return DotNetRelease.NOTFOUND;
        }

        public static int GetVersion()
        {
            int release = 0;
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                                                .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                release = Convert.ToInt32(key.GetValue("Release"));
            }
            return release;
        }
    }

    public class ResultResponseModel
    {
        public string message = string.Empty;
        public string patient_id = string.Empty;
    }
}
