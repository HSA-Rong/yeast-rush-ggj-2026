using System.Collections.Generic;
using UnityEngine;

public class EnemyVisibilityManager : MonoBehaviour
{
    public static EnemyVisibilityManager Instance { get; private set; }

    [Header("Reveal Settings")]
    public float revealDuration = 20f;

    private readonly List<Renderer[]> _enemyRenderers = new();
    private bool _revealed;
    private float _revealEndTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_revealed && Time.time >= _revealEndTime)
        {
            SetRevealed(false);
        }
    }

    public void RegisterEnemy(Contaminant enemy)
    {
        if (enemy == null) return;

        var renderers = enemy.GetComponentsInChildren<Renderer>(true);
        _enemyRenderers.Add(renderers);

        SetRendererArrayVisible(renderers, _revealed);
    }

    public void UnregisterEnemy(Contaminant enemy)
    {
        if (enemy == null) return;

        var renderers = enemy.GetComponentsInChildren<Renderer>(true);
        _enemyRenderers.Remove(renderers);
    }

    public void RevealFor(float seconds)
    {
        _revealEndTime = Time.time + seconds;
        SetRevealed(true);
    }

    private void SetRevealed(bool value)
    {
        _revealed = value;
        for (int i = 0; i < _enemyRenderers.Count; i++)
        {
            SetRendererArrayVisible(_enemyRenderers[i], _revealed);
        }
    }

    private void SetRendererArrayVisible(Renderer[] renderers, bool visible)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = visible;
        }
    }
}
