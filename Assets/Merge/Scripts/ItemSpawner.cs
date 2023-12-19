using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ItemSpawner : MonoBehaviour
{
    #region ��������A�C�e���̃f�[�^�BCreate���\�b�h�Ő������ĕԂ�
    [System.Serializable]
    class Data
    {
        [SerializeField] string _name;
        [SerializeField] GameObject _prefab;
        [SerializeField] int _count;

        public GameObject[] Create()
        {
            GameObject[] g = new GameObject[_count];
            for(int i= 0; i < _count; i++)
            {
                g[i] = Instantiate(_prefab);
                g[i].transform.localScale = Vector3.zero; // �傫��0�Ő�������
            }
            return g;
        }
    }
    #endregion

    [Header(Const.PreColorTag + "��������A�C�e��" + Const.SufColorTag)]
    [SerializeField] Data[] _itemData;
    [Header("�����ʒu")]
    [SerializeField] Transform[] _spawnPoints;
    [Header("���߂�ꂽY���W�ɐ�������")]
    [SerializeField] float _spawnHeight = 1;

    void Awake()
    {
        List<GameObject> items = Create();

        EntryPoint.OnInGameReset += () => Delete(items);
        Worldkeeper.OnRealityEnter += () => Delete(items);
        Worldkeeper.OnDreamEnter += () => Spawn(items);

        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnInGameReset -= () => Delete(items);
            Worldkeeper.OnRealityEnter -= () => Delete(items);
            Worldkeeper.OnDreamEnter -= () => Spawn(items);
        });
    }

    // �S��ނ̃A�C�e���𐶐����ĕԂ�
    List<GameObject> Create()
    {
        // ���������A�C�e���𐮗����邽�߂ɐe�����
        Transform itemParent = new GameObject("ItemParent").transform;

        // �A�C�e������ނ��Ƃɕ����������A�ꊇ�ŊǗ�����悤�Ƀ��X�g�Ɋi�[����
        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < _itemData.Length; i++)
        {
            GameObject[] temp = _itemData[i].Create();
            foreach (GameObject g in temp) g.transform.parent = itemParent;

            items.AddRange(temp);
        }

        return items;
    }

    // �����_���Ȉʒu�ɔz�u����
    void Spawn(List<GameObject> items)
    {
        // �ʒu�������_���ȏ��ňꎞ�I�Ȕz��Ɋi�[���āA�擪���珇�ɔz�u���Ă���
        Transform[] temp = _spawnPoints.OrderBy(_ => System.Guid.NewGuid()).ToArray();
        if (temp.Length < items.Count)
        {
            throw new System.Exception("�A�C�e���ɑ΂��Ĕz�u����ꏊ������Ȃ�");
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.localScale = Vector3.one;
            items[i].transform.position = new Vector3(temp[i].position.x, _spawnHeight, temp[i].position.z);
        }
    }

    // �z�u�����A�C�e��������
    void Delete(List<GameObject> items)
    {
        foreach (GameObject g in items)
        {
            g.transform.localScale = Vector3.zero;
        }
    }
}
