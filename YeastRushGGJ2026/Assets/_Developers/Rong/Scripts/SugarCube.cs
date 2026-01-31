using System;
using UnityEngine;

/// <summary>
/// Controls floating motion for a Sugar and disapeared when it touches player or contaminants.
/// </summary>

public class SugarCube : MonoBehaviour
{
    public event Action<SugarCube> OnAbsorbed;

    [Header("Float Motion")]
    public float floatAmplitude = 0.06f;
    public float floatFrequency = 1.2f;
    public float rotateSpeed = 30f;

    [Header("Absorb")]
    // public string handTag = "Hand";
    public string[] absorberTags = new[] { "Hand", "Enemy" };
    public float absorbDuration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public bool IsAbsorbing { get; private set; }

    private Vector3 _startPos;
    private Vector3 _startScale;
    private float _seed;

    [Header("Audio")]
    public AudioClip absorbSfx;

    void Awake()
    {
        _startPos = transform.position;
        _startScale = transform.localScale;
        _seed = UnityEngine.Random.value * 10f;
    }

    void OnEnable()
    {
        _startPos = transform.position;
        _startScale = transform.localScale;
        IsAbsorbing = false;
    }

    void Update()
    {
        if (IsAbsorbing) return;

        float y = Mathf.Sin((Time.time + _seed) * floatFrequency) * floatAmplitude;
        transform.position = _startPos + new Vector3(0f, y, 0f);

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsAbsorbing) return;

        /*
        if (other.CompareTag(handTag))
        {
            StartCoroutine(AbsorbRoutine());
        }
        */
        for (int i = 0; i < absorberTags.Length; i++)
        {
            if (other.CompareTag(absorberTags[i]) || other.transform.root.CompareTag(absorberTags[i]))
            {
                if (absorbSfx != null)
                {
                    AudioSource.PlayClipAtPoint(absorbSfx, transform.position, 0.8f);
                }
                foreach (Transform child in Camera.main.transform)
                {
                    if (child.CompareTag("Potato"))
                    {
                        Potato potatoScript = child.GetComponent<Potato>();
                        potatoScript.hasEatenSugar = true;
                        break;
                    }
                }

                StartCoroutine(AbsorbRoutine());
                return;
            }
        }
    }

    System.Collections.IEnumerator AbsorbRoutine()
    {
        IsAbsorbing = true;

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

        OnAbsorbed?.Invoke(this);
        Destroy(gameObject);
    }
}
