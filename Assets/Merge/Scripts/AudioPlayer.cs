using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

// �������g�ɑ΂��ă��b�Z�[�W���O���s�����ŁA�O������static�̂悤��
// �A�N�Z�X�o����A��V���O���g����MonoBehaviour�ɂȂ��Ă���
public class AudioPlayer : MonoBehaviour
{
    #region �Đ����鉹�̃f�[�^
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
        Stop, // ��~�p
    }

    struct Message
    {
        public PlayMode Mode;
        public AudioKey Key;
    }

    // �����Đ��o����ő吔
    const int Max = 10;

    [Header("�炷���̃f�[�^")]
    [SerializeField] AudioData[] _audioData;

    Dictionary<AudioKey, AudioData> _dataDict;
    AudioSource[] _sources = new AudioSource[Max];

    void Awake()
    {
        // �������g�ւ̃��b�Z�[�W���O��SE/BGM���Đ�
        MessageBroker.Default.Receive<Message>().Subscribe(msg =>
        {
            if      (msg.Mode == PlayMode.SE) playSe(msg.Key);
            else if (msg.Mode == PlayMode.BGM) playBgm(msg.Key);
            else if (msg.Mode == PlayMode.Stop) StopBgm();
        }).AddTo(this);

        // AudioSource����������ǉ�
        for (int i = 0; i < _sources.Length; i++)
        {
            _sources[i] = gameObject.AddComponent<AudioSource>();
        }

        // ���f�[�^�������ɒǉ�
        _dataDict = _audioData.ToDictionary(v => v.Key, v => v);
    }

    // ���Đ�
    void playSe(AudioKey key)
    {
        // �󂢂Ă�AudioSource�擾�B������BGM�Đ��p�Ɏ���Ă���
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

    // BGM�Đ��B���[�v����
    void playBgm(AudioKey key)
    {
        AudioSource source = _sources[_sources.Length - 1];
        source.clip = _dataDict[key].Clip;
        source.volume = _dataDict[key].Volume;
        source.loop = true;
        source.Play();
    }

    // BGM�̍Đ����~�߂�
    void StopBgm()
    {
        _sources[_sources.Length - 1].Stop();
    }

    /// <summary>
    /// SE�̍Đ�
    /// </summary>
    public static void PlaySE(AudioKey key)
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.SE, Key = key });
    }

    /// <summary>
    /// BGM�̍Đ�
    /// </summary>
    public static void PlayBGM(AudioKey key)
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.BGM, Key = key });
    }

    /// <summary>
    /// BGM���~�߂�
    /// </summary>
    public static void StopBGM()
    {
        MessageBroker.Default.Publish(new Message() { Mode = PlayMode.Stop });
    }
}
