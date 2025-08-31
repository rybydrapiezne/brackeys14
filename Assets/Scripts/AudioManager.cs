using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource mainTheme;
    public AudioSource desertTheme;
    public AudioSource battleTheme;
    public AudioSource walk;
    public AudioSource hover;
    public AudioSource click;
    public AudioSource menuPopup;

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

    private void Start()
    {
        StartCoroutine(fadeIn(desertTheme, 5f));
    }

    public static IEnumerator fadeIn(AudioSource source, float time)
    {
        source.volume = 0;
        source.Play();

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / time;
            source.volume = Mathf.Lerp(0, 1, elapsedTime);
            yield return null;
        }

        source.volume = 1;
    }

    public static IEnumerator fadeOut(AudioSource source, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / time;
            source.volume = Mathf.Lerp(1, 0, elapsedTime);
            yield return null;
        }

        source.volume = 0;
        source.Stop();
    }
}
