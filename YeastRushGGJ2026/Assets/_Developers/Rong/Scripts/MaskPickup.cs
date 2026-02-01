using System;
using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Header("Float Motion")]
    public float floatAmplitude = 0.06f;
    public float floatFrequency = 1.2f;
    public float rotateSpeed = 30f;

    [Header("Pickup")]
    public string[] pickerTags = new[] { "Hand" };
    public float absorbDuration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Reveal")]
    public float revealSeconds = 20f;

    [Header("Audio")]
    public AudioClip pickupSfx;
    public float sfxVolume = 0.8f;

    private bool _picked;
    private Vector3 _startPos;
    private float _seed;

    public event Action<MaskPickup> OnPicked;

    void Awake()
    {
        _startPos = transform.position;
        _seed = UnityEngine.Random.value * 10f;
    }

    void OnEnable()
    {
        _startPos = transform.position;
        _picked = false;
    }

    void Update()
    {
        if (_picked) return;

        float y = Mathf.Sin((Time.time + _seed) * floatFrequency) * floatAmplitude;
        transform.position = _startPos + new Vector3(0f, y, 0f);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_picked) return;

        for (int i = 0; i < pickerTags.Length; i++)
        {
            if (other.CompareTag(pickerTags[i]) || other.transform.root.CompareTag(pickerTags[i]))
            {
                if (pickupSfx != null)
                    AudioSource.PlayClipAtPoint(pickupSfx, transform.position, sfxVolume);

                // Reveal enemies for N seconds
                if (EnemyVisibilityManager.Instance != null)
                    EnemyVisibilityManager.Instance.RevealFor(revealSeconds);

                StartCoroutine(PickRoutine());
                return;
            }
        }
    }

    System.Collections.IEnumerator PickRoutine()
    {
        _picked = true;

        float t = 0f;
        Vector3 initialScale = transform.localScale;

        while (t < absorbDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / absorbDuration);
            float s = scaleCurve.Evaluate(k);
            transform.localScale = initialScale * s;
            yield return null;
        }

        OnPicked?.Invoke(this);
        Destroy(gameObject);
    }
}
