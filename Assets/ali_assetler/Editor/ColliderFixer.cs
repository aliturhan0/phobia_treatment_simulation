using UnityEngine;
using UnityEditor;

namespace AliEditorTools
{
    public class ColliderFixer : EditorWindow
    {
        [MenuItem("Ali/Fix Negative Colliders (Kırmızı Hataları Çöz)")]
        public static void FixNegativeColliders()
        {
            BoxCollider[] boxColliders = FindObjectsOfType<BoxCollider>();
            int fixedCount = 0;

            foreach (var bc in boxColliders)
            {
                // Check if any scale component is negative (using lossyScale for global scale)
                if (bc.transform.lossyScale.x < 0 || bc.transform.lossyScale.y < 0 || bc.transform.lossyScale.z < 0)
                {
                    GameObject obj = bc.gameObject;
                    
                    // We need a MeshFilter to use a MeshCollider
                    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        Undo.RecordObject(obj, "Fix Negative Collider");
                        
                        // Remove BoxCollider
                        DestroyImmediate(bc);
                        
                        // Add MeshCollider
                        MeshCollider mc = obj.AddComponent<MeshCollider>();
                        mc.convex = true; // Convex is required for dynamic interactions, though these might be static. It's safer for 'Convex' suggestion in logs.
                        
                        fixedCount++;
                        Debug.Log($"[Ali Fixer] Fixed collider on: {GetHierarchyPath(obj.transform)}");
                    }
                    else
                    {
                        Debug.LogWarning($"[Ali Fixer] Cannot fix {obj.name} - No MeshFilter found!");
                    }
                }
            }

            EditorUtility.DisplayDialog("İşlem Tamam", $"{fixedCount} adet sorunlu obje düzeltildi! \n(BoxCollider -> MeshCollider'a çevrildi)", "Tamamdır");
        }

        private static string GetHierarchyPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}
