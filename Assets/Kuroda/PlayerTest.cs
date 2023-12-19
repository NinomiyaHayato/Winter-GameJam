using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTest : MonoBehaviour
{
    [SerializeField]state _state = state.stop;
    [SerializeField] GameObject _camera;
    [SerializeField] float _playerMoveSpeed = 5;
    [SerializeField] GameObject _startPosition;
    Rigidbody _rb;
    Animator _anim;
    float _horizontal;
    float _vertical;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if(GetComponent<Animator>())
        {
            _anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_state == state.stop)
        {
            
        }//�^�C�g���A���U���g
        else
        {
            //input
            PlayerInput();
        }//�C���Q�[��
    }

    private void FixedUpdate()
    {
        if(_state == state.move)
        {
            PlayerMove();
        }
    }
    void PlayerInput()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
    }
    void PlayerMove()
    {
        _rb.AddForce(_camera.transform.forward * _vertical * _playerMoveSpeed);
        _rb.AddForce(_camera.transform.right * _horizontal * _playerMoveSpeed);
        Vector3 direction = _camera.transform.forward;
        direction.y = 0;
        transform.forward = direction;
        if(GetComponent<Animator>())
        {
            _anim.SetFloat("MoveSpeed",_rb.velocity.magnitude);
        }
    }

    void ResetsPlayer()
    {
        this.transform.position = _startPosition.transform.position;
        _state = state.move;
    }
}
enum state
{
    move,// �C���Q�[��
    stop,// �^�C�g���A���U���g
}