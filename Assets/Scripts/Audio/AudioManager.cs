using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource[] sfxs;
    [SerializeField] private float sfxMinDistance;
    private bool canPlaySFX;

    [Header("Background Music")]
    [SerializeField] private AudioSource[] bgms;
    public bool canPlayBGM;
    private int currentBGMIndex;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke(nameof(AllowSFX), .5f);
    }

    private void Update()
    {
        if (!canPlayBGM)
            StopAllBGM();
        else
        {
            if (!bgms[currentBGMIndex].isPlaying)
                PlayBGM(currentBGMIndex);
        }
    }

    private void AllowSFX() => canPlaySFX = true;

    public void PlaySFX(int _sfxIndex, Transform _transform = null, bool _allowSimultaneousSounds = false)
    {
        if ((!_allowSimultaneousSounds && sfxs[_sfxIndex].isPlaying) || !canPlaySFX)
            return;

        if (_transform != null &&
            Vector2.Distance(PlayerManager.instance.player.transform.position, _transform.position) > sfxMinDistance)
            return;

        if (_sfxIndex >= sfxs.Length)
        {
            Debug.LogError("SFX index out of range");
            return;
        }

        sfxs[_sfxIndex].pitch = Random.Range(.9f, 1.1f);
        sfxs[_sfxIndex].Play();
    }

    public void StopSFX(int _sfxIndex) => sfxs[_sfxIndex].Stop();

    public void StopSFXWithDelay(int _sfxIndex, float _delay) =>
        StartCoroutine(StopSFXWithDelayCoroutine(sfxs[_sfxIndex], _delay));

    private IEnumerator StopSFXWithDelayCoroutine(AudioSource _audio, float _delay)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > 0.1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(_delay);
        }

        _audio.Stop();
        _audio.volume = defaultVolume;
    }

    public void PlayRandomBGM()
    {
        currentBGMIndex = Random.Range(0, bgms.Length);
        PlayBGM(currentBGMIndex);
    }

    public void PlayBGM(int _bgmIndex)
    {
        currentBGMIndex = _bgmIndex;
        StopAllBGM();
        bgms[currentBGMIndex].Play();
    }

    public void StopAllBGM()
    {
        foreach (var bgm in bgms)
            bgm.Stop();
    }
}