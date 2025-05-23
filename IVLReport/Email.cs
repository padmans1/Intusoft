﻿using Amazon.Runtime;
using Amazon.S3.Transfer;
using Amazon.S3;
using Common;
using Common.ValidatorDatas;
using Common.Validators;
using INTUSOFT.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using ReportUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Drawing.Imaging;
using System.Drawing;
using RestSharp;
using INTUSOFT.Data.Repository;
using INTUSOFT.Data.NewDbModel;
using Emgu.CV.Structure;
using Emgu.CV;
using INTUSOFT.Imaging;
using Microsoft.Office.Interop.Word;
using System.Xml.Serialization;
using System.Xml;
using MailMessage = System.Net.Mail.MailMessage;

namespace IVLReport
{

    public partial class EmailWindow : Form
    {
        private static readonly Logger Exception_Log = LogManager.GetLogger("ExceptionLog");

        public enum MailMessageEnum { From, To, CC, BCC };
        public string vendorVal = "";
        public MailMessageEnum mailMessageEnum;
        SmtpClient smtp;
        string toaddr = "";
        MailMessage msg = new MailMessage();
        MailAddresses mailAddresses;
        public string ReportFileName;
        public string[] ImageFileNames;

        public string AIUserName = string.Empty;
        public string AIPassword = string.Empty;
        public string AIApiRequestType = string.Empty;


        string jsonPatientDetails;
        string jsonImageSettings;
        string jsonImageMaskSettings;
        private DataModel _dataModel;
        private RichTextBox currentTextBox;
        EmailValidator emailValidiation;
        public delegate void EmailSent(string isSent, EventArgs e);
        public event EmailSent _EmailSent;
       
        public delegate void WaitCursor(EventArgs e, bool isWait);
        public event WaitCursor _WaitCursor;
        private char semiColonCharacter = ';';
        private char underScoreCharacter = '_';
        private char dotCharacter = '.';
        private string fromaddr = "intusoft_emailer@intuvisionlabs.com";
        private string password = "IntusoftEmailer123";

        private string chironxUsername = "sriram@intuvisionlabs.com";
        private string chironxPassword = "intuvision123";

        private string lebenUsername = "equipagehe";
            
          
        //private string lebenUsername = "anil@intuvisionlabs.com";
        //private string lebenPassword = "Gt#8$DkMy";
        private string lebenPassword = "equipage123";

        //private string jioUsername = "anilpreston@gmail.com";
        ////private string lebenUsername = "anil@intuvisionlabs.com";
        ////private string lebenPassword = "Gt#8$DkMy";
        //private string jioPassword = "Anil@1234";
        private string bccAddr = "";
        public string BccEmail = "intuvisiontelemedbackup@gmail.com;";
        private string hostName = "mail.intuvisionlabs.com";
        private int hostPort = 587;
        private int hostTimeout = 30000;
        private string fileJson = @"mailList.json";
        private string emailCancelMsg = "Email has been cancelled";
        private string emailErrorMsg = "Email sending error";
        private string emailSentMsg = "Email has been sent";
        private string retryAgain = "Retry Again";
        private const string compressedDataString = "CompressedData";
        private string compressedData = "";
        private string dateMonthYearFormat = "yyyy-MM-dd_HH-mm-ss";
        private string emailJsonData = "EmailData.json";
        private string observationText = "observations.txt";
        private string rightEyeObservationText = "OD (Right Eye) Observations";
        private string leftEyeObservationText = "OS (Left Eye) Observations:";
        private string observationSpacing = "-------------------------------------";
        private string generalObservationText = "General Observations:";
        private string patientDetailsJson = "PatientDetails.json";
        private string CameraSettingDetailsJson = "CameraSettingDetails.json";
        private string ImageMaskSettingDetailsJson = "MaskSettingDetails.json";
        private string zipExtension = ".zip";
        private string sourceBatFilePath = @"startInusoft.bat";
        private string sourceBatFile = "startInusoft.ba";
        private string templateXmlText = "template.xml";
        private string enterValidFromMail = "Enter the valid from address";
        private string enterValidToMail = "Enter the valid to address";
        private string mailIdEndWithSemicolon = "Mail id has to end with semicolon";
        private string reportText = "_Report";
        private string mailIdEmpty = "Mail id should not be empty";
        private Dictionary<string, object> emailDic;// = new Dictionary<string, object>();
        string attachmentPath = "";
        INTUSOFT.EventHandler.IVLEventHandler eventHandler;
        string zipFileExportPath = string.Empty;
        EmailRouteSettings emailRouteSettings = null;
        public List<ImageInfo> Payload = new List<ImageInfo>();

        public EmailWindow(Dictionary<string, object> emailDicParam)
        {
            InitializeComponent();

            zipFileExportPath = ConfigVariables.CurrentSettings.ReportSettings.ExportZipFilePath.val;
            if (!string.IsNullOrEmpty(zipFileExportPath))
                if (!Directory.Exists(zipFileExportPath))
                    Directory.CreateDirectory(zipFileExportPath);
            eventHandler = INTUSOFT.EventHandler.IVLEventHandler.getInstance();
            _dataModel = DataModel.GetInstance();
            emailValidiation = new EmailValidator();
            if (emailDicParam != null)
                emailDic = emailDicParam;
            else
                emailDic = new Dictionary<string, object>();
            SetEmailVariableValues();
            emailRouteSettings = EmailRouteSettings.GetEmailRouteValues();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp = new SmtpClient();
            SetSmtpDetails();

        }
        public static SmtpPermission CreateConnectPermission()
        {
            SmtpPermission connectAccess = new
                SmtpPermission(SmtpAccess.ConnectToUnrestrictedPort);
            Console.WriteLine("Access? {0}", connectAccess.Access);
            return connectAccess;
        }

        private void SetSmtpDetails()
        {
            try
            {
                smtp.Host = emailRouteSettings.hostName;
                smtp.Port = emailRouteSettings.hostPort;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = hostTimeout;
                smtp.EnableSsl = true;
                string mailListStr = string.Empty;
                SmtpPermission permission = CreateConnectPermission();
                if (File.Exists(fileJson))
                {
                    mailListStr = File.ReadAllText(fileJson);
                    mailAddresses = (MailAddresses)JsonConvert.DeserializeObject(mailListStr, typeof(MailAddresses));
                }
                else
                    mailAddresses = new MailAddresses();
                mailAddress_lbx.DataSource = mailAddresses.mailAddressList;
                mailAddress_lbx.Visible = false;
                smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            }
            catch (Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();

                while (ex != null) // to get all the exception
                {
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
                string excepStr = stringBuilder.ToString();
                MessageBox.Show(excepStr);
            }

        }
        private void SetEmailVariableValues()
        {
            if (emailDic.ContainsKey("$emailSentText"))
                emailSentMsg = emailDic["$emailSentText"] as string;

            if (emailDic.ContainsKey("$emailFromAddressInvalidText"))
                enterValidFromMail = emailDic["$emailFromAddressInvalidText"] as string;

            if (emailDic.ContainsKey("$emailToAddressInvalidText"))
                enterValidToMail = emailDic["$emailToAddressInvalidText"] as string;

            if (emailDic.ContainsKey("$emailErrorText"))
                emailErrorMsg = emailDic["$emailErrorText"] as string;

            if (emailDic.ContainsKey("$emailCancelledText"))
                emailCancelMsg = emailDic["$emailCancelledText"] as string;

            if (emailDic.ContainsKey("$retryAgainText"))
                retryAgain = emailDic["$retryAgainText"] as string;

            if (emailDic.ContainsKey("$rightEyeObservationsText"))
                rightEyeObservationText = emailDic["$rightEyeObservationsText"] as string;

            if (emailDic.ContainsKey("$leftEyeObservationsText"))
                leftEyeObservationText = emailDic["$leftEyeObservationsText"] as string;

            if (emailDic.ContainsKey("$observationSpacingText"))
                observationSpacing = emailDic["$observationSpacingText"] as string;

            if (emailDic.ContainsKey("$generalObservationsText"))
                generalObservationText = emailDic["$generalObservationsText"] as string;

            if (emailDic.ContainsKey("$emailSemicolonNotPresentText"))
                mailIdEndWithSemicolon = emailDic["$emailSemicolonNotPresentText"] as string;

            if (emailDic.ContainsKey("$emailEmptyText"))
                mailIdEmpty = emailDic["$emailEmptyText"] as string;
        }
        /// <summary>
        /// Event to know the result of sent email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the message we sent

            if (e.Cancelled)
            {
                _EmailSent(emailCancelMsg, new EventArgs());
                CustomMessageBox.Show(emailCancelMsg);
                // prompt user with "send cancelled" message 
            }
            if (e.Error != null)
            {
                _EmailSent(emailErrorMsg + e.Error.Message, new EventArgs());
                CustomMessageBox.Show(emailErrorMsg + e.Error.Message + retryAgain);
                // prompt user with error message 
            }
            else
            {
                _EmailSent(emailSentMsg, new EventArgs());
                CustomMessageBox.Show(emailSentMsg);


                // prompt user with message sent!
                // as we have the message object we can also display who the message
                // was sent to etc 
            }
            this.Cursor = Cursors.Default;
        }

