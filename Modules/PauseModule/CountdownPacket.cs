using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseModule;

public class CountdownPacket : IPacketData
{
    public const byte MaxMessageLength = byte.MaxValue;

    #region Properties

    public int Countdown;

    public string Message = "<untitled>";

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => true;

    #endregion

    #region Methods

    public void WriteData(IPacket packet)
    {
        packet.Write(Countdown);
        packet.Write(Message);
    }


    public void ReadData(IPacket packet)
    {
        Countdown = packet.ReadInt();
        Message = packet.ReadString();
    }

    #endregion
}
