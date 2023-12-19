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
/// ���I�ɐ��������A�V�[����ɏ��߂���z�u����Ă��邱�Ƃ��O��̃v���C���[�N���X
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// �v���C���[���C���Q�[���̃~�X�����𖞂������ۂɌĂ΂��R�[���o�b�N
    /// </summary>
    public static UnityAction OnMissed;

    [SerializeField] CharacterController _controller;
    [Header("�A�C�e����G�ƏՓ˂��锻��")]
    [SerializeField] Collider _hitTrigger;
    [Header("�Q�[���J�n���̒n�_")]
    [SerializeField] Transform _respawnPoint;
    [Header(Const.PreColorTag + "�ړ����x" + Const.SufColorTag)]
    [SerializeField] float _walkSpeed = 1.0f;
    [SerializeField] float _runSpeed = 2.0f;

    ReactiveProperty<bool> _isMoving = new(false);

    /// <summary>
    /// �v���C���[�������Ă��邩�̃t���O
    /// ���̃t���O�����Ď��Ԓ�~/�������s���B
    /// </summary>
    public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;

    void Awake()
    {
        Cursor.visible = false;

        bool isValid = true;
        this.UpdateAsObservable().Where(_ => isValid).Subscribe(_ => _isMoving.Value = Move());

        // �A�C�e�����l������
        _hitTrigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.ItemTag))
            .Subscribe(c => OnGetItem(c.gameObject));

        // �G�ƏՓ˂���
        _hitTrigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.EnemyTag))
            .Subscribe(_ => OnHitEnemy());

        // �Q�[���̏�ԑJ�ڂɏ������������t�b�N
        EntryPoint.OnTitleEnter += Initialize;
        EntryPoint.OnInGameReset += Initialize;
        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnTitleEnter -= Initialize;
            EntryPoint.OnInGameReset -= Initialize;
        });
    }

    // �Q�[���J�n�O�ɏ���������
    void Initialize()
    {
        _isMoving.Value = false;

        if (_respawnPoint != null)
        {
            // CharacterController�̎d�l�ł������Ȃ���tranform.position�̏������������f����Ȃ�
            _controller.enabled = false;
            _controller.transform.position = _respawnPoint.position;
            _controller.enabled = true;
        }
    }

    // ���͂��󂯎���Ĉړ�����
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

        // �ړ������ꍇ��true��Ԃ�
        return input != default;
    }

    // �A�C�e�����l������
    void OnGetItem(GameObject item)
    {
        Destroy(item);
        PlantManager.StepProgress();
    }

    // �G�ƂԂ�����
    void OnHitEnemy()
    {
        OnMissed?.Invoke();
    }
}