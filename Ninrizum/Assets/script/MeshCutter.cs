using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCutter : MonoBehaviour
{
    // カットに使う平面を指定するTransform（平面の法線はcutPlane.up）
    public Transform cutPlane;

    // コンテキストメニューから呼べる「Cut Mesh」メソッド
    [ContextMenu("Cut Mesh")]
    void Cut()
    {
        // MeshFilter取得
        var mf = GetComponent<MeshFilter>();
        if (!mf)
        {
            Debug.LogError("MeshFilter not found!"); // MeshFilterがない場合はエラー表示して終了
            return;
        }

        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;   // 頂点配列
        int[] triangles = mesh.triangles;     // 三角形のインデックス配列

        // カット平面をローカル座標で作成
        Plane plane = new Plane(cutPlane.up, transform.InverseTransformPoint(cutPlane.position));

        // 左側（平面の裏側）用の頂点・三角形リスト
        List<Vector3> leftVerts = new List<Vector3>();
        List<int> leftTris = new List<int>();

        // 右側（平面の表側）用の頂点・三角形リスト
        List<Vector3> rightVerts = new List<Vector3>();
        List<int> rightTris = new List<int>();

        // 三角形ごとに処理
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            // 各頂点が平面のどちら側か判定（true: 表側, false: 裏側）
            bool s0 = plane.GetSide(v0);
            bool s1 = plane.GetSide(v1);
            bool s2 = plane.GetSide(v2);

            int sideCount = (s0 ? 1 : 0) + (s1 ? 1 : 0) + (s2 ? 1 : 0);

            if (sideCount == 0)
            {
                // すべて裏側なら左側メッシュに追加
                AddTriangle(leftVerts, leftTris, v0, v1, v2);
            }
            else if (sideCount == 3)
            {
                // すべて表側なら右側メッシュに追加
                AddTriangle(rightVerts, rightTris, v0, v1, v2);
            }
            else
            {
                // 頂点が混在する場合は三角形を分割
                SplitTriangle(plane, v0, v1, v2, s0, s1, s2, leftVerts, leftTris, rightVerts, rightTris);
            }
        }

        // 左右に分割したメッシュを新しいGameObjectとして生成
        CreateObject("LeftPart", leftVerts, leftTris, Color.red);
        CreateObject("RightPart", rightVerts, rightTris, Color.green);

        // 元のオブジェクトは非表示にする
        gameObject.SetActive(false);
    }

    // 頂点3つから三角形を作り、リストに追加するヘルパー関数
    void AddTriangle(List<Vector3> verts, List<int> tris, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        int baseIndex = verts.Count;
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        tris.Add(baseIndex);
        tris.Add(baseIndex + 1);
        tris.Add(baseIndex + 2);
    }

    // a-bの線分と平面pの交点を計算
    Vector3 Intersect(Vector3 a, Vector3 b, Plane p)
    {
        Vector3 dir = b - a;
        float dist = 0f;
        Ray ray = new Ray(a, dir);
        p.Raycast(ray, out dist);
        return a + dir.normalized * dist;
    }

    // 三角形の頂点のうち、一部が平面の片側にある場合に分割処理
    void SplitTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2, bool s0, bool s1, bool s2,
        List<Vector3> leftVerts, List<int> leftTris, List<Vector3> rightVerts, List<int> rightTris)
    {
        // 1頂点のみが異なる側にあるケースを想定（3通りの場合分け）

        Vector3 keep0, keep1, cut;
        bool keepSide;

        if (s0 == s1 && s0 != s2)
        {
            keep0 = v0; keep1 = v1; cut = v2; keepSide = s0;
        }
        else if (s0 == s2 && s0 != s1)
        {
            keep0 = v0; keep1 = v2; cut = v1; keepSide = s0;
        }
        else
        {
            keep0 = v1; keep1 = v2; cut = v0; keepSide = s1;
        }

        // 2つの交点を計算
        Vector3 i0 = Intersect(keep0, cut, plane);
        Vector3 i1 = Intersect(keep1, cut, plane);

        // 四角形状の形状を三角形2つに分割し、
        // それぞれの側のメッシュに追加
        if (keepSide)
        {
            // keep側の三角形2つを追加
            AddTriangle(rightVerts, rightTris, keep0, keep1, i1);
            AddTriangle(rightVerts, rightTris, keep0, i1, i0);

            // cut側の三角形1つを追加
            AddTriangle(leftVerts, leftTris, i0, i1, cut);
        }
        else
        {
            AddTriangle(leftVerts, leftTris, keep0, keep1, i1);
            AddTriangle(leftVerts, leftTris, keep0, i1, i0);
            AddTriangle(rightVerts, rightTris, i0, i1, cut);
        }
    }

    // 新しいメッシュオブジェクトを作成してシーンに追加する
    void CreateObject(string name, List<Vector3> verts, List<int> tris, Color color)
    {
        if (verts.Count == 0) return; // 頂点が無ければ何もしない

        GameObject go = new GameObject(name);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.transform.localScale = transform.localScale;

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();

        // 法線・接線を再計算
        m.RecalculateNormals();
        m.RecalculateTangents();

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        mf.mesh = m;

        // Unlitカラーシェーダーで色を設定
        mr.material = new Material(Shader.Find("Unlit/Color")) { color = color };
    }
}