# TeaInventory

A simple tea inventory manager written entirely in F#

Since it is in F#, which is not one of the standard .NET languages, it will require an additional installation:
http://www.microsoft.com/en-us/download/details.aspx?id=15834

To Install:
Paste the contents of the zip archive in the root folder to the desired directory

To Use:
The input box does not require clicking, it will detect all typing as long as the window has focus
To add a new tea type, type in the name of the category and click "Add Type"
To add a new tea, type in the name of the tea and click "Add Tea" while its category is highlighted
To remove a tea type, highlight the type that you want to delete and click "Delete Type"
To remove a tea, highlight the tea that you want to delete and click "Delete Tea"
To pick a random tea, select all teas that you want to pick from and then click "Pick Random Tea"
  Teas selected for random picking are shown in green type
  Clicking on a tea will toggle its selection
  Select All Teas will select all teas of all types
  Deselect All Teas will deselect all teas of all types
  Select All Types Of Kind selects all teas within the currently chosen type
  Deselect All Types Of Kind deselects all teas within the currently chosen type  

Features:
Allows you to define types of tea (ex: black, white, green, oolong)
Allows you to define invdividual teas (ex: earl grey)
Allows you to increment/decrement the amount of a tea
Allows you to scroll up and down through the list of tea types
Allows you to scroll up and down through the list of teas
Allows you to randomly choose from a set that you define
  Select All Teas will select all teas of all types
  Deselect All Teas will deselect all teas of all types
  Select All Types Of Kind selects all teas within the currently chosen type
  Deselect All Types Of Kind deselects all teas within the currently chosen type
  Clicking on an individual tea will toggle selection of that tea
  Pick Random Tea will display the name and amount available of the randomly chosen tea in the input/output box
    Additionally both lists will be scrolled to include the randomly chosen tea
Automatically saves and loads data
Safeguards are in place to prevent duplicate teas and types
Safeguards are in place to prevent the shared input/output box from using output as input

Written mostly in functional paradigm
Designed to easily allow addition of additional buttons (though I don't know where or what you would add)
