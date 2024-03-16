using System;

namespace TheHuntIsOn;

[Serializable]
public class SceneBoolData : PersistentBoolData
{
    #region Properties

    /// <summary>
    /// Gets or sets whether this flag is related to a boss.
    /// </summary>
    public bool BossFlag { get; set; }

    #endregion
}
