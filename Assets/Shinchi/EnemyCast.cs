using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Stateの種類")] states _state = states.stop;
    [SerializeField]
    [Tooltip("移動速度")] float _speed;
    [SerializeField]
    [Tooltip("Player認識範囲")] float _maxDistance;
    [SerializeField]
    [Tooltip("StoppingDistance範囲")] float _StoppingDistance;
    //[SerializeField]
    //[Tooltip("視野角"), Range(0, 180)] float _fov;
    [SerializeField]
    [Tooltip("スタートポジション")] Transform _startPos;
    [SerializeField]
    [Tooltip("Playerの位置")] Transform _targetTransformPos;
    [SerializeField]
    [Tooltip("WayPointsの数と位置")] Transform[] _wayPoints;

    int _currentWayPointIndex; // 現在のWayPointの数
    NavMeshAgent _agent; // NavMeshAgent
    // bool _isVisible = true;

    void Awake()
    {
        EntryPoint.OnInGameStart += ResetEnemy;
    }

    void OnDestroy()
    {
        EntryPoint.OnInGameStart -= ResetEnemy;
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
        // インゲーム外
        if (_state == states.stop)
        {
            Debug.Log("1");
        }
        // インゲーム
        else
        {
            Debug.Log("2");
            Raycast();
        }
    }

    void Raycast()
    {
        bool _hitPlayer = false;
        // RayCastをEnemy中心に円形に展開
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _maxDistance);
        // 当たったobjの中でPlayerを探す。見つけたら追いかける。それ以外は徘徊。
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log(hitColliders[i].gameObject.name);
            if (hitColliders[i].gameObject.tag == "Player")
            {
                _hitPlayer = true;
            }
        }
        if (!_hitPlayer)
        {
            Patrol();
        }
        else
        {
            NavMove();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
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
            Debug.Log(_currentWayPointIndex);
            // 目的地の番号を１に更新
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // 目的地を次の場所に設定
            _agent.SetDestination(_wayPoints[_currentWayPointIndex].position);
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