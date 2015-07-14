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
    class RoomsFinishes : IExternalCommand
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
                    RoomFinish(UIdoc, tx);
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
                    message = Tools.LangResMan.GetString("roomFinishes_unexpectedError", Tools.Cult) + ex.Message;
                    //Trace.WriteLine(ex.ToString());
                    if (tx.HasStarted())
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
        }


        void RoomFinish(UIDocument UIDoc, Transaction tx)
        {
            Document doc = UIDoc.Document;

            tx.Start(Tools.LangResMan.GetString("roomFinishes_transactionName", Tools.Cult));

            //Load the selection form
            RoomsFinishesControl userControl = new RoomsFinishesControl(UIDoc);
            userControl.InitializeComponent();

            if (userControl.ShowDialog() == true)
            {
                //Select wall types
                WallType plinte = userControl.SelectedWallType;
                WallType newWallType = userControl.DuplicatedWallType;

                //Get all finish properties
                double height = userControl.BoardHeight * 0.00328084;

                //Select Rooms in model
                IEnumerable<Room> ModelRooms = userControl.SelectedRooms;

                List<Wall> addedWalls = new List<Wall>();
                IList<ElementId> addedWallsIds = new List<ElementId>();

                //Loop on all rooms to get boundaries
                foreach (Room currentRoom in ModelRooms)
                {
                    ElementId roomLevelId = currentRoom.LevelId;

                    SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                    opt.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

                    IList<IList<Autodesk.Revit.DB.BoundarySegment>> boundarySegmentArray = currentRoom.GetBoundarySegments(opt);
                    if (null == boundarySegmentArray)  //the room may not be bound
                    {
                        continue;
                    }

                    foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundarySegArr in boundarySegmentArray)
                    {
                        if (0 == boundarySegArr.Count)
                        {
                            continue;
                        }
                        else
                        {
                            foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in boundarySegArr)
                            {
                                Wall currentWall = Wall.Create(doc, boundarySegment.Curve,newWallType.Id, roomLevelId, height, 0, false, false);
                                Parameter wallJustification = currentWall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM);
                                wallJustification.Set(2);

                                addedWalls.Add(currentWall);
                                addedWallsIds.Add(currentWall.Id);
                            }
                        }
                    }

                }

                FailureHandlingOptions options = tx.GetFailureHandlingOptions();

                options.SetFailuresPreprocessor(new PlintePreprocessor());
                // Now, showing of any eventual mini-warnings will be postponed until the following transaction.
                tx.Commit(options);

                tx.Start(Tools.LangResMan.GetString("roomFinishes_transactionName", Tools.Cult));

                Wall.ChangeTypeId(doc, addedWallsIds, plinte.Id);

                foreach (Wall addedWall in addedWalls)
                {
                    Parameter wallJustification = addedWall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM);
                    wallJustification.Set(3);
                }

                doc.Delete(newWallType.Id);

                tx.Commit();
            }
            else
            {
                tx.RollBack();
            }
        }

    }
}
