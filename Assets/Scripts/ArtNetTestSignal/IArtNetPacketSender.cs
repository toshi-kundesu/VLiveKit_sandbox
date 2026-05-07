namespace VLiveKit.Sandbox.ArtNet
{
    public interface IArtNetPacketSender
    {
        bool IsOpen { get; }
        string Status { get; }
        void Open(ArtNetSenderConfig config);
        void SendDmx(ArtNetSenderConfig config, byte[] dmxData);
        void Close();
    }
}
