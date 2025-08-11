using System;
using System.Collections.Generic;
using System.Linq;
using TheHuntIsOn.Modules.PauseModule;

namespace TheHuntIsOn;

public class PendingUnpause
{
    public int SequenceNumber;
    public long UnpauseTicks;
}

public class HuntLocalSaveData
{
    #region Properties

    // Whether the server is currently paused.
    public bool ServerPaused;

    // If paused, when the server should be unpaused.
    public long UnpauseTimeTicks;

    // Time to wait to respawn after a death.
    public int DeathTimerSeconds;

    #endregion

    #region Methods

    public void UpdatePauseState(PauseStateUpdatePacket packet)
    {
        ServerPaused = packet.ServerPaused;
        UnpauseTimeTicks = packet.UnpauseTimeTicks;
    }

    public bool IsServerPaused(out float? remainingSeconds)
    {
        remainingSeconds = null;
        if (!ServerPaused) return false;
        if (UnpauseTimeTicks == long.MaxValue) return true;

        var now = DateTime.UtcNow.Ticks;
        if (now >= UnpauseTimeTicks) return false;

        TimeSpan span = new(now - UnpauseTimeTicks);
        remainingSeconds = (float)span.TotalSeconds;
        return true;
    }

    #endregion
}
