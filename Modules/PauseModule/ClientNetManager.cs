using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.Modules.PauseModule;

public class ClientNetManager
{
    public event Action<ServerPausePacket> ServerPauseEvent;
    public event Action<ServerUnpausePacket> ServerUnpauseEvent;
    public event Action<CountdownPacket> CountdownEvent;

    #region Members

    private readonly INetClient _netClient;

    private readonly IClientAddonNetworkSender<ServerPacketId> _netSender;

    #endregion

    #region Constructors

    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        _netClient = netClient;

        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<ServerPausePacket>(ClientPacketId.ServerPause, packet => ServerPauseEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<ServerUnpausePacket>(ClientPacketId.ServerUnpause, packet => ServerUnpauseEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<CountdownPacket>(ClientPacketId.Countdown, packet => CountdownEvent?.Invoke(packet));
    }

    #endregion

    #region Methods

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
        => packetId switch
        {
            ClientPacketId.ServerPause => new PacketDataCollection<ServerPausePacket>(),
            ClientPacketId.ServerUnpause => new PacketDataCollection<ServerUnpausePacket>(),
            ClientPacketId.Countdown => new PacketDataCollection<CountdownPacket>(),
            _ => null,
        };

    #endregion
}