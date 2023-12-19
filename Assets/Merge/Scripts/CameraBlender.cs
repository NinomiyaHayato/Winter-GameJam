using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class CameraBlender
{
    [SerializeField] CinemachineVirtualCamera _titleVCam;
    [SerializeField] CinemachineVirtualCamera _inGameVCam;
    [SerializeField] CinemachineBrain _cinemachineBrain;
    [SerializeField] float _toTitleBlend = 1.0f;
    [SerializeField] float _toInGameBlend = 0.1f;

    /// <summary>
    /// ÉJÉÅÉâÇêÿÇËë÷Ç¶ÇÈ
    /// </summary>
    public async UniTask SwitchAsync(CameraType to, CancellationToken token)
    {
        if (to == CameraType.Title)
        {
            await BlendAsync(_inGameVCam, _titleVCam, _toTitleBlend, token);
        }
        else if (to == CameraType.InGame)
        {
            await BlendAsync(_titleVCam, _inGameVCam, _toInGameBlend, token);
        }
    }

    async UniTask BlendAsync(CinemachineVirtualCamera from, CinemachineVirtualCamera to, 
        float duration, CancellationToken token)
    {
        to.Priority = 11;
        from.Priority = 10;
        _cinemachineBrain.m_DefaultBlend.m_Time = duration;

        await UniTask.WaitForSeconds(duration, cancellationToken: token);
    }
}
