using System;
using UnityEditor;
using UnityEngine;

namespace VLiveKit.Sandbox.ArtNet.Editor
{
    public sealed class ArtNetTestSignalWindow : EditorWindow
    {
        private const int PreviewChannelCount = 64;
        private const float PreviewCellHeight = 18f;

        private readonly ArtNetSenderConfig senderConfig = new ArtNetSenderConfig();
        private readonly ArtNetSignalGeneratorSettings generatorSettings = new ArtNetSignalGeneratorSettings();
        private readonly ArtNetSignalGenerator generator = new ArtNetSignalGenerator();
        private readonly IArtNetPacketSender sender = new NullArtNetPacketSender();

        private byte[] currentFrame = Array.Empty<byte>();
        private double nextSendTime;
        private double startedAt;
        private Vector2 scrollPosition;
        private GUIStyle statusStyle;

        [MenuItem("VLiveKit/Tools/Art-Net Test Signal")]
        public static void Open()
        {
            ArtNetTestSignalWindow window = GetWindow<ArtNetTestSignalWindow>();
            window.titleContent = new GUIContent("Art-Net Test");
            window.minSize = new Vector2(420f, 560f);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += Tick;
            currentFrame = generator.BuildFrame(generatorSettings, 0f);
        }

        private void OnDisable()
        {
            EditorApplication.update -= Tick;
            sender.Close();
        }

        private void OnGUI()
        {
            EnsureStyles();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawHeader();
            DrawSenderSettings();
            DrawGeneratorSettings();
            DrawControls();
            DrawPreview();
            DrawIntegrationNotes();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Art-Net Test Signal", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Art-Net送信ライブラリを後から差し替えられるように、DMXフレーム生成・送信設定・UIを分離したテストツールのベースです。現状はNull送信なのでネットワーク送信は行いません。",
                MessageType.Info);
        }

        private void DrawSenderSettings()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("送信先", EditorStyles.boldLabel);

            senderConfig.TargetAddress = EditorGUILayout.TextField("IP / Broadcast", senderConfig.TargetAddress);
            senderConfig.Port = EditorGUILayout.IntField("Port", senderConfig.Port);

            using (new EditorGUILayout.HorizontalScope())
            {
                senderConfig.Net = EditorGUILayout.IntSlider("Net", senderConfig.Net, 0, 127);
                senderConfig.Subnet = EditorGUILayout.IntSlider("Subnet", senderConfig.Subnet, 0, 15);
            }

            senderConfig.Universe = EditorGUILayout.IntSlider("Universe", senderConfig.Universe, 0, 15);
            senderConfig.RefreshRate = EditorGUILayout.IntSlider("Refresh Rate", senderConfig.RefreshRate, 1, 60);

            EditorGUILayout.LabelField("Port Address", senderConfig.PortAddress.ToString());
        }

        private void DrawGeneratorSettings()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("テスト信号", EditorStyles.boldLabel);

            generatorSettings.Pattern = (ArtNetSignalPattern)EditorGUILayout.EnumPopup("Pattern", generatorSettings.Pattern);
            generatorSettings.ChannelCount = EditorGUILayout.IntSlider("Channels", generatorSettings.ChannelCount, 1, ArtNetSignalGeneratorSettings.MaxDmxChannels);
            generatorSettings.Intensity = EditorGUILayout.IntSlider("Intensity", generatorSettings.Intensity, 0, 255);
            generatorSettings.Speed = EditorGUILayout.Slider("Speed", generatorSettings.Speed, 0.01f, 10f);
            generatorSettings.Invert = EditorGUILayout.Toggle("Invert", generatorSettings.Invert);

            using (new EditorGUI.DisabledScope(generatorSettings.Pattern != ArtNetSignalPattern.Chase))
            {
                generatorSettings.ChaseWidth = EditorGUILayout.IntSlider("Chase Width", generatorSettings.ChaseWidth, 1, 128);
            }
        }

        private void DrawControls()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(sender.IsOpen))
                {
                    if (GUILayout.Button("Start", GUILayout.Height(28f)))
                    {
                        StartSending();
                    }
                }

                using (new EditorGUI.DisabledScope(!sender.IsOpen))
                {
                    if (GUILayout.Button("Stop", GUILayout.Height(28f)))
                    {
                        StopSending();
                    }
                }

                if (GUILayout.Button("Generate Once", GUILayout.Height(28f)))
                {
                    GenerateAndSendFrame(EditorApplication.timeSinceStartup);
                }
            }

            Rect statusRect = EditorGUILayout.GetControlRect(false, 24f);
            EditorGUI.LabelField(statusRect, sender.Status, statusStyle);
        }

        private void DrawPreview()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("DMX Preview", EditorStyles.boldLabel);

            currentFrame = generator.BuildFrame(generatorSettings, (float)(EditorApplication.timeSinceStartup - startedAt));

            int shownChannels = Mathf.Min(currentFrame.Length, PreviewChannelCount);
            int columns = Mathf.Max(1, Mathf.FloorToInt((position.width - 44f) / 34f));
            int rows = Mathf.CeilToInt(shownChannels / (float)columns);
            Rect rect = GUILayoutUtility.GetRect(10f, rows * PreviewCellHeight + 8f, GUILayout.ExpandWidth(true));

            for (int i = 0; i < shownChannels; i++)
            {
                int column = i % columns;
                int row = i / columns;
                float x = rect.x + column * 34f;
                float y = rect.y + row * PreviewCellHeight;
                byte value = currentFrame[i];
                Rect cell = new Rect(x, y, 30f, 14f);
                EditorGUI.DrawRect(cell, new Color(value / 255f, value / 255f, value / 255f));
                GUI.Label(cell, value.ToString(), EditorStyles.miniLabel);
            }

            EditorGUILayout.LabelField($"Previewing {shownChannels} / {currentFrame.Length} channels");
        }

        private void DrawIntegrationNotes()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("ライブラリ接続メモ", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "次の実装ステップ: IArtNetPacketSenderを実際のArt-Netライブラリで実装し、Openでソケット初期化、SendDmxでArtDMXパケット送信、Closeで解放する。UIと信号生成はそのまま使えます。",
                MessageType.None);
        }

        private void Tick()
        {
            if (!sender.IsOpen)
            {
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (now < nextSendTime)
            {
                return;
            }

            GenerateAndSendFrame(now);
            nextSendTime = now + (1.0 / senderConfig.RefreshRate);
            Repaint();
        }

        private void StartSending()
        {
            startedAt = EditorApplication.timeSinceStartup;
            nextSendTime = 0d;
            sender.Open(senderConfig);
        }

        private void StopSending()
        {
            sender.Close();
            Repaint();
        }

        private void GenerateAndSendFrame(double now)
        {
            currentFrame = generator.BuildFrame(generatorSettings, (float)(now - startedAt));
            sender.SendDmx(senderConfig, currentFrame);
            Repaint();
        }

        private void EnsureStyles()
        {
            if (statusStyle != null)
            {
                return;
            }

            statusStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
