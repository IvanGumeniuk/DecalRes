// Scale a mesh along some arbitrary global axis.
using UnityEngine;

public class MeshSquashB : MonoBehaviour
{
    // The direction along which the stretch/squash is performed...
    public Vector3 squashAxis = Vector3.up;

    // ...and the scaling along that axis.
    public float scale = 0.5f;

    Mesh mesh;
    Vector3[] origVerts;
    Vector3[] verts;

    void Start()
    {
        // Get the mesh data.
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;

        // Keep a copy of the original vertices and declare a new array of vertices.
        origVerts = mesh.vertices;
        verts = new Vector3[mesh.vertexCount];
    }

    void Update()
    {
        // rotate the squash axis opposite the sphere's rotation
        Quaternion rot = Quaternion.Inverse(transform.rotation);
        Vector3 axis = rot * squashAxis;

        // Create a new local coordinate space where one axis is the stretch/squash direction
        Vector3 r = axis;
        Vector3 g = Vector3.zero;
        Vector3 b = Vector3.zero;

        // OrthoNormalize creates two axes perpendicular to the stretch/squash axis!
        Vector3.OrthoNormalize(ref r, ref g, ref b);

        // A matrix transforms points into new coordinate space. 
        Matrix4x4 mat = new Matrix4x4();
        mat.SetRow(0, r);
        mat.SetRow(1, g);
        mat.SetRow(2, b);
        mat.SetRow(3, new Vector4(0, 0, 0, 1));

        // Its inverse transforms them back again.
        Matrix4x4 inv = mat.inverse;

        for (int i = 0; i < verts.Length; i++)
        {
            // Convert the original unaltered vertex to the new coordinate space.
            Vector3 vert = mat.MultiplyPoint3x4(origVerts[i]);

            // Change the vertex's position in the new space.
            vert.x *= scale;

            // Convert it back to the usual XYZ space.
            verts[i] = inv.MultiplyPoint3x4(vert);
        }

        // Set the updated vertex array.
        mesh.vertices = verts;

        // The normals have probably changed, so let Unity update them.
        mesh.RecalculateNormals();

        Debug.DrawRay(transform.position, axis, Color.red, 0.01f);
    }
}
// Thanks to Unity Community user "Andeeeee"

// local-to-world
//Matrix4x4 localToWorld = transform.localToWorldMatrix;
//Vector3 world_v = localToWorld.MultiplyPoint3x4(origVerts[i]);
//origVerts[i] = world_v;

// rotation matrix
//Quaternion rotation = transform.rotation;
//Matrix4x4 m = Matrix4x4.Rotate(rotation);
//verts[i] = m.MultiplyPoint3x4(origVerts[i]);