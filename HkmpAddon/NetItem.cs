using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TheHuntIsOn.HkmpAddon;

[JsonConverter(typeof(StringEnumConverter))]
public enum NetItem
{
    VengefulSpirit = 0,
    DesolateDive,
    HowlingWraiths,
    ShadeSoul,
    DescendingDark,
    AbyssShriek,
    MothwingCloak,
    MantisClaw,
    CrystalHeart,
    MonarchWings,
    IsmasTear,
    ShadeCloak,
    DreamNail,
    CycloneSlash,
    DashSlash,
    GreatSlash,
    Mask,
    SoulVessel,
    NailUpgrade,
    Movement1,
    Movement2,
    Movement3,
    Movement4,
    Movement5,
    Movement6,
    Dreamer
}