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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
#endregion

namespace RoomFinishes
{
    [Transaction(TransactionMode.Manual)]
    public class FloorFinishing : IExternalCommand
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
                    if (tx.HasStarted())
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Cancelled;
                }
                catch (ErrorMessageException errorEx)
                {
                    // checked exception need to show in error messagebox
                    message = errorEx.Message;
                    if (tx.HasStarted())
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Failed;
                }
                catch (Exception ex)
                {
                    // unchecked exception cause command failed
                    message = Tools.LangResMan.GetString("floorFinishes_unexpectedError", Tools.Cult) + ex.Message;
                    //Trace.WriteLine(ex.ToString());
                    if (tx.HasStarted())
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
        }

        void FloorFinish(UIDocument UIDoc, Transaction tx)
        {
            Document document = UIDoc.Document;

            FloorsFinishesSetup floorsFinishesSetup = new FloorsFinishesSetup();


            //Load the selection form

            FloorsFinishesControl floorsFinishesControl = new FloorsFinishesControl(UIDoc, floorsFinishesSetup);
            floorsFinishesControl.InitializeComponent();

            if (floorsFinishesControl.ShowDialog() == true)
            {
                CreateFloors(document, floorsFinishesSetup, tx);
            }
            else
            {
                if (tx.HasStarted())
                {
                    tx.RollBack();
                }
            }
        }

        public void CreateFloors(Document document, FloorsFinishesSetup floorsFinishesSetup, Transaction tx)
        {
            tx.Start(Tools.LangResMan.GetString("floorFinishes_transactionName", Tools.Cult));

            foreach (Room room in floorsFinishesSetup.SelectedRooms)
            {
                if (room != null)
                {
                    if (room.UnboundedHeight != 0)
                    {
                        //Get all finish properties
                        double height;
                        if (floorsFinishesSetup.RoomParameter == null)
                        {
                            height = floorsFinishesSetup.FloorHeight;
                        }
                        else
                        {
                            Parameter roomParameter = room.get_Parameter(floorsFinishesSetup.RoomParameter.Definition);
                            height = roomParameter.AsDouble();
                        }

                        SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();


                        IList<IList<Autodesk.Revit.DB.BoundarySegment>> boundarySegments = room.GetBoundarySegments(opt);

                        CurveArray curveArray = new CurveArray();
                        List<CurveLoop> curveLoops = new List<CurveLoop>();

                        if (boundarySegments.Count != 0)
                        {
                            foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundarySegments.First())
                            {
                                curveArray.Append(boundSeg.GetCurve());
                            }

                            foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundSegs in boundarySegments)
                            {
                                CurveLoop curveLoop = new CurveLoop();

                                foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundSegs)
                                {
                                    curveLoop.Append(boundSeg.GetCurve());
                                }

                                curveLoops.Add(curveLoop);
                            }

                            //Retrive room info
                            Level rmLevel = document.GetElement(room.LevelId) as Level;
                            Parameter param = room.get_Parameter(BuiltInParameter.ROOM_HEIGHT);
                            double rmHeight = param.AsDouble();

                            if (curveArray.Size != 0)
                            {

#if Version2022 || Version2023 || Version2024
                                Floor floor = Floor.Create(document, curveLoops, floorsFinishesSetup.SelectedFloorType.Id, rmLevel.Id);

#elif Version2019 || Version2020 || Version2021
                            Floor floor = document.Create.NewFloor(curveArray, floorsFinishesSetup.SelectedFloorType, rmLevel, false);
#endif

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
    }
}
