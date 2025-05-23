﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Common
{
    public partial class CustomMessageBox : Form
    {
        public delegate void ShiftControl(Dictionary<string, object> dic);
        public event ShiftControl shiftControlEvent;        
       public static string yesBtnText = string.Empty;
       public static string cancelBtnText = string.Empty;
       public static string okBtnText = string.Empty;
       public static string noBtnText = string.Empty;
       public static string restartNowBtnText = string.Empty;
       public static string restartLaterBtnText = string.Empty;


        public CustomMessageBox()
        {
            InitializeComponent();
            string appLogoFilePath = @"ImageResources\LogoImageResources\IntuSoft.ico";

            if (File.Exists(appLogoFilePath))
                this.Icon = new System.Drawing.Icon(appLogoFilePath, 256, 256);
            //#if DEBUG//has been commented to prevent the custom message box getting background.

//            this.TopMost = false;
//#else
//            this.TopMost = true;

//#endif
//            //this.ControlBox = false;

            this.KeyDown += CustomMessageBox_KeyDown;
            this.KeyUp += CustomMessageBox_KeyUp;
            this.KeyPreview = true;

        }


        void CustomMessageBox_KeyUp(object sender, KeyEventArgs e)
        {
            {
               // if(arg.ContainsKey("keyArg")
                if (e.KeyData == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("keyArg", e);
                    dic.Add("isKeyUp", true);
                    if(shiftControlEvent != null)//added for the defect no 0001677
                        shiftControlEvent(dic);
                    //arg["keyArg"] = e;
                    //arg["isKeyUp"] = true;
                    //_eventHandler.Notify(_eventHandler.IsShiftAndControl, arg);
                }
            }
        }

        void CustomMessageBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("keyArg", e);
                dic.Add("isKeyUp", false);
                if (shiftControlEvent != null)//added for the defect no 0001677
                    shiftControlEvent(dic);
                //arg["keyArg"] = e;
                //arg["isKeyUp"] = false;
                //_eventHandler.Notify(_eventHandler.IsShiftAndControl, arg);
            }
        }

        


        /// <summary>
        /// We can use this method to add image on message box.
        /// I had taken all images in ImageList control so that
        /// I can eaily add images. Image is displayed in 
        /// PictureBox control.
        /// </summary>
        /// <param name="MessageIcon">Type of image to be displayed.</param>
        private void addIconImage(CustomMessageBoxIcon MessageIcon)
        {
            switch (MessageIcon)
            {
                case CustomMessageBoxIcon.Error:
                    pictureBox1.Image = Bitmap.FromHicon(new Icon(SystemIcons.Error ,48, 48).Handle);  //Error is key name in imagelist control which uniqly identified images in ImageList control.
                    break;
                case CustomMessageBoxIcon.Information:
                    //pictureBox1.Image = imageList1.Images["Information"];
                    pictureBox1.Image = Bitmap.FromHicon(new Icon(SystemIcons.Information ,48, 48).Handle);//.Handle)  SystemIcons.Information;
                    
                    break;
                case CustomMessageBoxIcon.Question:
                    pictureBox1.Image = Bitmap.FromHicon(new Icon(SystemIcons.Question, 48, 48).Handle);
                    break;
                case CustomMessageBoxIcon.Warning:
                    pictureBox1.Image = Bitmap.FromHicon(new Icon(SystemIcons.Warning, 48, 48).Handle);
                    break;
            }
        }

        /// <summary>
        /// setMessage method is used to display message
        /// on form and it's height adjust automatically.
        /// I am displaying message in a Label control.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        private void setMessage(string messageText)
        {
            //int number = Math.Abs(messageText.Length / 30);
            //if (number != 0)
            //    this.lblMessageText.Height = number * 25;
            this.lblMessageText.Text = messageText;
        }

        /// <summary>
        /// This method is used to add button on message box.
        /// </summary>
        /// <param name="MessageButton">MessageButton is type of enumMessageButton
        /// through which I get type of button which needs to be displayed.</param>
        private void addButton(CustomMessageBoxButtons MessageButton)
        {
            switch (MessageButton)
            {
                case CustomMessageBoxButtons.OK:
                    {
                        //If type of enumButton is OK then we add OK button only.
                        Button btnOk = new Button();  //Create object of Button.
                       if(string.IsNullOrEmpty(okBtnText))
                        btnOk.Text = "OK"; 
                       else
                           btnOk.Text = okBtnText; 
                           //Here we set text of Button.
                        btnOk.DialogResult = DialogResult.OK;  //Set DialogResult property of button.
                        //btnOk.FlatStyle = FlatStyle.Popup;  //Set flat appearence of button.
                        btnOk.FlatAppearance.BorderSize = 0;
                        //btnOk.SetBounds(pnlShowMessage.ClientSize.Width - 80, 5, 75, 25);  // Set bounds of button.
                        //pnlShowMessage.Controls.Add(btnOk);  //Finally Add button control on panel.
                        btnOk.Dock = DockStyle.Fill;
                      
                        buttons_tableLayout.ColumnStyles[0].Width = 20.0f;
                        buttons_tableLayout.ColumnStyles[1].Width = 20.0f;
                        
                        buttons_tableLayout.ColumnStyles[3].Width = 20.0f;
                        buttons_tableLayout.ColumnStyles[4].Width = 20.0f;
                        buttons_tableLayout.Controls.Add(btnOk, 2,0);
                    }
                    break;
                case CustomMessageBoxButtons.OKCancel:
                    {
                        Button btnOk = new Button();
                        if (string.IsNullOrEmpty(okBtnText))
                            btnOk.Text = "OK";
                        else
                            btnOk.Text = okBtnText; 
                        btnOk.DialogResult = DialogResult.OK;
                        //btnOk.FlatStyle = FlatStyle.Popup;
                        btnOk.FlatAppearance.BorderSize = 0;
                        //btnOk.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        btnOk.Dock = DockStyle.Fill;
                        buttons_tableLayout.ColumnStyles[0].Width = 10.0f;
                        buttons_tableLayout.Controls.Add(btnOk, 1, 0);
                      
                        //pnlShowMessage.Controls.Add(btnOk);
                        Button btnCancel = new Button();
                        if (string.IsNullOrEmpty(cancelBtnText))
                            btnCancel.Text = "Cancel";
                        else
                            btnCancel.Text = cancelBtnText;
                        btnCancel.DialogResult = DialogResult.Cancel;
                        //btnCancel.FlatStyle = FlatStyle.Popup;
                        btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.Dock = DockStyle.Fill;

                        //btnCancel.SetBounds((pnlShowMessage.ClientSize.Width - (btnOk.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        //pnlShowMessage.Controls.Add(btnCancel);
                        buttons_tableLayout.Controls.Add(btnCancel, 2, 0);

                    }
                    break;
                case CustomMessageBoxButtons.YesNo:
                    {
                        Button btnYes = new Button();
                        if (string.IsNullOrEmpty(yesBtnText))
                            btnYes.Text = "Yes";
                        else
                            btnYes.Text = yesBtnText;
                        btnYes.DialogResult = DialogResult.Yes;
                        //btnYes.FlatStyle = FlatStyle.Popup;
                        btnYes.FlatAppearance.BorderSize = 0;
                        btnYes.Dock = DockStyle.Fill;

                        //btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnNo.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        buttons_tableLayout.Controls.Add(btnYes, 1, 0);
                        buttons_tableLayout.ColumnStyles[4].Width = 20.0f;

                        Button btnNo = new Button();
                        if (string.IsNullOrEmpty(noBtnText))
                            btnNo.Text = "No";
                        else
                            btnNo.Text = noBtnText;
                        btnNo.DialogResult = DialogResult.No;
                        //btnNo.FlatStyle = FlatStyle.Popup;
                        btnNo.FlatAppearance.BorderSize = 0;
                       // btnNo.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        //pnlShowMessage.Controls.Add(btnNo);
                        btnNo.Dock = DockStyle.Fill;

                        buttons_tableLayout.ColumnStyles[0].Width = 20.0f;
                        buttons_tableLayout.ColumnStyles[2].Width = 5.0f;

                        buttons_tableLayout.Controls.Add(btnNo, 3, 0);
                            
                     
                       // pnlShowMessage.Controls.Add(btnYes);
                    }
                    break;
                case CustomMessageBoxButtons.RestartNowRestartLater:
                    {
                        Button btnYes = new Button();
                        if (string.IsNullOrEmpty(restartNowBtnText))
                            btnYes.Text = "Restart Now";
                        else
                            btnYes.Text = restartNowBtnText;
                        btnYes.DialogResult = DialogResult.Yes;
                        //btnYes.FlatStyle = FlatStyle.Popup;
                        btnYes.FlatAppearance.BorderSize = 0;
                        btnYes.Dock = DockStyle.Fill;
                        //btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnNo.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        buttons_tableLayout.Controls.Add(btnYes, 2, 0);
                        buttons_tableLayout.ColumnStyles[0].Width = 20.0f;
                        buttons_tableLayout.ColumnStyles[1].Width = 20.0f;

                        buttons_tableLayout.ColumnStyles[3].Width = 20.0f;
                        buttons_tableLayout.ColumnStyles[4].Width = 20.0f;
                        #region has been commented by sriram to remove restart later option for configuration settings
                        //Button btnNo = new Button();
                        //if (string.IsNullOrEmpty(restartLaterBtnText))
                        //btnNo.Text = "Restart Later";
                        //else
                        //    btnNo.Text = restartLaterBtnText;
                        //btnNo.DialogResult = DialogResult.No;
                        ////btnNo.FlatStyle = FlatStyle.Popup;
                        //btnNo.FlatAppearance.BorderSize = 0;
                        //// btnNo.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        ////pnlShowMessage.Controls.Add(btnNo);
                        //btnNo.Dock = DockStyle.Fill;
                        //buttons_tableLayout.ColumnStyles[0].Width = 20.0f;
                        //buttons_tableLayout.ColumnStyles[2].Width = 5.0f;
                        //buttons_tableLayout.Controls.Add(btnNo, 3, 0);
                        // pnlShowMessage.Controls.Add(btnYes);
                        #endregion
                    }
                    break;
                case CustomMessageBoxButtons.YesNoCancel:
                    {
                        Button btnCancel = new Button();
                        if (string.IsNullOrEmpty(cancelBtnText))
                            btnCancel.Text = "Cancel";
                        else
                            btnCancel.Text = cancelBtnText;
                        btnCancel.DialogResult = DialogResult.Cancel;
                        //btnCancel.FlatStyle = FlatStyle.Popup;
                        btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.Dock = DockStyle.Fill;

                       // btnCancel.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                       // pnlShowMessage.Controls.Add(btnCancel);
                        buttons_tableLayout.Controls.Add(btnCancel, 2, 0);

                        Button btnNo = new Button();
                        if (string.IsNullOrEmpty(noBtnText))
                            btnNo.Text = "No";
                        else
                            btnNo.Text = noBtnText;
                        btnNo.DialogResult = DialogResult.No;
                        //btnNo.FlatStyle = FlatStyle.Popup;
                        btnNo.FlatAppearance.BorderSize = 0;
                        btnNo.Dock = DockStyle.Fill;

                       // btnNo.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        //pnlShowMessage.Controls.Add(btnNo);
                        buttons_tableLayout.Controls.Add(btnNo, 1, 0);

                        Button btnYes = new Button();
                        if (string.IsNullOrEmpty(yesBtnText))
                            btnYes.Text = "Yes";
                        else
                            btnYes.Text = yesBtnText;
                        btnYes.DialogResult = DialogResult.Yes;
                        //btnYes.FlatStyle = FlatStyle.Popup;
                        btnYes.FlatAppearance.BorderSize = 0;
                        btnYes.Dock = DockStyle.Fill;

                        //btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + btnNo.ClientSize.Width + 10 + 80)), 5, 75, 25);
                       // pnlShowMessage.Controls.Add(btnYes);
                        buttons_tableLayout.Controls.Add(btnYes, 0, 0);

                    }
                    break;
            }
        }

        /// <summary>
        /// Show method is overloaded which is used to display message
        /// and this is static method so that we don't need to create 
        /// object of this class to call this method.
        /// </summary>
        /// <param name="messageText"></param>
        public static DialogResult Show(string messageText, int width = 442, int height = 135)
        {
            CustomMessageBox frmMessage = new CustomMessageBox();
            frmMessage.SetSize(width, height);
            frmMessage.setMessage(messageText);
            //frmMessage.addIconImage(CustomMessageBoxIcon.Information);
            frmMessage.addButton(CustomMessageBoxButtons.OK);
          
           return frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle, int width = 442, int height = 135)
        {
            CustomMessageBox frmMessage = new CustomMessageBox();
            frmMessage.SetSize(width, height);
            frmMessage.setMessage(messageText);
            frmMessage.Text = messageTitle;
            //frmMessage.addIconImage(enumMessageIcon.Information);
            frmMessage.addButton(CustomMessageBoxButtons.OK);
            
           return  frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle, CustomMessageBoxIcon messageIcon, int width = 442, int height = 135)
        {
            CustomMessageBox frmMessage = new CustomMessageBox();
            frmMessage.SetSize(width, height);
            frmMessage.setMessage(messageText);
            
            frmMessage.Text = messageTitle;
            frmMessage.addIconImage(messageIcon);
            frmMessage.addButton(CustomMessageBoxButtons.OK);
                     
         return  frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle, CustomMessageBoxButtons messageButton, int width = 442, int height = 135)
        {
            CustomMessageBox frmMessage = new CustomMessageBox();
            frmMessage.SetSize(width, height);
            frmMessage.setMessage(messageText);
            frmMessage.Text = messageTitle;
            frmMessage.resizeTabelLayoutPanel();
            frmMessage.addButton(messageButton);
           return frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle, CustomMessageBoxButtons messageButton, CustomMessageBoxIcon messageIcon, int width = 442, int height = 135)
        {
            CustomMessageBox frmMessage = new CustomMessageBox();
            frmMessage.SetSize(width, height);
            frmMessage.setMessage(messageText);
             frmMessage.Text = messageTitle;
            frmMessage.addIconImage(messageIcon);
            frmMessage.addButton(messageButton);
            return  frmMessage.ShowDialog();
        }

        private void SetSize(int width, int height)
        {
            if (width != 442 || height != 135)
                this.Size = new Size(width, height);
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void resizeTabelLayoutPanel()
        {
            information_tableLayout.ColumnStyles[0].Width = 15.0f;
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
        //internal static void Show(string messageText, string messageTitle, enumMessageIcon messageIcon, enumMessageButton messageButton)
        //{
        //    frmShowMessage frmMessage = new frmShowMessage();
        //    frmMessage.setMessage(messageText);
        //    frmMessage.Text = messageTitle;
        //    frmMessage.addIconImage(messageIcon);
        //    frmMessage.addButton(messageButton);
        //    frmMessage.ShowDialog();
        //}
    }

    #region constant defiend in form of enumration which is used in CustomMessageBox class.

    public enum CustomMessageBoxIcon
    {
        Error,
        Warning,
        Information,
        Question,
    }

    public enum CustomMessageBoxButtons
    {
        OK,
        YesNo,
        YesNoCancel,
        RestartNowRestartLater,
        DoYouWantToDoThisForAllConflicts,
        OKCancel
    }
    #endregion
}
