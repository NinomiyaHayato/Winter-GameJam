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
/// ゲームのミスもしくはクリアになる条件はこのクラスに依存する
/// </summary>
public class GameRule : MonoBehaviour
{
    [Header("植物の進捗率")]
    [SerializeField] PlantManager _plantManager;

    // 適当にフラグで管理する
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
    /// ゲームクリアもしくはミスをするまで待ち、結果を返す
    /// </summary>
    public async UniTask<PlayResult> WaitForPlayResultAsync()
    {
        // 仮、スペースキーで返す。
        await UniTask.WaitUntil(() => _isMiss || _isClear);

        if (_isMiss) return PlayResult.Miss;
        else return PlayResult.Clear;
    }
}
