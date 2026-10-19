using UnityEngine;

/// <summary>
/// Debug helper. Put it on any empty GameObject. It draws a wireframe around EVERY collider in the
/// scene (in the Scene view, and in the Game view if Gizmos is switched on), even when nothing is selected.
///   Yellow = trigger, Red = Mesh Collider, Green = everything else.
/// Delete it (or untick the component) when you're done.
/// </summary>
public class ShowColliders : MonoBehaviour
{
    [SerializeField] private bool showTriggers = true;
    [SerializeField] private bool showMeshColliders = true;
    [SerializeField] private bool showOthers = true;

    private void OnDrawGizmos()
    {
        Collider[] colliders = FindObjectsByType<Collider>(FindObjectsInactive.Exclude);

        foreach (Collider col in colliders)
        {
            if (!col.enabled) continue;

            bool isMesh = col is MeshCollider;
            if (col.isTrigger) { if (!showTriggers) continue; Gizmos.color = Color.yellow; }
            else if (isMesh)   { if (!showMeshColliders) continue; Gizmos.color = Color.red; }
            else               { if (!showOthers) continue; Gizmos.color = Color.green; }

            Transform t = col.transform;

            if (col is BoxCollider box)
            {
                Gizmos.matrix = t.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.matrix = Matrix4x4.TRS(t.TransformPoint(sphere.center), t.rotation, Vector3.one);
                Vector3 s = t.lossyScale;
                Gizmos.DrawWireSphere(Vector3.zero, sphere.radius * Mathf.Max(s.x, s.y, s.z));
            }
            else if (col is MeshCollider mesh && mesh.sharedMesh != null)
            {
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawWireMesh(mesh.sharedMesh, t.position, t.rotation, t.lossyScale);
            }
            else
            {
                // Capsules, character controllers, terrain etc.: draw the bounding box.
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}
