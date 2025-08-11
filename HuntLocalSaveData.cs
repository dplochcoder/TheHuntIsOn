using System;
using System.Collections.Generic;
using System.Linq;

namespace TheHuntIsOn;

public class PendingUnpause
{
    public int SequenceNumber;
    public long UnpauseTicks;
}

public class HuntLocalSaveData
{
    #region Properties

    // The number of pause events that have started.
    public int StartedPauses;

    // The number of pause events that have been finished.
    public int FinishedPauses;

    // Unpauses that have been scheduled but not finished yet.
    public List<PendingUnpause> PendingUnpauses = [];

    // Time to wait to respawn after a death.
    public int DeathTimer;

    #endregion

    #region Methods

    public void StartServerPause(int sequenceNumber)
    {
        if (sequenceNumber < StartedPauses) return;

        StartedPauses = sequenceNumber + 1;
        FinishedPauses = sequenceNumber;
        PendingUnpauses = [.. PendingUnpauses.Where(p => p.SequenceNumber >= FinishedPauses)];
    }

    public void ScheduleServerUnpause(int sequenceNumber, int duration)
    {
        if (sequenceNumber < FinishedPauses) return;

        var existing = PendingUnpauses.First(p => p.SequenceNumber == sequenceNumber);
        var target = DateTime.UtcNow.AddSeconds(duration).Ticks;

        if (existing != null) existing.UnpauseTicks = Math.Min(existing.UnpauseTicks, target);
        else PendingUnpauses.Add(new()
        {
            SequenceNumber = sequenceNumber,
            UnpauseTicks = target,
        });
    }

    public bool IsServerPaused(out float? remaining)
    {
        remaining = null;
        if (FinishedPauses >= StartedPauses) return false;

        var pending = PendingUnpauses.First(p => p.SequenceNumber == StartedPauses - 1);
        if (pending != null)
        {
            var now = DateTime.UtcNow.Ticks;
            if (now >= pending.UnpauseTicks) return false;

            TimeSpan span = new(now - pending.UnpauseTicks);
            remaining = (float)span.TotalSeconds;
        }
        
        return true;
    }

    #endregion
}
