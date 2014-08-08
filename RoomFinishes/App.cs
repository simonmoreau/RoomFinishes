#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
#endregion

namespace RoomFinishes
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                //Define localisation values
                Tools.GetLocalisationValues();

                //Create the panel for BIM42 Tools
                RibbonPanel Bim42Panel = a.CreateRibbonPanel(Tools.LangResMan.GetString("roomFinishes_ribbon_panel_name", Tools.Cult));

                //Create icons in this panel
                Icons.CreateIcons(Bim42Panel);

                return Result.Succeeded;
            }
            catch
            {
                // Return Failure
                return Result.Failed;
            }

        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }

    class Icons
    {
        public static void CreateIcons(RibbonPanel bim42Panel)
        {
            //Retrive dll path
            string DllPath =  Assembly.GetExecutingAssembly().Location;

            //Add RoomsFinishes Button
            string ButtonText = Tools.LangResMan.GetString("roomFinishes_button_name", Tools.Cult);
            PushButtonData FinishButtonData = new PushButtonData("RoomsFiButton", ButtonText, DllPath, "RoomFinishes.RoomsFinishes.RoomsFinishes");
            FinishButtonData.ToolTip = Tools.LangResMan.GetString("roomFinishes_toolTip", Tools.Cult);
            FinishButtonData.LargeImage = RetriveImage("RoomFinishes.Resources.RoomFinishLarge.png");
            FinishButtonData.Image = RetriveImage("RoomFinishes.Resources.RoomFinishSmall.png");
            FinishButtonData.SetContextualHelp(CreateContextualHelp("BIM42Help"));
            //bim42Panel.AddItem(FinishButtonData);

            //Add FloorFinishes Button
            string floorButtonText = Tools.LangResMan.GetString("floorFinishes_ribbon_panel_name", Tools.Cult);
            PushButtonData floorButtonData = new PushButtonData("FloorFiButton", floorButtonText, DllPath, "RoomFinishes.RoomsFinishes.FloorFinishes");
            floorButtonData.ToolTip = Tools.LangResMan.GetString("floorFinishes_toolTip", Tools.Cult);
            floorButtonData.LargeImage = RetriveImage("RoomFinishes.Resources.FloorFinishesLarge.png");
            floorButtonData.Image = RetriveImage("RoomFinishes.Resources.FloorFinishesSmall.png");
            floorButtonData.SetContextualHelp(CreateContextualHelp("BIM42Help"));

            //Group RoomsFinishes button
            SplitButtonData sbRoomData = new SplitButtonData("splitButton2", "BIM 42");
            SplitButton sbRoom = bim42Panel.AddItem(sbRoomData) as SplitButton;
            sbRoom.AddPushButton(FinishButtonData);
            sbRoom.AddPushButton(floorButtonData);

        }

        private static ImageSource RetriveImage(string imagePath)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);

            switch (imagePath.Substring(imagePath.Length - 3))
            {
                case "jpg":
                    var jpgDecoder = new System.Windows.Media.Imaging.JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return jpgDecoder.Frames[0];
                case "bmp":
                    var bmpDecoder = new System.Windows.Media.Imaging.BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return bmpDecoder.Frames[0];
                case "png":
                    var pngDecoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return pngDecoder.Frames[0];
                case "ico":
                    var icoDecoder = new System.Windows.Media.Imaging.IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return icoDecoder.Frames[0];
                default:
                    return null;
            }
        }

        private static ContextualHelp CreateContextualHelp(string helpFile)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if RMFR
            //Get the french documentation
            string HelpName =  helpFile + "Fr.chm";
#else
            //Get the english documentation
            string HelpName = helpFile + "En.chm";
#endif

            //Retrive the directory or create it
            DirectoryInfo HelpDirectoryInfo = Directory.CreateDirectory(Path.Combine(dir, "BIM42Documentation"));
            string HelpPath = Path.Combine(HelpDirectoryInfo.FullName, HelpName);

            //if the help file does not exist, extract it in the HelpDirectory
            if (!File.Exists(HelpPath))
            {
#if RMFR
                //Get the french documentation
                Tools.ExtractRessource("RoomFinishes.Resources.BIM42HelpFr.chm", HelpPath);
#else
            //Get the english documentation
                Tools.ExtractRessource("RoomFinishes.Resources.BIM42HelpEn.chm", HelpPath);
#endif

            }

            return new ContextualHelp(ContextualHelpType.ChmFile, HelpPath);
        }
    }
}
