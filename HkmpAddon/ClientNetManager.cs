using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.HkmpAddon;

public class ClientNetManager
{
    public event Action<NetItem[]> GrantItemsEvent;

    private readonly INetClient _netClient;
    
    private readonly IClientAddonNetworkSender<ServerPacketId> _netSender;
    
    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        _netClient = netClient;
        _netSender = netClient.GetNetworkSender<ServerPacketId>(addon);

        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);
        
        netReceiver.RegisterPacketHandler<GrantItemsPacket>(
            ClientPacketId.GrantItems,
            packetData => GrantItemsEvent?.Invoke(packetData.NetItems));
    }

    public void SendEvent(NetEvent netEvent)
    {
        if (!_netClient.IsConnected) return;

        _netSender.SendCollectionData(ServerPacketId.EventTriggered, new EventTriggeredPacket
        {
            NetEvent = netEvent
        });
    }

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
    {
        switch (packetId)
        {
            case ClientPacketId.GrantItems:
                return new PacketDataCollection<GrantItemsPacket>();
        }

        return null;
    }
}