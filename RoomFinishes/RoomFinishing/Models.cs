using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomFinishes.RoomFinishing
{
    public class SkirtingBoardSetup
    {
        public WallType SelectedWallType { get; set; }
        public WallType DuplicatedWallType { get; set; }
        public double BoardHeight { get; set; }
        public bool JoinWall { get; set; }
        public List<Room> SelectedRooms { get; set; }
    }
}
