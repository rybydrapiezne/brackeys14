using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip walk;
    public AudioClip hover;
    public AudioClip click;

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
