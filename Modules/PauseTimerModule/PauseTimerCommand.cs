using Hkmp.Api.Command.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseTimerCommand(ServerNetManager netManager) : IServerCommand
{
    public string Trigger => "/pausetimer";

    public string[] Aliases => ["/pt"];

    public bool AuthorizedOnly => true;

    private static readonly List<PauseTimerSubcommand> subcommands =
    [
        new PauseSubcommand(),
        new UnpauseSubcommand(),
        new CountdownSubcommand(),
        new ClearCountdownsSubcommand(),
        new SetDeathTimerSubcommand()
    ];

    private static string AllSubcommands() => string.Join("|", [.. subcommands.Select(s => s.Name()).OrderBy(s => s)]);

    private static bool TryGetSubcommand(string name, out PauseTimerSubcommand subcommand)
    {
        foreach (var item in subcommands)
        {
            if (item.Name() == name)
            {
                subcommand = item;
                return true;
            }
        }

        subcommand = null;
        return false;
    }

    internal static bool MinArguments(ICommandSender commandSender, string[] arguments, int min)
    {
        if (arguments.Length >= min) return true;

        commandSender.SendMessage("Missing arguments.");
        return false;
    }

    internal static bool MaxArguments(ICommandSender commandSender, string[] arguments, int max)
    {
        if (arguments.Length <= max) return true;

        commandSender.SendMessage("Too many arguments.");
        return false;
    }

    internal static bool ParseInt(ICommandSender commandSender, string arg, out int value)
    {
        if (int.TryParse(arg, out value) && value >= 0) return true;
        
        commandSender.SendMessage($"Invalid integer '{arg}'");
        return false;
    }

    private static void UpdateCountdowns(ServerNetManager netManager, DateTime now, Func<Countdown, Countdown> map)
    {
        UpdateCountdownsPacket packet = new() { Countdowns = [.. TheHuntIsOn.LocalSaveData.GlobalCountdowns.Select(map)] };
        TheHuntIsOn.LocalSaveData.UpdateCountdowns(now, packet);
        netManager.BroadcastPacket(packet);
    }

    internal static void PauseCountdowns(ServerNetManager netManager, DateTime now) => UpdateCountdowns(netManager, now, c => c.Pause(now));

    internal static void UnpauseCountdowns(ServerNetManager netManager, DateTime now, DateTime unpauseWhen) => UpdateCountdowns(netManager, now, c => c.UnpauseAt(now, unpauseWhen));

    public void Execute(ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            commandSender.SendMessage($"Usage: '/pt <{AllSubcommands()}>'; Use /pt help <command> for details");
            return;
        }

        PauseTimerSubcommand subcommand;
        string name = arguments[0].ToLower();
        if (name == "help")
        {
            if (arguments.Length == 1) commandSender.SendMessage($"Usage: '/pt help <{AllSubcommands()}>'");
            else if (TryGetSubcommand(arguments[1], out subcommand)) commandSender.SendMessage($"Usage: {subcommand.Usage()}");
            else commandSender.SendMessage($"Unrecognized command '{arguments[1]}'; Expected '/pt help <{AllSubcommands()}>'");
            return;
        }

        if (!TryGetSubcommand(arguments[0], out subcommand))
        {
            commandSender.SendMessage($"Expected '/pt <{AllSubcommands()}>'; Use /pt help <command> for details");
            return;
        }

        if (!subcommand.Execute(netManager, commandSender, [.. arguments.Skip(1)])) commandSender.SendMessage($"Usage: {subcommand.Usage()}");
    }
}

internal abstract class PauseTimerSubcommand
{
    public abstract string Name();

    public abstract string Usage();

    public abstract bool Execute(ServerNetManager netManager, ICommandSender send, string[] arguments);
}

internal class PauseSubcommand : PauseTimerSubcommand
{
    public override string Name() => "pause";

    public override string Usage() => "'/pt pause [X]': Pause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        UpdatePauseStatePacket packet = new() { ServerPaused = true, UnpauseTimeTicks = long.MaxValue };
        var now = DateTime.UtcNow;
        if (arguments.Length == 1)
        {
            if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;

            var unpauseAt = now.AddSeconds(seconds);
            packet.UnpauseTimeTicks = unpauseAt.Ticks;
            PauseTimerCommand.PauseCountdowns(netManager, now);
            PauseTimerCommand.UnpauseCountdowns(netManager, now, unpauseAt);
        }
        else PauseTimerCommand.PauseCountdowns(netManager, now);

