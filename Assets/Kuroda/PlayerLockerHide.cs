using Cinemachine;
using UnityEngine;

public class PlayerLockerHide : MonoBehaviour
{
    [SerializeField] GameObject _playerVirtualCamera;
    [SerializeField][Tooltip("ロッカーから離れた時のロッカーとの距離")]
    float _lockerExitDistance;
    CinemachineVirtualCamera _cvc;
    [SerializeField][Tooltip("最低でもロッカーにいる時間")] float _lockerTimerMin = 0.5f;
    float _lockerTimer = 0;

    GameObject _hitCollisionGameobject;
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
        if(gameObject.tag == "Hide" && Input.GetKeyDown(KeyCode.W))
        {
            //自分の位置をロッカーの目の前に変更
            this.gameObject.transform.position =
                _hitCollisionGameobject.transform.position + _hitCollisionGameobject.gameObject.transform.forward * _lockerExitDistance;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Locker")
        {
            gameObject.tag = "Hide";//Enemyに感知されないタグに変更
            GetComponent<MeshRenderer>().enabled = false;//プレイヤーを透明に
            //カメラをロッカーについているシネマシーンに変更
            collision.gameObject.transform.GetChild(0).
                gameObject.GetComponent<CinemachineVirtualCamera>().Priority = _cameraPriority + 1;
            //プレイヤーの向きをロッカーの正面に変更
            gameObject.transform.rotation = Camera.main.transform.rotation;
            _hitCollisionGameobject = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Locker")
        {
            gameObject.tag = "Player";//タグをプレイヤーに戻す
            GetComponent<MeshRenderer>().enabled = true;//プレイヤーを透明から戻す
            //カメラをプレイヤーについているシネマシーンに戻す
            collision.gameObject.transform.GetChild(0).
                gameObject.GetComponent<CinemachineVirtualCamera>().Priority = _cameraPriority - 1;
            //プレイヤーについているシネマシーンの向きをロッカーの正面に変更
            _cvc.GetCinemachineComponent<CinemachinePOV>().
                m_HorizontalAxis.Value = Camera.main.transform.localEulerAngles.y;
        }
    }
}
