using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource mainTheme;
    public AudioSource walk;
    public AudioSource hover;
    public AudioSource click;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
