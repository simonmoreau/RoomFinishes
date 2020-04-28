// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System.Linq;


namespace RoomFinishes.Test
{
    [TestFixture]
    public class RoomFinishingTestClass
    {
        private Document document;
        [SetUp]
        public void RunBeforeTest(UIApplication uiApplication)
        {
            string versionName = uiApplication.Application.VersionName.Replace("Autodesk Revit ", "");

            string path = $"G:\\My Drive\\05 - Travail\\Revit Dev\\RoomFinishes\\Test Models\\RoomFinishes_TestModel_{versionName}.rvt";

            UIDocument uIDocument = uiApplication.OpenAndActivateDocument(path);
            document = uIDocument.Document;
            Console.WriteLine($"Run 'SetUp' in {GetType().Name}");
            Console.WriteLine($"Open the RoomFinishes_TestModel_{versionName} model.");
        }

        [TearDown]
        public void RunAfterTest()
        {
            Console.WriteLine($"Run 'TearDown' in {GetType().Name}");
        }

        [Test]
        public void CreateSkirtingBoardTest()
        {
            int[] roomsIds = new int[] { 201348, 201351, 201408, 201411, 201433, 201436, 201439, 202008 };
            int wallTypeId = 201956;

            RoomFinishes.SkirtingBoard skirtingBoard = new SkirtingBoard();
            RoomFinishes.SkirtingBoardSetup skirtingBoardSetup = new SkirtingBoardSetup();

            skirtingBoardSetup.BoardHeight = 1;
            skirtingBoardSetup.JoinWall = true;

            //Select the wall type in the document
            IEnumerable<WallType> wallTypes = from elem in new FilteredElementCollector(document).OfClass(typeof(WallType))
                                              let type = elem as WallType
                                              where type.Id == new ElementId(wallTypeId)
                                              select type;
            skirtingBoardSetup.SelectedWallType = wallTypes.FirstOrDefault();
            skirtingBoardSetup.SelectedRooms = roomsIds.Select(roomId => document.GetElement(new ElementId(roomId)) as Autodesk.Revit.DB.Architecture.Room).ToList();

            using (Transaction tx = new Transaction(document))
            {
                skirtingBoard.CreateSkirtingBoard(document, tx, skirtingBoardSetup);
            }
        }

        [Test]
        public void CreateLargeSkirtingBoardTest()
        {
            int[] roomsIds = new int[] { 202166, 202168, 202171, 202173, 202175, 202177, 202179, 202195 };
            int wallTypeId = 251;

            RoomFinishes.SkirtingBoard skirtingBoard = new SkirtingBoard();
            RoomFinishes.SkirtingBoardSetup skirtingBoardSetup = new SkirtingBoardSetup();

            skirtingBoardSetup.BoardHeight = 1;
            skirtingBoardSetup.JoinWall = true;

            //Select the wall type in the document
            IEnumerable<WallType> wallTypes = from elem in new FilteredElementCollector(document).OfClass(typeof(WallType))
                                              let type = elem as WallType
                                              where type.Id == new ElementId(wallTypeId)
                                              select type;
            skirtingBoardSetup.SelectedWallType = wallTypes.FirstOrDefault();
            skirtingBoardSetup.SelectedRooms = roomsIds.Select(roomId => document.GetElement(new ElementId(roomId)) as Autodesk.Revit.DB.Architecture.Room).ToList();

            using (Transaction tx = new Transaction(document))
            {
                skirtingBoard.CreateSkirtingBoard(document, tx, skirtingBoardSetup);
            }
        }

    }
}
