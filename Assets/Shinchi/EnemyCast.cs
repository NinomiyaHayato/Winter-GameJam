using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCast : MonoBehaviour
{
    [SerializeField]
    [Tooltip("State�̎��")] states _state = states.move;
    [SerializeField]
    [Tooltip("�ړ����x")] float _speed;
    [SerializeField]
    [Tooltip("Player�F���͈�")] float _maxDistance;
    [SerializeField]
    [Tooltip("StoppingDistance�͈�")] float _StoppingDistance;
    [SerializeField]
    [Tooltip("����p"), Range(0, 180)] float _fov;
    [SerializeField]
    [Tooltip("�X�^�[�g�|�W�V����")] Transform _startPos;
    [SerializeField]
    [Tooltip("Player�̈ʒu")] Transform _targetTransformPos;
    [SerializeField]
    [Tooltip("WayPoints�̐��ƈʒu")] Transform[] _wayPoints;
    [SerializeField]
    [Tooltip("���_�����������I�u�W�F�N�g")] Transform _lookAtTarget;

    int _currentWayPointIndex; // ���݂�WayPoint�̐�

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
        _agent.speed = _speed; // NavMeshAgent�̃X�s�[�h��ϐ���
        _agent.stoppingDistance = _StoppingDistance; // NavMeshAgent��StoppingDistance��ϐ���
        _agent.SetDestination(_wayPoints[0].position); // �ŏ��Ɍ�����WayPoint
        TitleEnemy();
    }


    void Update()
    {
        // �C���Q�[��
        if (_state == states.move)
        {
            //Debug.Log("1");
            Raycast();
        }
        // �C���Q�[���O
        else
        {
            // Debug.Log("2");
        }
    }

    void Raycast()
    {
        bool _hitPlayer = false;
        RaycastHit hit;
        // Player�ւ̕���
        Vector3 forwardTarget = this._targetTransformPos.transform.position - this.transform.position;
        // DrawRay�͂���Ȃ�����������Ă�������
        Debug.DrawRay(transform.position, forwardTarget.normalized * _maxDistance, Color.red, 0.1f);
        // hit�̔���擾
        Physics.Raycast(this.transform.position, forwardTarget, out hit, _maxDistance);
        // RayCast��Enemy���S�ɉ~�`�ɓW�J
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _maxDistance);
        // ��������obj�̒���Player��T���B��������ǂ�������B����ȊO�͜p�j�B
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
        //�@Player��ǂ�������d�g��
        _agent.destination = _targetTransformPos.transform.position;
    }

    void Patrol()
    {
        // �p�j
        // �C���X�y�N�^�[���珄�񂷂�ړI�n�̐��𒲐��ł���
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            //Debug.Log(_currentWayPointIndex);
            // �ړI�n�̔ԍ����P�ɍX�V
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;
            // �ړI�n�����̏ꏊ�ɐݒ�
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
    move, // �C���Q�[��
    stop, // �A�E�g�Q�[��
}