using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Stateの種類")] states _state = states.move;
    [SerializeField]
    [Tooltip("移動速度")] float _speed;
    [SerializeField]
    [Tooltip("Player認識範囲")] float _maxDistance;
    [SerializeField]
    [Tooltip("StoppingDistance範囲")] float _StoppingDistance;
    [SerializeField]
    [Tooltip("視野角"), Range(0, 180)] float _fov;
    [SerializeField]
    [Tooltip("スタートポジション")] Transform _startPos;
    [SerializeField]
    [Tooltip("Playerの位置")] Transform _targetTransformPos;
    [SerializeField]
    [Tooltip("WayPointsの数と位置")] Transform[] _wayPoints;
    [SerializeField]
    [Tooltip("視点方向を示すオブジェクト")] Transform _lookAtTarget;

    int _currentWayPointIndex; // 現在のWayPointの数

    NavMeshAgent _agent; // NavMeshAgent

    void Awake()
    {
        EntryPoint.OnInGameStart += ResetEnemy;
        EntryPoint.OnInGameReset += ResetEnemy;

        Worldkeeper.OnDreamEnter += Dreaming;
        Worldkeeper.OnRealityEnter += Realty;
    }

    void OnDestroy()
    {
        EntryPoint.OnInGameStart -= ResetEnemy;
        EntryPoint.OnInGameReset -= ResetEnemy;

        Worldkeeper.OnDreamEnter -= Dreaming;
        Worldkeeper.OnRealityEnter -= Realty;
    }

    void Dreaming()
    {
        var m = GetComponent<MeshRenderer>();
        var col = GetComponent<Collider>();
        var rb = GetComponent<Rigidbody>();


        rb.isKinematic = true;
        m.enabled = false;
        col.enabled = false;

        _agent.speed = 0;
    }

    void Realty()
    {
        var m = GetComponent<MeshRenderer>();
        var col = GetComponent<Collider>();
        var rb = GetComponent<Rigidbody>();


        rb.isKinematic = false;
        m.enabled = true;
        col.enabled = true;

        _agent.speed = _speed;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed; // NavMeshAgentのスピードを変数に
        _agent.stoppingDistance = _StoppingDistance; // NavMeshAgentのStoppingDistanceを変数に
        _agent.SetDestination(_wayPoints[0].position); // 最初に向かうWayPoint
        TitleEnemy();
    }


    void Update()
    {
        // インゲーム
        if (_state == states.move)
        {
            //Debug.Log("1");
            Raycast();
        }
        // インゲーム外
        else
        {
            // Debug.Log("2");
        }
    }

    void Raycast()
    {
        bool _hitPlayer = false;
        RaycastHit hit;
        // Playerへの方向
        Vector3 forwardTarget = this._targetTransformPos.transform.position - this.transform.position;
        // DrawRayはいらなかったら消してください
        Debug.DrawRay(transform.position, forwardTarget.normalized * _maxDistance, Color.red, 0.1f);
        // hitの判定取得
        Physics.Raycast(this.transform.position, forwardTarget, out hit, _maxDistance);
        // RayCastをEnemy中心に円形に展開
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _maxDistance);
        // 当たったobjの中でPlayerを探す。見つけたら追いかける。それ以外は徘徊。
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (name != _targetTransformPos.name)
            {
                if (hitColliders[i].gameObject.tag == "Player" && hit.collider.gameObject.tag == "Player")
                {
                    _hitPlayer = true;
                }
            }
        }
        if (_hitPlayer && FOV())
        {
            NavMove();
        }
        else
        {
            Patrol();
        }
    }

    bool FOV()
    {
        Vector3 look = _lookAtTarget.position - this.transform.position;
        Vector3 target = this._targetTransformPos.position - this.transform.position;
        float cosHalfSight = Mathf.Cos(_fov / 2 * Mathf.Deg2Rad);
        float cosTarget = Vector3.Dot(look, target) / (look.magnitude * target.magnitude);
        return cosTarget > cosHalfSight && target.magnitude < _fov;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, _maxDistance);
    }

    void NavMove()
    {
        //　Playerを追いかける仕組み
        _agent.destination = _targetTransformPos.transform.position;
    }

    void Patrol()
    {
        // 徘徊
        // インスペクターから巡回する目的地の数を調整できる
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            //Debug.Log(_currentWayPointIndex);
            // 目的地の番号を１に更新
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // 目的地を次の場所に設定
            _agent.SetDestination(_wayPoints[_currentWayPointIndex].position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            AudioPlayer.PlaySE(AudioKey.SE_EnemyAttack);
        }
    }

    void TitleEnemy()
    {
        _state = states.stop;
    }

    void ResetEnemy()
    {
        this.transform.position = _startPos.transform.position;
        _state = states.move;
    }
}

enum states
{
    move, // インゲーム
    stop, // アウトゲーム
}