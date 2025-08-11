using Hkmp.Api.Command.Server;
using System.Collections.Generic;
using System.Linq;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PtCommand : IServerCommand
{
    public string Trigger => "/pt";

    public string[] Aliases => [];

    public bool AuthorizedOnly => true;

    private readonly ServerNetManager netManager;

    public PtCommand(ServerNetManager netManager) => this.netManager = netManager;

    private static readonly List<PtSubcommand> subcommands = [
        new PauseSubcommand(),
        new UnpauseSubcommand(),
        new CountdownSubcommand()
    ];

    private static string AllSubcommands() => string.Join("|", [.. subcommands.Select(s => s.Name()).OrderBy(s => s)]);

    private static bool TryGetSubcommand(string name, out PtSubcommand subcommand)
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

    internal static bool ParseInt(ICommandSender commandSender, string arg, out int value)
    {
        if (int.TryParse(arg, out value)) return true;
        
        commandSender.SendMessage($"Invalid integer '{arg}'");
        return false;
    }

    public void Execute(ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            commandSender.SendMessage($"Usage: '/pt <{AllSubcommands()}>'; Use /pt help <command> for details");
            return;
        }

        PtSubcommand subcommand;
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

internal abstract class PtSubcommand
{
    public abstract string Name();

    public abstract string Usage();

    public abstract bool Execute(ServerNetManager netManager, ICommandSender send, string[] arguments);
}

internal class PauseSubcommand : PtSubcommand
{
    public override string Name() => "pause";

    public override string Usage() => "'/pt pause [X]': Pause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length > 1)
        {
            commandSender.SendMessage("Too many arguments.");
            return false;
        }

        var seq = TheHuntIsOn.LocalSaveData.FinishedPauses;
        if (arguments.Length == 1)
        {
            if (!PtCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;
            netManager.BroadcastPacket(new ServerUnpausePacket() { SequenceNumber = seq, Countdown = seconds });
        }
        netManager.BroadcastPacket(new ServerPausePacket() { SequenceNumber = seq });

        return true;
    }
}

internal class UnpauseSubcommand : PtSubcommand
{
    public override string Name() => "unpause";

    public override string Usage() => "'/pt unpause [X]': Unpause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length > 1)
        {
            commandSender.SendMessage("Too many arguments.");
            return false;
        }

        var started = TheHuntIsOn.LocalSaveData.StartedPauses;
        var finished = TheHuntIsOn.LocalSaveData.FinishedPauses;
        if (started <= finished)
        {
            commandSender.SendMessage("Server is already unpaused.");
            return true;
        }

        ushort countdown = 0;
        if (arguments.Length == 1 && !PtCommand.ParseInt(commandSender, arguments[0], out countdown)) return false;

        netManager.BroadcastPacket(new ServerUnpausePacket() { SequenceNumber = finished, Countdown = countdown });
        return true;
    }
}

internal class CountdownSubcommand : PtSubcommand
{
    public override string Name() => "countdown";

    public override string Usage() => "'/pt countdown X [msg...]': Start a countdown for all players on the server lasting X seconds, with an optional message attached.";

    public override bool Execute(ServerNetManager netManager, ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            commandSender.SendMessage("Missing arguments.");
            return false;
        }

        if (!PtCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;

        string msg = "<untitled>";
        if (arguments.Length > 1)
        {
            msg = string.Join(" ", arguments.Skip(1));
            if (msg.Length > CountdownPacket.MaxMessageLength)
            {
                commandSender.SendMessage("Countdown message is too long.");
                return false;
            }
        }

        netManager.BroadcastPacket(new CountdownPacket()
        {
            Countdown = seconds,
            Message = msg
        });
        return true;
    }
}
