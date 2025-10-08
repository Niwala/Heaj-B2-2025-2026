using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ShaderNotesOverlay : VisualElement
{
    const string parentPrefKey = "ShaderTraining.parentID";
    const string notePrefKey = "ShaderTraining.noteID";

    public static ShaderTraining currentNote;
    public static List<NoteHierarchy> roots = new List<NoteHierarchy>();

    private static StyleSheet styles;
    private Button label;

    public ShaderNotesOverlay()
    {
        //Load styles
        if (styles == null)
            LoadStyles();
        styleSheets.Add(styles);
        AddToClassList("toolbar");
        label = new Button();

        //Load shader notes
        LoadNotes();
        OpenSpecific(currentNote);

        //Serie list popup
        SerieListPopup popup = new SerieListPopup(this);
        Add(popup);


        //Controls
        VisualElement controls = new VisualElement();
        controls.AddToClassList("group");
        Add(controls);

        //Controls > Previous
        Button previousBtn = new Button();
        previousBtn.AddToClassList("button");
        previousBtn.clicked += OpenPrevious;
        previousBtn.text = "◀";
        controls.Add(previousBtn);

        //Controls > Label
        label.AddToClassList("label");
        label.clicked += () => { popup.ToggleVisibility(label); popup.Focus(); };
        controls.Add(label);

        //Controls > Next
        Button nextBtn = new Button();
        nextBtn.AddToClassList("button");
        nextBtn.clicked += OpenNext;
        nextBtn.text = "▶";
        controls.Add(nextBtn);
    }

    private static void LoadNotes()
    {
        roots.Clear();
        currentNote = null;

        GameObject[] rootGo = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < rootGo.Length; i++)
        {
            ShaderTraining note = rootGo[i].GetComponent<ShaderTraining>();
            if (note != null)
            {
                NoteHierarchy hierarchy = new NoteHierarchy(note);
                roots.Add(hierarchy);

                if (currentNote == null)
                {
                    foreach (var child in hierarchy.childs)
                    {
                        if (child.isActiveAndEnabled)
                        {
                            currentNote = note;
                            break;
                        }
                    }
                }
            }
        }
    }

    private static void LoadStyles([CallerFilePath] string filePath = "")
    {
        filePath = Path.GetDirectoryName(filePath);
        filePath = filePath.Remove(0, Application.dataPath.Length - "Assets".Length);
        filePath += "\\ShaderTrainingStyles.uss";
        styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(filePath);
    }

    private (int, int) GetIDs(ShaderTraining note)
    {
        if (currentNote == null)
            return (-1, -1);

        int parentIndex = roots.FindIndex(x => x.note.transform == currentNote.transform.parent);
        if (parentIndex == -1)
            return (-1, -1);

        int noteIndex = roots[parentIndex].childs.IndexOf(currentNote);
        if (noteIndex == -1)
            return (-1, -1);

        return (parentIndex, noteIndex);
    }

    public void OpenSpecific(ShaderTraining note)
    {
        (int parentID, int noteID) = GetIDs(note);

        //Null check
        if (parentID == -1)
        {
            parentID = EditorPrefs.GetInt(parentPrefKey, 0);
            noteID = EditorPrefs.GetInt(notePrefKey, 0);

            if (parentID >= roots.Count)
            {
                parentID = 0;
                noteID = 0;
            }

            if (noteID >= roots[parentID].Count)
            {
                noteID = 0;
            }

            note = roots[parentID][noteID];
        }

        EditorPrefs.SetInt(parentPrefKey, parentID);
        EditorPrefs.SetInt(notePrefKey, noteID);

        //Enable / Disable objects
        foreach (var root in roots)
        {
            root.note.gameObject.SetActive(note.transform.parent == root.transform);
            foreach (var child in root.childs)
                child.gameObject.SetActive(child == note);
        }

        //Apply new selection
        currentNote = note;
        Selection.activeGameObject = currentNote.gameObject;

        (parentID, noteID) = GetIDs(note);
        string newLabel = $"<b>{roots[parentID].name} serie</b> : {noteID + 1} / {roots[parentID].Count}";
        label.text = newLabel;
    }

    public void OpenPrevious()
    {
        (int parentID, int noteID) = GetIDs(currentNote);

        //Null check
        if (parentID == -1)
        {
            OpenSpecific(null);
            return;
        }

        //Open previous
        if (noteID == 0)
        {
            if (parentID > 0)
            {
                parentID--;
                noteID = roots[parentID].Count - 1;
            }
        }
        else
        {
            noteID--;
        }
        OpenSpecific(roots[parentID][noteID]);
    }

    public void OpenNext()
    {
        (int parentID, int noteID) = GetIDs(currentNote);

        //Null check
        if (parentID == -1)
        {
            OpenSpecific(null);
            return;
        }

        //Open next
        if (noteID == roots[parentID].Count - 1)
        {
            if (parentID < roots.Count - 1)
            {
                parentID++;
                noteID = 0;
            }
        }
        else
        {
            noteID++;
        }
        OpenSpecific(roots[parentID][noteID]);
    }
}


