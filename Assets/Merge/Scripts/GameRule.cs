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
    /// <summary>
    /// ゲームクリアもしくはミスをするまで待ち、結果を返す
    /// </summary>
    public async UniTask<PlayResult> WaitForPlayResultAsync()
    {
        // 仮、スペースキーで返す。
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        return PlayResult.Clear;
    }
}
