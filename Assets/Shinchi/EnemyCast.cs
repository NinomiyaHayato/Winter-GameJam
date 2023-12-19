using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    [SerializeField]
    [Tooltip("State�̎��")] states _state = states.stop;
    [SerializeField]
    [Tooltip("�ړ����x")] float _speed;
    [SerializeField]
    [Tooltip("Player�F���͈�")] float _maxDistance;
    [SerializeField]
    [Tooltip("StoppingDistance�͈�")] float _StoppingDistance;
    //[SerializeField]
    //[Tooltip("����p"), Range(0, 180)] float _fov;
    [SerializeField]
    [Tooltip("�X�^�[�g�|�W�V����")] Transform _startPos;
    [SerializeField]
    [Tooltip("Player�̈ʒu")] Transform _targetTransformPos;
    [SerializeField]
    [Tooltip("WayPoints�̐��ƈʒu")] Transform[] _wayPoints;

    int _currentWayPointIndex; // ���݂�WayPoint�̐�
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
        _agent.speed = _speed; // NavMeshAgent�̃X�s�[�h��ϐ���
        _agent.stoppingDistance = _StoppingDistance; // NavMeshAgent��StoppingDistance��ϐ���
        _agent.SetDestination(_wayPoints[0].position); // �ŏ��Ɍ�����WayPoint
        TitleEnemy();
    }


    void Update()
    {
        // �C���Q�[���O
        if (_state == states.stop)
        {
            Debug.Log("1");
        }
        // �C���Q�[��
        else
        {
            Debug.Log("2");
            Raycast();
        }
    }

    void Raycast()
    {
        bool _hitPlayer = false;
        // RayCast��Enemy���S�ɉ~�`�ɓW�J
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _maxDistance);
        // ��������obj�̒���Player��T���B��������ǂ�������B����ȊO�͜p�j�B
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
        //�@Player��ǂ�������d�g��
        _agent.destination = _targetTransformPos.transform.position;
    }

    void Patrol()
    {
        // �p�j
        // �C���X�y�N�^�[���珄�񂷂�ړI�n�̐��𒲐��ł���
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Debug.Log(_currentWayPointIndex);
            // �ړI�n�̔ԍ����P�ɍX�V
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // �ړI�n�����̏ꏊ�ɐݒ�
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
    move, // �C���Q�[��
    stop, // �A�E�g�Q�[��
}