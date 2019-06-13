using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ColonelMenuOptions
{

    private const string kUILayerName = "UI";

    private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
    private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
    private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
    private const string kKnobPath = "UI/Skin/Knob.psd";
    private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
    private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
    private const string kMaskPath = "UI/Skin/UIMask.psd";

    [MenuItem("GameObject/UI/Buttons/DoubleClickButton", false, 2065)]
    private static void AddDoubleClickButton(MenuCommand menuCommand)
    {
        GameObject btn = ColonelDefaultControls.CreateDoubleClickButton(GetStandardResources());
        //这个方法对创建的UI进行配置
        //比如创建唯一的名字
        //根据是否选中画布创建父级
        //是UGUI源码的一部分，我们只需要复制到我们脚本中来即可，稍后我会附上源码
        PlaceUIElementRoot(btn, menuCommand);
    }

    [MenuItem("GameObject/UI/Buttons/LongClickButton", false, 2066)]
    public static void AddLongClickButton(MenuCommand menuCommand)
    {
        GameObject lcButton = ColonelDefaultControls.CreateLongClickButton(GetStandardResources());
        PlaceUIElementRoot(lcButton, menuCommand);
    }

    private static DefaultControls.Resources GetStandardResources()
    {
        DefaultControls.Resources resources = new DefaultControls.Resources();
        resources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
        resources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
        resources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
        resources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
        resources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
        resources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
        resources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
        return resources;
    }

    private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;

        bool explicitParentChoice = true;
        if (parent == null)
        {
            parent = GetOrCreateCanvasGameObject();            
            explicitParentChoice = false;
        }
        if (parent.GetComponentInParent<Canvas>() == null)
        {
            // Create canvas under context GameObject,
            // and make that be the parent which UI element is added under.
            GameObject canvas = CreateNewUI();
            canvas.transform.SetParent(parent.transform, false);
            parent = canvas;
        }

        // Setting the element to be a child of an element already in the scene should
        // be sufficient to also move the element to that scene.
        // However, it seems the element needs to be already in its destination scene when the
        // RegisterCreatedObjectUndo is performed; otherwise the scene it was created in is dirtied.
        SceneManager.MoveGameObjectToScene(element, parent.scene);

        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);

        if (parent.transform.Find(element.name) != null)
            element.name = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);

        if (element.transform.parent == null)
        {
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
        }

        // We have to fix up the undo name since the name of the object was only known after reparenting it.
        Undo.SetCurrentGroupName("Create " + element.name);

        GameObjectUtility.SetParentAndAlign(element, parent);
        if (!explicitParentChoice) // not a context click, so center in sceneview
            SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

        Selection.activeGameObject = element;
    }

    private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
    {
        SceneView sceneView = SceneView.lastActiveSceneView;

        // Couldn't find a SceneView. Don't set position.
        if (sceneView == null || sceneView.camera == null)
            return;

        // Create world space Plane from canvas position.
        Vector2 localPlanePosition;
        Camera camera = sceneView.camera;
        Vector3 position = Vector3.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
        {
            // Adjust for canvas pivot
            localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
            localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

            localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
            localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

            // Adjust for anchoring
            position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
            position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

            Vector3 minLocalPosition;
            minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
            minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

            Vector3 maxLocalPosition;
            maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
            maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

            position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
            position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
        }

        itemTransform.anchoredPosition = position;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localScale = Vector3.one;
    }

    // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (IsValidCanvas(canvas))
            return canvas.gameObject;

        // No canvas in the scene at all? Then create a new one.
        return CreateNewUI();
    }

    static public GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        root.layer = LayerMask.NameToLayer(kUILayerName);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

        CreateEventSystem();
        return root;
    }

    private static void CreateEventSystem()
    {
        var eventSystem = new GameObject("EventSystem");
        var esys = eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
    }

    static bool IsValidCanvas(Canvas canvas)
    {
        if (canvas == null || !canvas.gameObject.activeInHierarchy)
            return false;

        // It's important that the non-editable canvas from a prefab scene won't be rejected,
        // but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
        if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
            return false;

        return true;
    }
}
