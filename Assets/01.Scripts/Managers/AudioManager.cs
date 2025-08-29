using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixerGroup bgmGroup, sfxGroup;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f, sfxVolume = 1f;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private SfxEntry[] sfxEntries;
    [SerializeField] private int channels = 20;

    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    public enum Sfx { BlackHole, ButtonPress, Dead, EnemyDead, EnemyHit, EnemyRaser, Fire, IceSound, Lose, Shield,ShieldOn,Win,Hit}
    [Serializable] public struct SfxEntry { public Sfx key; public AudioClip[] clips; }

    AudioSource _bgm;
    AudioHighPassFilter _hpf;
    AudioSource[] _sfxPool;
    int _cursor;
    Dictionary<Sfx, AudioClip[]> _map;

    void Awake()
    {
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetBgmVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSfxVolume);



        var root = new GameObject("AudioRoot");
        root.transform.SetParent(transform, false);
        _bgm = root.AddComponent<AudioSource>();

        _bgm.loop = true;
        _bgm.clip = bgmClip;
        _bgm.volume = bgmVolume;
        _bgm.outputAudioMixerGroup = bgmGroup;

        _hpf = root.AddComponent<AudioHighPassFilter>(); _hpf.enabled = false;

        _sfxPool = new AudioSource[channels];

        for (int i = 0; i < channels; i++)
        {
            var a = root.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.volume = sfxVolume;
            a.outputAudioMixerGroup = sfxGroup;
            _sfxPool[i] = a;
        }

        _map = new();

        foreach (var e in sfxEntries)
            if (e.clips?.Length > 0)
                _map[e.key] = e.clips;
    }
    void Start()
    {
        if (m_MusicMasterSlider) SetMasterVolume(m_MusicMasterSlider.value);
        if (m_MusicBGMSlider) SetBgmVolume(m_MusicBGMSlider.value);
        if (m_MusicSFXSlider) SetSfxVolume(m_MusicSFXSlider.value);
    }

    public void PlayBgm(bool on)
    {
        if (on)
        {
            if (_bgm.clip) _bgm.Play();
        }
        else
            _bgm.Stop();
    }
    public void EffectBgm(bool on) => _hpf.enabled = on;

    public void PlaySfx(Sfx key, float pitch = 1f)
    {
        if (!_map.TryGetValue(key, out var clips) || clips.Length == 0) return;
        int tries = _sfxPool.Length;
        while (tries-- > 0)
        {
            _cursor = (_cursor + 1) % _sfxPool.Length;
            var a = _sfxPool[_cursor];
            if (a.isPlaying) continue;
            a.pitch = pitch;
            a.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            a.Play();
            return;
        }
        // ���� �ٻڸ� ���� ������ ä�� �����
        var force = _sfxPool[_cursor];
        force.pitch = pitch;
        force.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        force.Play();
    }

    public void SetBgmVolume(float v)
    {
        _bgm.volume = bgmVolume = Mathf.Clamp01(v);
    }
    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        foreach (var a in _sfxPool) a.volume = sfxVolume;
    }

    public void SetMasterVolume(float volume)
    {
        // Master ������ BGM �� SFX ��ο� ������ ��ġ�� AudioMixer �Ķ���ͷ� ����
        float adjustedVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        m_AudioMixer.SetFloat("Master", adjustedVolume);  // "Master"�� AudioMixer�� ������ �Ķ���� �̸�
    }
}