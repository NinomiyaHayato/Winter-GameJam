using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

// ���̐��E�̏�ԂŃC���Q�[�����I�������猻�����E�Ƀ��Z�b�g�����
// ����ɐG�ꂽ�ۂɖ��̐��E�ɓ���
// ���̐��E�̎������Ԃ̓v���C���[�̎��Ԓ�~�Ɉˑ�����
// ���̐��E�̎������Ԃ͂��̃N���X���Ǘ�����
public class Worldkeeper : MonoBehaviour
{
    enum State
    {
        Reality,
        Dream,
    }

    [Header(Const.PreColorTag + "���̐��E�̎�������(�b)" + Const.SufColorTag)]
    [SerializeField] float _duration = 5.0f;
    [Header("���̐��E�p�̃|�X�g�G�t�F�N�g")]
    [SerializeField] Volume _volume;
    [Header("�G���Ɩ��̐��E�ɓ���")]
    [SerializeField] Collider[] _triggers;

    State _currentState = State.Reality;

    void Awake()
    {
        ToReality();

        // �ʏ�̃I�u�W�F�N�g�j�����ɉ����āA�C���Q�[���̃��Z�b�g���ɃL�����Z�������
        CancellationTokenSource cts = new();
        EntryPoint.OnPreInGameStart += OnStart;
        EntryPoint.OnInGameReset += OnReset;

        this.OnDestroyAsObservable().Subscribe(_ =>
        {
            cts?.Cancel();
            EntryPoint.OnPreInGameStart -= OnStart;
            EntryPoint.OnInGameReset -= OnReset;
        });

        // �ȉ�2�̃��\�b�h�̓R�[���o�b�N�ւ̓o�^/�����p
        void OnStart() { cts = new(); }
        void OnReset() { cts?.Cancel(); }

        // �������E�Ńv���C���[�����ꂩ�̃x�b�h�ɐڐG�����ꍇ�ɖ��̐��E�ɐ؂�ւ���
        foreach (Collider t in _triggers)
        {
            t.OnTriggerEnterAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Where(_ => _currentState == State.Reality)
                .Where(_ => cts != null && !cts.IsCancellationRequested)
                .Subscribe(_ => SwitchToDreamAsync(cts.Token).Forget());
        }
    }

    // ���̐��E�ɐ؂�ւ��Ĉ�莞�Ԍ�Ɍ������E�ɖ߂�
    async UniTaskVoid SwitchToDreamAsync(CancellationToken token)
    {
        // �L�����Z�����ꂽ�ۂ͌������E�ɖ߂�
        token.Register(ToReality);

        ToDream();
        await TimerAsync(token);
        ToReality();
    }

    void ToDream()
    {
        AudioPlayer.PlayBGM(AudioKey.BGM_Dream);
        _currentState = State.Dream;
        _volume.enabled = true;
    }

    void ToReality()
    {
        AudioPlayer.PlayBGM(AudioKey.BGM_Reality);
        _currentState = State.Reality;
        _volume.enabled = false;
    }

    // ���Ԃ�����Ă���ԁA���̐��E�̌o�ߎ��Ԃ�i�߂�
    async UniTask TimerAsync(CancellationToken token)
    {
        float time = 0;
        while (time <= _duration && !token.IsCancellationRequested)
        {
            time += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.TimeUpdate, token);
        }
    }

    void OnDrawGizmos()
    {
        // �x�b�h�̓����蔻��̉���
        if (_triggers != null && _triggers.Length > 0)
        {
            Gizmos.color = Color.green;
            foreach (Collider t in _triggers)
            {
                Gizmos.DrawWireCube(t.transform.position, t.bounds.size);
            }
        }
    }
}
