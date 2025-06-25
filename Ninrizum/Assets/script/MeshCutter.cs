using UnityEngine;
using System.Collections.Generic;

public class MeshCutter : MonoBehaviour
{
    public Transform cutPlane;

    [ContextMenu("Cut Mesh")]
    public void Cut()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogError("MeshFilterがありません");
            return;
        }

        Mesh originalMesh = mf.sharedMesh;
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;

        List<Vector3> leftVerts = new List<Vector3>();
        List<int> leftTris = new List<int>();
        List<Vector3> rightVerts = new List<Vector3>();
        List<int> rightTris = new List<int>();

        Plane plane = new Plane(
            cutPlane.up,
            transform.InverseTransformPoint(cutPlane.position) // ローカル空間で計算
        );

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            bool s0 = plane.GetSide(v0);
            bool s1 = plane.GetSide(v1);
            bool s2 = plane.GetSide(v2);

            if (s0 == s1 && s1 == s2)
            {
                // 全て同じ側にある → その側に追加
                if (s0)
                    AddTriangle(rightVerts, rightTris, v0, v1, v2);
                else
                    AddTriangle(leftVerts, leftTris, v0, v1, v2);
            }
            else
            {
                // 平面をまたいでいる三角形 → ここでは無視（※簡易版のため）
                continue;
            }
        }

        // 分割後のメッシュを作成してオブジェクトに反映
        CreateObject("LeftPart", leftVerts, leftTris, Color.red);
        CreateObject("RightPart", rightVerts, rightTris, Color.green);

        gameObject.SetActive(false); // 元のオブジェクト非表示
    }

    void AddTriangle(List<Vector3> verts, List<int> tris, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        int start = verts.Count;
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        tris.Add(start);
        tris.Add(start + 1);
        tris.Add(start + 2);
    }

    void CreateObject(string name, List<Vector3> verts, List<int> tris, Color color)
    {
        if (verts.Count == 0) return;

        GameObject obj = new GameObject(name);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.localScale = transform.localScale;

        Mesh m = new Mesh();
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        mf.mesh = m;
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        mr.material = mat;
    }
}