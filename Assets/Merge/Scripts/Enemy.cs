using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;

public class Enemy : MonoBehaviour
{
    [SerializeField] CharacterController _controller;
    [Header("�L�����N�^�[�̌����𔽉f����I�u�W�F�N�g")]
    [SerializeField] Transform _rotateChild;
    [Header(Const.PreColorTag + "���E�̔��a" + Const.SufColorTag)]
    [SerializeField] float _sightRadius = 5.0f;
    [Header(Const.PreColorTag + "�v���C���[���������̈ړ����x" + Const.SufColorTag)]
    [SerializeField] float _freeMoveSpeed = 1.0f;
    [Header(Const.PreColorTag + "�v���C���[�ǐՎ��̈ړ����x" + Const.SufColorTag)]
    [SerializeField] float _chaseMoveSpeed = 2.0f;

    void Start()
    {
        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask UpdateAsync(CancellationToken token)
    {
        Player player = FindPlayer();
        Transform transform = this.transform;
        Collider[] detected = new Collider[1]; // �v���C���[�̂݌��o����̂Œ���1�ŏ\��

        while (!token.IsCancellationRequested)
        {
            // ����Ƀv���C���[���ړ��������ǂ����̔��肪����̂ŁA������await���Ȃ��Ɩ������[�v�Ɋׂ�
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);

            // �v���C���[���ړ��������Ŏ��Ԓ�~�𔻒肷��
            if (!player.IsMoving.Value) continue;

            if (DetectPlayer(detected))
            {
                LookToPlayer(transform, player);
                MoveToPlayer(transform, player);
            }
        }
    }

    // �V�[����̃v���C���[���擾
    Player FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Const.PlayerTag);
        if (player == null)
        {
            throw new System.NullReferenceException("�v���C���[���V�[����ɑ��݂��Ȃ�: " + Const.PlayerTag);
        }
        return player.GetComponent<Player>();
    }

    // �v���C���[�����E���ɂ��邩���^�O�Ŕ���
    bool DetectPlayer(Collider[] detected)
    {
        for (int i = 0; i < detected.Length; i++) detected[i] = null;
        Physics.OverlapSphereNonAlloc(transform.position, _sightRadius, detected, Const.PlayerLayer);

        foreach (Collider c in detected)
        {
            if (c == null) break;
            if (c.gameObject.CompareTag(Const.PlayerTag)) return true;
        }

        return false;
    }

    // �v���C���[�Ɍ�����
    void LookToPlayer(Transform transform, Player player)
    {
        _rotateChild.forward = transform.NormalizedDirection(player.transform);
    }

    // �v���C���[�Ɍ����Ĉړ�����
    void MoveToPlayer(Transform transform, Player player)
    {
        Vector3 dir = transform.NormalizedDirection(player.transform);
        _controller.Move(dir * Time.deltaTime * _chaseMoveSpeed);
    }

    void OnDrawGizmos()
    {
        // ���E�͈̔͂�`��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _sightRadius);
    }
}
