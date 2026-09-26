using UnityEditor;
using UnityEngine;

public class ObjectPainterWindow : EditorWindow
{
    private GameObject prefab;

    private float brushRadius = 10f;
    private int objectsPerStroke = 5;

    private float minScale = 0.8f;
    private float maxScale = 1.2f;

    private bool randomRotation = true;
    private bool alignToSurface = false;

    private float minSpacing = 2f;

    private bool eraseMode = false;
    private float eraseRadius = 5f;

    private LayerMask placementLayers = ~0;

    private Transform parentContainer;

    private bool painting;

    private GUIStyle headerStyle;

    [MenuItem("Tools/Object Painter")]
    public static void ShowWindow()
    {
        ObjectPainterWindow window =
            GetWindow<ObjectPainterWindow>("Object Painter");

        window.minSize = new Vector2(320f, 500f);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13
        };
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Space(8);

        EditorGUILayout.LabelField(
            "OBJECT PAINTER",
            headerStyle
        );

        GUILayout.Space(8);

        prefab = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            prefab,
            typeof(GameObject),
            false
        );

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Painting",
            EditorStyles.boldLabel
        );

        brushRadius = EditorGUILayout.Slider(
            "Brush Radius",
            brushRadius,
            0.5f,
            100f
        );

        objectsPerStroke = EditorGUILayout.IntSlider(
            "Objects / Stroke",
            objectsPerStroke,
            1,
            50
        );

        minSpacing = EditorGUILayout.Slider(
            "Minimum Spacing",
            minSpacing,
            0f,
            20f
        );

        GUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Transform",
            EditorStyles.boldLabel
        );

        randomRotation = EditorGUILayout.Toggle(
            "Random Rotation",
            randomRotation
        );

        alignToSurface = EditorGUILayout.Toggle(
            "Align To Surface",
            alignToSurface
        );

        minScale = EditorGUILayout.FloatField(
            "Min Scale",
            minScale
        );

        maxScale = EditorGUILayout.FloatField(
            "Max Scale",
            maxScale
        );

        GUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Parenting",
            EditorStyles.boldLabel
        );

        parentContainer = (Transform)EditorGUILayout.ObjectField(
            "Parent",
            parentContainer,
            typeof(Transform),
            true
        );

        if (GUILayout.Button("Create Parent Container"))
        {
            CreateParentContainer();
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Erase",
            EditorStyles.boldLabel
        );

        eraseMode = EditorGUILayout.Toggle(
            "Erase Mode",
            eraseMode
        );

        if (eraseMode)
        {
            eraseRadius = EditorGUILayout.Slider(
                "Erase Radius",
                eraseRadius,
                0.5f,
                30f
            );
        }

        GUILayout.Space(15);

        EditorGUILayout.HelpBox(
            eraseMode
                ? "Left-click in the Scene view to erase painted objects."
                : "Left-click or drag in the Scene view to paint objects.",
            MessageType.Info
        );

        if (GUILayout.Button("Clear All Painted Objects"))
        {
            ClearAllObjects();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (prefab == null && !eraseMode)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f,
                placementLayers,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        Handles.color = eraseMode
            ? Color.red
            : Color.yellow;

        Handles.DrawWireDisc(
            hit.point,
            hit.normal,
            eraseMode ? eraseRadius : brushRadius
        );

        SceneView.RepaintAll();

        if (e.type == EventType.MouseDown &&
            e.button == 0 &&
            !e.alt)
        {
            painting = true;

            Paint(hit);

            e.Use();
        }

        if (e.type == EventType.MouseDrag &&
            e.button == 0 &&
            painting &&
            !e.alt)
        {
            Paint(hit);

            e.Use();
        }

        if (e.type == EventType.MouseUp &&
            e.button == 0)
        {
            painting = false;

            e.Use();
        }
    }

    private void Paint(RaycastHit hit)
    {
        if (eraseMode)
        {
            Erase(hit.point);
            return;
        }

        if (prefab == null)
            return;

        for (int i = 0; i < objectsPerStroke; i++)
        {
            Vector2 randomCircle =
                Random.insideUnitCircle * brushRadius;

            Vector3 position =
                hit.point +
                new Vector3(
                    randomCircle.x,
                    0f,
                    randomCircle.y
                );

            RaycastHit surfaceHit;

            Vector3 rayOrigin =
                position + Vector3.up * 100f;

            if (!Physics.Raycast(
                    rayOrigin,
                    Vector3.down,
                    out surfaceHit,
                    200f,
                    placementLayers,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            position = surfaceHit.point;

            if (IsTooClose(position))
                continue;

            CreateObject(
                position,
                surfaceHit.normal
            );
        }
    }

    private void CreateObject(
        Vector3 position,
        Vector3 normal)
    {
        GameObject instance =
            (GameObject)PrefabUtility.InstantiatePrefab(
                prefab
            );

        if (instance == null)
            return;

        Undo.RegisterCreatedObjectUndo(
            instance,
            "Paint Object"
        );

        instance.transform.position = position;

        if (randomRotation)
        {
            float rotation =
                Random.Range(0f, 360f);

            instance.transform.rotation =
                Quaternion.Euler(
                    0f,
                    rotation,
                    0f
                );
        }

        if (alignToSurface)
        {
            Quaternion surfaceRotation =
                Quaternion.FromToRotation(
                    Vector3.up,
                    normal
                );

            instance.transform.rotation =
                surfaceRotation *
                instance.transform.rotation;
        }

        float scale =
            Random.Range(
                minScale,
                maxScale
            );

        instance.transform.localScale =
            Vector3.one * scale;

        if (parentContainer != null)
        {
            Undo.SetTransformParent(
                instance.transform,
                parentContainer,
                "Parent Painted Object"
            );
        }
    }

    private bool IsTooClose(Vector3 position)
    {
        if (parentContainer == null)
            return false;

        foreach (Transform child in parentContainer)
        {
            if (Vector3.Distance(
                    child.position,
                    position) < minSpacing)
            {
                return true;
            }
        }

        return false;
    }

    private void Erase(Vector3 center)
    {
        if (parentContainer == null)
            return;

        for (int i = parentContainer.childCount - 1;
             i >= 0;
             i--)
        {
            Transform child =
                parentContainer.GetChild(i);

            if (Vector3.Distance(
                    child.position,
                    center) <= eraseRadius)
            {
                Undo.DestroyObjectImmediate(
                    child.gameObject
                );
            }
        }
    }

    private void CreateParentContainer()
    {
        GameObject container =
            new GameObject(
                prefab != null
                    ? prefab.name + "_Painted"
                    : "PaintedObjects"
            );

        Undo.RegisterCreatedObjectUndo(
            container,
            "Create Paint Container"
        );

        parentContainer =
            container.transform;

        Selection.activeGameObject =
            container;
    }

    private void ClearAllObjects()
    {
        if (parentContainer == null)
            return;

        Undo.RegisterFullObjectHierarchyUndo(
            parentContainer.gameObject,
            "Clear Painted Objects"
        );

        for (int i = parentContainer.childCount - 1;
             i >= 0;
             i--)
        {
            Undo.DestroyObjectImmediate(
                parentContainer.GetChild(i).gameObject
            );
        }
    }
}