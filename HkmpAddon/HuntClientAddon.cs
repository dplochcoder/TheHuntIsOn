using Hkmp.Api.Client;

namespace TheHuntIsOn.HkmpAddon;

public class HuntClientAddon : ClientAddon
{
    public ClientNetManager NetManager { get; private set; }
    
    public override void Initialize(IClientApi clientApi)
    {
        NetManager = new ClientNetManager(this, clientApi.NetClient);
    }

    protected override string Name => AddonIdentifier.Name;
    protected override string Version => AddonIdentifier.Version;
    public override bool NeedsNetwork => true;
}