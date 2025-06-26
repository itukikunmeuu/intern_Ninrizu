using UnityEngine;

public class MeshCutterController : MonoBehaviour
{
    [SerializeField] GameObject objectA;
    [SerializeField] GameObject objectB;
    [SerializeField] Material cutMaterial; // カット後の断面に使うマテリアル

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Bの中心点と上方向でカット
            Vector3 cutPoint = objectB.transform.position;
            Vector3 cutNormal = objectB.transform.up; // 例: 上方向でカット

            // MeshCutterの静的メソッドを呼び出し
        }   
    }
}
