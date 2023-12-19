using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

// 自分自身に対してメッセージングを行う事で、外部からstaticのように
// アクセス出来る、非シングルトンのMonoBehaviourになっている
public class AudioPlayer : MonoBehaviour
{
    #region 再生する音のデータ
    [System.Serializable]
    class AudioData
    {
        [SerializeField] AudioKey _key;
        [SerializeField] AudioClip _clip;
        [Range(0, 1)]
        [SerializeField] float _volume = 1;

        public AudioKey Key => _key;
        public AudioClip Clip => _clip;
        public float Volume => _volume;
    }
    #endregion

    enum PlayMode
    {
        BGM,
        SE,
        Stop, // 停止用
    }

    struct Message
    {
        public PlayMode Mode;
        public AudioKey Key;
    }

    // 同時再生出来る最大数
    const int Max = 10;

    [Header("鳴らす音のデータ")]
    [SerializeField] AudioData[] _audioData;

    Dictionary<AudioKey, AudioData> _dataDict;
    AudioSource[] _sources = new AudioSource[Max];

    void Awake()
    {
        // 自分自身へのメッセージングでSE/BGMを再生
        MessageBroker.Default.Receive<Message>().Subscribe(msg =>
        {
            if      (msg.Mode == PlayMode.SE) playSe(msg.Key);
            else if (msg.Mode == PlayMode.BGM) playBgm(msg.Key);
            else if (msg.Mode == PlayMode.Stop) StopBgm();
        }).AddTo(this);

        // AudioSourceをたくさん追加
        for (int i = 0; i < _sources.Length; i++)
        {
            _sources[i] = gameObject.AddComponent<AudioSource>();
        }

        // 音データを辞書に追加
        _dataDict = _audioData.ToDictionary(v => v.Key, v => v);
    }

    // 音再生
    void playSe(AudioKey key)
    {
        // 空いてるAudioSource取得。末尾はBGM再生用に取っておく
        AudioSource source = default;
        for(int i = 0; i < _sources.Length - 1; i++)
        {
            if (!_sources[i].isPlaying)
            {
                source = _sources[i];
                break;
            }
        }

        if (source == default) return;

        source.clip = _dataDict[key].Clip;
        source.volume = _dataDict[key].Volume;
        source.Play();
    }

    // BGM再生。ループする
    void playBgm(AudioKey key)
    {
        AudioSource source = _sources[_sources.Length - 1];
        source.clip = _dataDict[key].Clip;
        source.volume = _dataDict[key].Volume;
        source.loop = true;
        source.Play();
    }

    // BGMの再生を止める
    void StopBgm()
    {
        _sources[_sources.Length - 1].Stop();
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    public static void PlaySE(AudioKey key)
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.SE, Key = key });
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    public static void PlayBGM(AudioKey key)
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.BGM, Key = key });
    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public static void StopBGM()
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.Stop });
    }
}
