using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    states _state = states.stop; // ステート

    [SerializeField] float _speed;
    [SerializeField] float _maxDistance;
    [SerializeField] Transform _startPos;
    [SerializeField] Transform _targetTransformPos;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Transform[] _wayPoints;

    int _currentWayPointIndex;
    NavMeshAgent _agent;

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
        _agent.SetDestination(_wayPoints[0].position);
        TitleEnemy();
    }


    void Update()
    {
        // インゲーム外
        if (_state == states.stop)
        {

        }
        // インゲーム
        else
        {
            Raycast();
        }
    }

    void NavMove()
    {
       //if(Vector3.Distance()
    }

    void Patrol()
    {
        // インスペクターから巡回する目的地の数を調整できる
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            // 目的地の番号を１に更新
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // 目的地を次の場所に設定
            _agent.SetDestination(_wayPoints[_currentWayPointIndex].position);
        }
    }


    void Raycast()
    {
        bool _hitPlayer = false;

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _maxDistance);
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