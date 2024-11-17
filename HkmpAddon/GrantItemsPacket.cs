using Hkmp.Networking.Packet;

namespace TheHuntIsOn.HkmpAddon;

internal class GrantItemsPacket : IPacketData
{
    public NetItem[] NetItems { get; set; }
    
    public void WriteData(IPacket packet)
    {
        var length = (byte)NetItems.Length;
        packet.Write(length);

        for (var i = 0; i < length; i++)
        {
            packet.Write((byte) NetItems[i]);
        }
    }

    public void ReadData(IPacket packet)
    {
        var length = packet.ReadByte();
        NetItems = new NetItem[length];
        for (var i = 0; i < length; i++)
        {
            NetItems[i] = (NetItem)packet.ReadByte();
        }
    }

    public bool IsReliable => true;
    public bool DropReliableDataIfNewerExists => false;
}