using Cinemachine;
using UnityEngine;

public class PlayerLockerHide : MonoBehaviour
{
    [SerializeField] GameObject _playerVirtualCamera;
    [SerializeField][Tooltip("���b�J�[���痣�ꂽ���̃��b�J�[�Ƃ̋���")]
    float _lockerExitDistance;
    CinemachineVirtualCamera _cvc;
    
    int _cameraPriority;
    // Start is called before the first frame update
    void Start()
    {
        _cvc = _playerVirtualCamera.GetComponent<CinemachineVirtualCamera>();
        _cameraPriority = _cvc.Priority;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Locker")
        {
            gameObject.tag = "Hide";//Enemy�Ɋ��m����Ȃ��^�O�ɕύX
            GetComponent<MeshRenderer>().enabled = false;//�v���C���[�𓧖���
            //�J���������b�J�[�ɂ��Ă���V�l�}�V�[���ɕύX
            collision.gameObject.transform.GetChild(0).
                gameObject.GetComponent<CinemachineVirtualCamera>().Priority = _cameraPriority + 1;
            //�v���C���[�̌��������b�J�[�̐��ʂɕύX
            gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Locker")
        {
            gameObject.tag = "Player";//�^�O���v���C���[�ɖ߂�
            GetComponent<MeshRenderer>().enabled = true;//�v���C���[�𓧖�����߂�
            //�J�������v���C���[�ɂ��Ă���V�l�}�V�[���ɖ߂�
            collision.gameObject.transform.GetChild(0).
                gameObject.GetComponent<CinemachineVirtualCamera>().Priority = _cameraPriority - 1;
            //�v���C���[�ɂ��Ă���V�l�}�V�[���̌��������b�J�[�̐��ʂɕύX
            _cvc.GetCinemachineComponent<CinemachinePOV>().
                m_HorizontalAxis.Value = Camera.main.transform.localEulerAngles.y;
            //�����̈ʒu�����b�J�[�̖ڂ̑O�ɕύX
            this.gameObject.transform.position =
                collision.transform.position + collision.gameObject.transform.forward*_lockerExitDistance;
        }
    }
}
