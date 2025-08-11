using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseModule;

public class ServerPausePacket : IPacketData
{
    #region Properties

    public int SequenceNumber;

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => false;

    #endregion

    #region Methods

    public void WriteData(IPacket packet) => packet.Write(SequenceNumber);

    public void ReadData(IPacket packet) => SequenceNumber = packet.ReadUShort();

    #endregion
}
