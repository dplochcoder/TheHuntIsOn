using TheHuntIsOn.Modules;

namespace TheHuntIsOn;

/// <summary>
/// Represents when the platforms form the <see cref="ShadeModule"/> should appear.
/// </summary>
public enum ShadePlatformMode
{
    /// <summary>
    /// The platforms are not there.
    /// </summary>
    Off,

    /// <summary>
    /// The platforms are only there, when the conditions are met.
    /// </summary>
    WithPrerequisites,

    /// <summary>
    /// The platforms are always there.
    /// </summary>
    On
}
