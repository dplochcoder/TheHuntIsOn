using Hkmp.Api.Server.Networking;
using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class ServerNetManager
{
    private readonly IServerAddonNetworkSender<ClientPacketId> _netSender;

    public ServerNetManager(ServerAddon addon, INetServer netServer) => _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);

    public void BroadcastPacket(ServerPausePacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.ServerPause, packet);
    public void BroadcastPacket(ServerUnpausePacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.ServerUnpause, packet);
    public void BroadcastPacket(CountdownPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.Countdown, packet);
    public void BroadcastPacket(SetDeathTimerPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.SetDeathTimer, packet);
}