using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

public class ItemSpawner : MonoBehaviour
{
    #region 生成するアイテムのデータ。Createメソッドで生成して返す
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

    [Header(Const.PreColorTag + "生成するアイテム" + Const.SufColorTag)]
    [SerializeField] Data[] _itemData;
    [Header("生成位置")]
    [SerializeField] Transform[] _spawnPoints;

    void Start()
    {
        List<GameObject> items = Create();

        EntryPoint.OnPreInGameStart += Spawn;
        EntryPoint.OnInGameReset += Delete;
    }

    // 全種類のアイテムを生成して返す
    List<GameObject> Create()
    {
        // 生成したアイテムを整理するために親を作る
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
