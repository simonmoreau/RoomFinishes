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

            ContextualHelp helpFile = CreateContextualHelp("BIM42Help");

            //Add RoomsFinishes Button
            string ButtonText = Tools.LangResMan.GetString("roomFinishes_button_name", Tools.Cult);
            PushButtonData FinishButtonData = new PushButtonData("RoomsFiButton", ButtonText, DllPath, "RoomFinishes.SkirtingBoard");
            FinishButtonData.ToolTip = Tools.LangResMan.GetString("roomFinishes_toolTip", Tools.Cult);
            FinishButtonData.LargeImage = RetriveImage("RoomFinishes.Resources.room-finishes-large.png");
            FinishButtonData.Image = RetriveImage("RoomFinishes.Resources.room-finishes-small.png");
            FinishButtonData.SetContextualHelp(helpFile);
            //bim42Panel.AddItem(FinishButtonData);

            //Add FloorFinishes Button
            string floorButtonText = Tools.LangResMan.GetString("floorFinishes_ribbon_panel_name", Tools.Cult);
            PushButtonData floorButtonData = new PushButtonData("FloorFiButton", floorButtonText, DllPath, "RoomFinishes.FloorFinishing");
            floorButtonData.ToolTip = Tools.LangResMan.GetString("floorFinishes_toolTip", Tools.Cult);
            floorButtonData.LargeImage = RetriveImage("RoomFinishes.Resources.floor-finishes-large.png");
            floorButtonData.Image = RetriveImage("RoomFinishes.Resources.floor-finishes-small.png");
            floorButtonData.SetContextualHelp(helpFile);

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

            FileInfo dllFileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);

            string helpFilePath = Path.Combine(dllFileInfo.Directory.FullName, "RoomFinishes_Help.html");

            FileInfo helpFileInfo = new FileInfo(helpFilePath);
            if (helpFileInfo.Exists)
            {
                return new ContextualHelp(ContextualHelpType.Url, helpFilePath);
            }
            else
            {
                string dirPath = dllFileInfo.Directory.FullName;
                //Get the english documentation
                string HelpName = helpFile;

                string HelpPath = Path.Combine(dirPath, HelpName);

                //if the help file does not exist, extract it in the HelpDirectory
                //Extract the english documentation

                Tools.ExtractRessource("RoomFinishes.Resources.BIM42HelpEn.chm", HelpPath);

                return new ContextualHelp(ContextualHelpType.ChmFile, HelpPath);
            }
        }
    }
}