        #region UI Events
        /// <summary>
        /// this event raises whenever the send button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void send_btn_Click(object sender, EventArgs e)
        {
            _dataModel.mailData = new EmailsData();
            _dataModel.mailData.EmailReplyTo = replyTo_tbx.Text;
            _dataModel.mailData.EmailTo = to_tbx.Text;
            _dataModel.mailData.EmailCC = Cc_tbx.Text;
            _dataModel.mailData.EmailBCC = Bcc_tbx.Text;
            _dataModel.mailData.Subject = sbj_tbx.Text;
            SendEmail(_dataModel.mailData);

            #region commented test code
            /*MailMessage mail = new MailMessage("t.s.r.kishoreraja@gmail.com", "t.s.r.kishoreraja@gmail.com");
             SmtpClient client = new SmtpClient("smtp.gmail.com",25);
             //client.Port = 465;
             client.EnableSsl = true;
             client.DeliveryMethod = SmtpDeliveryMethod.Network;
             client.UseDefaultCredentials = false;
             //client.Host = "smtp.gmail.com";
             mail.Subject = "this is a test email.";
             mail.Body = "this is my test email body";
             
             try
             {
                 client.Send(mail);
             }
                 catch(Exception ex)
             {
                 MessageBox.Show(ex.Message);
             } */
            #endregion
        }

