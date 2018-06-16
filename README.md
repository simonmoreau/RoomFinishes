<p align="center"><img width=12.5% src="https://raw.githubusercontent.com/simonmoreau/RoomFinishes/master/RoomFinishes/Resources/Room.jpg"></p>
<h1 align="center">
  Room Finishing
</h1>

<h4 align="center">A Revit addin to model skirting boards and floor finishes.</h4>

# Overview

Room Finishing is a Revit addin to model skirting boards and floor finishes.

Creating architectural details for shop drawings can be particularly long and tedious when it comes to modeling every detail in a room.

The Room Finishing application allows you to automatically create a skirting board or a finish floor all the way around any architectural room.

Just create a type of wall to be used as a baseboard or a type of floor to be used as finish, select a set of rooms or a single room and the application will create the skirting board and the finish with the proper height.

More information about about the inner working of Room Finishing can be found [here](https://www.bim42.com/2014/02/modellingskirtingboards/), [here](https://www.bim42.com/2016/07/room-finishes-update-2/) and [here](https://www.bim42.com/2014/08/room-finishes-update/).

![Overview](https://raw.githubusercontent.com/simonmoreau/RoomFinishes/master/RoomFinishes/Resources/RoomFinishes.gif)

# Getting Started

Edit _RoomFinishes.csproj_, and make sure that the following lines a correctly pointing to your Revit installation folder:
* Line 72:     <HintPath>$(ProgramW6432)\Autodesk\Revit 2019\RevitAPI.dll</HintPath>
* Line 76:     <HintPath>$(ProgramW6432)\Autodesk\Revit 2019\RevitAPIUI.dll</HintPath>

Open the solution in Visual Studio 2017, buid it, and click on "Attach to process" to run Revit in debug mode. You can found more detail on how to run and debug a Revit addin in this [great blog post](http://archi-lab.net/debugging-revit-add-ins/).

## Installation

There is two ways to install this plugin in Revit:

### The easy way

Download the installer on the [Autodesk App Exchange](https://apps.autodesk.com/RVT/en/Detail/Index?id=5641957956279354474&appLang=en&os=Win64)

### The (not so) easy way

You install Room Finishing just [like any other Revit add-in](http://help.autodesk.com/view/RVT/2018/ENU/?guid=GUID-4FFDB03E-6936-417C-9772-8FC258A261F7), by copying the add-in manifest (_"RoomFinishes.addin"_) and the assembly DLL (_"RoomFinishes.dll"_) in the Revit Add-Ins folder (%APPDATA%\Autodesk\Revit\Addins\2019).

If you specify the full DLL pathname in the add-in manifest, it can also be located elsewhere.

## Built With

* .NET Framework 4.7 and [Visual Studio Community](https://www.visualstudio.com/vs/community/)
* The Visual Studio Revit C# and VB add-in templates from [The Building Coder](http://thebuildingcoder.typepad.com/blog/2017/04/revit-2018-visual-studio-c-and-vb-net-add-in-wizards.html)

# Development

Want to contribute? Great, I would be happy to integrate your improvements!

To fix a bug or enhance an existing module, follow these steps:

* Fork the repo
* Create a new branch (`git checkout -b improve-feature`)
* Make the appropriate changes in the files
* Add changes to reflect the changes made
* Commit your changes (`git commit -am 'Improve feature'`)
* Push to the branch (`git push origin improve-feature`)
* Create a Pull Request

# Bug / Feature Request

If you find a bug (values not added, error while running the application, ...), kindly open an issue [here](https://github.com/simonmoreau/RoomFinishes/issues/new) by including a screenshot of your problem and the expected result.

If you'd like to request a new function, feel free to do so by opening an issue [here](https://github.com/simonmoreau/RoomFinishes/issues/new). Please include workflows samples and their corresponding results.

# License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

# Contact information

This software is an open-source project mostly maintained by myself, Simon Moreau. If you have any questions or request, feel free to contact me at [simon@bim42.com](mailto:simon@bim42.com) or on Twitter [@bim42](https://twitter.com/bim42?lang=en).

# Revision list

| **Version Number** | **Version Description** |
| :-------------: |:-------------|
1.6.0|Support for Autodesk® Revit® 2019 Version.
1.5.0|Support for Autodesk® Revit® 2018 Version. Room Separation Line are no longer being used as a support for skirting board.
1.4.0|Support for Autodesk® Revit® 2017 Version. Add Join Geometry to join the skirting board with the wall. Use Revit default unit system. Remove unwanted warnings. Order Wall Types and Floor types by name. Bug fixes.
1.0.0|First release