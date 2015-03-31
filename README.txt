# TeaInventory

A simple tea inventory manager written entirely in F#

I actually don't know how to distribute my software, but copy the executable and font.png into the same folder

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
