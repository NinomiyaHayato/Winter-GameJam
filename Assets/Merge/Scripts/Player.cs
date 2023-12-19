using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Events;

/// <summary>
/// 動的に生成せず、シーン上に初めから配置されていることが前提のプレイヤークラス
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// プレイヤーがインゲームのミス条件を満たした際に呼ばれるコールバック
    /// </summary>
    public static UnityAction OnMissed;

    [SerializeField] CharacterController _controller;
    [Header("アイテムや敵と衝突する判定")]
    [SerializeField] Collider _hitTrigger;
    [Header("ゲーム開始時の地点")]
    [SerializeField] Transform _respawnPoint;
    [Header(Const.PreColorTag + "移動速度" + Const.SufColorTag)]
    [SerializeField] float _walkSpeed = 1.0f;
    [SerializeField] float _runSpeed = 2.0f;

    ReactiveProperty<bool> _isMoving = new(false);

    /// <summary>
    /// プレイヤーが動いているかのフラグ
    /// このフラグを見て時間停止/解除を行う。
    /// </summary>
    public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;

    void Awake()
    {
        Cursor.visible = false;

        bool isValid = true;
        this.UpdateAsObservable().Where(_ => isValid).Subscribe(_ => _isMoving.Value = Move());

        // アイテムを獲得する
        _hitTrigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.ItemTag))
            .Subscribe(c => OnGetItem(c.gameObject));

        // 敵と衝突する
        _hitTrigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.EnemyTag))
            .Subscribe(_ => OnHitEnemy());

        // ゲームの状態遷移に初期化処理をフック
        EntryPoint.OnTitleEnter += Initialize;
        EntryPoint.OnInGameReset += Initialize;
        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnTitleEnter -= Initialize;
            EntryPoint.OnInGameReset -= Initialize;
        });
    }

    // ゲーム開始前に初期化する
    void Initialize()
    {
        _isMoving.Value = false;

        if (_respawnPoint != null)
        {
            // CharacterControllerの仕様でこうしないとtranform.positionの書き換えが反映されない
            _controller.enabled = false;
            _controller.transform.position = _respawnPoint.position;
            _controller.enabled = true;
        }
    }

    // 入力を受け取って移動する
    bool Move()
    {
        Vector3 input = default;
        if (Input.GetKey(KeyCode.W)) input.z++;
        if (Input.GetKey(KeyCode.S)) input.z--;
        if (Input.GetKey(KeyCode.A)) input.x--;
        if (Input.GetKey(KeyCode.D)) input.x++;

        float speed = _walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed = _runSpeed;

        Quaternion cameraRot = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        Vector3 velocity = cameraRot * input * Time.deltaTime * speed;

        _controller.Move(velocity);

        // 移動した場合はtrueを返す
        return input != default;
    }

    // アイテムを獲得した
    void OnGetItem(GameObject item)
    {
        Destroy(item);
        PlantManager.StepProgress();
    }

    // 敵とぶつかった
    void OnHitEnemy()
    {
        OnMissed?.Invoke();
    }
}