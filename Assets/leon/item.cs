using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    [SerializeField]
    [Tooltip("")]
    GameObject createPrefab;

    [SerializeField]
    [Tooltip("a")]
    Transform[] potisons;

    private float time;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime; //�O�̃t���[�����玞�Ԃ����Z

        if (time > 5.0f)
        {
            // �z�񂩂烉���_���ɐ����ʒu��I��
            Transform randomposition = potisons[Random.Range(0, potisons.Length)];

            // �I�����ꂽ�����ʒu�͈͓̔��Ń����_���ȍ��W���擾
            //float x = Random.Range(randomposition.position.x - 1, randomposition.position.x + 1);
            //float y = Random.Range(randomposition.position.y - 1, randomposition.position.y + 1);
            //float z = Random.Range(randomposition.position.z - 1, randomposition.position.z + 1);

            // �v���n�u�������_���Ɍ��܂����ʒu�ɐ���
            Instantiate(createPrefab, randomposition.position, createPrefab.transform.rotation);

            // �o�ߎ��ԃ��Z�b�g
            time = 0f;
        }
    }
}
