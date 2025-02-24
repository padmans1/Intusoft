﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace INTUSOFT.Configuration.AdvanceSettings
{
    [Serializable]
    public class PostProcessing
    {
        private IVLControlProperties enablePostProcessing = null;

        public IVLControlProperties EnablePostProcessing
        {
            get { return enablePostProcessing; }
            set { enablePostProcessing = value; }
        }

        public PostProcessing()
        {
            EnablePostProcessing = new IVLControlProperties();
            EnablePostProcessing.name = "EnablePostProcessing";
            EnablePostProcessing.val = "true";
            EnablePostProcessing.type = "bool";
            EnablePostProcessing.control = "System.Windows.Forms.RadioButton";
            EnablePostProcessing.text = "Enable Post Processing";
            EnablePostProcessing.min = 1;
            EnablePostProcessing.max = 10000;

        }
    }

     [Serializable]
    public class PostProcessingSettings
    {
         public PostProcessing PostProcessing;
        public ImageShiftSettings ImageShiftSettings;
        public VignattingSettings VignattingSettings;
        public MaskSettings MaskSettings;
        public OverlaySettings OverlaySettings;
        public ColorCorrectionSettings ColorCorrectionSettings;
        public HotSpotSettings HotSpotSettings;
        public UnsharpMaskSettings UnsharpMaskSettings;
        public ClaheSettings ClaheSettings;
        public LutSettings LutSettings;
        public BrightnessContrastSettings BrightnessContrastSettings;
        public GammaSettings GammaSettings;
        public PostProcessingSettings()
        {
           
            PostProcessing = new AdvanceSettings.PostProcessing();
            ImageShiftSettings = new ImageShiftSettings();
            VignattingSettings = new VignattingSettings();
            MaskSettings = new MaskSettings();
            ColorCorrectionSettings = new ColorCorrectionSettings();
            HotSpotSettings = new HotSpotSettings();
            UnsharpMaskSettings = new UnsharpMaskSettings();
            ClaheSettings = new ClaheSettings();
            LutSettings = new LutSettings();
            BrightnessContrastSettings = new BrightnessContrastSettings();
            OverlaySettings = new OverlaySettings();
            GammaSettings = new GammaSettings();
        }
    }
    [Serializable]
     public class ImageShiftSettings
     {
         // Image shift parameters
         //public bool isApplyImageShift = true;
         //public int ImageShiftX = 4;
         //public int ImageShiftY = 3;


         private  IVLControlProperties isApplyImageShift = null;

         public IVLControlProperties _IsApplyImageShift
         {
             get { return isApplyImageShift; }
             set { isApplyImageShift = value; }
         }
         private  IVLControlProperties ImageShiftX = null;

         public IVLControlProperties _ImageShiftX
         {
             get { return ImageShiftX; }
             set { ImageShiftX = value; }
         }
         private  IVLControlProperties ImageShiftY = null;

         public IVLControlProperties _ImageShiftY
         {
             get { return ImageShiftY; }
             set { ImageShiftY = value; }
         }
         private IVLControlProperties isApplyHSV = null;

         public IVLControlProperties IsApplyHSV
         {
             get { return isApplyHSV; }
             set { isApplyHSV = value; }
         }
         private IVLControlProperties liveCC = null;

         public IVLControlProperties LiveCC
         {
             get { return liveCC; }
             set { liveCC = value; }
         }

        private IVLControlProperties livePP = null;

         public IVLControlProperties LivePP
         {
             get { return livePP; }
             set { livePP = value; }
         }
         private IVLControlProperties hsvBoost = null;

         public IVLControlProperties HsvBoost
         {
             get { return hsvBoost; }
             set { hsvBoost = value; }
         }
         public ImageShiftSettings()
         {

             LivePP = new IVLControlProperties();
             LivePP.name = "LivePP";
             LivePP.type = "bool";
             LivePP.text = "Apply Live PP";
             LivePP.val = false.ToString();
             LivePP.control = "System.Windows.Forms.RadioButton";

             LiveCC = new IVLControlProperties();
             LiveCC.name = "LiveCC";
             LiveCC.type = "bool";
             LiveCC.text = "Apply Live CC";
             LiveCC.val = false.ToString();
             LiveCC.control = "System.Windows.Forms.RadioButton";

             IsApplyHSV = new IVLControlProperties();
             IsApplyHSV.name = "IsApplyHSV";
             IsApplyHSV.type = "bool";
             IsApplyHSV.text = "Is Apply HSV";
             IsApplyHSV.val = true.ToString();
             IsApplyHSV.control = "System.Windows.Forms.RadioButton";

             _IsApplyImageShift = new IVLControlProperties();
             _IsApplyImageShift.name = "isApplyImageShift";
             _IsApplyImageShift.val = true.ToString();
             _IsApplyImageShift.type = "bool";
             _IsApplyImageShift.control = "System.Windows.Forms.RadioButton";
             _IsApplyImageShift.text = "Apply Image Shift";

             _ImageShiftX = new IVLControlProperties();
             _ImageShiftX.name = "ImageShiftX";
             _ImageShiftX.val = "4";
             _ImageShiftX.type = "int";
             _ImageShiftX.control = "System.Windows.Forms.NumericUpDown";
             _ImageShiftX.text = "Image ShiftX";
             _ImageShiftX.min = 1;
             _ImageShiftX.max = 100;
             
             _ImageShiftY = new IVLControlProperties();
             _ImageShiftY.name = "ImageShiftY";
             _ImageShiftY.val = "3";
             _ImageShiftY.type = "int";
             _ImageShiftY.control = "System.Windows.Forms.NumericUpDown";
             _ImageShiftY.text = "Image ShiftY";
             _ImageShiftY.min = 1;
             _ImageShiftY.max = 100;

             HsvBoost = new IVLControlProperties();
             HsvBoost.name = "HsvBoost";
             HsvBoost.val = "8";
             HsvBoost.type = "int";
             HsvBoost.control = "System.Windows.Forms.NumericUpDown";
             HsvBoost.text = "Hsv Boost";
             HsvBoost.min = 1;
             HsvBoost.max = 100;

         }
     }
    [Serializable]
    public class OverlaySettings
    {
        private IVLControlProperties showPosteriorOverlay = null;

        public IVLControlProperties ShowPosteriorOverlay
        {
            get { return showPosteriorOverlay; }
            set { showPosteriorOverlay = value; }
        }

        private IVLControlProperties showAnteriorOverlay = null;

        public IVLControlProperties ShowAnteriorOverlay
        {
            get { return showAnteriorOverlay; }
            set { showAnteriorOverlay = value; }
        }

        private IVLControlProperties showOpticDiscOverlay = null;

        public IVLControlProperties ShowOpticDiscOverlay
        {
            get { return showOpticDiscOverlay; }
            set { showOpticDiscOverlay = value; }
        }

        public OverlaySettings()
        {
            ShowPosteriorOverlay = new IVLControlProperties();
            ShowPosteriorOverlay.name = "ShowPosteriorOverlay";
            ShowPosteriorOverlay.val = "true";
            ShowPosteriorOverlay.type = "bool";
            ShowPosteriorOverlay.control = "System.Windows.Forms.RadioButton";
            ShowPosteriorOverlay.text = "Show Posterior Overlay";
            ShowPosteriorOverlay.min = 1;
            ShowPosteriorOverlay.max = 10000;


            ShowAnteriorOverlay = new IVLControlProperties();
            ShowAnteriorOverlay.name = "ShowAnteriorOverlay";
            ShowAnteriorOverlay.val = "true";
            ShowAnteriorOverlay.type = "bool";
            ShowAnteriorOverlay.control = "System.Windows.Forms.RadioButton";
            ShowAnteriorOverlay.text = "Show Anterior Overlay";
            ShowAnteriorOverlay.min = 1;
            ShowAnteriorOverlay.max = 10000;

            ShowOpticDiscOverlay = new IVLControlProperties();
            ShowOpticDiscOverlay.name = "ShowOpticDiscOverlay";
            ShowOpticDiscOverlay.val = "false";
            ShowOpticDiscOverlay.type = "bool";
            ShowOpticDiscOverlay.control = "System.Windows.Forms.RadioButton";
            ShowOpticDiscOverlay.text = "Show Optic Disc Overlay";
            ShowOpticDiscOverlay.min = 1;
            ShowOpticDiscOverlay.max = 10000;

        }
    }

    [Serializable]
    public class MaskSettings
    {
        private  IVLControlProperties liveMaskWidth = null;

        public IVLControlProperties LiveMaskWidth
        {
            get { return liveMaskWidth; }
            set { liveMaskWidth = value; }
        }
        private  IVLControlProperties liveMaskHeight = null;

        public IVLControlProperties LiveMaskHeight
        {
            get { return liveMaskHeight; }
            set { liveMaskHeight = value; }
        }

        private IVLControlProperties captureMaskWidth = null;

        public IVLControlProperties CaptureMaskWidth
        {
            get { return captureMaskWidth; }
            set { captureMaskWidth = value; }
        }

        private IVLControlProperties captureMaskHeight = null;

        public IVLControlProperties CaptureMaskHeight
        {
            get { return captureMaskHeight; }
            set { captureMaskHeight = value; }
        }

        private  IVLControlProperties isApplyMask = null;

        public IVLControlProperties _IsApplyMask
        {
            get { return isApplyMask; }
            set { isApplyMask = value; }
        }
        private  IVLControlProperties ApplyLiveMask = null;

        public IVLControlProperties _ApplyLiveMask
        {
            get { return ApplyLiveMask; }
            set { ApplyLiveMask = value; }
        }

        private IVLControlProperties ApplyLogo = null;

        public IVLControlProperties _ApplyLogo
        {
            get { return ApplyLogo; }
            set { ApplyLogo = value; }
        }
        public MaskSettings()
        {
            LiveMaskWidth = new IVLControlProperties();
            LiveMaskWidth.name = "LiveMaskWidth";
            LiveMaskWidth.val = "2100";
            LiveMaskWidth.type = "int";
            LiveMaskWidth.control = "System.Windows.Forms.NumericUpDown";
            LiveMaskWidth.text = "Live Mask Width";
            LiveMaskWidth.min = 1;
            LiveMaskWidth.max = 10000;

            LiveMaskHeight = new IVLControlProperties();
            LiveMaskHeight.name = "LiveMaskHeight";
            LiveMaskHeight.val = "2100";
            LiveMaskHeight.type = "int";
            LiveMaskHeight.control = "System.Windows.Forms.NumericUpDown";
            LiveMaskHeight.text = "Live Mask Height";
            LiveMaskHeight.min = 1;
            LiveMaskHeight.max = 10000;

            CaptureMaskWidth = new IVLControlProperties();
            CaptureMaskWidth.name = "CaptureMaskWidth";
            CaptureMaskWidth.val = "2000";
            CaptureMaskWidth.type = "int";
            CaptureMaskWidth.control = "System.Windows.Forms.NumericUpDown";
            CaptureMaskWidth.text = "Capture Mask Width";
            CaptureMaskWidth.min = 1;
            CaptureMaskWidth.max = 10000;

            CaptureMaskHeight = new IVLControlProperties();
            CaptureMaskHeight.name = "CaptureMaskHeight";
            CaptureMaskHeight.val = "2000";
            CaptureMaskHeight.type = "int";
            CaptureMaskHeight.control = "System.Windows.Forms.NumericUpDown";
            CaptureMaskHeight.text = "Capture Mask Height";
            CaptureMaskHeight.min = 1;
            CaptureMaskHeight.max = 10000;

            _IsApplyMask = new IVLControlProperties();
            _IsApplyMask.name = "isApplyMask";
            _IsApplyMask.val = true.ToString();
            _IsApplyMask.type = "bool";
            _IsApplyMask.control = "System.Windows.Forms.RadioButton";
            _IsApplyMask.text = "Apply Mask";

            _ApplyLogo = new IVLControlProperties();
            _ApplyLogo.name = "ApplyLogo";
            _ApplyLogo.val = true.ToString();
            _ApplyLogo.type = "bool";
            _ApplyLogo.control = "System.Windows.Forms.RadioButton";
            _ApplyLogo.text = "Apply Logo";

            _ApplyLiveMask = new IVLControlProperties();
            _ApplyLiveMask.name = "ApplyLiveMask";
            _ApplyLiveMask.val = true.ToString();
            _ApplyLiveMask.type = "bool";
            _ApplyLiveMask.control = "System.Windows.Forms.RadioButton";
            _ApplyLiveMask.text = "Apply Live Mask";
        }
    }

    [Serializable]
    public class VignattingSettings
    {
        private IVLControlProperties isApplyVignatting = null;

        public IVLControlProperties _IsApplyVignatting
        {
            get { return isApplyVignatting; }
            set { isApplyVignatting = value; }
        }

        private  IVLControlProperties VignattingPercentageFactorLive = null;

        public IVLControlProperties _VignattingPercentageFactorLive
        {
            get { return VignattingPercentageFactorLive; }
            set { VignattingPercentageFactorLive = value; }
        }

        private  IVLControlProperties VignattingRadiusLive = null;

        public IVLControlProperties _VignattingRadiusLive
        {
            get { return VignattingRadiusLive; }
            set { VignattingRadiusLive = value; }
        }

        private  IVLControlProperties VignattingPercentageFactorPostProcessing = null;

        public IVLControlProperties _VignattingPercentageFactorPostProcessing
        {
            get { return VignattingPercentageFactorPostProcessing; }
            set { VignattingPercentageFactorPostProcessing = value; }
        }

        private  IVLControlProperties VignattingRadiusPostProcessing = null;

        public IVLControlProperties _VignattingRadiusPostProcessing
        {
            get { return VignattingRadiusPostProcessing; }
            set { VignattingRadiusPostProcessing = value; }
        }

        private  IVLControlProperties ApplyLiveParabola = null;

        public IVLControlProperties _ApplyLiveParabola
        {
            get { return ApplyLiveParabola; }
            set { ApplyLiveParabola = value; }
        }

        public VignattingSettings()
        {
            _IsApplyVignatting = new IVLControlProperties();
            _IsApplyVignatting.name = "isApplyVignatting";
            _IsApplyVignatting.val = false.ToString();
            _IsApplyVignatting.type = "bool";
            _IsApplyVignatting.control = "System.Windows.Forms.RadioButton";
            _IsApplyVignatting.text = "Apply Vignatting";

            _ApplyLiveParabola = new IVLControlProperties();
            _ApplyLiveParabola.name = "ApplyLiveParabola";
            _ApplyLiveParabola.val = false.ToString();
            _ApplyLiveParabola.type = "bool";
            _ApplyLiveParabola.control = "System.Windows.Forms.RadioButton";
            _ApplyLiveParabola.text = "Apply Live Parabola";

            _VignattingRadiusLive = new IVLControlProperties();
            _VignattingRadiusLive.name = "VignattingRadiusLive";
            _VignattingRadiusLive.val = "800";
            _VignattingRadiusLive.type = "int";
            _VignattingRadiusLive.control = "System.Windows.Forms.NumericUpDown";
            _VignattingRadiusLive.text = "Vignatting Radius Live";

            _VignattingRadiusPostProcessing = new IVLControlProperties();
            _VignattingRadiusPostProcessing.name = "VignattingRadiusPostProcessing";
            _VignattingRadiusPostProcessing.val = "1000";
            _VignattingRadiusPostProcessing.type = "int";
            _VignattingRadiusPostProcessing.control = "System.Windows.Forms.NumericUpDown";
            _VignattingRadiusPostProcessing.text = "Vignatting Radius Post Processing";

            _VignattingPercentageFactorLive = new IVLControlProperties();
            _VignattingPercentageFactorLive.name = "VignattingPercentageFactorLive";
            _VignattingPercentageFactorLive.val = "0.9";
            _VignattingPercentageFactorLive.type = "float";
            _VignattingPercentageFactorLive.control = "System.Windows.Forms.NumericUpDown";
            _VignattingPercentageFactorLive.text = "Vignatting Percentage Factor Live";

            _VignattingPercentageFactorPostProcessing = new IVLControlProperties();
            _VignattingPercentageFactorPostProcessing.name = "VignattingPercentageFactorPostProcessing";
            _VignattingPercentageFactorPostProcessing.val = "1.0";
            _VignattingPercentageFactorPostProcessing.type = "float";
            _VignattingPercentageFactorPostProcessing.control = "System.Windows.Forms.NumericUpDown";
            _VignattingPercentageFactorPostProcessing.text = "Vignatting Percentage Factor Post Processing";
        }
    }

    [Serializable]
    public class HotSpotSettings
    {

        private IVLControlProperties isApplyHotspotCorrection = null;

        public IVLControlProperties _IsApplyHotspotCorrection
        {
            get { return isApplyHotspotCorrection; }
            set { isApplyHotspotCorrection = value; }
        }
        private  IVLControlProperties hotspotRadius1 = null;

        public IVLControlProperties _HotspotRadius1
        {
            get { return hotspotRadius1; }
            set { hotspotRadius1 = value; }
        }
        private  IVLControlProperties hotspotRadius2 = null;

        public IVLControlProperties _HotspotRadius2
        {
            get { return hotspotRadius2; }
            set { hotspotRadius2 = value; }
        }
        private  IVLControlProperties ShadowRadSpot1 = null;

        public IVLControlProperties _ShadowRadSpot1
        {
            get { return ShadowRadSpot1; }
            set { ShadowRadSpot1 = value; }
        }
        private  IVLControlProperties ShadowradSpot2 = null;

        public IVLControlProperties _ShadowradSpot2
        {
            get { return ShadowradSpot2; }
            set { ShadowradSpot2 = value; }
        }

        private  IVLControlProperties HotSpotRedPeak = null;

        public IVLControlProperties _HotSpotRedPeak
        {
            get { return HotSpotRedPeak; }
            set { HotSpotRedPeak = value; }
        }

        private  IVLControlProperties HotSpotGreenPeak = null;

        public IVLControlProperties _HotSpotGreenPeak
        {
            get { return HotSpotGreenPeak; }
            set { HotSpotGreenPeak = value; }
        }

        private  IVLControlProperties HotSpotBluePeak = null;

        public IVLControlProperties _HotSpotBluePeak
        {
            get { return HotSpotBluePeak; }
            set { HotSpotBluePeak = value; }
        }

        private  IVLControlProperties HotSpotRedRadius = null;

        public IVLControlProperties _HotSpotRedRadius
        {
            get { return HotSpotRedRadius; }
            set { HotSpotRedRadius = value; }
        }

        private  IVLControlProperties HotSpotGreenRadius = null;

        public IVLControlProperties _HotSpotGreenRadius
        {
            get { return HotSpotGreenRadius; }
            set { HotSpotGreenRadius = value; }
        }

        private  IVLControlProperties HotSpotBlueRadius = null;

        public IVLControlProperties _HotSpotBlueRadius
        {
            get { return HotSpotBlueRadius; }
            set { HotSpotBlueRadius = value; }
        }

        private  IVLControlProperties ShadowRedPercentage = null;

        public IVLControlProperties _ShadowRedPercentage
        {
            get { return ShadowRedPercentage; }
            set { ShadowRedPercentage = value; }
        }

        private  IVLControlProperties ShadowGreenPercentage = null;

        public IVLControlProperties _ShadowGreenPercentage
        {
            get { return ShadowGreenPercentage; }
            set { ShadowGreenPercentage = value; }
        }

        private  IVLControlProperties ShadowBlueBPercentage = null;

        public IVLControlProperties _ShadowBlueBPercentage
        {
            get { return ShadowBlueBPercentage; }
            set { ShadowBlueBPercentage = value; }
        }
        private  IVLControlProperties GainSlope = null;

        public IVLControlProperties _GainSlope
        {
            get { return GainSlope; }
            set { GainSlope = value; }
        }
        public HotSpotSettings()
        {
            _IsApplyHotspotCorrection = new IVLControlProperties();
            _IsApplyHotspotCorrection.name = "isApplyHotspotCorrection";
            _IsApplyHotspotCorrection.val = true.ToString();
            _IsApplyHotspotCorrection.type = "bool";
            _IsApplyHotspotCorrection.control = "System.Windows.Forms.RadioButton";
            _IsApplyHotspotCorrection.text = "Apply Hotspot Correction";

            _HotspotRadius1 = new IVLControlProperties();
            _HotspotRadius1.name = "hotspotRadius1";
            _HotspotRadius1.val = "-20";
            _HotspotRadius1.type = "int";
            _HotspotRadius1.control = "System.Windows.Forms.NumericUpDown";
            _HotspotRadius1.text = "Hotspot Radius1";
            _HotspotRadius1.min = -255;
            _HotspotRadius1.max = 255;


            _HotspotRadius2 = new IVLControlProperties();
            _HotspotRadius2.name = "hotspotRadius2";
            _HotspotRadius2.val = "100";
            _HotspotRadius2.type = "int";
            _HotspotRadius2.control = "System.Windows.Forms.NumericUpDown";
            _HotspotRadius2.text = "Hotspot Radius2";
            _HotspotRadius2.min = -255;
            _HotspotRadius2.max = 255;


            _ShadowRadSpot1 = new IVLControlProperties();
            _ShadowRadSpot1.name = "ShadowRadSpot1";
            _ShadowRadSpot1.val = "170";
            _ShadowRadSpot1.type = "int";
            _ShadowRadSpot1.control = "System.Windows.Forms.NumericUpDown";
            _ShadowRadSpot1.text = "Shadow RadSpot1";
            _ShadowRadSpot1.min = 100;
            _ShadowRadSpot1.max = 10000;

            _ShadowradSpot2 = new IVLControlProperties();
            _ShadowradSpot2.name = "ShadowradSpot2";
            _ShadowradSpot2.val = "400";
            _ShadowradSpot2.type = "int";
            _ShadowradSpot2.control = "System.Windows.Forms.NumericUpDown";
            _ShadowradSpot2.text = "Shadow RadSpot2";
            _ShadowradSpot2.min = 100;
            _ShadowradSpot2.max = 10000;

            _GainSlope = new IVLControlProperties();
            _GainSlope.name = "gainSlope";
            _GainSlope.val = "5";
            _GainSlope.type = "int";
            _GainSlope.control = "System.Windows.Forms.NumericUpDown";
            _GainSlope.text = "Gain Slope";
            _GainSlope.min = 1;
            _GainSlope.max = 30;


            _HotSpotRedPeak = new IVLControlProperties();
            _HotSpotRedPeak.name = "hotSpotRedPeak";
            _HotSpotRedPeak.val = "0";
            _HotSpotRedPeak.type = "int";
            _HotSpotRedPeak.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotRedPeak.text = "HotSpot RedPeak";
            _HotSpotRedPeak.min = 0;
            _HotSpotRedPeak.max = 255;

            _HotSpotGreenPeak = new IVLControlProperties();
            _HotSpotGreenPeak.name = "HotSpotGreenPeak";
            _HotSpotGreenPeak.val = "0";
            _HotSpotGreenPeak.type = "int";
            _HotSpotGreenPeak.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotGreenPeak.text = "HotSpot GreenPeak";
            _HotSpotGreenPeak.min = 0;
            _HotSpotGreenPeak.max = 255;

            _HotSpotBluePeak = new IVLControlProperties();
            _HotSpotBluePeak.name = "HotSpotBluePeak";
            _HotSpotBluePeak.val = "0";
            _HotSpotBluePeak.type = "int";
            _HotSpotBluePeak.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotBluePeak.text = "HotSpot BluePeak";
            _HotSpotBluePeak.min = 0;
            _HotSpotBluePeak.max = 255;

            _HotSpotRedRadius = new IVLControlProperties();
            _HotSpotRedRadius.name = "HotSpotRedRadius";
            _HotSpotRedRadius.val = "0";
            _HotSpotRedRadius.type = "int";
            _HotSpotRedRadius.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotRedRadius.text = "HotSpot RedRadius";
            _HotSpotRedRadius.min = 0;
            _HotSpotRedRadius.max = 10000;

            _HotSpotGreenRadius = new IVLControlProperties();
            _HotSpotGreenRadius.name = "HotSpotGreenRadius";
            _HotSpotGreenRadius.val = "0";
            _HotSpotGreenRadius.type = "int";
            _HotSpotGreenRadius.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotGreenRadius.text = "HotSpot GreenRadius";
            _HotSpotGreenRadius.min = 0;
            _HotSpotGreenRadius.max = 10000;

            _HotSpotBlueRadius = new IVLControlProperties();
            _HotSpotBlueRadius.name = "HotSpotBlueRadius";
            _HotSpotBlueRadius.val = "0";
            _HotSpotBlueRadius.type = "int";
            _HotSpotBlueRadius.control = "System.Windows.Forms.NumericUpDown";
            _HotSpotBlueRadius.text = "HotSpot BlueRadius";
            _HotSpotBlueRadius.min = 0;
            _HotSpotBlueRadius.max = 10000;

            _ShadowRedPercentage = new IVLControlProperties();
            _ShadowRedPercentage.name = "ShadowRedPercentage";
            _ShadowRedPercentage.val = "20";
            _ShadowRedPercentage.type = "int";
            _ShadowRedPercentage.control = "System.Windows.Forms.NumericUpDown";
            _ShadowRedPercentage.text = "Shadow RedPercentage";
            _ShadowRedPercentage.min = 0;
            _ShadowRedPercentage.max = 100;

            _ShadowGreenPercentage = new IVLControlProperties();
            _ShadowGreenPercentage.name = "ShadowGreenPercentage";
            _ShadowGreenPercentage.val = "15";
            _ShadowGreenPercentage.type = "int";
            _ShadowGreenPercentage.control = "System.Windows.Forms.NumericUpDown";
            _ShadowGreenPercentage.text = "Shadow GreenPercentage";
            _ShadowGreenPercentage.min = 0;
            _ShadowGreenPercentage.max = 100;

            _ShadowBlueBPercentage = new IVLControlProperties();
            _ShadowBlueBPercentage.name = "ShadowBlueBPercentage";
            _ShadowBlueBPercentage.val = "10";
            _ShadowBlueBPercentage.type = "int";
            _ShadowBlueBPercentage.control = "System.Windows.Forms.NumericUpDown";
            _ShadowBlueBPercentage.text = "Shadow BlueBPercentage";
            _ShadowBlueBPercentage.min = 0;
            _ShadowBlueBPercentage.max = 100;

        }
    }
    [Serializable]
    public class ColorCorrectionSettings
    {
        //Post Processing Color Correction
        //public bool isApplyColorCorrection = true;
        //public float RRCompensation = 1f;
        //public float RGCompensation = 0.4f;
        //public float RBCompensation = 0f;
        //public float GRCompensation = 0f;
        //public float GGCompensation = 0.95f;
        //public float GBCompensation = 0f;
        //public float BRCompensation = 0f;
        //public float BGCompensation = 0.2f;
        //public float BBCompensation = 1.5f;

        private  IVLControlProperties isApplyColorCorrection = null;

        public IVLControlProperties _IsApplyColorCorrection
        {
            get { return isApplyColorCorrection; }
            set { isApplyColorCorrection = value; }
        }
        private  IVLControlProperties RRCompensation = null;

        public IVLControlProperties _RRCompensation
        {
            get { return RRCompensation; }
            set { RRCompensation = value; }
        }
        private  IVLControlProperties RGCompensation = null;

        public IVLControlProperties _RGCompensation
        {
            get { return RGCompensation; }
            set { RGCompensation = value; }
        }
        private  IVLControlProperties RBCompensation = null;

        public IVLControlProperties _RBCompensation
        {
            get { return RBCompensation; }
            set { RBCompensation = value; }
        }
        private  IVLControlProperties GRCompensation = null;

        public IVLControlProperties _GRCompensation
        {
            get { return GRCompensation; }
            set { GRCompensation = value; }
        }
        private  IVLControlProperties GGCompensation = null;

        public IVLControlProperties _GGCompensation
        {
            get { return GGCompensation; }
            set { GGCompensation = value; }
        }
        private  IVLControlProperties GBCompensation = null;

        public IVLControlProperties _GBCompensation
        {
            get { return GBCompensation; }
            set { GBCompensation = value; }
        }
        private  IVLControlProperties BRCompensation = null;

        public IVLControlProperties _BRCompensation
        {
            get { return BRCompensation; }
            set { BRCompensation = value; }
        }
        private  IVLControlProperties BGCompensation = null;

        public IVLControlProperties _BGCompensation
        {
            get { return BGCompensation; }
            set { BGCompensation = value; }
        }
        private  IVLControlProperties BBCompensation = null;

        public IVLControlProperties _BBCompensation
        {
            get { return BBCompensation; }
            set { BBCompensation = value; }
        }
        public ColorCorrectionSettings()
        {
            _IsApplyColorCorrection = new IVLControlProperties();
            _IsApplyColorCorrection.name = "isApplyColorCorrection";
            _IsApplyColorCorrection.val = true.ToString();
            _IsApplyColorCorrection.type = "bool";
            _IsApplyColorCorrection.control = "System.Windows.Forms.RadioButton";
            _IsApplyColorCorrection.text = "Apply Color Correction";

            _RRCompensation = new IVLControlProperties();
            _RRCompensation.name = "RRCompensation";
            _RRCompensation.val = "1.20";
            _RRCompensation.type = "float";
            _RRCompensation.control = "System.Windows.Forms.NumericUpDown";
            _RRCompensation.text = "RRCompensation";
            _RRCompensation.min = 0;
            _RRCompensation.max = 10;

            _RGCompensation = new IVLControlProperties();
            _RGCompensation.name = "RGCompensation";
            _RGCompensation.val = "0.2";
            _RGCompensation.type = "float";
            _RGCompensation.control = "System.Windows.Forms.NumericUpDown";
            _RGCompensation.text = "RGCompensation";
            _RGCompensation.min = 0;
            _RGCompensation.max = 10;

            _RBCompensation = new IVLControlProperties();
            _RBCompensation.name = "RBCompensation";
            _RBCompensation.val = "0.0";
            _RBCompensation.type = "float";
            _RBCompensation.control = "System.Windows.Forms.NumericUpDown";
            _RBCompensation.text = "RBCompensation";
            _RBCompensation.min = 0;
            _RBCompensation.max = 10;

            _GRCompensation = new IVLControlProperties();
            _GRCompensation.name = "GRCompensation";
            _GRCompensation.val = "0.0";
            _GRCompensation.type = "float";
            _GRCompensation.control = "System.Windows.Forms.NumericUpDown";
            _GRCompensation.text = "GRCompensation";
            _GRCompensation.min = 0;
            _GRCompensation.max = 10;

            _GGCompensation = new IVLControlProperties();
            _GGCompensation.name = "GGCompensation";
            _GGCompensation.val = "0.95";
            _GGCompensation.type = "float";
            _GGCompensation.control = "System.Windows.Forms.NumericUpDown";
            _GGCompensation.text = "GGCompensation";
            _GGCompensation.min = 0;
            _GGCompensation.max = 10;

            _GBCompensation = new IVLControlProperties();
            _GBCompensation.name = "GBCompensation";
            _GBCompensation.val = "0.0";
            _GBCompensation.type = "float";
            _GBCompensation.control = "System.Windows.Forms.NumericUpDown";
            _GBCompensation.text = "GBCompensation";
            _GBCompensation.min = 0;
            _GBCompensation.max = 10;

            _BRCompensation = new IVLControlProperties();
            _BRCompensation.name = "BRCompensation";
            _BRCompensation.val = "0.0";
            _BRCompensation.type = "float";
            _BRCompensation.control = "System.Windows.Forms.NumericUpDown";
            _BRCompensation.text = "BRCompensation";
            _BRCompensation.min = 0;
            _BRCompensation.max = 10;

            _BGCompensation = new IVLControlProperties();
            _BGCompensation.name = "BGCompensation";
            _BGCompensation.val = "0.20";
            _BGCompensation.type = "float";
            _BGCompensation.control = "System.Windows.Forms.NumericUpDown";
            _BGCompensation.text = "BGCompensation";
            _BGCompensation.min = 0;
            _BGCompensation.max = 10;

            _BBCompensation = new IVLControlProperties();
            _BBCompensation.name = "BBCompensation";
            _BBCompensation.val = "1.3";
            _BBCompensation.type = "float";
            _BBCompensation.control = "System.Windows.Forms.NumericUpDown";
            _BBCompensation.text = "BBCompensation";
            _BBCompensation.min = 0;
            _BBCompensation.max = 10;
        }
    }

    [Serializable]
    public class UnsharpMaskSettings
    {
        private  IVLControlProperties UnSharpAmount = null;

        public IVLControlProperties _UnSharpAmount
        {
            get { return UnSharpAmount; }
            set { UnSharpAmount = value; }
        }

        private  IVLControlProperties IsApplyUnsharpSettings = null;

        public IVLControlProperties _IsApplyUnsharpSettings
        {
            get { return IsApplyUnsharpSettings; }
            set { IsApplyUnsharpSettings = value; }
        }

        private  IVLControlProperties UnSharpRadius = null;

        public IVLControlProperties _UnSharpRadius
        {
            get { return UnSharpRadius; }
            set { UnSharpRadius = value; }
        }

        private  IVLControlProperties Threshold = null;

        public IVLControlProperties _Threshold
        {
            get { return Threshold; }
            set { Threshold = value; }
        }

        private IVLControlProperties MedFilter = null;

        public IVLControlProperties _MedFilter
        {
            get { return MedFilter; }
            set { MedFilter = value; }
        }
        public UnsharpMaskSettings()
        {
            _IsApplyUnsharpSettings = new IVLControlProperties();
            _IsApplyUnsharpSettings.name = "IsApplyUnsharpSettings";
            _IsApplyUnsharpSettings.val = true.ToString();
            _IsApplyUnsharpSettings.type = "bool";
            _IsApplyUnsharpSettings.control = "System.Windows.Forms.RadioButton";
            _IsApplyUnsharpSettings.text = "Apply Unsharp Mask ";

            _UnSharpAmount = new IVLControlProperties();
            _UnSharpAmount.name = "UnSharpAmount";
            _UnSharpAmount.val = "1";
            _UnSharpAmount.type = "float";
            _UnSharpAmount.control = "System.Windows.Forms.NumericUpDown";
            _UnSharpAmount.text = "UnSharp Amount";
            _UnSharpAmount.min = 0;
            _UnSharpAmount.max = 3;

            _UnSharpRadius = new IVLControlProperties();
            _UnSharpRadius.name = "UnSharpAmount";
            _UnSharpRadius.val = "5";
            _UnSharpRadius.type = "int";
            _UnSharpRadius.control = "System.Windows.Forms.NumericUpDown";
            _UnSharpRadius.text = "UnSharp Radius";
            _UnSharpRadius.min = 0;
            _UnSharpRadius.max = 255;

            _Threshold = new IVLControlProperties();
            _Threshold.name = "Threshold";
            _Threshold.val = "40";
            _Threshold.type = "int";
            _Threshold.control = "System.Windows.Forms.NumericUpDown";
            _Threshold.text = "Threshold";
            _Threshold.min = 0;
            _Threshold.max = 255;

            _MedFilter = new IVLControlProperties();
            _MedFilter.name = "MedFilter";
            _MedFilter.val = "3";
            _MedFilter.type = "int";
            _MedFilter.control = "System.Windows.Forms.NumericUpDown";
            _MedFilter.text = "Median Filter";
            _MedFilter.min = 1;
            _MedFilter.max = 255;



        }
      
    }
    [Serializable]
    public class ClaheSettings
    {
private static IVLControlProperties ClipValue = null;

        private  IVLControlProperties ClipValueB = null;

        public IVLControlProperties _ClipValue
        {
            get { return ClaheSettings.ClipValue; }
            set { ClaheSettings.ClipValue = value; }
        }

        public IVLControlProperties _ClipValueB
        {
            get { return ClipValueB; }
            set { ClipValueB = value; }
        }

        private   IVLControlProperties ClipValueG = null;

        public IVLControlProperties _ClipValueG
        {
            get { return ClipValueG; }
            set { ClipValueG = value; }
        }
        private  IVLControlProperties ClipValueR = null;

        public IVLControlProperties _ClipValueR
        {
            get { return ClipValueR; }
            set { ClipValueR = value; }
        }

        private  IVLControlProperties IsApplyClaheSettings = null;

        public IVLControlProperties _IsApplyClaheSettings
        {
            get { return IsApplyClaheSettings; }
            set { IsApplyClaheSettings = value; }
        }
        public ClaheSettings()
        {
            
            _IsApplyClaheSettings = new IVLControlProperties();
            _IsApplyClaheSettings.name = "isApplyClaheSettings";
            _IsApplyClaheSettings.val = true.ToString();
            _IsApplyClaheSettings.type = "bool";
            _IsApplyClaheSettings.control = "System.Windows.Forms.RadioButton";
            _IsApplyClaheSettings.text = "Apply Clahe";

            _ClipValueR = new IVLControlProperties();
            _ClipValueR.name = "ClipValueR";
            _ClipValueR.val = "0.5";
            _ClipValueR.type = "float";
            _ClipValueR.control = "System.Windows.Forms.NumericUpDown";
            _ClipValueR.text = "Clip Value Red";
            _ClipValueR.min = 0;
            _ClipValueR.max = 1;


            _ClipValue = new IVLControlProperties();
            _ClipValue.name = "ClipValue";
            _ClipValue.val = "0.002";
            _ClipValue.type = "float";
            _ClipValue.control = "System.Windows.Forms.NumericUpDown";
            _ClipValue.text = "Clip Value";
            _ClipValue.min = 0;
            _ClipValue.max = 1;

            _ClipValueG = new IVLControlProperties();
            _ClipValueG.name = "ClipValueG";
            _ClipValueG.val = "0.5";
            _ClipValueG.type = "float";
            _ClipValueG.control = "System.Windows.Forms.NumericUpDown";
            _ClipValueG.text = "Clip Value Green";
            _ClipValueG.min = 0;
            _ClipValueG.max = 1;


            _ClipValueB = new IVLControlProperties();
            _ClipValueB.name = "ClipValueB";
            _ClipValueB.val = "0.5";
            _ClipValueB.type = "float";
            _ClipValueB.control = "System.Windows.Forms.NumericUpDown";
            _ClipValueB.text = "Clip Value Blue";
            _ClipValueB.min = 0;
            _ClipValueB.max = 1;
        }
    }

    [Serializable]
    public class BrightnessContrastSettings
    {
        private IVLControlProperties BrightnessVal = null;

        public IVLControlProperties _BrightnessVal
        {
            get { return BrightnessVal; }
            set { BrightnessVal = value; }
        }

        private IVLControlProperties ContrastVAl = null;

        public IVLControlProperties _ContrastVAl
        {
            get { return ContrastVAl; }
            set { ContrastVAl = value; }
        }

        private IVLControlProperties IsApplyBrightness = null;

        public IVLControlProperties _IsApplyBrightness
        {
            get { return IsApplyBrightness; }
            set { IsApplyBrightness = value; }
        }

        private IVLControlProperties IsApplyContrast = null;

        public IVLControlProperties _IsApplyContrast
        {
            get { return  IsApplyContrast; }
            set {  IsApplyContrast = value; }
        }
        public BrightnessContrastSettings()
        {

            _IsApplyContrast = new IVLControlProperties();
            _IsApplyContrast.name = "IsApplyContrast";
            _IsApplyContrast.val = false.ToString();
            _IsApplyContrast.type = "bool";
            _IsApplyContrast.control = "System.Windows.Forms.RadioButton";
            _IsApplyContrast.text = "Apply Contrast";

            _IsApplyBrightness = new IVLControlProperties();
            _IsApplyBrightness.name = "IsApplyBrightness";
            _IsApplyBrightness.val = false.ToString();
            _IsApplyBrightness.type = "bool";
            _IsApplyBrightness.control = "System.Windows.Forms.RadioButton";
            _IsApplyBrightness.text = "Apply Brightness";

            _BrightnessVal = new IVLControlProperties();
            _BrightnessVal.name = "BrightnessVal";
            _BrightnessVal.val = "3";
            _BrightnessVal.type = "int";
            _BrightnessVal.control = "System.Windows.Forms.NumericUpDown";
            _BrightnessVal.text = "Brightness Value";
            _BrightnessVal.min = 0;
            _BrightnessVal.max = 100;

            _ContrastVAl = new IVLControlProperties();
            _ContrastVAl.name = "ContrastVAl";
            _ContrastVAl.val = "1";
            _ContrastVAl.type = "int";
            _ContrastVAl.control = "System.Windows.Forms.NumericUpDown";
            _ContrastVAl.text = "Contrast Value";
            _ContrastVAl.min = -20;
            _ContrastVAl.max = 20;

        }
    }
    [Serializable]
    public class LutSettings
    {

        private  IVLControlProperties IsApplyLutSettings = null;

        public IVLControlProperties _IsApplyLutSettings
        {
            get { return IsApplyLutSettings; }
            set { IsApplyLutSettings = value; }
        }

        

        
        private  IVLControlProperties LUTSineFactor = null;

        public IVLControlProperties _LUTSineFactor
        {
            get { return LUTSineFactor; }
            set { LUTSineFactor = value; }
        }

        private  IVLControlProperties LUTInterval1 = null;

        public IVLControlProperties _LUTInterval1
        {
            get { return LUTInterval1; }
            set { LUTInterval1 = value; }
        }

        private  IVLControlProperties LUTInterval2 = null;

        public IVLControlProperties _LUTInterval2
        {
            get { return LUTInterval2; }
            set { LUTInterval2 = value; }
        }

        private  IVLControlProperties LUTOffset= null;

        public IVLControlProperties _LUTOffset
        {
            get { return LUTOffset; }
            set { LUTOffset = value; }
        }


        private IVLControlProperties IsChannelWiseLUT = null;

        public IVLControlProperties _IsChannelWiseLUT
        {
            get { return IsChannelWiseLUT; }
            set { IsChannelWiseLUT = value; }
        }

        private IVLControlProperties lUT_SineFactor_R = null;

        public IVLControlProperties LUT_SineFactor_R
        {
            get { return lUT_SineFactor_R; }
            set { lUT_SineFactor_R = value; }
        }

        private IVLControlProperties lUT_interval1_R = null;

        public IVLControlProperties LUT_interval1_R
        {
            get { return lUT_interval1_R; }
            set { lUT_interval1_R = value; }
        }

        private IVLControlProperties lUT_interval2_R = null;

        public IVLControlProperties LUT_interval2_R
        {
            get { return lUT_interval2_R; }
            set { lUT_interval2_R = value; }
        }

        private IVLControlProperties lUT_Offset_R = null;

        public IVLControlProperties LUT_Offset_R
        {
            get { return lUT_Offset_R; }
            set { lUT_Offset_R = value; }
        }

        private IVLControlProperties lUT_SineFactor_G = null;

        public IVLControlProperties LUT_SineFactor_G
        {
            get { return lUT_SineFactor_G; }
            set { lUT_SineFactor_G = value; }
        }

        private IVLControlProperties lUT_interval1_G = null;

        public IVLControlProperties LUT_interval1_G
        {
            get { return lUT_interval1_G; }
            set { lUT_interval1_G = value; }
        }

        private IVLControlProperties lUT_interval2_G = null;

        public IVLControlProperties LUT_interval2_G
        {
            get { return lUT_interval2_G; }
            set { lUT_interval2_G = value; }
        }

        private IVLControlProperties lUT_Offset_G = null;

        public IVLControlProperties LUT_Offset_G
        {
            get { return lUT_Offset_G; }
            set { lUT_Offset_G = value; }
        }

        private IVLControlProperties lUT_SineFactor_B = null;

        public IVLControlProperties LUT_SineFactor_B
        {
            get { return lUT_SineFactor_B; }
            set { lUT_SineFactor_B = value; }
        }

        private IVLControlProperties lUT_interval1_B = null;

        public IVLControlProperties LUT_interval1_B
        {
            get { return lUT_interval1_B; }
            set { lUT_interval1_B = value; }
        }

        private IVLControlProperties lUT_interval2_B = null;

        public IVLControlProperties LUT_interval2_B
        {
            get { return lUT_interval2_B; }
            set { lUT_interval2_B = value; }
        }

        private IVLControlProperties lUT_Offset_B = null;

        public IVLControlProperties LUT_Offset_B
        {
            get { return lUT_Offset_B; }
            set { lUT_Offset_B = value; }
        }

        public LutSettings()
        {
            _IsApplyLutSettings = new IVLControlProperties();
            _IsApplyLutSettings.name = "IsApplyLutSettings";
            _IsApplyLutSettings.val = true.ToString();
            _IsApplyLutSettings.type = "bool";
            _IsApplyLutSettings.control = "System.Windows.Forms.RadioButton";
            _IsApplyLutSettings.text = "Apply Lut";

           

            _LUTSineFactor = new IVLControlProperties();
            _LUTSineFactor.name = "LUTSineFactor";
            _LUTSineFactor.val = "35";
            _LUTSineFactor.type = "int";
            _LUTSineFactor.control = "System.Windows.Forms.NumericUpDown";
            _LUTSineFactor.text = "LutSine Factor";
            _LUTSineFactor.min = 0;
            _LUTSineFactor.max = 255;

            _LUTInterval1 = new IVLControlProperties();
            _LUTInterval1.name = "LUTInterval1";
            _LUTInterval1.val = "50";
            _LUTInterval1.type = "int";
            _LUTInterval1.control = "System.Windows.Forms.NumericUpDown";
            _LUTInterval1.text = "Lut Interval1";
            _LUTInterval1.min = 0;
            _LUTInterval1.max = 255;

            _LUTInterval2 = new IVLControlProperties();
            _LUTInterval2.name = "LUTInterval2";
            _LUTInterval2.val = "130";
            _LUTInterval2.type = "int";
            _LUTInterval2.control = "System.Windows.Forms.NumericUpDown";
            _LUTInterval2.text = "Lut Interval2";
            _LUTInterval2.min = 0;
            _LUTInterval2.max = 255;

            _LUTOffset = new IVLControlProperties();
            _LUTOffset.name = "LUTOffset";
            _LUTOffset.val = "0";
            _LUTOffset.type = "int";
            _LUTOffset.control = "System.Windows.Forms.NumericUpDown";
            _LUTOffset.text = "Lut Offset";
            _LUTOffset.min = 0;
            _LUTOffset.max = 255;


            _IsChannelWiseLUT = new IVLControlProperties();
            _IsChannelWiseLUT.name = "IsChannelWiseLUT";
            _IsChannelWiseLUT.val = false.ToString();
            _IsChannelWiseLUT.type = "bool";
            _IsChannelWiseLUT.control = "System.Windows.Forms.RadioButton";
            _IsChannelWiseLUT.text = "Apply Channel wise Lut";


            LUT_SineFactor_R = new IVLControlProperties();
            LUT_SineFactor_R.name = "LUTSineFactorR";
            LUT_SineFactor_R.val = "20";
            LUT_SineFactor_R.type = "int";
            LUT_SineFactor_R.control = "System.Windows.Forms.NumericUpDown";
            LUT_SineFactor_R.text = "R LutSine Factor";
            LUT_SineFactor_R.min = 0;
            LUT_SineFactor_R.max = 255;

            LUT_interval1_R = new IVLControlProperties();
            LUT_interval1_R.name = "LUTInterval1R";
            LUT_interval1_R.val = "100";
            LUT_interval1_R.type = "int";
            LUT_interval1_R.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval1_R.text = "R Lut Interval1";
            LUT_interval1_R.min = 0;
            LUT_interval1_R.max = 255;

            LUT_interval2_R = new IVLControlProperties();
            LUT_interval2_R.name = "LUTInterval2R";
            LUT_interval2_R.val = "150";
            LUT_interval2_R.type = "int";
            LUT_interval2_R.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval2_R.text = "R Lut Interval2";
            LUT_interval2_R.min = 0;
            LUT_interval2_R.max = 255;

            LUT_Offset_R = new IVLControlProperties();
            LUT_Offset_R.name = "LUTOffsetR";
            LUT_Offset_R.val = "0";
            LUT_Offset_R.type = "int";
            LUT_Offset_R.control = "System.Windows.Forms.NumericUpDown";
            LUT_Offset_R.text = "R Lut Offset";
            LUT_Offset_R.min = 0;
            LUT_Offset_R.max = 255;


            LUT_SineFactor_G = new IVLControlProperties();
            LUT_SineFactor_G.name = "LUTSineFactorG";
            LUT_SineFactor_G.val = "20";
            LUT_SineFactor_G.type = "int";
            LUT_SineFactor_G.control = "System.Windows.Forms.NumericUpDown";
            LUT_SineFactor_G.text = "G LutSine Factor";
            LUT_SineFactor_G.min = 0;
            LUT_SineFactor_G.max = 255;

            LUT_interval1_G = new IVLControlProperties();
            LUT_interval1_G.name = "LUTInterval1G";
            LUT_interval1_G.val = "80";
            LUT_interval1_G.type = "int";
            LUT_interval1_G.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval1_G.text = "G Lut Interval1";
            LUT_interval1_G.min = 0;
            LUT_interval1_G.max = 255;

            LUT_interval2_G = new IVLControlProperties();
            LUT_interval2_G.name = "LUTInterval2G";
            LUT_interval2_G.val = "100";
            LUT_interval2_G.type = "int";
            LUT_interval2_G.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval2_G.text = "G Lut Interval2";
            LUT_interval2_G.min = 0;
            LUT_interval2_G.max = 255;

            LUT_Offset_G = new IVLControlProperties();
            LUT_Offset_G.name = "LUTOffsetG";
            LUT_Offset_G.val = "0";
            LUT_Offset_G.type = "int";
            LUT_Offset_G.control = "System.Windows.Forms.NumericUpDown";
            LUT_Offset_G.text = "G Lut Offset";
            LUT_Offset_G.min = 0;
            LUT_Offset_G.max = 255;


            LUT_SineFactor_B = new IVLControlProperties();
            LUT_SineFactor_B.name = "LUTSineFactorB";
            LUT_SineFactor_B.val = "35";
            LUT_SineFactor_B.type = "int";
            LUT_SineFactor_B.control = "System.Windows.Forms.NumericUpDown";
            LUT_SineFactor_B.text = "B LutSine Factor";
            LUT_SineFactor_B.min = 0;
            LUT_SineFactor_B.max = 255;

            LUT_interval1_B = new IVLControlProperties();
            LUT_interval1_B.name = "LUTInterval1B";
            LUT_interval1_B.val = "50";
            LUT_interval1_B.type = "int";
            LUT_interval1_B.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval1_B.text = "B Lut Interval1";
            LUT_interval1_B.min = 0;
            LUT_interval1_B.max = 255;

            LUT_interval2_B = new IVLControlProperties();
            LUT_interval2_B.name = "LUTInterval2B";
            LUT_interval2_B.val = "130";
            LUT_interval2_B.type = "int";
            LUT_interval2_B.control = "System.Windows.Forms.NumericUpDown";
            LUT_interval2_B.text = "B Lut Interval2";
            LUT_interval2_B.min = 0;
            LUT_interval2_B.max = 255;

            LUT_Offset_B = new IVLControlProperties();
            LUT_Offset_B.name = "LUTOffsetB";
            LUT_Offset_B.val = "0";
            LUT_Offset_B.type = "int";
            LUT_Offset_B.control = "System.Windows.Forms.NumericUpDown";
            LUT_Offset_B.text = "B Lut Offset";
            LUT_Offset_B.min = 0;
            LUT_Offset_B.max = 255;


        }
    }

    [Serializable]
    public class GammaSettings
    {
        private IVLControlProperties isApplyGammaSettings = null;

        public IVLControlProperties IsApplyGammaSettings
        {
            get { return isApplyGammaSettings; }
            set { isApplyGammaSettings = value; }
        }

        private IVLControlProperties gammaValue = null;

        public IVLControlProperties GammaValue
        {
            get { return gammaValue; }
            set { gammaValue = value; }
        }
        public GammaSettings()
        {
            IsApplyGammaSettings = new IVLControlProperties();
            IsApplyGammaSettings.name = "IsApplyGammaSettings";
            IsApplyGammaSettings.val = false.ToString();
            IsApplyGammaSettings.type = "bool";
            IsApplyGammaSettings.control = "System.Windows.Forms.RadioButton";
            IsApplyGammaSettings.text = "Apply Gamma";

            GammaValue = new IVLControlProperties();
            GammaValue.name = "GammaValue";
            GammaValue.val = "1.15";
            GammaValue.type = "float";
            GammaValue.control = "System.Windows.Forms.NumericUpDown";
            GammaValue.text = "Gamma Value";
            GammaValue.min = 0;
            GammaValue.max = 10;

        }
    }


}
