using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.Modules.PauseModule;

public class ClientNetManager
{
    public event Action<PauseStateUpdatePacket> PauseStateUpdateEvent;
    public event Action<CountdownPacket> CountdownEvent;
    public event Action<SetDeathTimerPacket> SetDeathTimerEvent;

    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<PauseStateUpdatePacket>(ClientPacketId.PauseStateUpdate, packet => PauseStateUpdateEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<CountdownPacket>(ClientPacketId.Countdown, packet => CountdownEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<SetDeathTimerPacket>(ClientPacketId.SetDeathTimer, packet => SetDeathTimerEvent?.Invoke(packet));
    }

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
        => packetId switch
        {
            ClientPacketId.PauseStateUpdate => new PacketDataCollection<PauseStateUpdatePacket>(),
            ClientPacketId.Countdown => new PacketDataCollection<CountdownPacket>(),
            ClientPacketId.SetDeathTimer => new PacketDataCollection<SetDeathTimerPacket>(),
            _ => null,
        };
}