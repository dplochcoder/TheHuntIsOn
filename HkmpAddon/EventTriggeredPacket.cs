using Hkmp.Networking.Packet;

namespace TheHuntIsOn.HkmpAddon;

internal class EventTriggeredPacket : IPacketData
{
    public NetEvent NetEvent { get; set; }
    
    public void WriteData(IPacket packet)
    {
        packet.Write((byte) NetEvent);
    }

    public void ReadData(IPacket packet)
    {
        NetEvent = (NetEvent) packet.ReadByte();
    }

    public bool IsReliable => true;
    public bool DropReliableDataIfNewerExists => false;
}