using UnityEngine;

namespace VLiveKit.Sandbox.ArtNet
{
    [System.Serializable]
    public sealed class ArtNetSenderConfig
    {
        public const int DefaultPort = 6454;

        [SerializeField] private string targetAddress = "255.255.255.255";
        [SerializeField, Range(1, 65535)] private int port = DefaultPort;
        [SerializeField, Range(0, 127)] private int net;
        [SerializeField, Range(0, 15)] private int subnet;
        [SerializeField, Range(0, 15)] private int universe;
        [SerializeField, Range(1, 60)] private int refreshRate = 30;

        public string TargetAddress
        {
            get => targetAddress;
            set => targetAddress = string.IsNullOrWhiteSpace(value) ? "255.255.255.255" : value.Trim();
        }

        public int Port
        {
            get => port;
            set => port = Mathf.Clamp(value, 1, 65535);
        }

        public int Net
        {
            get => net;
            set => net = Mathf.Clamp(value, 0, 127);
        }

        public int Subnet
        {
            get => subnet;
            set => subnet = Mathf.Clamp(value, 0, 15);
        }

        public int Universe
        {
            get => universe;
            set => universe = Mathf.Clamp(value, 0, 15);
        }

        public int RefreshRate
        {
            get => refreshRate;
            set => refreshRate = Mathf.Clamp(value, 1, 60);
        }

        public int PortAddress => (Net << 8) | (Subnet << 4) | Universe;
    }
}
