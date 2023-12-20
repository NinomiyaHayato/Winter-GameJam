using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine.Events;

// 夢の世界の状態でインゲームが終了したら現実世界にリセットされる
// 判定に触れた際に夢の世界に入る
// 夢の世界の持続時間はプレイヤーの時間停止に依存する
// 夢の世界の持続時間はこのクラスが管理する
public class Worldkeeper : MonoBehaviour
{
    enum State
    {
        Reality,
        Dream,
    }

    /// <summary>
    /// 夢の世界に遷移した際に呼ばれるコールバック
    /// </summary>
    public static event UnityAction OnDreamEnter;
    /// <summary>
    /// 現実世界に遷移した際に呼ばれるコールバック
    /// インゲーム開始時、現実世界から始まる際は呼ばれない。
    /// </summary>
    public static event UnityAction OnRealityEnter;

    [Header("夢と現実を示すUIを操作")]
    [SerializeField] DoDirection _uiController;
    [Header(Const.PreColorTag + "夢の世界の持続時間(秒)" + Const.SufColorTag)]
    [SerializeField] float _duration = 5.0f;
    [Header("夢の世界用のポストエフェクト")]
    [SerializeField] Volume _volume;
    [Header("触れると夢の世界に入る")]
    [SerializeField] Collider[] _triggers;

    State _currentState = State.Reality;

    void Awake()
    {
        // 通常のオブジェクト破棄時に加えて、インゲームのリセット時にキャンセルされる
        CancellationTokenSource cts = new();
        EntryPoint.OnPreInGameStart += OnStart;
        EntryPoint.OnInGameReset += OnReset;

        this.OnDestroyAsObservable().Subscribe(_ =>
        {
            cts?.Cancel();
            EntryPoint.OnPreInGameStart -= OnStart;
            EntryPoint.OnInGameReset -= OnReset;
        });

        // 以下2つのメソッドはコールバックへの登録/解除用
        void OnStart() { cts = new(); }
        void OnReset() { cts?.Cancel(); }

        // 現実世界でプレイヤーが何れかのベッドに接触した場合に夢の世界に切り替える
        foreach (Collider t in _triggers)
        {
            t.OnTriggerEnterAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Where(_ => _currentState == State.Reality)
                .Where(_ => cts != null && !cts.IsCancellationRequested)
                .Subscribe(_ => SwitchToDreamAsync(cts.Token).Forget());
        }
    }

    // 夢の世界に切り替えて一定時間後に現実世界に戻す
    async UniTaskVoid SwitchToDreamAsync(CancellationToken token)
    {
        // キャンセルされた際は現実世界に戻す
        token.Register(ToReality);

        ToDream();
        await TimerAsync(token);
        ToReality();
    }

    void ToDream()
    {
        _uiController.ImageChange();
        _currentState = State.Dream;
        _volume.enabled = true;
        OnDreamEnter?.Invoke();
    }

    void ToReality()
    {
        _uiController.ImageChange();
        _currentState = State.Reality;
        _volume.enabled = false;
        OnRealityEnter?.Invoke();
    }

    // 時間が流れている間、夢の世界の経過時間を進める
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
        // ベッドの当たり判定の可視化
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
