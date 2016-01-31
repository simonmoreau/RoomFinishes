#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RoomsFinishesControl : Window
    {
        private Document _doc;
        private UIDocument _UIDoc;

        private WallType _selectedWallType;
        public WallType SelectedWallType
        {
            get { return _selectedWallType; }
        }

        private WallType _duplicatedWallType;
        public WallType DuplicatedWallType
        {
            get { return _duplicatedWallType; }
        }

        private double _boardHeight;
        public double BoardHeight
        {
            get { return _boardHeight; }
        }

        private IEnumerable<Room> _selectedRooms;
        public IEnumerable<Room> SelectedRooms
        {
            get { return _selectedRooms; }
        }

        private IEnumerable<WallType> _wallTypes;
        public RoomsFinishesControl(UIDocument UIDoc)
        {
            InitializeComponent();
            _doc = UIDoc.Document;
            _UIDoc = UIDoc;

            //Fill out Text in form
            this.Title = Tools.LangResMan.GetString("roomFinishes_TaskDialogName", Tools.Cult);
            this.all_rooms_radio.Content = Tools.LangResMan.GetString("roomFinishes_all_rooms_radio", Tools.Cult);
            this.board_height_label.Content = Tools.LangResMan.GetString("roomFinishes_board_height_label", Tools.Cult);
            this.select_wall_label.Content = Tools.LangResMan.GetString("roomFinishes_select_wall_label", Tools.Cult);
            this.selected_rooms_radio.Content = Tools.LangResMan.GetString("roomFinishes_selected_rooms_radio", Tools.Cult);
            this.Cancel_Button.Content = Tools.LangResMan.GetString("roomFinishes_Cancel_Button", Tools.Cult);
            this.Ok_Button.Content = Tools.LangResMan.GetString("roomFinishes_OK_Button", Tools.Cult);



            //Select the wall type in the document
            _wallTypes = from elem in new FilteredElementCollector(_doc).OfClass(typeof(WallType))
                                              let type = elem as WallType
                                              where type.Kind == WallKind.Basic
                                              select type;

            // Bind ArrayList with the ListBox
            WallTypeListBox.ItemsSource = _wallTypes;
            WallTypeListBox.SelectedItem = WallTypeListBox.Items[0];
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Tools.GetValueFromString(Height_TextBox.Text, _doc.GetUnits()) != null)
            {
                _boardHeight = (double)Tools.GetValueFromString(Height_TextBox.Text, _doc.GetUnits());

                if (WallTypeListBox.SelectedItem != null)
                {
                    //Select wall type for skirting board
                    _selectedWallType = WallTypeListBox.SelectedItem as WallType;
                    //Duplicate and double thickness of the wall type
                    _duplicatedWallType = CreateNewWallType(_selectedWallType);

                    this.DialogResult = true;
                    this.Close();

                    //Select the rooms
                    _selectedRooms = SelectRooms();
                }
            }
            else
            {
                TaskDialog.Show(Tools.LangResMan.GetString("roomFinishes_TaskDialogName", Tools.Cult),
                    Tools.LangResMan.GetString("roomFinishes_heightValueError", Tools.Cult), TaskDialogCommonButtons.Close, TaskDialogResult.Close);
                this.Activate();
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private WallType CreateNewWallType(WallType wallType)
        {
            WallType newWallType;
            List<string> wallTypesNames = _wallTypes.Select(o => o.Name).ToList();

            if (!wallTypesNames.Contains("newWallTypeName"))
            {
                newWallType = wallType.Duplicate("newWallTypeName") as WallType;
            }
            else
            {
                newWallType = wallType.Duplicate("newWallTypeName2") as WallType;
            }

            

            CompoundStructure cs = newWallType.GetCompoundStructure();

            IList<CompoundStructureLayer> layers = cs.GetLayers();
            int layerIndex = 0;

            foreach (CompoundStructureLayer csl in layers)
            {
                double layerWidth = csl.Width * 2;
                if (cs.GetRegionsAssociatedToLayer(layerIndex).Count == 1)
                {
                    try
                    {
                        cs.SetLayerWidth(layerIndex, layerWidth);
                    }
                    catch
                    {
                        throw new ErrorMessageException(Tools.LangResMan.GetString("roomFinishes_verticallyCompoundError", Tools.Cult));
                    }
                }
                else
                {
                    throw new ErrorMessageException(Tools.LangResMan.GetString("roomFinishes_verticallyCompoundError", Tools.Cult));
                }

                layerIndex++;
            }

            newWallType.SetCompoundStructure(cs);

            return newWallType;
        }

        private IEnumerable<Room> SelectRooms()
        {
            //Create a set of selected elements ids
            ICollection<ElementId> selectedObjectsIds = _UIDoc.Selection.GetElementIds();

            //Create a set of rooms
            IEnumerable<Room> ModelRooms = null;
            IList<Room> tempList = new List<Room>();

            if (all_rooms_radio.IsChecked.Value)
            {
                // Find all rooms in current view
                ModelRooms = from elem in new FilteredElementCollector(_doc, _doc.ActiveView.Id).OfClass(typeof(SpatialElement))
                             let room = elem as Room
                             select room;
            }
            else
            {
                if (selectedObjectsIds.Count != 0)
                {
                    // Find all rooms in selection
                    ModelRooms = from elem in new FilteredElementCollector(_doc, selectedObjectsIds).OfClass(typeof(SpatialElement))
                                 let room = elem as Room
                                 select room;
                    tempList = ModelRooms.ToList();
                }

                if (tempList.Count == 0)
                {
                    //Create a selection filter on rooms
                    ISelectionFilter filter = new RoomSelectionFilter();

                    IList<Reference> rs = _UIDoc.Selection.PickObjects(ObjectType.Element, filter,
                        Tools.LangResMan.GetString("roomFinishes_SelectRooms", Tools.Cult));

                    foreach (Reference r in rs)
                    {
                        tempList.Add(_doc.GetElement(r) as Room);
                    }

                    ModelRooms = tempList;
                }
            }

            return ModelRooms;
        }

    }

    public class RoomSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rooms)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}