        /// <summary>
        /// To send email
        /// </summary>
        /// <param name="emailData"></param>
        public void SendEmail(EmailsData emailData, bool toCloud = false)
        {
            toaddr = to_tbx.Text;
            if (emailValidiation.IsValidEmail(fromaddr).isValidMail)//checks for the valid email address
            {
                this.Cursor = Cursors.WaitCursor;
                if (ValidateEmailData())
                {
                    msg.Subject = sbj_tbx.Text;
                    msg.From = new MailAddress(fromaddr.TrimEnd(semiColonCharacter));
                    msg.Body = body_tbx.Text;

                    AddAttachment(ReportFileName);
                    if (!toCloud)// this code is meant for the zip file/ pdf report to be sent to the gmail id existing implementation
                    {
                        if (!string.IsNullOrEmpty(emailData.Subject))
                            msg.Subject = emailData.Subject;
                        else
                            msg.Subject = sbj_tbx.Text;

                        if (!string.IsNullOrEmpty(emailData.Body))
                            msg.Body = emailData.Body;
                        else
                            msg.Body = body_tbx.Text;

                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(emailRouteSettings.fromaddr, emailRouteSettings.password);//, "cpanel33.interactivedns.com");
                        smtp.SendAsync(msg, msg);
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
            }
            else
                CustomMessageBox.Show(enterValidToMail);
            this.Cursor = Cursors.WaitCursor;
        }
        string token = string.Empty;

        private void uploadReportRequest(string jsonData)
        {
            RestRequest request = new RestRequest((string)_dataModel.ReportData["$apiRequestType"], Method.Post);
            request.AddBody(jsonData);
            var restClient = new RestClient();
            RestResponse response = restClient.ExecuteAsync(request).Result;
            var result = JsonConvert.DeserializeObject<ResponseDTO>(response.Content);
            _EmailSent($"Upload {result.message}", new EventArgs());

            Console.WriteLine(response.Content);
        }
        public void Upload2Cloud(object callback)
        {
            try
            {
                //if (ValidateEmailData())
                {
                    msg.Subject = sbj_tbx.Text;
                    msg.From = new MailAddress(fromaddr.TrimEnd(semiColonCharacter));
                    msg.Body = body_tbx.Text;

                    string url = string.Empty;
                    Dictionary<string, string> ImageDetails = new Dictionary<string, string>();
                    Dictionary<string, string> credentials = new Dictionary<string, string>();
                    int count = 0;
                    _WaitCursor(new EventArgs(), false);

                    if (vendorVal == "Vendor1")
                    {
                        //AddAttachment(ReportFileName);// To create zip file to upload to chironX cloud
                        //FileInfo finf = new FileInfo(attachmentPath);
                        //File.Copy(attachmentPath, zipFileExportPath + Path.DirectorySeparatorChar + finf.Name);// copy file from temprory location to upload to cloud location
                        //CustomMessageBox.Show("Files pushed to uploader successfully", "Uploading to Cloud", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information);//.GetValue("name"));
                        //_WaitCursor(new EventArgs(), true);
                        ////credentials.Add("email", chironxUsername);
                        ////credentials.Add("hashedPassword", chironxPassword);
                        //////url = "http://chironapi.chironx.cloud/api/chironx/authenticate/login";
                        ////url = "http://chironapp.chironx.cloud/api/chironx/authenticate/login";
                        ////ImageDetails.Add("filePath", attachmentPath);
                        ///
                        var baseURL = (string)_dataModel.ReportData["$apiRequestType"];
                        HttpClient httpClient = new HttpClient();
                        //httpClient.BaseAddress = new Uri($"{baseURL}");
                        LoginCredentials loginCredentials = new LoginCredentials
                        {
                            username = _dataModel.ReportData["$userName"].ToString(),
                            password = _dataModel.ReportData["$password"].ToString()
                        };
                        StringContent adCredentials = new StringContent(JsonConvert.SerializeObject(loginCredentials));
                       var httpResult =  httpClient.PostAsync($"{baseURL}/reports/login", adCredentials);
                        httpResult.Wait();
                      var httpResponseVal =   httpResult.Result.Content.ReadAsStringAsync();
                        httpResponseVal.Wait();
                        if(httpResult.Result.StatusCode == HttpStatusCode.OK)
                        {
                            var token = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(httpResponseVal.Result);
                            if (token.statusCode == 200)
                            {
                                //var token = (TokenResponse)JsonConvert.DeserializeObject(httpResponseVal.Result, typeof(TokenResponse));
                                httpClient.DefaultRequestHeaders.Add("Authorization", token.access_token);
                                FileInfo image_le_id = null;
                                FileInfo image_re_id = null;
                                for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
                                {
                                    FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);

                                    if (_dataModel.CurrentImageNames[i].Contains("OS"))
                                        image_le_id = finf;
                                    else
                                        image_re_id = finf;
                                }


                                PatientDetails patientDetails = new PatientDetails()
                                {
                                    patient_fname = (string)_dataModel.ReportData["$FirstName"],
                                    patient_lname = (string)_dataModel.ReportData["$LastName"],
                                    patient_id = $"{_dataModel.ReportData["$MRN"]}_{_dataModel.ReportData["$VisitId"]}",
                                    gender = ((string)_dataModel.ReportData["$Gender"])[0].ToString(),
                                    patient_age = DateTime.Now.Year - ((DateTime)_dataModel.ReportData["$dob"]).Year,
                                    image_le_id = image_le_id != null ? image_le_id.Name.Split('.')[0] : string.Empty,
                                    image_re_id = image_re_id != null ? image_re_id.Name.Split('.')[0] : string.Empty,
                                    image_le_name = image_le_id != null ? image_le_id.Name : string.Empty,
                                    image_re_name = image_re_id != null ? image_re_id.Name : string.Empty,
                                    doc_email = (string)_dataModel.ReportData["$doctorEmailID"],
                                    doc_name = (string)_dataModel.ReportData["$Doctor"],
                                    clinic_name = (string)_dataModel.ReportData["$HospitalName"]
                                };

                                StringContent patDetContent = new StringContent(System.Text.Json.JsonSerializer.Serialize<PatientDetails>(patientDetails), System.Text.Encoding.UTF8, "application/json");
                                httpResult = httpClient.PostAsync($"{baseURL}/reports/patient-details", patDetContent);
                                httpResult.Wait();
                                httpResponseVal = httpResult.Result.Content.ReadAsStringAsync();
                                var result = System.Text.Json.JsonSerializer.Deserialize<patientAddResponse>(httpResponseVal.Result);

                                if (result.statusCode == 200)
                                {
                                    INTUSOFT.Configuration.IVLConfig config = INTUSOFT.Configuration.IVLConfig.getInstance();
                                    var accessKey = ConfigVariables.CurrentSettings.ReportSettings.AccessKeyID.val.ToString();
                                    var secretKey = ConfigVariables.CurrentSettings.ReportSettings.SecretAccessKey.val.ToString();
                                    var bucketName = ConfigVariables.CurrentSettings.ReportSettings.S3BucketName.val.ToString();

                                    for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
                                    {
                                        try
                                        {
                                            FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);
                                            BasicAWSCredentials basicCredentials = new BasicAWSCredentials(accessKey, secretKey);

                                            // Create a new Amazon S3 client
                                            AmazonS3Client s3Client = new AmazonS3Client(basicCredentials, Amazon.RegionEndpoint.APSouth1);

                                            TransferUtility fileTransferUtility = new TransferUtility(s3Client);
                                            if (_dataModel.CurrentImageNames[i].Contains("OS"))
                                            {
                                                if (finf != null)
                                                    fileTransferUtility.Upload(finf.FullName, bucketName, $"original_iv_{patientDetails.image_le_id}.{finf.Name.Split('.')[1]}");

                                            }
                                            else
                                            {
                                                if (finf != null)
                                                    fileTransferUtility.Upload(finf.FullName, bucketName, $"original_iv_{patientDetails.image_re_id}.{finf.Name.Split('.')[1]}");

                                            }
                                        }
                                        catch (AmazonS3Exception ex)
                                        {

                                            _EmailSent("Images uploaded failed", new EventArgs());
                                            CustomMessageBox.Show("Images uploaded failed");
                                            return;
                                        }
                                    }

                                    _EmailSent("Images uploaded successfully for Analysis", new EventArgs());
                                    CustomMessageBox.Show("Images uploaded successfully for Analysis");
                                    return;
                                }
                                else
                                {
                                    _EmailSent(result.body, new EventArgs());
                                    CustomMessageBox.Show(result.body);
                                    return;
                                }

                            }
                            else
                            {
                                _EmailSent("Images Upload Failed, Invalid Patient Details", new EventArgs());
                                CustomMessageBox.Show("Images Upload Failed, Invalid Patient Details");
                                return;
                            }
                            

                        }
                        else
                        {
                            _EmailSent("Images Upload Failed, Invalid Username or Password", new EventArgs());
                            CustomMessageBox.Show("Images Upload Failed, Invalid Username or Password");
                            return;
                        }



                    }
                    else if (vendorVal == "Vendor2")
                    {
                        credentials.Add("username", lebenUsername);
                        credentials.Add("password", lebenPassword);
                        credentials.Add("apiRequestType", "DIAB_RETINA");
                        url = "https://api.netra.ai/v1/getToken";

                        GetToken(credentials);
                        //System.Threading.Thread.Sleep(20000);

                    }
                    else if (vendorVal == "Vendor3")
                    {


                    }
                    else if (vendorVal == "Vendor4")
                    {
                        credentials.Add("email", AIUserName);
                        credentials.Add("password", AIPassword);

                        Dictionary<string, string> values = new Dictionary<string, string>();

                        values.Add("urlToken", "https://portal.swasteye.in/getToken");
                        values.Add("urlImageUpload", "https://portal.swasteye.in/sendImages");
                        values.Add("patientID", (string)_dataModel.ReportData["$MRN"]);
                        values.Add("patientName", (string)_dataModel.ReportData["$Name"]);
                        values.Add("doctorName", (string)_dataModel.ReportData["$Doctor"]);
                        values.Add("diabetestype", (string)_dataModel.ReportData["$Diabetestype"]);
                        for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
                        {
                            FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);

                            if (_dataModel.CurrentImageNames[i].Contains("OS"))
                                values.Add("leftImage", finf.FullName);
                            else
                                values.Add("rightImage", finf.FullName);

                        }
                        values.Add("emailID", (string)_dataModel.ReportData["$emailID"]);
                        values.Add("vendor", "Vendor4");
                        UploadImages.UploadImagesDetails(values, vendorVal, (string)_dataModel.ReportData["$userName"], (string)_dataModel.ReportData["$password"], "DIAB_RETINA");

                        //UploadImages.Upload(values);
                    }
                    else if (vendorVal == "Vendor6")
                    {
                        int leftEyeCnt = 0, rightEyeCnt = 0;
                            for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
                            {
                                FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);

                            if (_dataModel.CurrentImageNames[i].Contains("OS"))
                                leftEyeCnt++;
                            else
                                rightEyeCnt++;
                            }
                        if (leftEyeCnt > 1 || rightEyeCnt > 1)
                        {
                            _WaitCursor(new EventArgs(), true);
                            CustomMessageBox.Show("Select max of 1 right image and max of 1 left image");
                    
                            return;
                        }
                        
                        credentials.Add("email", AIUserName);
                        credentials.Add("password", AIPassword);

                        Dictionary<string, string> values = new Dictionary<string, string>();
                        int dob = DateTime.Now.Year -((DateTime)_dataModel.ReportData["$dob"]).Year;

                        values.Add("urlToken", "");
                        values.Add("urlImageUpload", ConfigVariables.CurrentSettings.ReportSettings.ApiRequestType.val);
                        values.Add("patientID", (string)_dataModel.ReportData["$MRN"]);
      
                        values.Add("firstName", (string)_dataModel.ReportData["$FirstName"]);
                        values.Add("lastName", (string)_dataModel.ReportData["$LastName"]);
                        values.Add("dob", dob.ToString());
                        values.Add("gender", (string)_dataModel.ReportData["$Gender"]);
                        

                        for (int i=0; i < _dataModel.CurrentImgFiles.Length; i++)
                        {
                            FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);

                            if (_dataModel.CurrentImageNames[i].Contains("OS"))
                                values.Add("leftImage", ApplyWhiteMaskToImage(finf.FullName, _dataModel.MaskSettingsArr[i], _dataModel.CurrentImageNames[i]));
                            else
                                values.Add("rightImage", ApplyWhiteMaskToImage(finf.FullName, _dataModel.MaskSettingsArr[i], _dataModel.CurrentImageNames[i]));
                        }
                        values.Add("emailID", (string)_dataModel.ReportData["$emailID"]);
                        values.Add("vendor", "Vendor6");
                        UploadImages.UploadImagesDetails(values, vendorVal, ConfigVariables.CurrentSettings.ReportSettings.UserName.val, ConfigVariables.CurrentSettings.ReportSettings.Password.val, "DIAB_RETINA");
                    }

                    //var releases = GetReleases(restSharpRequestHandler);
                    ////List out the retreived releases 

                    if (vendorVal == "Vendor1")
                    {
                        //var releases = Login(credentials, url);

                        //foreach (JProperty release in releases.Result.Children())
                        //{
                        //    if (count == 1)
                        //        token = release.Value.ToString();
                        //    count++;
                        //}
                        ////ImageDetails.Add("url", "http://chironapi.chironx.cloud/api/chironx/bulkupload/images");// old url for chironx
                        //ImageDetails.Add("url", "http://chironapp.chironx.cloud/api/chironx/upload/Data?q=Intuvision");// New url for bulk upload of images along with hardware id

                        //count = 0;

                        //releases = UploadFiles(ImageDetails, token, vendorVal);

                        //    foreach (JProperty release in releases.Result.Children())
                        //    {
                        //        if (count == 1)
                        //        {
                        //            CustomMessageBox.Show("Files Uploaded to cloud successfully", "Uploading to Cloud", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information);//.GetValue("name"));
                        //            _WaitCursor(new EventArgs(), true);
                        //        } count++;
                        //    }

                    }

