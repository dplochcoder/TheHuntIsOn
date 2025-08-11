using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseServerAddon : ServerAddon
{
    protected override string Name => AddonIdentifier.Name;

    protected override string Version => AddonIdentifier.Version;

    public override bool NeedsNetwork => true;

    private ServerNetManager NetManager;

    public override void Initialize(IServerApi serverApi) => serverApi.CommandManager.RegisterCommand(new PtCommand(new ServerNetManager(this, serverApi.NetServer)));
}
