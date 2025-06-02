using UnityEngine;
using UnityEditor;

public class ClickPlacerWindow : EditorWindow
{
    private GameObject prefabToPlace;
    [SerializeField]
    private GameObject[] prefabOptions = new GameObject[0];
    private Transform parent_Transform;
    bool randomizeYRotation = false;
    bool randomizeAllRotation = false;
    private bool isPlacingEnabled = false;
    private bool align_to_normal;
    private Vector3 object_offset;
    float base_scale = 1;
    float random_scale_min = 0;
    float random_scale_max = 0;
    private LayerMask layers_to_effect = ~0;

    SerializedObject so;
    SerializedProperty prefabArrayProp;

    [MenuItem("Tools/Click Object Placer")]
    public static void ShowWindow()
    {
        GetWindow<ClickPlacerWindow>("Click Placer");
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        prefabArrayProp = so.FindProperty("prefabOptions");
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Click Object Placer", EditorStyles.boldLabel);

        so.Update();
        EditorGUILayout.PropertyField(prefabArrayProp, new GUIContent("Prefabs to Place"), true);
        so.ApplyModifiedProperties();

        parent_Transform = (Transform)EditorGUILayout.ObjectField("Parent for the object", parent_Transform, typeof(Transform), true);


        int[] layerIndices;
        string[] layerNames = GetLayerNames(out layerIndices);

        // Convert current LayerMask to mask relative to `layerIndices`
        int mask = 0;
        for (int i = 0; i < layerIndices.Length; i++)
        {
            if ((layers_to_effect.value & (1 << layerIndices[i])) != 0)
                mask |= (1 << i);
        }

        // Show mask field
        mask = EditorGUILayout.MaskField("Placement Layer Mask", mask, layerNames);

        // Convert back to real LayerMask
        int newMask = 0;
        for (int i = 0; i < layerIndices.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
                newMask |= (1 << layerIndices[i]);
        }

        layers_to_effect.value = newMask;

        randomizeYRotation = EditorGUILayout.Toggle("Random Y Rotation", randomizeYRotation);
        randomizeAllRotation = EditorGUILayout.Toggle("Random All Rotation", randomizeAllRotation);

        object_offset = EditorGUILayout.Vector3Field("Object Offset", object_offset);
        align_to_normal = EditorGUILayout.Toggle("Align to normal", align_to_normal);

        base_scale = EditorGUILayout.FloatField("Base Scale", base_scale);
        random_scale_min = EditorGUILayout.FloatField("Random Scale Minimum", random_scale_min);
        random_scale_max = EditorGUILayout.FloatField("Random Scale Maximum", random_scale_max);

        GUI.backgroundColor = isPlacingEnabled ? Color.green : Color.red;
        if (GUILayout.Button(isPlacingEnabled ? "Placing: ON (Click to Disable)" : "Placing: OFF (Click to Enable)"))
        {
            isPlacingEnabled = !isPlacingEnabled;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox("Click on a mesh collider in the Scene view to place the object.", MessageType.Info);

    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacingEnabled || prefabOptions.Length <= 0)
            return;


        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control && !e.shift)
        {
            Debug.Log("Click script working");
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float maxDistance = 200f; // Set your max placement distance

            int terrainLayer = LayerMask.NameToLayer("Terrain");
            int terrainLayerMask = 1 << terrainLayer;
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layers_to_effect))
            {
                // Optional sanity check: only place on terrain or specific layers
                if (hit.collider == null)
                    return;



                if (prefabOptions != null && prefabOptions.Length > 0)
                {
                    GameObject prefabToPlace = prefabOptions[Random.Range(0, prefabOptions.Length)];
                    if (prefabToPlace != null)
                    {
                        GameObject placedObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
                        Undo.RegisterCreatedObjectUndo(placedObj, "Place Object");
                        if (parent_Transform != null)
                        {
                            placedObj.transform.SetParent(parent_Transform);
                        }
                        Terrain terrain = hit.collider.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            RaycastHit terrainHit;
                            if (terrain.GetComponent<TerrainCollider>().Raycast(ray, out terrainHit, maxDistance))
                            {
                                Debug.Log("Placing object on terrain");
                                Vector3 pos = terrainHit.point;
                                //pos.y = terrain.SampleHeight(pos) + terrain.GetPosition().y; // Terrain world height
                                placedObj.transform.position = pos;
                                //Vector3 hitPoint = terrainHit.point;
                                // Use hitPoint for placement
                            }
                        }
                        else
                        {
                            Debug.Log("Couldn't find terrain");
                            placedObj.transform.position = hit.point;

                        }
                        placedObj.transform.position += object_offset;
                        // if (randomizeYRotation)
                        // {
                        //     placedObj.transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
                        // }
                        
                        float scale = base_scale;
                        if (random_scale_max > 0)
                        {
                            scale = Random.Range(random_scale_min, random_scale_max);
                        }
                        placedObj.transform.localScale = Vector3.one * scale;


                        if (align_to_normal && hit.normal != Vector3.zero)
                        {
                            // Step 1: Align up with surface normal
                            Quaternion alignToNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);

                            // Step 2: Apply random rotation around the normal
                            Quaternion randomSpin = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);

                            // Combine both
                            placedObj.transform.rotation = randomSpin * alignToNormal;
                        }
                        else if (randomizeYRotation)
                        {
                            // Only apply Y-axis spin if not aligning to normal
                            placedObj.transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
                        }
                        if (randomizeAllRotation)
                        {
                            placedObj.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        }
                        // Only align to normal if it’s valid
                        // if (hit.normal != Vector3.zero && align_to_normal)
                        //     placedObj.transform.up = hit.normal;

                        e.Use(); // Prevent default click behavior
                    }
                }

            }
            else
            {
                Debug.Log("Raycast missed or too far away");
            }
        }
    }
    
    private static string[] GetLayerNames(out int[] layerIndices)
    {
    var names = new System.Collections.Generic.List<string>();
    var indices = new System.Collections.Generic.List<int>();
    for (int i = 0; i < 32; i++)
    {
        string layerName = LayerMask.LayerToName(i);
        if (!string.IsNullOrEmpty(layerName))
        {
            names.Add(layerName);
            indices.Add(i);
        }
    }
    layerIndices = indices.ToArray();
    return names.ToArray();
}

}
