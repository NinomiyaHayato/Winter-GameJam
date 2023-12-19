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
    /// <summary>
    /// �Q�[���N���A�������̓~�X������܂ő҂��A���ʂ�Ԃ�
    /// </summary>
    public async UniTask<PlayResult> WaitForPlayResultAsync()
    {
        // ���A�X�y�[�X�L�[�ŕԂ��B
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        return PlayResult.Clear;
    }
}
