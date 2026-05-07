using UnityEngine;

namespace VLiveKit.Sandbox.ArtNet
{
    [System.Serializable]
    public sealed class ArtNetSignalGeneratorSettings
    {
        public const int MaxDmxChannels = 512;

        [SerializeField] private ArtNetSignalPattern pattern = ArtNetSignalPattern.LinearRamp;
        [SerializeField, Range(1, MaxDmxChannels)] private int channelCount = MaxDmxChannels;
        [SerializeField, Range(1, MaxDmxChannels)] private int chaseWidth = 16;
        [SerializeField, Range(0, 255)] private int intensity = 255;
        [SerializeField, Min(0.01f)] private float speed = 1f;
        [SerializeField] private bool invert;

        public ArtNetSignalPattern Pattern
        {
            get => pattern;
            set => pattern = value;
        }

        public int ChannelCount
        {
            get => channelCount;
            set => channelCount = Mathf.Clamp(value, 1, MaxDmxChannels);
        }

        public int ChaseWidth
        {
            get => chaseWidth;
            set => chaseWidth = Mathf.Clamp(value, 1, MaxDmxChannels);
        }

        public int Intensity
        {
            get => intensity;
            set => intensity = Mathf.Clamp(value, 0, 255);
        }

        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0.01f, value);
        }

        public bool Invert
        {
            get => invert;
            set => invert = value;
        }
    }
}
