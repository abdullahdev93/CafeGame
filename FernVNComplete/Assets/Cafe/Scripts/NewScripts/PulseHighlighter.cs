using UnityEngine;

public class PulseHighlighter : MonoBehaviour
{
    public float speed = 3f;
    public float scale = 1.1f;
    private Vector3 baseScale;
    private bool pulsing;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (!pulsing) return;
        float s = 1f + (Mathf.Sin(Time.unscaledTime * speed) * 0.5f + 0.5f) * (scale - 1f);
        transform.localScale = baseScale * s;
    }

    public static void Attach(Transform target, bool enable)
    {
        if (target == null) return;
        var pulse = target.GetComponent<PulseHighlighter>();
        if (enable)
        {
            if (pulse == null) pulse = target.gameObject.AddComponent<PulseHighlighter>();
            pulse.pulsing = true;
        }
        else
        {
            if (pulse != null)
            {
                pulse.pulsing = false;
                pulse.transform.localScale = pulse.baseScale;
                Destroy(pulse);
            }
        }
    }
}
