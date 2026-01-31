using UnityEngine;

/// <summary>
/// Get the postion of the player (XR Rig).
/// </summary>

public class PlayerLocator : MonoBehaviour
{
    public static Transform Player;

    void Awake()
    {
        if (Player == null)
        {
            Player = transform;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

