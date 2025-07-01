using UnityEngine;
using System.Collections.Generic;
using EzySlice; 

public class MeshCutter : MonoBehaviour
{
    [SerializeField] Material cutMaterial;
    [SerializeField] float separateDistance = 0.1f;
    [SerializeField] float forcePower = 3f; // 断片に与える力の大きさ

    // カット処理を外部から呼び出せるように
    public void CutCarrot(GameObject carrot)
    {
        Vector3 cutPosition = carrot.transform.position;
        Vector3 cutNormal = Vector3.right; // 縦切り（X軸方向）

        // EzySliceでスライス
        GameObject[] result = carrot.SliceInstantiate(cutPosition, cutNormal, cutMaterial);

        if (result != null && result.Length == 2)
        {
            // 断片ごとに進行方向を変える
            Vector3[] directions = {
                (Vector3.right + Vector3.forward - Vector3.up).normalized,   // 右前下
                (-Vector3.right + Vector3.forward - Vector3.up).normalized   // 左前下
            };

            for (int i = 0; i < 2; i++)
            {
                var go = result[i];

                // 元のにんじんの位置・回転・スケールを引き継ぐ
                go.transform.position = carrot.transform.position + (i == 0 ? cutNormal : -cutNormal) * separateDistance;
                go.transform.rotation = carrot.transform.rotation;
                go.transform.localScale = carrot.transform.localScale;

                // Rigidbody追加で物理挙動
                var rb = go.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = go.AddComponent<Rigidbody>();
                }

                // ColliderがなければMeshColliderを追加
                if (go.GetComponent<Collider>() == null)
                {
                    var meshCol = go.AddComponent<MeshCollider>();
                    meshCol.convex = true;
                }

                // 斜め前下方向に力を加える
                rb.AddForce(directions[i] * forcePower, ForceMode.Impulse);
            }

            // 元のにんじんを削除
            Destroy(carrot);
        }
    }
}
