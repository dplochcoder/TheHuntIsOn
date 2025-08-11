using Hkmp.Api.Server.Networking;
using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class ServerNetManager(ServerAddon addon, INetServer netServer)
{
    private readonly IServerAddonNetworkSender<ClientPacketId> _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);

    public void BroadcastPacket(PauseStateUpdatePacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.PauseStateUpdate, packet);
    public void BroadcastPacket(CountdownPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.Countdown, packet);
    public void BroadcastPacket(SetDeathTimerPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.SetDeathTimer, packet);
}