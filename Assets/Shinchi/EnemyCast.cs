using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class EnemyCast : MonoBehaviour
{
    State _state = State.stop; // �X�e�[�g

    [SerializeField] float _speed;
    [SerializeField] float _maxDistance;
    [SerializeField] GameObject _startPos;
    [SerializeField] Transform _targetTransform;
    [SerializeField] LayerMask _layerMask;

    NavMeshAgent _agent;
    RaycastHit hit;

    void Start()
    {

    }


    void Update()
    {
        // �C���Q�[���O
        if (_state == State.stop)
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