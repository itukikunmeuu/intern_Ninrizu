using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCutter : MonoBehaviour
{
    // �J�b�g�Ɏg�����ʂ��w�肷��Transform�i���ʂ̖@����cutPlane.up�j
    public Transform cutPlane;

    // �R���e�L�X�g���j���[����Ăׂ�uCut Mesh�v���\�b�h
    [ContextMenu("Cut Mesh")]
    void Cut()
    {
        // MeshFilter�擾
        var mf = GetComponent<MeshFilter>();
        if (!mf)
        {
            Debug.LogError("MeshFilter not found!"); // MeshFilter���Ȃ��ꍇ�̓G���[�\�����ďI��
            return;
        }

        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;   // ���_�z��
        int[] triangles = mesh.triangles;     // �O�p�`�̃C���f�b�N�X�z��

        // �J�b�g���ʂ����[�J�����W�ō쐬
        Plane plane = new Plane(cutPlane.up, transform.InverseTransformPoint(cutPlane.position));

        // �����i���ʂ̗����j�p�̒��_�E�O�p�`���X�g
        List<Vector3> leftVerts = new List<Vector3>();
        List<int> leftTris = new List<int>();

        // �E���i���ʂ̕\���j�p�̒��_�E�O�p�`���X�g
        List<Vector3> rightVerts = new List<Vector3>();
        List<int> rightTris = new List<int>();

        // �O�p�`���Ƃɏ���
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            // �e���_�����ʂ̂ǂ��瑤������itrue: �\��, false: �����j
            bool s0 = plane.GetSide(v0);
            bool s1 = plane.GetSide(v1);
            bool s2 = plane.GetSide(v2);

            int sideCount = (s0 ? 1 : 0) + (s1 ? 1 : 0) + (s2 ? 1 : 0);

            if (sideCount == 0)
            {
                // ���ׂė����Ȃ獶�����b�V���ɒǉ�
                AddTriangle(leftVerts, leftTris, v0, v1, v2);
            }
            else if (sideCount == 3)
            {
                // ���ׂĕ\���Ȃ�E�����b�V���ɒǉ�
                AddTriangle(rightVerts, rightTris, v0, v1, v2);
            }
            else
            {
                // ���_�����݂���ꍇ�͎O�p�`�𕪊�
                SplitTriangle(plane, v0, v1, v2, s0, s1, s2, leftVerts, leftTris, rightVerts, rightTris);
            }
        }

        // ���E�ɕ����������b�V����V����GameObject�Ƃ��Đ���
        CreateObject("LeftPart", leftVerts, leftTris, Color.red);
        CreateObject("RightPart", rightVerts, rightTris, Color.green);

        // ���̃I�u�W�F�N�g�͔�\���ɂ���
        gameObject.SetActive(false);
    }

    // ���_3����O�p�`�����A���X�g�ɒǉ�����w���p�[�֐�
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

    // a-b�̐����ƕ���p�̌�_���v�Z
    Vector3 Intersect(Vector3 a, Vector3 b, Plane p)
    {
        Vector3 dir = b - a;
        float dist = 0f;
        Ray ray = new Ray(a, dir);
        p.Raycast(ray, out dist);
        return a + dir.normalized * dist;
    }

    // �O�p�`�̒��_�̂����A�ꕔ�����ʂ̕Б��ɂ���ꍇ�ɕ�������
    void SplitTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2, bool s0, bool s1, bool s2,
        List<Vector3> leftVerts, List<int> leftTris, List<Vector3> rightVerts, List<int> rightTris)
    {
        // 1���_�݂̂��قȂ鑤�ɂ���P�[�X��z��i3�ʂ�̏ꍇ�����j

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

        // 2�̌�_���v�Z
        Vector3 i0 = Intersect(keep0, cut, plane);
        Vector3 i1 = Intersect(keep1, cut, plane);

        // �l�p�`��̌`����O�p�`2�ɕ������A
        // ���ꂼ��̑��̃��b�V���ɒǉ�
        if (keepSide)
        {
            // keep���̎O�p�`2��ǉ�
            AddTriangle(rightVerts, rightTris, keep0, keep1, i1);
            AddTriangle(rightVerts, rightTris, keep0, i1, i0);

            // cut���̎O�p�`1��ǉ�
            AddTriangle(leftVerts, leftTris, i0, i1, cut);
        }
        else
        {
            AddTriangle(leftVerts, leftTris, keep0, keep1, i1);
            AddTriangle(leftVerts, leftTris, keep0, i1, i0);
            AddTriangle(rightVerts, rightTris, i0, i1, cut);
        }
    }

    // �V�������b�V���I�u�W�F�N�g���쐬���ăV�[���ɒǉ�����
    void CreateObject(string name, List<Vector3> verts, List<int> tris, Color color)
    {
        if (verts.Count == 0) return; // ���_��������Ή������Ȃ�

        GameObject go = new GameObject(name);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.transform.localScale = transform.localScale;

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();

        // �@���E�ڐ����Čv�Z
        m.RecalculateNormals();
        m.RecalculateTangents();

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        mf.mesh = m;

        // Unlit�J���[�V�F�[�_�[�ŐF��ݒ�
        mr.material = new Material(Shader.Find("Unlit/Color")) { color = color };
    }
}