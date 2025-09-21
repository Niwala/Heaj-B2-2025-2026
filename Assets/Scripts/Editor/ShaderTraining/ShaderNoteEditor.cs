using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShaderTraining), true)]
public class ShaderNoteEditor : MarkdownComponentEditor
{

    private bool enable;
    private ShaderTraining note;
    private double lastRepaint;

    protected override void OnEnable()
    {
        base.OnEnable();
        enable = true;
        RefreshTimer();

        //Enable overlay if missing
        if (SceneView.lastActiveSceneView != null && !TempOverlayController.overlays.ContainsKey(SceneView.lastActiveSceneView))
        {
            TempOverlayController.Enable();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        enable = false;
    }

    private async void RefreshTimer()
    {
        if (!enable || note == null || !note.requireSceneRepaint)
            return;

        double repaintTime = Time.realtimeSinceStartupAsDouble;
        float deltaTime = (float)(repaintTime - lastRepaint);
        lastRepaint = repaintTime;

        note.SyncShader(deltaTime);
        SceneView.lastActiveSceneView?.Repaint();

        await Task.Delay(16);
        RefreshTimer();
    }
}