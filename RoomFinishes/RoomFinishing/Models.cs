using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomFinishes
{
    public class SkirtingBoardSetup
    {
        public WallType SelectedWallType { get; set; }
        public double BoardHeight { get; set; }
        public bool JoinWall { get; set; }
        public List<Room> SelectedRooms { get; set; }
    }

    public class FloorsFinishesSetup
    {
        public FloorType SelectedFloorType { get; set; }
        public double FloorHeight { get; set; }
        public Parameter RoomParameter { get; set; }
        public List<Room> SelectedRooms { get; set; }
    }
}
