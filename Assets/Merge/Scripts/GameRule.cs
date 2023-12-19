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
    [Header("�A���̐i����")]
    [SerializeField] PlantManager _plantManager;

    // �K���Ƀt���O�ŊǗ�����
    bool _isMiss;
    bool _isClear;

    void Awake()
    {
        EntryPoint.OnInGameReset += () => { _isMiss = false; _isClear = false; };
        EntryPoint.OnInGameReset -= () => { _isMiss = false; _isClear = false; };

        Player.OnMissed += () => _isMiss = true;
        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            Player.OnMissed -= () => _isMiss = true;
        });

        _plantManager.ObserveEveryValueChanged(v => v.CurrentProgress)
            .Where(f => Mathf.CeilToInt(f) >= 100)
            .Subscribe(v => _isClear = true).AddTo(this);
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
