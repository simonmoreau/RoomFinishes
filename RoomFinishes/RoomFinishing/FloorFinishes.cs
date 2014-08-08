#region Namespaces
using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Globalization;
using System.Resources;
using RoomFinishes;
#endregion

namespace RoomFinishes.RoomsFinishes
{
    [Transaction(TransactionMode.Manual)]
    class FloorFinishes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UIdoc = commandData.Application.ActiveUIDocument;
            Document doc = UIdoc.Document;

            using (Transaction tx = new Transaction(doc))
            {
                try
                {
                    // Add Your Code Here
                    FloorFinish(UIdoc, tx);
                    // Return Success
                    return Result.Succeeded;
                }

                catch (Autodesk.Revit.Exceptions.OperationCanceledException exceptionCanceled)
                {
                    message = exceptionCanceled.Message;
                    tx.RollBack();
                    return Autodesk.Revit.UI.Result.Cancelled;
                }
                catch (ErrorMessageException errorEx)
                {
                    // checked exception need to show in error messagebox
                    message = errorEx.Message;
                    tx.RollBack();
                    return Autodesk.Revit.UI.Result.Failed;
                }
                catch (Exception ex)
                {
                    // unchecked exception cause command failed
                    message = Tools.LangResMan.GetString("floorFinishes_unexpectedError", Tools.Cult) + ex.Message;
                    //Trace.WriteLine(ex.ToString());
                    tx.RollBack();
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
        }

        void FloorFinish(UIDocument UIDoc, Transaction tx)
        {
            Document _doc = UIDoc.Document;

            tx.Start(Tools.LangResMan.GetString("floorFinishes_transactionName", Tools.Cult));

            //Load the selection form

            FloorsFinishesControl userControl = new FloorsFinishesControl(UIDoc);
            userControl.InitializeComponent();

            if (userControl.ShowDialog() == true)
            {
                //Select floor types
                FloorType flType = userControl.SelectedFloorType;

                //Select Rooms in model
                IEnumerable<Room> ModelRooms = userControl.SelectedRooms;

                foreach (Room tempRoom in ModelRooms)
                {
                    if (tempRoom != null)
                    {
                        if (tempRoom.UnboundedHeight != 0)
                        {
                            //Get all finish properties
                            double height;
                            if (userControl.RoomParameter == null)
                            {
                                height = userControl.FloorHeight * 0.00328084;
                            }
                            else
                            {
                                Parameter tempRoomParam = tempRoom.get_Parameter(userControl.RoomParameter.Definition.Name);
                                height = tempRoomParam.AsDouble();
                            }

                            string name = tempRoom.Name;

                            SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();

                            IList<IList<Autodesk.Revit.DB.BoundarySegment>> boundarySegments = tempRoom.GetBoundarySegments(opt);

                            CurveArray crvArray = new CurveArray();

                            if (boundarySegments.Count != 0)
                            {
                                foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundarySegments.First())
                                {
                                    crvArray.Append(boundSeg.Curve);
                                }

                                //Retrive room info
                                Level rmLevel = _doc.GetElement(tempRoom.LevelId) as Level;
                                Parameter param = tempRoom.get_Parameter(BuiltInParameter.ROOM_HEIGHT);
                                double rmHeight = param.AsDouble();

                                if (crvArray.Size != 0)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    Floor floor = _doc.Create.NewFloor(crvArray, flType, rmLevel, false);

                                    //Change some param on the floor
                                    param = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                                    param.Set(height);
                                }
                            }
                        }
                    }

                }
				

                tx.Commit();
            }
            else
            {
                tx.RollBack();
            }
        }
    }
}
