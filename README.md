# TheHuntIsOn

TODO

## Modules
### ItemNetworkModule
The ItemNetworkModule networks obtained items by the speedrunner and then grants the hunters items based on this.
The module uses a configuration file next to the `.dll` file named `networked-items.json`.
This file determines which items that the speedrunner picks up will trigger another set of items to be granted to the
hunters. If there is no such file found next to the `.dll` file, it will generate a default file.

An example of the format of the file is the following:
```json
{
    "MothwingCloak": {
        "items": [
            "CrystalHeart", "Mask"
        ],
        "message": "Speedrunner has obtained Mothwing Cloak. The hunters have now access to Crystal Heart and gained an additional mask."
    },
    "MantisClaw": {
        "items": [
            "MonarchWings"
        ],
        "message": "Speedrunner has obtained Mantis Claw. All players have now access to Monarch Wings."
    }
}
```
This configuration will make it so that when the speedrunner obtains the Mothwing Cloak, the hunters will 
automatically get Crystal Heart and get an additional mask. The message will also be broadcast to all players to
the server. And if the speedrunner obtains Mantis Claw, the hunters will get Monarch Wings.

The first string in each entry represents the trigger and can be one of the following:  
`VengefulSpirit`, `DesolateDive`, `HowlingWraiths`, `ShadeSoul`, `DescendingDark`, `AbyssShriek`, `MothwingCloak`,
`MantisClaw`, `CrystalHeart`, `MonarchWings`, `IsmasTear`, `ShadeCloak`, `DreamNail`, `CycloneSlash`, `DashSlash`,
`GreatSlash`, `Movement1`, `Movement2`, `Movement3`, `Movement4`, `Movement5`, `Movement6`

The triggers `MovementX` trigger when the speedrunner obtains the X-th movement item. The following are counted as
movement items:
Mothwing Cloak, Mantis Claw, Crystal Heart, Monarch Wings, Isma's Tear, Shade Cloak

The items granted under `"items": [ ... ]` can be one of the following:  
`VengefulSpirit`, `DesolateDive`, `HowlingWraiths`, `ShadeSoul`, `DescendingDark`, `AbyssShriek`, `MothwingCloak`,
`MantisClaw`, `CrystalHeart`, `MonarchWings`, `IsmasTear`, `ShadeCloak`, `DreamNail`, `CycloneSlash`, `DashSlash`,
`GreatSlash`, `Mask`, `NailUpgrade`