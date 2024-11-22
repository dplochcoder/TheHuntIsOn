# TheHuntIsOn

Provides functions split into modules that allow teams to be modified to have various effects.

Under the mod settings the role and the module affection can be selected. **Each module is bound to the selected role**, so if you've chosen the role "Hunter" all modules which are either set to "Hunter" or "Both" are active.

## Other

### Focus Cost
Adjusts the cost of focus to heal. **Note that the extra soul is only taken once the heal has been successful.**

### Focus Speed
Adjusts the time focus takes to provide a heal.

### Retain Bosses
If enabled, certain modules which causes bosses to not appear normally will ignore them instead.

### Shade Platform Mode
Places platforms at certain locations where normally a shade skip can be performed to remove the need for the shade. 
If set to conditional the actual requirements to perform the shade skip will be respected.

## Modules

### Arena Module
Prevents most boss arenas from being entered.

### Bench Module
If enabled, benches do not longer provide healing. *As the code for healing in the game is a bit sketchy, the hud is disabled and enabled quickly to sync it properly.*

### Charm Module
Automatically equips picked up charms and set their cost to 1. **Note, that this doesn't consider actual available notches!**

### Completion Module
Unlocks many walls, doors, dive ground etc. all around the map as it has been visited already. **Unless the setting "Retain Bosses" is enabled, certain bosses will be missing/already defeated as well**

### Dream Entrance Module
Places the entrance points (corpses) for dream bosses outside of their boss arena.

### Dream Heal Module
Exiting a dream sequence, like a boss fight or a dreamer does not longer provide a full heal.

### Elevator Module
Removes all small CoT elevators and places platform to help climbing them (if the cannot be climbed already by having claw for example). Also removes the lever in the big elevator and adds a door transition there to enter the other half more quickly.

### Enemy Module
Removes all enemies in the game. If "Retain Bosses" is enabled, bosses and their adds will still spawn.

### Lifeseed Module
Causes killing lifeblood seed to not provide any lifeblood.

### Mask Module
Removes the full heal provided by completing a mask. The one extra hp by the mask itself is still provided.

### Notch Module
Picking up new charm notches will no longer fully heal you.

### Respawn Module
Causes the respawn after dying to always locate to KP regardless of the last bench used.

### Shade Module
Removes the shade and all it's effect (removing geo and breaking the soul vessel).

### Stag Module
Disables stag travelling.

### Tram Module
Disables trams.

### Auto Trigger Boss Module
Start certain bosses (like dream warrior) encounter automatically once you enter their range.

### Intangible Gates Module
Makes all arena gates intangible for the player character.

### EventNetworkModule
The EventNetworkModule networks certain events, such as obtained items by the speedrunner and then grants the hunters
items based on this and sends a message.
The module uses a configuration file next to the `.dll` file named `networked-event.json`.
This file determines which events that the speedrunner causes will trigger a set of items to be granted to the hunters.
If there is no such file found next to the `.dll` file, it will generate a default file.

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
`GreatSlash`, `Mask`, `SoulVessel`, `Movement1`, `Movement2`, `Movement3`, `Movement4`, `Movement5`, `Movement6`,
`Dreamer`, `Tram`, `Stag`, `Toll`, `Grub`, `DreamWarrior`, `ShopPurchase`, `NailUpgrade`, `RelicSale`, `LeverHit`, `Notch4`, `Notch5`, `Notch6`, `Notch7`, `Notch8`, `Notch9`, `Notch10`, `Notch11`

The triggers `MovementX` trigger when the speedrunner obtains the X-th movement item. The following are counted as
movement items:
Mothwing Cloak, Mantis Claw, Crystal Heart, Monarch Wings, Isma's Tear, Shade Cloak

The triggers `NotchX` trigger when the speedrunner obtains the X-th Charm Notch.

The items granted under `"items": [ ... ]` can be one of the following:  
`VengefulSpirit`, `DesolateDive`, `HowlingWraiths`, `ShadeSoul`, `DescendingDark`, `AbyssShriek`, `MothwingCloak`,
`MantisClaw`, `CrystalHeart`, `MonarchWings`, `IsmasTear`, `ShadeCloak`, `DreamNail`, `CycloneSlash`, `DashSlash`,
`GreatSlash`, `Mask`, `SoulVessel`, `NailUpgrade`

Note that sales and purchases made using QoL's NPCSellAll module do not currently trigger events.

### Helper Platform Module
Places some platforms around the world to help reaching some ledges with fewer items than actually required.
