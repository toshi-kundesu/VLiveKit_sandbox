using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private Vector2 position = new Vector2(10f, 10f);
    [SerializeField] private Color textColor = Color.white;

    private float timeLeft;
    private int frames;
    private float fps;
    private GUIStyle style;

    private void Start()
    {
        timeLeft = updateInterval;
        style = new GUIStyle
        {
            fontSize = 24,
            normal = { textColor = textColor }
        };
    }

    private void Update()
    {
        timeLeft -= Time.unscaledDeltaTime;
        frames++;

        if (timeLeft > 0f)
        {
            return;
        }

        fps = frames / updateInterval;
        frames = 0;
        timeLeft = updateInterval;
    }

    private void OnGUI()
    {
        style.normal.textColor = textColor;
        GUI.Label(new Rect(position.x, position.y, 300f, 50f), $"FPS: {fps:F1}", style);
    }
}
