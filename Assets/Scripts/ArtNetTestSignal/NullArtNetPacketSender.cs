namespace VLiveKit.Sandbox.ArtNet
{
    public sealed class NullArtNetPacketSender : IArtNetPacketSender
    {
        public bool IsOpen { get; private set; }
        public string Status { get; private set; } = "ライブラリ未接続: UIプレビューのみ";

        public void Open(ArtNetSenderConfig config)
        {
            IsOpen = true;
            Status = "ライブラリ未接続: 送信せずにフレーム生成を実行中";
        }

        public void SendDmx(ArtNetSenderConfig config, byte[] dmxData)
        {
            Status = $"ライブラリ未接続: {dmxData?.Length ?? 0}ch のDMXフレームを生成";
        }

        public void Close()
        {
            IsOpen = false;
            Status = "停止中";
        }
    }
}
