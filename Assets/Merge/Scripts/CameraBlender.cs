using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class CameraBlender
{
    [System.Serializable]
    class Data
    {
        [field: SerializeField] public CinemachineVirtualCamera Camera { get; private set; }
        [Tooltip("���̃J�����ɑJ�ڂ��Ă���Ƃ��̒l")]
        [field: SerializeField] public float Blend { get; private set; }
    }

    [SerializeField] CinemachineBrain _cinemachineBrain;
    [Header("�e��J�����̐ݒ�")]
    [SerializeField] Data _title;
    [SerializeField] Data _inGame;
    [SerializeField] Data _ending;
    [Header("�f�t�H���g�̃J����")]
    [SerializeField] CinemachineVirtualCamera _default;

    CinemachineVirtualCamera _currentCamera;

    /// <summary>
    /// �J������؂�ւ���
    /// </summary>
    public async UniTask SwitchAsync(CameraType to, CancellationToken token)
    {
        if (_currentCamera == null) _currentCamera = _default;

        if (to == CameraType.Title)
        {
            await BlendAsync(_title.Camera, _title.Blend, token);
            _currentCamera = _title.Camera;
        }
        else if (to == CameraType.InGame)
        {
            await BlendAsync(_inGame.Camera, _inGame.Blend, token);
            _currentCamera = _inGame.Camera;
        }
        else if (to == CameraType.Ending)
        {
            await BlendAsync(_ending.Camera, _ending.Blend, token);
            _currentCamera = _ending.Camera;
        }
    }

    async UniTask BlendAsync(CinemachineVirtualCamera to, float duration, CancellationToken token)
    {
        _currentCamera.Priority = 10;
        to.Priority = 11;
        _cinemachineBrain.m_DefaultBlend.m_Time = duration;

        await UniTask.WaitForSeconds(duration, cancellationToken: token);
    }
}
