using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Events;

public class EntryPoint : MonoBehaviour
{
    /// <summary>
    /// �^�C�g���ɑJ�ڂ����^�C�~���O�ŌĂ΂��
    /// </summary>
    public static event UnityAction OnTitleEnter;
    /// <summary>
    /// �C���Q�[���J�n���A�^�C�g������C���Q�[���̃J�����ɐ؂�ւ���O�ɌĂ΂��
    /// </summary>
    public static event UnityAction OnPreInGameStart;
    /// <summary>
    /// �C���Q�[���J�n���A�^�C�g������C���Q�[���̃J�����֐؂�ւ����I������^�C�~���O�ŌĂ΂��
    /// </summary>
    public static event UnityAction OnInGameStart;
    /// <summary>
    /// �~�X�������̓N���A�ŃC���Q�[���I�����A�t�F�[�h�A�E�g���ĉ�ʂ��^���ÂɂȂ����^�C�~���O�ŌĂ΂��
    /// </summary>
    public static event UnityAction OnInGameReset;

    [Header("�Q�[�����[���̐���")]
    [SerializeField] GameRule _gameRule;
    [Header("�Q�[���̏�Ԃɉ����ăJ������؂�ւ���")]
    [SerializeField] CameraBlender _cameraBlender;
    [Header("�^�C�g����ʂ�UI")]
    [SerializeField] CanvasGroup _titleUiRoot;
    [Header("�t�F�[�h�̐ݒ�")]
    [SerializeField] Image _fadeImage;
    [Tooltip("�Z����������ƃJ�����̐؂�ւ��ɊԂɍ���Ȃ��̂Œ���")]
    [SerializeField] float _fadeDuration = 1.0f;

    void Start()
    {
        Initialize();

        CancellationTokenSource cts = new();
        GameLoopAsync(cts.Token).Forget();
    }

    void Initialize()
    {
        // �Q�[���N�����Ƀt�F�[�h���Ȃ��̂Ńt�F�[�h�p�̉摜������0�ɐݒ�
        Color fadeImageColor = _fadeImage.color;
        fadeImageColor.a = 0;
        _fadeImage.color = fadeImageColor;
    }

    async UniTask GameLoopAsync(CancellationToken token)
    {
        // �u�^�C�g���v�Ɓu�C���Q�[���v���J��Ԃ��B
        while (!token.IsCancellationRequested)
        {
            ResetGameAll();
            OnTitleEnter?.Invoke();

            // �^�C�g����ʂœ��͂�҂�
            await WaitForInputAsync(token);
            await ToInGameEffectAsync(token);

            // �C���Q�[�������[�v����
            while (!token.IsCancellationRequested)
            {
                OnPreInGameStart?.Invoke();

                // �C���Q�[���p�̃J�����ɐ؂�ւ�
                await _cameraBlender.SwitchAsync(CameraType.InGame, token);
                OnInGameStart?.Invoke();

                // �C���Q�[���̌��ʂ�҂�
                PlayResult playResult = await _gameRule.WaitForPlayResultAsync();

                // �C���Q�[�������Z�b�g
                await ResetInGameAsync(token);

                // �Q�[���N���A�Ń��[�v�𔲂��ă^�C�g����ʂɖ߂�
                if (playResult == PlayResult.Clear) break;
            }
        }
    }

    // �^�C�g���ɓ���O�A�Q�[���N�����̏�ԂɑS�ă��Z�b�g����
    void ResetGameAll()
    {
        _titleUiRoot.alpha = 1;
    }

    // ��ʃN���b�N��҂�
    async UniTask WaitForInputAsync(CancellationToken token)
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
    }

    // �u�^�C�g���v����u�C���Q�[���v�ɑJ�ڂ���ۂ̉��o
    async UniTask ToInGameEffectAsync(CancellationToken token)
    {
        _titleUiRoot.alpha = 0;
        await UniTask.Yield(token);
    }

    // �~�X�������̓N���A�����ۂɃt�F�[�h���ăQ�[�������Z�b�g����
    async UniTask ResetInGameAsync(CancellationToken token)
    {
        await _fadeImage.FadeOutAsync(_fadeDuration / 2, token);
        await _cameraBlender.SwitchAsync(CameraType.Title, token);
        OnInGameReset?.Invoke();
        await _fadeImage.FadeInAsync(_fadeDuration / 2, token);
    }
}