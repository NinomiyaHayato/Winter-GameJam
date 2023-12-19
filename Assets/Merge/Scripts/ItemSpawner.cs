using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

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
            for(int i= 0; i < _count; i++) g[i] = Instantiate(_prefab);
            return g;
        }
    }
    #endregion

    [Header(Const.PreColorTag + "��������A�C�e��" + Const.SufColorTag)]
    [SerializeField] Data[] _itemData;
    [Header("�����ʒu")]
    [SerializeField] Transform[] _spawnPoints;

    void Start()
    {
        List<GameObject> items = Create();

        EntryPoint.OnPreInGameStart += Spawn;
        EntryPoint.OnInGameReset += Delete;
    }

    // �S��ނ̃A�C�e���𐶐����ĕԂ�
    List<GameObject> Create()
    {
        // ���������A�C�e���𐮗����邽�߂ɐe�����
        Transform itemParent = new GameObject("ItemParent").transform;

        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < _itemData.Length; i++)
        {
            GameObject[] temp = _itemData[i].Create();
            foreach (GameObject g in temp) g.transform.parent = itemParent;

            items.AddRange(temp);
        }

        return items;
    }

    void Spawn()
    {
        
    }

    void Delete()
    {

    }
}