                    else if (vendorVal == "Vendor2")
                    {
                        // List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                        Dictionary<string, string> values = new Dictionary<string, string>();
                        //values.Add(new KeyValuePair<string,string>("token",token));
                        //credentials.Add("username", lebenUsername);
                        //credentials.Add("password", lebenPassword);
                        //credentials.Add("apiRequestType", "DIAB_RETINA");
                        string dob = ((DateTime)_dataModel.ReportData["$dob"]).ToString("yyyyMMdd");

                        values.Add("url", "https://api.netra.ai/v1/doPatientMultiImgAnalysis");
                        values.Add("firstName", (string)_dataModel.ReportData["$FirstName"]);
                        values.Add("lastName", (string)_dataModel.ReportData["$LastName"]);
                        values.Add("dob", dob);
                        values.Add("gender", (string)_dataModel.ReportData["$Gender"]);
                        values.Add("patientID", (string)_dataModel.ReportData["$MRN"]);


                        //                  ImageData leftData = new ImageData();
                        //leftData.baseName = "leftImage.jpg";
                        //leftData.imgData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgLeftData")].Value;
                        //string imgLeftData = JsonConvert.SerializeObject(leftData) ;
                        //ImageData rightData = new ImageData();
                        //rightData.baseName = "rightImage.jpg";
                        //rightData.imgData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgRightData")].Value;
                        //string imgRightData = JsonConvert.SerializeObject(rightData);
                        values.Add("emailID", (string)_dataModel.ReportData["$emailID"]);
                        string phoneNumArr = (string)_dataModel.ReportData["$PhoneNumber"];

                        string phoneNumber = phoneNumArr.Substring(2);
                        string phoneCode = phoneNumArr.Substring(0, 2);
                        values.Add("phoneCode", phoneCode);
                        values.Add("phoneNumber", phoneNumber);
                        // uploadImage(values);

                        UploadImages.UploadImagesDetails(values, vendorVal, lebenUsername, lebenPassword, "DIAB_RETINA");
                        _WaitCursor(new EventArgs(), true);
                    }


