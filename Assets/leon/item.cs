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
        time += Time.deltaTime; //前のフレームから時間を加算

        if (time > 5.0f)
        {
            // 配列からランダムに生成位置を選択
            Transform randomposition = potisons[Random.Range(0, potisons.Length)];

            // 選択された生成位置の範囲内でランダムな座標を取得
            //float x = Random.Range(randomposition.position.x - 1, randomposition.position.x + 1);
            //float y = Random.Range(randomposition.position.y - 1, randomposition.position.y + 1);
            //float z = Random.Range(randomposition.position.z - 1, randomposition.position.z + 1);

            // プレハブをランダムに決まった位置に生成
            Instantiate(createPrefab, randomposition.position, createPrefab.transform.rotation);

            // 経過時間リセット
            time = 0f;
        }
    }
}
