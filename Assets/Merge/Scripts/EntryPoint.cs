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
    /// タイトルに遷移したタイミングで呼ばれる
    /// </summary>
    public static event UnityAction OnTitleEnter;
    /// <summary>
    /// インゲーム開始時、タイトルからインゲームのカメラに切り替える前に呼ばれる
    /// </summary>
    public static event UnityAction OnPreInGameStart;
    /// <summary>
    /// インゲーム開始時、タイトルからインゲームのカメラへ切り替えが終わったタイミングで呼ばれる
    /// </summary>
    public static event UnityAction OnInGameStart;
    /// <summary>
    /// ミスもしくはクリアでインゲーム終了時、フェードアウトして画面が真っ暗になったタイミングで呼ばれる
    /// </summary>
    public static event UnityAction OnInGameReset;

    [Header("ゲームルールの制御")]
    [SerializeField] GameRule _gameRule;
    [Header("ゲームの状態に応じてカメラを切り替える")]
    [SerializeField] CameraBlender _cameraBlender;
    [Header("タイトル画面のUI")]
    [SerializeField] CanvasGroup _titleUiRoot;
    [Header("フェードの設定")]
    [SerializeField] Image _fadeImage;
    [Tooltip("短くしすぎるとカメラの切り替えに間に合わないので注意")]
    [SerializeField] float _fadeDuration = 1.0f;

    void Start()
    {
        Initialize();

        CancellationTokenSource cts = new();
        GameLoopAsync(cts.Token).Forget();
    }

    void Initialize()
    {
        // ゲーム起動時にフェードしないのでフェード用の画像をαを0に設定
        Color fadeImageColor = _fadeImage.color;
        fadeImageColor.a = 0;
        _fadeImage.color = fadeImageColor;
    }

    async UniTask GameLoopAsync(CancellationToken token)
    {
        // 「タイトル」と「インゲーム」を繰り返す。
        while (!token.IsCancellationRequested)
        {
            ResetGameAll();
            OnTitleEnter?.Invoke();

            // タイトル画面で入力を待つ
            await WaitForInputAsync(token);
            await ToInGameEffectAsync(token);

            // インゲームをループする
            while (!token.IsCancellationRequested)
            {
                OnPreInGameStart?.Invoke();

                // インゲーム用のカメラに切り替え
                await _cameraBlender.SwitchAsync(CameraType.InGame, token);
                OnInGameStart?.Invoke();

                // インゲームの結果を待つ
                PlayResult playResult = await _gameRule.WaitForPlayResultAsync();

                // インゲームをリセット
                await ResetInGameAsync(token);

                // ゲームクリアでループを抜けてタイトル画面に戻る
                if (playResult == PlayResult.Clear) break;
            }
        }
    }

    // タイトルに入る前、ゲーム起動時の状態に全てリセットする
    void ResetGameAll()
    {
        _titleUiRoot.alpha = 1;
    }

    // 画面クリックを待つ
    async UniTask WaitForInputAsync(CancellationToken token)
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
    }

    // 「タイトル」から「インゲーム」に遷移する際の演出
    async UniTask ToInGameEffectAsync(CancellationToken token)
    {
        _titleUiRoot.alpha = 0;
        await UniTask.Yield(token);
    }

    // ミスもしくはクリアした際にフェードしてゲームをリセットする
    async UniTask ResetInGameAsync(CancellationToken token)
    {
        await _fadeImage.FadeOutAsync(_fadeDuration / 2, token);
        await _cameraBlender.SwitchAsync(CameraType.Title, token);
        OnInGameReset?.Invoke();
        await _fadeImage.FadeInAsync(_fadeDuration / 2, token);
    }
}