                    else if (vendorVal == "Vendor3")
                    {
                        if (_dataModel.CurrentImgFiles.Length == 2)
                        {
                            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                            //values.Add(new KeyValuePair<string,string>("token",token));
                            values.Add(new KeyValuePair<string, string>("url", "http://netraservice.azurewebsites.net/uploadImages"));
                            values.Add(new KeyValuePair<string, string>("firstName", (string)_dataModel.ReportData["$FirstName"]));
                            values.Add(new KeyValuePair<string, string>("lastName", (string)_dataModel.ReportData["$LastName"]));
                            string dob = ((DateTime)_dataModel.ReportData["$dob"]).ToString("yyyyMMdd");
                            values.Add(new KeyValuePair<string, string>("dob", dob));
                            values.Add(new KeyValuePair<string, string>("gender", (string)_dataModel.ReportData["$Gender"]));
                            values.Add(new KeyValuePair<string, string>("patientID", (string)_dataModel.ReportData["$MRN"]));
                            if (string.IsNullOrEmpty((string)_dataModel.ReportData["$PhoneNumber"]) || String.IsNullOrEmpty((string)_dataModel.ReportData["$emailID"]))
                            {
                                CustomMessageBox.Show("Please check and fill mobile number and email ID field in the registration page.", "Warning", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Warning, 442, 135);
                                UploadImages__waitCursor();
                            }
                            else if (String.IsNullOrEmpty((string)_dataModel.ReportData["$DeviceID"]) || String.IsNullOrEmpty((string)_dataModel.ReportData["$hospitalName"]))
                            {
                                CustomMessageBox.Show("Please check and fill device ID, and hospital name field in the settings page.", "Warning", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Warning, 442, 135);
                                UploadImages__waitCursor();
                            }
                            else
                            {
                                values.Add(new KeyValuePair<string, string>("emailID", (string)_dataModel.ReportData["$emailID"]));

                                string phoneNumArr = (string)_dataModel.ReportData["$PhoneNumber"];

                                string phoneNumber = phoneNumArr.Substring(2);
                                string phoneCode = phoneNumArr.Substring(0, 2);
                                values.Add(new KeyValuePair<string, string>("phoneCode", phoneCode));
                                values.Add(new KeyValuePair<string, string>("phoneNumber", phoneNumber));
                                values.Add(new KeyValuePair<string, string>("hospitalName", (string)_dataModel.ReportData["$hospitalName"]));
                                values.Add(new KeyValuePair<string, string>("deviceID", (string)_dataModel.ReportData["$DeviceID"]));
                                // uploadImage(values);

                                UploadImages._WaitCursor += UploadImages__waitCursor;
                                //UploadImages.UploadImagesDetails(values, vendorVal, AIUserName, AIPassword, AIApiRequestType);
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Please select two  images and try to upload", "Warning", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Warning, 442, 135);
                            UploadImages__waitCursor();
                        }
                    }

                    else if (vendorVal == "Vendor4")
                    {

                    }

                    else if (vendorVal == "Vendor6")
                    {

                    }

                    else if(vendorVal == "Vendor5")
                    {
                       // File.WriteAllText(@"test.json", createJsonFile());
                        uploadReportRequest(createJsonFile());
                    }
                }
            }
            catch (Exception ex)
            {

                Common.ExceptionLogWriter.WriteLog(ex, Exception_Log);
            }
        }

        void UploadImages__waitCursor()
        {
            _WaitCursor(new EventArgs(), true);
        }

        public static String ConvertImageURLToBase64(String url)
        {
            StringBuilder _sb = new StringBuilder();

            Byte[] _byte = GetImage(url);

            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

            return _sb.ToString();
        }

        private static byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                //WebProxy myProxy = new WebProxy();
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                //HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                // Bitmap bm = new Bitmap(@"C:\Users\Sriram\Desktop\OS_010_20180316161747.jpg");
                buf = File.ReadAllBytes(@url);
                //bm.Save(stream,ImageFormat.Jpeg);

                //using (BinaryReader br = new BinaryReader(stream))
                //{
                //    int len = (int)(response.ContentLength);
                //    buf = br.ReadBytes(len);
                //    br.Close();
                //}

                //stream.Close();
                //response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }

        /// <summary>
        /// To add the attachment
        /// </summary>
        /// <param name="ReportFileName"></param>
        private void AddAttachment(string ReportFileName)
        {
            System.Net.Mail.Attachment attachment = null;// attachment of report file name

            var mailistJson = JsonConvert.SerializeObject(mailAddresses);
            System.IO.File.WriteAllText(fileJson, mailistJson);//writing mail list to email json.

            if (!string.IsNullOrEmpty(_dataModel.appDir))
                compressedData = _dataModel.appDir + compressedDataString;
            DirectoryInfo dirInf = new DirectoryInfo(compressedData);
            //the below ode has been added to delete the files in compressed data folder if files or directories exist.
            if (!dirInf.Exists)
            {
                //string[] files = Directory.GetDirectories(compressedData);
                //foreach (string item in files)
                //{
                //    Directory.Delete(item, true);
                //}
                //files = Directory.GetFiles(compressedData);
                //foreach (string item in files)
                //{
                //    File.Delete(item);
                //}
                dirInf.Create();


            }
            // else

            if (!string.IsNullOrEmpty(ReportFileName))
            {
                attachmentPath = ReportFileName;

            }
            else if (ImageFileNames != null)
            {

                string text = DateTime.Now.ToString(dateMonthYearFormat);
                text += reportText;
                dirInf = Directory.CreateDirectory(dirInf.FullName + Path.DirectorySeparatorChar + text);

                CreateAttachment(dirInf);
                attachmentPath = CreateZipFileOfAttachment(dirInf);
            }
            attachment = new System.Net.Mail.Attachment(attachmentPath);
            msg.Attachments.Add(attachment);
        }


        /// <summary>
        /// To create the zip file added by kishore on 19 September 2017.
        /// </summary>
        /// <param name="dirInf"></param>
        /// <returns>zip file path</returns>
        private string CreateZipFileOfAttachment(DirectoryInfo dirInf)
        {
            string zipFilePath = dirInf.Parent.FullName + Path.DirectorySeparatorChar + dirInf.Name + zipExtension;
            ZipFile.CreateFromDirectory(dirInf.FullName, zipFilePath);
            return zipFilePath;
        }


        /// <summary>
        /// To create the attachment file
        /// </summary>
        /// <param name="dirInf"></param>
        private void CreateAttachment(DirectoryInfo dirInf)
        {
            AddObservationFile2Attachment(dirInf);

            AddEmailData2Attachment(dirInf);

            AddImages2Attachment(dirInf);

            AddPatientDetails2Attachment(dirInf);

            AddBatchFile2Attachment(dirInf);

            AddReportTemplate2Attachment(dirInf);
        }


        /// <summary>
        /// To add observation file to the attacchment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddObservationFile2Attachment(DirectoryInfo dirInf)
        {
            StreamWriter st = new StreamWriter(@dirInf.FullName + Path.DirectorySeparatorChar + observationText);
            st.WriteLine(rightEyeObservationText);
            st.WriteLine(observationSpacing);
            st.WriteLine(observationSpacing);
            st.WriteLine(leftEyeObservationText);
            st.WriteLine(observationSpacing);
            st.WriteLine(observationSpacing);
            st.WriteLine(generalObservationText);
            st.WriteLine(observationSpacing);
            st.WriteLine(observationSpacing);
            st.Flush();

            st.Close();
            st.Dispose();
        }


        /// <summary>
        /// to add email data into attachment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddEmailData2Attachment(DirectoryInfo dirInf)
        {
            var emailDataObj = JsonConvert.SerializeObject(_dataModel.mailData);
            System.IO.File.WriteAllText(@dirInf.FullName + Path.DirectorySeparatorChar + emailJsonData, emailDataObj);
        }


        /// <summary>
        /// To add settings of the images to the attachment folder.
        /// </summary>
        private void AddImageSettings2PatientDetails()
        {
            List<string> ImageSettingsList = new List<string>();
            List<string> ImageMaskSettingsList = new List<string>();// new List<INTUSOFT.Imaging.CaptureLog>();

            for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
            {
                string cameraSettings = "";
                INTUSOFT.Imaging.CaptureLog data1 = new INTUSOFT.Imaging.CaptureLog();
                if (!string.IsNullOrEmpty(_dataModel.CurrentImageCameraSettings[i]))
                {
                    cameraSettings = _dataModel.CurrentImageCameraSettings[i];
                }
                else
                {
                    INTUSOFT.Configuration.IVLConfig config = INTUSOFT.Configuration.IVLConfig.getInstance();
                    data1.currentCameraType = (INTUSOFT.Imaging.ImagingMode)Enum.Parse(typeof(INTUSOFT.Configuration.ImagingMode), config.Mode.ToString());
                    //INTUSOFT.Configuration.XmlConfigUtility.Serialize(data1, "@cameraSettings.xml");
                    //cameraSettings = File.ReadAllText("@cameraSettings.xml");
                    //File.Delete("@cameraSettings.xml");
                    cameraSettings = JsonConvert.SerializeObject(data1);
                }
                ImageSettingsList.Add(cameraSettings);


            }
            for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
            {

                INTUSOFT.Imaging.MaskSettings data = null;//varaible data of type MaskSettings(in Reports.cs) and is set to null.By Ashutosh 22-08-2017.
                string maskSettings = "";
                if (!string.IsNullOrEmpty(_dataModel.CurrentImageMaskSettings[i]))
                    maskSettings = _dataModel.CurrentImageMaskSettings[i];
                else
                {
                    data = new INTUSOFT.Imaging.MaskSettings();
                    data.maskHeight = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.PostProcessingSettings.MaskSettings.CaptureMaskHeight.val);
                    data.maskWidth = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.PostProcessingSettings.MaskSettings.CaptureMaskWidth.val);
                    data.maskCentreX = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.CameraSettings._ImageOpticalCentreX.val);
                    data.maskCentreY = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.CameraSettings._ImageOpticalCentreY.val);
                    //INTUSOFT.Configuration.XmlConfigUtility.Serialize(data, "@maskSettings.xml");
                    //maskSettings = File.ReadAllText("@maskSettings.xml");
                    //File.Delete("@maskSettings.xml");
                    maskSettings = JsonConvert.SerializeObject(data);
                }
                ImageMaskSettingsList.Add(maskSettings);

            }
            _dataModel.patientDetails.CameraSettings = ImageSettingsList;
            _dataModel.patientDetails.MaskSettings = ImageMaskSettingsList;
        }


        /// <summary>
        /// to add images to the attachment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddImages2Attachment(DirectoryInfo dirInf)
        {
            int indx = 0;

            if (ImageFileNames != null)
                for (int i = 0; i < ImageFileNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ImageFileNames[i]))
                    {
                        FileInfo finf = new FileInfo(ImageFileNames[i]); // to get  file name
                        string[] seperator = finf.Name.Split('.');
                        string[] ImageNameArr = _dataModel.CurrentImageNames[i].Split(new char[0]);
                        string destFilePath = dirInf.FullName + Path.DirectorySeparatorChar + ImageNameArr[0] + underScoreCharacter + ImageNameArr[2] + dotCharacter + seperator[seperator.Length - 1]; // create destination file path using the new directory created for zip
                        string srcFilePath = ImageFileNames[i]; // to get source file path
                        if (!File.Exists(destFilePath) && File.Exists(srcFilePath)) // check if file exists in source path and file doesn't exist in destination
                            File.Copy(srcFilePath, destFilePath);// copy file from source to destination
                        FileInfo finf1 = new FileInfo(destFilePath);
                        _dataModel.patientDetails.observationPaths[i] = finf1.Name;
                    }
                    indx++;
                }
            string logoDestPath = dirInf.FullName + Path.DirectorySeparatorChar + "hospitalLogo.png";
            string logoPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + "ImageResources" + Path.DirectorySeparatorChar + "LogoImageResources" + Path.DirectorySeparatorChar + "hospitalLogo.png";
            if (File.Exists(logoPath))
                File.Copy(logoPath, logoDestPath);
        }


        /// <summary>
        /// To add batch file to the attachment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddBatchFile2Attachment(DirectoryInfo dirInf)
        {
            string destinationBatFilePath = @dirInf.FullName + Path.DirectorySeparatorChar + sourceBatFile;// the extention is ba in order to avoid blocking of bat file has to be renamed when it is extractedar
            if (!File.Exists(destinationBatFilePath) && File.Exists(sourceBatFilePath))
            {
                File.Copy(sourceBatFilePath, destinationBatFilePath);// copy file from source to destination
            }
        }


        /// <summary>
        /// to add patient details to the attachment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddPatientDetails2Attachment(DirectoryInfo dirInf)
        {
            AddImageSettings2PatientDetails();

            _dataModel.patientDetails.ImageNames = _dataModel.CurrentImageNames.ToList();
            _dataModel.patientDetails.ReporteeName = _dataModel._operator;
            _dataModel.patientDetails.VisitDateTime = _dataModel.visitDateTime;

            var json = JsonConvert.SerializeObject(_dataModel.patientDetails);
            jsonPatientDetails = json.ToString();
            System.IO.File.WriteAllText(@dirInf.FullName + Path.DirectorySeparatorChar + patientDetailsJson, jsonPatientDetails);
        }


        /// <summary>
        /// to add report template xml file to the attachment folder.
        /// </summary>
        /// <param name="dirInf"></param>
        private void AddReportTemplate2Attachment(DirectoryInfo dirInf)
        {
            string sourceTemplatePath = _dataModel.CurrentTemplate;
            string destinationTemplatePath = @dirInf.FullName + Path.DirectorySeparatorChar + templateXmlText;
            if (!File.Exists(destinationTemplatePath) && File.Exists(sourceTemplatePath)) // check if file exists in source path and file doesn't exist in destination
                File.Copy(sourceTemplatePath, destinationTemplatePath);// copy file from source to destination
        }


        /// <summary>
        /// To validate the email ids
        /// </summary>
        private bool ValidateEmailData()
        {
            Array mailMessageArray = Enum.GetValues(typeof(MailMessageEnum)); //to get the mailmessageenum in the array
            for (int i = 0; i < mailMessageArray.Length; i++)
            {
                MailMessageEnum mailType = ((MailMessageEnum)mailMessageArray.GetValue(i));
                string emailData = "";
                bool isMandatoryEmail = false;
                string mailTypeStr = mailType.ToString();
                string output = "";
                foreach (char letter in mailTypeStr)//to add a space in the enum datas whenever a upper case letter is found in the data
                {
                    if (Char.IsUpper(letter) && output.Length > 0)
                        output += " " + letter;
                    else
                        output += letter;
                    mailTypeStr = output;
                }
                mailTypeStr = "'In Email " + mailTypeStr + "' ";
                switch (mailType)
                {
                    case MailMessageEnum.From:
                        {
                            emailData = _dataModel.mailData.EmailReplyTo;
                            isMandatoryEmail = true;
                            break;
                        }
                    case MailMessageEnum.CC:
                        {
                            emailData = _dataModel.mailData.EmailCC;
                            isMandatoryEmail = false;
                            break;
                        }

                    case MailMessageEnum.BCC:
                        {
                            emailData = _dataModel.mailData.EmailBCC;
                            isMandatoryEmail = false;
                            break;
                        }
                    case MailMessageEnum.To:
                        {
                            emailData = _dataModel.mailData.EmailTo;
                            isMandatoryEmail = true;

                            break;
                        }
                }
                if (!string.IsNullOrEmpty(emailData))
                {
                    if (emailData.Contains(semiColonCharacter))//checks for the presence of ;
                    {
                        List<string> emailList = SplitMailIds(emailData);// splitEmail.ToList();
                        emailList.RemoveAt(emailList.Count - 1);
                        mailMessageEnum = mailType;
                        MailErrorData mailErrData = AddMailIDToMsg(emailList);

                        if (!mailErrData.isValidMail)
                        {
                            int index = (int)(mailErrData.mailErrorCode); //to get the index of the mailErrorCode enum
                            CustomMessageBox.Show(mailTypeStr + "  " + mailErrData.MailErrorMessage[index]);
                            return false;
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show(mailTypeStr + " " + mailIdEndWithSemicolon);
                        return false;
                    }
                }
                else
                {
                    if (isMandatoryEmail)
                    {
                        CustomMessageBox.Show(mailTypeStr + " " + mailIdEmpty);
                        return false;
                    }
                }
            }
            this.Cursor = Cursors.Default;
            return true;
        }




        private MailErrorData AddMailIDToMsg(List<string> emailList)
        {
            MailErrorData mailErrorData = new MailErrorData();
            for (int i = 0; i < emailList.Count; i++)
            {
                mailErrorData = emailValidiation.IsValidEmail(emailList[i]);
                if (mailErrorData.isValidMail)
                {
                    switch (mailMessageEnum)
                    {

                        case MailMessageEnum.From:
                            {
                                msg.ReplyToList.Add(new MailAddress(emailList[i]));
                                break;
                            }
                        case MailMessageEnum.To:
                            {
                                msg.To.Add(emailList[i]);
                                break;
                            }
                        case MailMessageEnum.CC:
                            {
                                msg.CC.Add(emailList[i]);
                                break;
                            }

                        case MailMessageEnum.BCC:
                            {
                                msg.Bcc.Add(emailList[i]);
                                break;
                            }

                    }
                    if (!mailAddresses.mailAddressList.Contains(emailList[i]))//checks whether the given to address is not there in the list box
                        mailAddresses.mailAddressList.Add(emailList[i]);//add the given to address to the list box
                }
                else
                    break;
            }
            this.Cursor = Cursors.Default;
            return mailErrorData;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            addText(mailAddress_lbx.Text + semiColonCharacter, currentTextBox);
        }

        private void toaddr_lbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addText(mailAddress_lbx.Text + semiColonCharacter, currentTextBox);
        }

        private void Cc_tbx_Enter(object sender, EventArgs e)
        {
            currentTextBox = sender as RichTextBox;
        }

        private void Bcc_tbx_Enter(object sender, EventArgs e)
        {
            currentTextBox = sender as RichTextBox;
        }

        private void to_tbx_Enter(object sender, EventArgs e)
        {
            currentTextBox = sender as RichTextBox;
        }

        private void to_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressEventOfTextBox(sender as RichTextBox, e);
        }

        private void Cc_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressEventOfTextBox(sender as RichTextBox, e);
        }

        private void Bcc_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressEventOfTextBox(sender as RichTextBox, e);
        }

        private void HandleKeyPressEventOfTextBox(RichTextBox textbox, KeyPressEventArgs e)
        {
            Keys k = (Keys)(e.KeyChar);
            switch (k)
            {
                case Keys.Enter:
                    {
                        e.Handled = true;
                        addText(mailAddress_lbx.Text + semiColonCharacter, textbox);
                        textbox.SelectionStart = textbox.Text.Length;
                        break;
                    }
                case Keys.Space:
                    {
                        e.Handled = true;
                        textbox.Text += semiColonCharacter;
                        textbox.SelectionStart = textbox.Text.Length;
                        break;
                    }
            }
            getSearchStringFromtbx(textbox, e);
        }
        #endregion
        #region private user defined methods

        private void CreateZipFile()
        {
        }

        private void addText(string text, RichTextBox t)
        {

            if (!string.IsNullOrEmpty(t.Text))//checks whether the string is not null or empty
            {
                string[] splitStr = t.Text.Split(semiColonCharacter);
                string validateString = splitStr[splitStr.Length - 1];

                if (emailValidiation.IsValidEmail(validateString).isValidMail)
                {
                    if (t.Text.Substring(t.Text.Length - 1, 1) == ";")
                        t.Text += text;
                    else
                        t.Text += (semiColonCharacter + text);
                }
                else
                {
                    t.Text = "";// reset textbox
                    splitStr[splitStr.Length - 1] = text; // replace last array element with the listbox email
                    t.Text = string.Join(";", splitStr); ;// merge string to the textbox   
                }
            }
            else
                t.Text = text;
        }

        private void getSearchStringFromtbx(RichTextBox t, KeyPressEventArgs e)
        {
            mailAddress_lbx.BringToFront();
            mailAddress_lbx.ClearSelected();
            string[] toEmailsArr = t.Text.Split(semiColonCharacter);

            string keyCharString = null;
            if (char.IsControl(e.KeyChar))
                e.Handled = true;
            else
                keyCharString = e.KeyChar.ToString();

            string searchString2 = string.Empty;

            string lastStr = toEmailsArr[toEmailsArr.Length - 1]; // get the last string from the text box 
            if (string.IsNullOrEmpty(lastStr))// check if the last string is null or empty so the search string will be the key char pressed from the key board provided it is not a control key
            {
                if (!string.IsNullOrEmpty(keyCharString))// check keycharstring is not null which means its not an control key
                    searchString2 = keyCharString; // search string will be the key char pressed from the keyboard
            }
            else if (lastStr[lastStr.Length - 1] != semiColonCharacter)// last char of the last string is not ";"
            {
                if (!string.IsNullOrEmpty(keyCharString))// check keycharstring is not null which means its not an control key
                    searchString2 = lastStr.Trim(new char[0]) + keyCharString; // search string will be the last string + key char pressed from the keyboard
                else
                    searchString2 = lastStr.Trim(new char[0]);// else the search string will be the last string 
            }
            if (string.IsNullOrEmpty(searchString2))// if search string is empty hide the list box of mail ids
                mailAddress_lbx.Visible = false;
            else
                FindMyStringInList(mailAddress_lbx, searchString2);// get the actual mail id from the search string and highlight it in the list box
        }

        /// <summary>
        /// to search the string entered in to textbox is already in the listbox 
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        int FindMyStringInList(ListBox lb, string searchString)
        {
            for (int i = 0; i < lb.Items.Count; ++i)
            {
                string lbString = lb.Items[i].ToString();
                if (lbString.ToLower().Contains(searchString.ToLower()))//checks the searchstring is there or not
                {
                    mailAddress_lbx.Visible = true;
                    mailAddress_lbx.Text = lb.Items[i].ToString();

                    return i;
                }
                else
                    mailAddress_lbx.Visible = false;
            }
            return -1;
        }

        private List<string> SplitMailIds(string email_IDs)
        {
            List<string> splitStr = email_IDs.Split(semiColonCharacter).ToList();
            return splitStr;
        }

        private void testAddListboxItems_btn_Click(object sender, EventArgs e)
        {
            toaddr = to_tbx.Text;
            //IsValidEmail(toaddr);
        }

        private void from_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                e.Handled = true;
            getSearchStringFromtbx(sender as RichTextBox, e);
        }

        private void replyTo_tbx_Enter(object sender, EventArgs e)
        {
            currentTextBox = sender as RichTextBox;
        }

        private void replyTo_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressEventOfTextBox(sender as RichTextBox, e);
        }
        #endregion



        public async Task<JToken> Login(Dictionary<string, string> credentials, string URL)
        {
            string responseMsg = null;
            if (vendorVal == "Vendor1")
            {
                HttpClient p = new HttpClient();
                //new System.Uri("http://chironapi.chironx.cloud/api/chironx/authenticate/login")
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new System.Uri(URL));
                var values = new List<KeyValuePair<string, string>>();

                foreach (KeyValuePair<string, string> item in credentials)
                {
                    values.Add(item);
                }
                //values.Add(new KeyValuePair<string, string>("email", "sriram@intuvisionlabs.com"));
                //values.Add(new KeyValuePair<string, string>("hashedPassword", "intuvision123"));

                request.Content = new FormUrlEncodedContent(values);
                HttpResponseMessage response = await p.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    responseMsg = await response.Content.ReadAsStringAsync();
                }
            }
            return (JToken)JsonConvert.DeserializeObject(responseMsg);


        }

        public async Task<JToken> UploadFiles(Dictionary<string, string> details, string token, int vendorName)
        {
            HttpClient p = new HttpClient();
            string responseMsg = string.Empty;
            // HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new System.Uri("http://chironapi.chironx.cloud/api/chironx/bulkupload/images"));

            HttpResponseMessage response = new HttpResponseMessage();
            //values.Add(new KeyValuePair<string, string>("email", "sriram@intuvisionlabs.com"));
            //values.Add(new KeyValuePair<string, string>("hashedPassword", "intuvision123"));
            // request.Headers.Add("Authorization","Bearer " + token);
            if (vendorName == 1)
            {
                p.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("images");
                form.Add(content, "images");
                var stream = File.OpenRead(details["filePath"]);
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "images",
                    FileName = details["filePath"]
                };
                form.Add(content);
                try
                {
                    form.Add(new StringContent(_dataModel.DeviceID), "HardwareId");

                }
                catch (Exception)
                {

                    throw;
                }
                response = await p.PostAsync(details["url"], form);
                //response.Content.ReadAsStringAsync().Result;
            }

            else if (vendorName == 2)
            {

                ///* Check the file actually has some content to display to the user */
                //byte[] bytes = File.ReadAllBytes(details["filePath"]);
                //string file = "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
                //System.Diagnostics.Trace.WriteLine(file);
                //System.Diagnostics.Trace.WriteLine(details["url"]);

                //p.DefaultRequestHeaders.Add("token", token);
                //p.DefaultRequestHeaders.Add("image", file);
                //p.DefaultRequestHeaders.Add("imageName", details["imageName"]);
                //response = await p.PostAsync(details["url"], new StringContent(""));
            }

            if (response.IsSuccessStatusCode)
            {
                responseMsg = await response.Content.ReadAsStringAsync();
            }
            return (JToken)JsonConvert.DeserializeObject(responseMsg);
        }
        JToken result;
        public async void GetToken(Dictionary<string, string> credentials)
        {

            //List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            //values.Add(new KeyValuePair<string, string>("username", "anil@intuvisionlabs.com"));
            //values.Add(new KeyValuePair<string, string>("password", "Gt#8$DkMy"));
            //values.Add(new KeyValuePair<string, string>("apiRequestType", "DIAB_RETINA"));
            result = await PostFormUrlEncoded("https://api.netra.ai/v1/getToken", credentials);
            Console.WriteLine(result);
            int count = 0;
            foreach (JProperty release in result)
            {
                if (count == 1)
                    token = release.Value.ToString();
                count++;
            }
        }

        public async Task<JToken> PostFormUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");   

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    string resultResponse = string.Empty;
                    resultResponse = await response.Content.ReadAsStringAsync();
                    return (JToken)JsonConvert.DeserializeObject(resultResponse);
                }

            }
        }

        void uploadImage(List<KeyValuePair<string, string>> uploadValues)
        {
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.netra.ai/v1/doAnalysis");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.netra.ai/v1/doPatientAnalysis");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            // string requestBody = string.Format("{{\"token\":\"{0}\",\"image\":\"{1}\",\"imageName\":\"{2}\"}}", token, base64, "DIAB_RETINA");
            string requestBody = "{";

            for (int i = 0; i < uploadValues.Count; i++)
            {
                string iterValue2UploadStr = "\"" + uploadValues[i].Key + "\"" + ":" + "\"" + uploadValues[i].Value + "\"";
                if (i < uploadValues.Count - 1)
                    iterValue2UploadStr += ",";
                requestBody += iterValue2UploadStr;
            }
            requestBody += "}";
            string token = uploadValues[uploadValues.FindIndex(x => x.Key == "token")].Value;
            string firstName = uploadValues[uploadValues.FindIndex(x => x.Key == "firstName")].Value;
            string lastName = uploadValues[uploadValues.FindIndex(x => x.Key == "lastName")].Value;
            string dob = uploadValues[uploadValues.FindIndex(x => x.Key == "dob")].Value;
            string gender = uploadValues[uploadValues.FindIndex(x => x.Key == "gender")].Value;
            string patientID = uploadValues[uploadValues.FindIndex(x => x.Key == "patientID")].Value;

            //ImageData leftData = new ImageData();
            //leftData.baseName = "leftImage.jpg";
            //leftData.imgData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgLeftData")].Value;
            string imgLeftData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgLeftData")].Value;
            //ImageData rightData = new ImageData();
            //rightData.baseName = "rightImage.jpg";
            //rightData.imgData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgRightData")].Value;
            string imgRightData = uploadValues[uploadValues.FindIndex(x => x.Key == "imgRightData")].Value;
            string emailID = uploadValues[uploadValues.FindIndex(x => x.Key == "emailID")].Value;
            string phoneCode = uploadValues[uploadValues.FindIndex(x => x.Key == "phoneCode")].Value;
            string phoneNumber = uploadValues[uploadValues.FindIndex(x => x.Key == "phoneNumber")].Value;

            string requestBody1 = "{\"token\":\"" + token + "\", \"firstName\":\"" + firstName + "\", \"lastName\":\"" + lastName + "\", \"dob\":\"" + dob + "\", \"gender\":\"" + gender + "\", \"patientID\":\"" + patientID + "\", \"emailID\":\"" + emailID + "\", \"phoneCode\":\"" + phoneCode + "\", \"phoneNumber\":\"" + phoneNumber + "\", \"imgLeftData\": \"data:image/jpeg;base64," + imgLeftData + "\", \"imgLeftName\":\"" + uploadValues[uploadValues.FindIndex(x => x.Key == "imgLeftName")].Value + "\", \"imgRightData\": \"data:image/jpeg;base64," + imgRightData + "\", \"imgRightName\":\"" + uploadValues[uploadValues.FindIndex(x => x.Key == "imgRightName")].Value + "\"}";

            //string requestBody1 = string.Format("{{\"token\":\"{0}\",\"firstName\":\"{1}\",\"lastName\":\"{2}\",\"dob\":\"{3}\",\"gender\":\"{4}\",\"patientID\":\"{5}\",\"imgLeftData\":\"{6}\",\"imgRightData\":\"{7}\",\"emailID\":\"{8}\",\"phoneCode\":\"{9}\",\"phoneNumber\":\"{10}\"}}",
            //    token, firstName, lastName, dob, gender, patientID, imgLeftData, imgRightData, emailID, phoneCode, phoneNumber);// base64, "DIAB_RETINA");
            using (var streamWriter = (new StreamWriter(httpWebRequest.GetRequestStream())))
            {
                streamWriter.Write(requestBody1);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    this.UseWaitCursor = false;
                    if (result.Contains("200"))
                        CustomMessageBox.Show("Files Uploaded to cloud successfully", "Uploading to Cloud", CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information);//.GetValue("name"));

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Dictionary<string, object> getPatientInfo()
        {
            Dictionary<string, object> jDict = new Dictionary<string, object>();
            jDict.Add("OrganaizationId", INTUSOFT.Configuration.ConfigVariables.CurrentSettings.ReportSettings.ImagingCenterId.val);//from settings
            jDict.Add("OperatorId", INTUSOFT.Configuration.ConfigVariables.CurrentSettings.ReportSettings.UserName.val);//from settings
            jDict.Add("PatientId", $"{_dataModel.ReportData["$MRN"]}");
            jDict.Add("VisitId", (NewDataVariables.GetCurrentPat().visits.ToList().IndexOf(NewDataVariables.Active_Visit) + 1).ToString());
            jDict.Add("VisitDate", NewDataVariables.Active_Visit.createdDate.ToString("dd-MM-yyyy HH:mm:ss"));
            jDict.Add("Address1", INTUSOFT.Configuration.ConfigVariables.CurrentSettings.ReportSettings.Address1.val);
            jDict.Add("Address2", INTUSOFT.Configuration.ConfigVariables.CurrentSettings.ReportSettings.Address2.val);
            jDict.Add("PatientName", $"{_dataModel.ReportData["$Name"]}");
            jDict.Add("Age", $"{_dataModel.ReportData["$Age"]}");
            jDict.Add("Gender", $"{_dataModel.ReportData["$Gender"]}");
            jDict.Add("Phone", $"{_dataModel.ReportData["$PhoneNumber"]}");
            jDict.Add("ClinicalHistory", string.Empty);// NewDataVariables.Active_Visit.medicalHistory.GetMedicalHistory());
            jDict.Add("DoctorName", $"{_dataModel.ReportData["$Doctor"]}");
            jDict.Add("ReportTitle", $"{_dataModel.ReportData["$NameOfTheReport"]}");
            jDict.Add("HospitalName", $"{_dataModel.ReportData["$HospitalName"]}");
            jDict.Add("ScreeningCenter", INTUSOFT.Configuration.ConfigVariables.CurrentSettings.ReportSettings.ImagingCenter.val);//from settings

            //more fields can be added if required

            return jDict;
        }
        private string createJsonFile()
        {

            for (int i = 0; i < _dataModel.CurrentImgFiles.Length; i++)
            {
                ImageInfo info = new ImageInfo();
                info.Id = "image" + (i + 1);
                FileInfo finf = new FileInfo(_dataModel.CurrentImgFiles[i]);
                info.ImageData = ApplyWhiteMaskToImage(finf.FullName, _dataModel.MaskSettingsArr[i], _dataModel.CurrentImageNames[i]);
                var imageMode = string.Empty;
                var camSettings = JsonConvert.DeserializeObject<CaptureLog>(_dataModel.CurrentImageCameraSettings[i]);
                if (camSettings.currentCameraType != INTUSOFT.Imaging.ImagingMode.Anterior_FFA &&
                    camSettings.currentCameraType != INTUSOFT.Imaging.ImagingMode.Anterior_45 &&
                     camSettings.currentCameraType != INTUSOFT.Imaging.ImagingMode.Anterior_Prime   )
                    imageMode = "posterior";
                else 
                    imageMode = "anterior";
                    if (_dataModel.CurrentImageNames[i].Contains("OS"))
                    info.Metadata = $"left image, {imageMode}";
                else
                    info.Metadata = $"right image, {imageMode}";
                Payload.Add(info);

            }

            var patientData = getPatientInfo();
            patientData.Add("Payload", Payload);

            return JsonConvert.SerializeObject(patientData, Newtonsoft.Json.Formatting.Indented);

        }
        private string ApplyWhiteMaskToImage(string fileName, string maskSettings, string imageName)
        {
            Bitmap bm = null;
            LoadSaveImage.LoadImage(fileName, ref bm);

            Bitmap tempBm = new Bitmap(bm.Width, bm.Height, bm.PixelFormat);
            MaskSettings data = new MaskSettings();
            try
            {
                if (!string.IsNullOrEmpty(maskSettings))
                    data = (MaskSettings)Newtonsoft.Json.JsonConvert.DeserializeObject(maskSettings, typeof(MaskSettings));
                else data = new MaskSettings()
                {
                    maskHeight = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.PostProcessingSettings.MaskSettings.CaptureMaskHeight.val),
                    maskWidth = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.PostProcessingSettings.MaskSettings.CaptureMaskWidth.val),
                    maskCentreX = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.CameraSettings._ImageOpticalCentreX.val),
                    maskCentreY = Convert.ToInt32(INTUSOFT.Configuration.ConfigVariables.CurrentSettings.CameraSettings._ImageOpticalCentreY.val),
                };
            }
            catch (Exception)
            {

                using (StringReader sr = new StringReader(maskSettings))//StringReader-Initializes a new instance of the System.IO.StringReader class that reads from the specified string(maskSettings).
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    using (XmlReader xmlReader = XmlReader.Create(sr, settings))//sr-from which XML data is read.settings- object used to configure.
                    {
                        XmlSerializer xmlSer = new XmlSerializer(typeof(INTUSOFT.Imaging.MaskSettings));
                        var data1 = (INTUSOFT.Imaging.MaskSettings)xmlSer.Deserialize(xmlReader);
                        data = new MaskSettings { maskCentreX = data1.maskCentreX, maskCentreY = data1.maskCentreY, maskHeight = data1.maskHeight, maskWidth = data1.maskWidth };
                    }
                }
            }

            Color maskBgColor = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), _dataModel.ReportData["$ChangeMaskColour"] as string));//colour chosen by user in string form converted to enum object and given to maskBgColor.By Ashutosh 22-08-2017
            PostProcessing.GetInstance().ApplyColorMask(ref bm, data, maskBgColor);
            PostProcessing.GetInstance().ApplyLogo(ref bm,imageName, maskBgColor, imageName.Contains("OS") ? LeftRightPosition.Left : LeftRightPosition.Right);//passing arguments to ApplyLogo for application of logo suitable to mask colour. By Ashutosh 29-08-2017.
            return getBase64String(bm);
        }

        private string getBase64String(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            var base64Str = Convert.ToBase64String(byteImage);
            ms.Dispose();
            bmp.Dispose();
            return base64Str;
        }
    }

    public class PatientDetails
    {
        public string doc_name { get; set; }
        public string clinic_name { get; set; }

        public string patient_fname { get; set;   }
        public string patient_mname { get; set; }
        public string patient_lname { get; set; }
        public string patient_id { get; set; }
        public string gender { get; set; }
        public int patient_age { get; set; }
        public string image_re_name { get; set; }= "intuvision_re.jpeg";
        public string image_le_name { get; set; } = "intuvision_le.jpeg";
        public string image_le_id { get; set; }
        public string image_re_id { get; set; }
        public string doc_email { get; set; }
    }

    public class LoginCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class TokenResponse
    {
        public int statusCode { get; set; }
        public string access_token { get; set; }
    }
    public class patientAddResponse
    {
        public int statusCode { get; set; }
        public string body { get; set; }
    }

    public class ImageInfo
    {
        public string Id;
        public string Metadata;
        public string ImageData;
    }
    public class ResponseDTO
    {
        public int StatusCode { get; set; }
        public string message { get; set; }
    }
}

