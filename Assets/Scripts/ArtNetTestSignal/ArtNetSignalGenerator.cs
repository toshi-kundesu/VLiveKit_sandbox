using UnityEngine;

namespace VLiveKit.Sandbox.ArtNet
{
    public sealed class ArtNetSignalGenerator
    {
        public byte[] BuildFrame(ArtNetSignalGeneratorSettings settings, float timeSeconds)
        {
            int channelCount = Mathf.Clamp(settings.ChannelCount, 1, ArtNetSignalGeneratorSettings.MaxDmxChannels);
            byte[] frame = new byte[channelCount];
            WriteFrame(frame, settings, timeSeconds);
            return frame;
        }

        public void WriteFrame(byte[] frame, ArtNetSignalGeneratorSettings settings, float timeSeconds)
        {
            if (frame == null || settings == null)
            {
                return;
            }

            int channelCount = Mathf.Min(frame.Length, settings.ChannelCount, ArtNetSignalGeneratorSettings.MaxDmxChannels);
            float phase = Mathf.Max(0f, timeSeconds) * settings.Speed;

            for (int i = 0; i < channelCount; i++)
            {
                int value = EvaluateChannel(settings, i, channelCount, phase);
                frame[i] = (byte)(settings.Invert ? 255 - value : value);
            }
        }

        private static int EvaluateChannel(ArtNetSignalGeneratorSettings settings, int channelIndex, int channelCount, float phase)
        {
            switch (settings.Pattern)
            {
                case ArtNetSignalPattern.Blackout:
                    return 0;
                case ArtNetSignalPattern.FullOn:
                    return settings.Intensity;
                case ArtNetSignalPattern.LinearRamp:
                    return Mathf.RoundToInt(settings.Intensity * (channelIndex / Mathf.Max(1f, channelCount - 1f)));
                case ArtNetSignalPattern.Chase:
                    int head = Mathf.FloorToInt(phase * settings.ChaseWidth) % Mathf.Max(1, channelCount);
                    int distance = Mathf.Abs(channelIndex - head);
                    distance = Mathf.Min(distance, channelCount - distance);
                    return distance < settings.ChaseWidth ? settings.Intensity : 0;
                case ArtNetSignalPattern.SineWave:
                    float normalized = Mathf.Sin((channelIndex * 0.125f) + (phase * Mathf.PI * 2f)) * 0.5f + 0.5f;
                    return Mathf.RoundToInt(settings.Intensity * normalized);
                default:
                    return 0;
            }
        }
    }
}
