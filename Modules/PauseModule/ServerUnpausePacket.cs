using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseModule;

public class ServerUnpausePacket : IPacketData
{
    #region Properties

    public int SequenceNumber;

    public int Countdown;

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => false;

    #endregion

    #region Methods

    public void WriteData(IPacket packet)
    {
        packet.Write(SequenceNumber);
        packet.Write(Countdown);
    }

    public void ReadData(IPacket packet)
    {
        SequenceNumber = packet.ReadUShort();
        Countdown = packet.ReadUShort();
    }

    #endregion
}
