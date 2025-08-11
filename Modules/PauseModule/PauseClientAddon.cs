using Hkmp.Api.Client;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseClientAddon : ClientAddon
{
    protected override string Name => AddonIdentifier.Name;

    protected override string Version => AddonIdentifier.Version;

    public override bool NeedsNetwork => true;

    private readonly PauseModule pauseModule;
    public ClientNetManager NetManager;

    internal PauseClientAddon(PauseModule pauseModule) => this.pauseModule = pauseModule;

    public override void Initialize(IClientApi clientApi)
    {
        NetManager = new ClientNetManager(this, clientApi.NetClient);
        pauseModule.SetClientApi(clientApi);
    }
}
