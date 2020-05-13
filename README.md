# Selection Navigator for Unity
# Description:
Adds a selection navigation possibility to Unity.

You can go through the previously selected Objects with ctrl+G (back) and alt+G (forward).
Once you go back and then click on any new object, the list head is reset.

The script does not work for objects in another scene, object referenced in other scenes are removed from the list on scene change.

## Installation:
### Using Unity's package manager:
Add the line
```
"ch.giezi.tools.selectionnavigator": "https://github.com/GieziJo/SelectionNavigator.git#master"
```
to the file `Packages/manifest.json` under `dependencies`.

### Alternative:
Copy the file `SelectionNavigator.cs` to your Editor folder inside Unity.