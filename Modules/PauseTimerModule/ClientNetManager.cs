using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.Modules.PauseModule;

public class ClientNetManager
{
    public event Action<SetDeathTimerPacket> SetDeathTimerEvent;
    public event Action<UpdateCountdownsPacket> UpdateCountdownsEvent;
    public event Action<UpdatePauseStatePacket> UpdatePauseStateEvent;

    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<SetDeathTimerPacket>(ClientPacketId.SetDeathTimer, packet => SetDeathTimerEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<UpdateCountdownsPacket>(ClientPacketId.UpdateCountdowns, packet => UpdateCountdownsEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<UpdatePauseStatePacket>(ClientPacketId.UpdatePauseState, packet => UpdatePauseStateEvent?.Invoke(packet));
    }

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
        => packetId switch
        {
            ClientPacketId.SetDeathTimer => new PacketDataCollection<SetDeathTimerPacket>(),
            ClientPacketId.UpdateCountdowns => new PacketDataCollection<UpdateCountdownsPacket>(),
            ClientPacketId.UpdatePauseState => new PacketDataCollection<UpdatePauseStatePacket>(),
            _ => null,
        };
}