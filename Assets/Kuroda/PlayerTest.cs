using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTest : MonoBehaviour
{
    [SerializeField] state _state = state.stop;
    [SerializeField] float _playerMoveSpeed = 5;
    [SerializeField]
    [Tooltip("�_�b�V�����̃X�s�[�h�{��")]
    float _playerDashSpeed = 1.5f;
    [SerializeField] GameObject _startPosition;
    [SerializeField][Tooltip("�X�^�~�i�̍ő�l")] float _maxDashStaminaTimer = 5;
    public float _currentStamina;//���݂̃X�^�~�i
    [SerializeField ,Range(0,1)][Tooltip("�����{�[�_�[(����)")] float _dashBordar = 0.5f;
    public bool _dashPossible = false;
    

    Rigidbody _rb;
    Animator _anim;
    float _horizontal;
    float _vertical;
    // Start is called before the first frame update

    private void Awake()
    {
        EntryPoint.OnInGameStart += ResetsPlayer;
    }

    private void OnDestroy()
    {
        EntryPoint.OnInGameStart -= ResetsPlayer;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _currentStamina = _maxDashStaminaTimer;
        if (GetComponent<Animator>())
        {
            _anim = GetComponent<Animator>();
        }
        TitlePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == state.move)
        {
            //input
            PlayerInput();
        }//�C���Q�[��
    }

    private void FixedUpdate()
    {
        if (_state == state.move)
        {
            PlayerMove();
        }
    }
    void PlayerInput()
    {
        if (gameObject.tag == "Player")
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");
        }
    }
    void PlayerMove()
    {
        float dash = 1;
        if (_vertical > 0 && Input.GetKey(KeyCode.LeftShift) && _dashPossible == true && _currentStamina >= 0)
        {
            dash = _playerDashSpeed;
            _currentStamina -= Time.deltaTime;
        }
        if (!Input.GetKey(KeyCode.LeftShift) && _maxDashStaminaTimer * _dashBordar >= _currentStamina || _currentStamina <= 0)
        {
            _dashPossible = false;
        }
        if(_maxDashStaminaTimer * _dashBordar <= _currentStamina)
        {
            _dashPossible = true;
        }
        if(_maxDashStaminaTimer > _currentStamina)
        {
            _currentStamina += Time.deltaTime * 1/2;
        }
        _rb.AddForce(Camera.main.transform.forward * _vertical * _playerMoveSpeed * dash);
        Debug.Log(_vertical * _playerMoveSpeed * dash +  " " + _horizontal * _playerMoveSpeed);
        _rb.AddForce(Camera.main.transform.right * _horizontal * _playerMoveSpeed);
        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0;
        transform.forward = direction;
        if (GetComponent<Animator>())
        {
            _anim.SetFloat("MoveSpeed", _rb.velocity.magnitude);
        }
    }

    void ResetsPlayer()
    {
        this.transform.position = _startPosition.transform.position;
        _state = state.move;
    }

    void TitlePlayer()
    {
        _state = state.stop;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            Destroy(other.gameObject);
            Debug.Log("�A�C�e���̎擾");
            PlantManager.StepProgress();
            _rb.velocity = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && this.gameObject.tag == "Player")
        {
            Debug.Log("GAMEOVER");
            // GAMEOVER�̏���
        }
    }
}
enum state
{
    move,// �C���Q�[��
    stop,// �^�C�g���A���U���g
}