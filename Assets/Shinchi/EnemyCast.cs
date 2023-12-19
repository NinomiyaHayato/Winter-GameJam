using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    states _state = states.stop; // �X�e�[�g

    [SerializeField] float _speed;
    [SerializeField] float _maxDistance;
    [SerializeField] GameObject _startPos;
    [SerializeField] Transform _targetTransform;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Transform[] _wayPoints;

    int _currentWayPointIndex;
    NavMeshAgent _agent;
    RaycastHit hit;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(_wayPoints[0].position);
    }


    void Update()
    {
        // �C���Q�[���O
        if (_state == states.stop)
        {

        }
        // �C���Q�[��
        else
        {
            Raycast();
        }
    }

    void NavMove()
    {

    }

    void Patrol()
    {
        // �C���X�y�N�^�[���珄�񂷂�ړI�n�̐��𒲐��ł���
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            // �ړI�n�̔ԍ����P�ɍX�V
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // �ړI�n�����̏ꏊ�ɐݒ�
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

    void ResetEnemy()
    {

    }
}

enum states
{
    move, // �C���Q�[��
    stop, // �A�E�g�Q�[��
}