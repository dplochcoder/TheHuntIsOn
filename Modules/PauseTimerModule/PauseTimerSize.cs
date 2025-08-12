namespace TheHuntIsOn.Modules.PauseTimerModule;

public enum PauseTimerSize
{
    Normal,
    Small,
    Large,
}

public static class PauseTimerSizeExtensions
{
    public static int FontSize(this PauseTimerSize size) => size switch { PauseTimerSize.Normal => 24, PauseTimerSize.Small => 14, PauseTimerSize.Large => 36 };

    public static float Spacing(this PauseTimerSize size) => size switch { PauseTimerSize.Normal => 0.75f, PauseTimerSize.Small => 0.5f, PauseTimerSize.Large => 1.25f };
}
