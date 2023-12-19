using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;

public class Enemy : MonoBehaviour
{
    [SerializeField] CharacterController _controller;
    [Header("キャラクターの向きを反映するオブジェクト")]
    [SerializeField] Transform _rotateChild;
    [Header(Const.PreColorTag + "視界の半径" + Const.SufColorTag)]
    [SerializeField] float _sightRadius = 5.0f;
    [Header(Const.PreColorTag + "プレイヤー未発見時の移動速度" + Const.SufColorTag)]
    [SerializeField] float _freeMoveSpeed = 1.0f;
    [Header(Const.PreColorTag + "プレイヤー追跡時の移動速度" + Const.SufColorTag)]
    [SerializeField] float _chaseMoveSpeed = 2.0f;

    void Start()
    {
        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask UpdateAsync(CancellationToken token)
    {
        Player player = FindPlayer();
        Transform transform = this.transform;
        Collider[] detected = new Collider[1]; // プレイヤーのみ検出するので長さ1で十分

        while (!token.IsCancellationRequested)
        {
            // 直後にプレイヤーが移動したかどうかの判定があるので、ここでawaitしないと無限ループに陥る
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);

            // プレイヤーが移動したかで時間停止を判定する
            if (!player.IsMoving.Value) continue;

            if (DetectPlayer(detected))
            {
                LookToPlayer(transform, player);
                MoveToPlayer(transform, player);
            }
        }
    }

    // シーン上のプレイヤーを取得
    Player FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Const.PlayerTag);
        if (player == null)
        {
            throw new System.NullReferenceException("プレイヤーがシーン上に存在しない: " + Const.PlayerTag);
        }
        return player.GetComponent<Player>();
    }

    // プレイヤーが視界内にいるかをタグで判定
    bool DetectPlayer(Collider[] detected)
    {
        for (int i = 0; i < detected.Length; i++) detected[i] = null;
        Physics.OverlapSphereNonAlloc(transform.position, _sightRadius, detected, Const.PlayerLayer);

        foreach (Collider c in detected)
        {
            if (c == null) break;
            if (c.gameObject.CompareTag(Const.PlayerTag)) return true;
        }

        return false;
    }

    // プレイヤーに向ける
    void LookToPlayer(Transform transform, Player player)
    {
        _rotateChild.forward = transform.NormalizedDirection(player.transform);
    }

    // プレイヤーに向けて移動する
    void MoveToPlayer(Transform transform, Player player)
    {
        Vector3 dir = transform.NormalizedDirection(player.transform);
        _controller.Move(dir * Time.deltaTime * _chaseMoveSpeed);
    }

    void OnDrawGizmos()
    {
        // 視界の範囲を描画
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _sightRadius);
    }
}
