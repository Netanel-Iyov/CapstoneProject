using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// The class handles all sounds in the app
/// </summary>
public class SoundManager
{

    public AudioSource audioSource
    {
        get
        {
            var tryGetComp = Camera.main.gameObject.GetComponent<AudioSource>();
            if (tryGetComp != null) return tryGetComp;

            var temp = Camera.main.gameObject.AddComponent<AudioSource>();
            return temp;
        }
    }
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }
    }

    private SoundManager()
    {
        InitAudio();

    }



    #region SOUND
    private void InitAudio()
    {


        audioSource.transform.position = Camera.main.transform.position;
        audioSource.Stop();
        audioSource.loop = false;
    }

    /// <summary>
    /// For short audio, need to keep async
    /// </summary>
    /// <param name="ac"></param>
    /// <returns></returns>
    public async Task PlayAudioAsync(AudioClip ac)
    {
        PlayAudio(ac);
        try { await Task.Delay(Mathf.RoundToInt(ac.length * 1000 + 100)); }
        catch { } // every cancellation throw exception, but its ok
    }
    public void PlayAudio(AudioClip ac)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.clip = ac;
        audioSource.volume = SettingsModel.Instance.AudioLevel;
        audioSource.Play();
    }

    public void UpdateCurrentPlayingAudioVolume()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.volume = SettingsModel.Instance.AudioLevel;
    }
    public void StopAudio()
    {
        if (audioSource) audioSource.Stop();
    }

    public void PlayBeep(float volume = 0.5f, float frequency = 440f, float duration = 0.1f)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        int sampleRate = 44100;
        int sampleLength = (int)(sampleRate * duration);
        float[] samples = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
        }

        AudioClip beep = AudioClip.Create("Beep", sampleLength, 1, sampleRate, false);
        beep.SetData(samples, 0);
        audioSource.clip = beep;
        audioSource.volume = volume;
        audioSource.Play();
    }

    #endregion
}
