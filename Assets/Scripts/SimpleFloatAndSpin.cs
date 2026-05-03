using UnityEngine;

/// <summary>
/// Adds a small showcase motion to any GameObject: steady rotation plus optional bobbing.
/// Drop it onto a cube, light, camera prop, or imported model to make a scene feel alive.
/// </summary>
public sealed class SimpleFloatAndSpin : MonoBehaviour
{
    [Header("Spin")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

    [Header("Float")]
    [SerializeField] private bool enableFloat = true;
    [SerializeField, Min(0f)] private float floatAmplitude = 0.25f;
    [SerializeField, Min(0f)] private float floatFrequency = 1f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        if (!enableFloat || floatAmplitude <= 0f || floatFrequency <= 0f)
        {
            return;
        }

        float offsetY = Mathf.Sin(Time.time * Mathf.PI * 2f * floatFrequency) * floatAmplitude;
        transform.localPosition = startPosition + Vector3.up * offsetY;
    }
}