public class SerieListPopup : VisualElement
{
    private ShaderNotesOverlay toolbar;
    private bool open;

    public SerieListPopup(ShaderNotesOverlay toolbar)
    {
        this.toolbar = toolbar;
        AddToClassList("serie-list");

        //Scroll
        ScrollView scroll = new ScrollView();
        scroll.AddToClassList("serie-list-scroll");
        scroll.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        Add(scroll);

        //Items
        foreach (var note in GetNotes())
        {
            Button item = new Button();
            item.AddToClassList("serie-list-item");
            item.text = note.name;
            item.clicked += () =>
            {
                toolbar.OpenSpecific(note.item);
                if (open)
                    ToggleVisibility(null);
            };
            scroll.contentContainer.Add(item);
        }

        this.RegisterCallback<PointerLeaveEvent>(ClickOut);
    }

    private IEnumerable<(string name, ShaderTraining item)> GetNotes()
    {
        foreach (var root in ShaderNotesOverlay.roots)
        {
            yield return (root.note.name + " Serie", root.childs.FirstOrDefault());
            foreach (var item in root.childs)
            {
                yield return ("    " + item.name, item);
            }
        }
    }

    public void ToggleVisibility(VisualElement origin)
    {
        open = !open;
        style.display = open ? DisplayStyle.Flex : DisplayStyle.None;

        if (origin != null)
        {
            style.left = origin.worldBound.center.x - 150;
        }
    }

    private void ClickOut(PointerLeaveEvent e)
    {
        if (open)
            ToggleVisibility(null);
    }
}

[InitializeOnLoad]
public static class TempOverlayController
{
    private const string requiredSceneName = "Shader Training";
    public static Dictionary<SceneView, ShaderNotesOverlay> overlays = new Dictionary<SceneView, ShaderNotesOverlay>();

    static TempOverlayController()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void Enable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void OnSceneGUI(SceneView sceneView)
    {
        //Check scene validity
        if (SceneManager.GetActiveScene().name != requiredSceneName)
        {
            if (overlays.ContainsKey(sceneView))
            {
                overlays[sceneView].RemoveFromHierarchy();
                overlays.Remove(sceneView);
            }

            SceneView.duringSceneGui -= OnSceneGUI;
            return;
        }

        //Create overlay if missing
        ShaderNotesOverlay overlay = sceneView.rootVisualElement.Q<ShaderNotesOverlay>();
        if (overlay == null)
        {
            overlay = new ShaderNotesOverlay();
            overlays.Add(sceneView, overlay);
            sceneView.rootVisualElement.Add(overlay);
        }

        //Add open buttons in world space
        if (ShaderNotesOverlay.currentNote != null)
        {
            Handles.BeginGUI();
            if (ShaderNotesOverlay.currentNote.practice != null)
                AddOpenButton(ShaderNotesOverlay.currentNote.practice);
            if (ShaderNotesOverlay.currentNote.solution != null)
                AddOpenButton(ShaderNotesOverlay.currentNote.solution);
            Handles.EndGUI();
        }

        void AddOpenButton(MeshRenderer renderer)
        {
            Bounds bounds = renderer.bounds;
            Vector3 worldPos = bounds.center + Vector3.up * (bounds.extents.y + 0.2f);
            Vector2 p = sceneView.camera.WorldToScreenPoint(worldPos);
            p.y = sceneView.camera.pixelHeight - p.y;

            float dpiScale = EditorGUIUtility.pixelsPerPoint;
            p /= dpiScale;


            if (GUI.Button(new Rect(p.x - 50, p.y, 100, 16), "Open"))
            {
                AssetDatabase.OpenAsset(renderer.sharedMaterial.shader);
            }
        }
    }
}