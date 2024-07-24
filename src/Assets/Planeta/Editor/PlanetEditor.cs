using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planeta))]
public class PlanetEditor : Editor
{
    Planeta planet;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed && planet.autoUpdate)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        if (GUILayout.Button("Generate Structures"))
        {
            planet.GenerateItems();
        }

        if (GUILayout.Button("Generate Enemies"))
        {
            planet.GenerateEnemies(null);
        }

        if (GUILayout.Button("Generate Tower and Portals"))
        {
            planet.GenerateTorre();
            planet.GeneratePortalsAltars();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingFold, ref shapeEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool fold, ref Editor editor)
    {
        if (settings != null)
        {
            fold = EditorGUILayout.InspectorTitlebar(fold, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (fold)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
        
    }

    private void OnEnable()
    {
        planet = (Planeta)target;
    }
}
