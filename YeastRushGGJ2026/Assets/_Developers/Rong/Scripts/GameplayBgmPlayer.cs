using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameplayBgmPlayer : MonoBehaviour
{
    public AudioClip bgmClip;
    public float volume = 0.6f;

    private AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = true;
        _src.spatialBlend = 0f; // 2D
        _src.volume = volume;

        if (bgmClip != null)
            _src.clip = bgmClip;
    }

    void OnEnable()
    {
        if (_src.clip != null && !_src.isPlaying)
            _src.Play();
    }

    void OnDisable()
    {
        if (_src != null && _src.isPlaying)
            _src.Stop();
    }
}
