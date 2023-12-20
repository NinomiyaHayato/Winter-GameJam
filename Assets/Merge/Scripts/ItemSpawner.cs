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
            for(int i= 0; i < _count; i++)
            {
                g[i] = Instantiate(_prefab);
                g[i].transform.localScale = Vector3.zero; // 大きさ0で生成する
            }
            return g;
        }
    }
    #endregion

    [Header(Const.PreColorTag + "生成するアイテム" + Const.SufColorTag)]
    [SerializeField] Data[] _itemData;
    [Header("生成位置")]
    [SerializeField] Transform[] _spawnPoints;
    [Header("決められたY座標に生成する")]
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

    // 全種類のアイテムを生成して返す
    List<GameObject> Create()
    {
        // 生成したアイテムを整理するために親を作る
        Transform itemParent = new GameObject("ItemParent").transform;

        // アイテムを種類ごとに複数個生成し、一括で管理するようにリストに格納する
        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < _itemData.Length; i++)
        {
            GameObject[] temp = _itemData[i].Create();
            foreach (GameObject g in temp) g.transform.parent = itemParent;

            items.AddRange(temp);
        }

        return items;
    }

    // ランダムな位置に配置する
    void Spawn(List<GameObject> items)
    {
        // 位置をランダムな順で一時的な配列に格納して、先頭から順に配置していく
        Transform[] temp = _spawnPoints.OrderBy(_ => System.Guid.NewGuid()).ToArray();
        if (temp.Length < items.Count)
        {
            throw new System.Exception("アイテムに対して配置する場所が足りない");
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.localScale = Vector3.one;
            items[i].transform.position = new Vector3(temp[i].position.x, _spawnHeight, temp[i].position.z);
        }
    }

    // 配置したアイテムを消す
    void Delete(List<GameObject> items)
    {
        foreach (GameObject g in items)
        {
            g.transform.localScale = Vector3.zero;
        }
    }
}