        TheHuntIsOn.LocalSaveData.UpdatePauseState(packet);
        netManager.BroadcastPacket(packet);
        return true;
    }
}

internal class UnpauseSubcommand : PauseTimerSubcommand
{
    public override string Name() => "unpause";

    public override string Usage() => "'/pt unpause [X]': Unpause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        var isPaused = TheHuntIsOn.LocalSaveData.ServerPaused;
        if (!isPaused)
        {
            commandSender.SendMessage("Server is already unpaused.");
            return true;
        }

        var now = DateTime.UtcNow;
        UpdatePauseStatePacket packet = new();
        if (arguments.Length == 1)
        {
            if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;

            var unpauseAt = now.AddSeconds(seconds);
            packet.ServerPaused = true;
            packet.UnpauseTimeTicks = unpauseAt.Ticks;
            PauseTimerCommand.UnpauseCountdowns(netManager, now, unpauseAt);
        }
        else
        {
            packet.ServerPaused = false;
            packet.UnpauseTimeTicks = 0;
            PauseTimerCommand.UnpauseCountdowns(netManager, now, now);
        }

        TheHuntIsOn.LocalSaveData.UpdatePauseState(packet);
        netManager.BroadcastPacket(packet);
        return true;
    }
}

internal class CountdownSubcommand : PauseTimerSubcommand
{
    public override string Name() => "countdown";

    public override string Usage() => "'/pt countdown X [msg...]': Start a countdown for all players on the server lasting X seconds, with an optional message attached.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MinArguments(commandSender, arguments, 1)) return false;

        if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;

        var now = DateTime.UtcNow;
        Countdown countdown = new() { FinishTimeTicks = now.AddSeconds(seconds).Ticks };

        // Respect any active pauses or timed unpauses.
        if (TheHuntIsOn.LocalSaveData.IsServerPaused(out var remaining))
        {
            if (remaining.HasValue) countdown = countdown.UnpauseAt(now, now.AddSeconds(remaining.Value));
            else countdown = countdown.Pause(now);
        }

        if (arguments.Length > 1)
        {
            countdown.Message = string.Join(" ", arguments.Skip(1));
            if (countdown.Message.Length > UpdateCountdownsPacket.MaxMessageLength)
            {
                commandSender.SendMessage("Countdown message is too long.");
                return false;
            }
        }

        UpdateCountdownsPacket packet = new() { Countdowns = [.. TheHuntIsOn.LocalSaveData.GlobalCountdowns.Concat([countdown])] };
        if (packet.Countdowns.Count > UpdateCountdownsPacket.MaxCountdowns)
        {
            commandSender.SendMessage("Too many countdowns. Try '/pt clearcountdowns'.");
            return true;
        }

        TheHuntIsOn.LocalSaveData.UpdateCountdowns(now, packet);
        netManager.BroadcastPacket(packet);
        return true;
    }
}

internal class ClearCountdownsSubcommand : PauseTimerSubcommand
{
    public override string Name() => "clearcountdowns";

    public override string Usage() => "'/pt clearcountdowns': clear all outstanding countdowns";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 0)) return false;

        UpdateCountdownsPacket packet = new();
        TheHuntIsOn.LocalSaveData.UpdateCountdowns(DateTime.UtcNow, packet);
        netManager.BroadcastPacket(packet);
        return true;
    }
}

internal class SetDeathTimerSubcommand : PauseTimerSubcommand
{
    public override string Name() => "deathtimer";

    public override string Usage() => "'/pt deathtimer [X]': get the current respawn delay on death, or else set it";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        if (arguments.Length == 0)
        {
            var time = TheHuntIsOn.LocalSaveData.DeathTimerSeconds;
            if (time == 0) commandSender.SendMessage("deathtimer is not set.");
            else commandSender.SendMessage($"deathtimer is set to {time} seconds.");
            return true;
        }

        if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out int seconds)) return false;

        netManager.BroadcastPacket(new SetDeathTimerPacket() { DeathTimer = seconds });
        return true;
    }
}
