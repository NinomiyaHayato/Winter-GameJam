using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Events;

/// <summary>
/// �Q�[���̃~�X�������̓N���A�ɂȂ�����͂��̃N���X�Ɉˑ�����
/// </summary>
public class GameRule : MonoBehaviour
{
    [Header("�v���C���[���~�X�������ǂ���")]
    [SerializeField] Player _player;
    [Header("�A���̐i����")]
    [SerializeField] PlantManager _plantManager;

    // �K���Ƀt���O�ŊǗ�����
    bool _isMiss;
    bool _isClear;

    void Awake()
    {
        EntryPoint.OnInGameReset += FlagReset;
        this.OnDestroyAsObservable().Subscribe(_ => EntryPoint.OnInGameReset -= FlagReset);

        _player.OnMissed += () => _isMiss = true;
        _plantManager.OnProgressComplete += () => _isClear = true;

        // �R�[���o�b�N�o�^/�����p
        void FlagReset() { _isMiss = false; _isClear = false; }
    }

    /// <summary>
    /// �Q�[���N���A�������̓~�X������܂ő҂��A���ʂ�Ԃ�
    /// </summary>
    public async UniTask<PlayResult> WaitForPlayResultAsync()
    {
        // ���A�X�y�[�X�L�[�ŕԂ��B
        await UniTask.WaitUntil(() => _isMiss || _isClear);

        if (_isMiss) return PlayResult.Miss;
        else return PlayResult.Clear;
    }
}
