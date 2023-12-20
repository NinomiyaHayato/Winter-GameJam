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
    [Header("プレイヤーがミスしたかどうか")]
    [SerializeField] Player _player;
    [Header("植物の進捗率")]
    [SerializeField] PlantManager _plantManager;

    // 適当にフラグで管理する
    bool _isMiss;
    bool _isClear;

    void Awake()
    {
        EntryPoint.OnInGameReset += FlagReset;
        this.OnDestroyAsObservable().Subscribe(_ => EntryPoint.OnInGameReset -= FlagReset);

        _player.OnMissed += () => _isMiss = true;
        _plantManager.OnProgressComplete += () => _isClear = true;

        // コールバック登録/解除用
        void FlagReset() { _isMiss = false; _isClear = false; }
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
