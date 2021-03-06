using UnityEngine;
using UnityEditor;
using PCG;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(FunctionView))]
public class PCGViewEditor : Editor
{
    private const int MAX_CACHED_RESULTS = 16;

    static private Color32[] colors;
    static private Texture2D texture;

    static private List<KeyValuePair<string, object>> cachedFunctionResults = new List<KeyValuePair<string,object>>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FunctionView view = (FunctionView)target;
        
        Function function = view.GetFunction();

        string fullFunctionText = view.functionText;

        object result = null;

        for (int i = 0; i < cachedFunctionResults.Count; i++)
        {
            if (cachedFunctionResults[i].Key == fullFunctionText)
            {
                result = cachedFunctionResults[i].Value;
                break;
            }
        }
        
        if (result == null)
        {
            if (function != null)
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                sw.Start();

                result = function.Evaluate();

                sw.Stop();

                Debug.Log("FunctionViewEditor -> " + fullFunctionText + " execution took " + sw.Elapsed);
            }
            else
            {
                result = "ERROR!!";
            }

            cachedFunctionResults.Add(new KeyValuePair<string, object>(fullFunctionText, result));

            if (cachedFunctionResults.Count > MAX_CACHED_RESULTS)
                cachedFunctionResults.RemoveAt(0);
        }

        if (result is Matrix)
        {
            Matrix matrix = (Matrix)result;

            if (colors == null || colors.Length != matrix.size * matrix.size)
                colors = new Color32[matrix.size * matrix.size];

            if (texture == null || texture.width != matrix.size || texture.height != matrix.size)
            {
                if (texture)
                    Component.DestroyImmediate(texture);

                texture = new Texture2D(matrix.size, matrix.size, TextureFormat.RGB24, false);
            }

            for (int i = 0; i < matrix.size * matrix.size; i++)
                colors[i] = new Color32(matrix.values[i], matrix.values[i], matrix.values[i], 0);

            if (colors.Length > 0)
            {
                texture.SetPixels32(colors);
                texture.Apply();
            }

            //EditorGUILayout.LabelField("Result", "See in preview");

            if (texture)
            {
                EditorGUILayout.LabelField(new GUIContent("Result"), new GUIContent(texture), GUILayout.MaxHeight(texture.height));

                //Rect rect = EditorGUILayout.GetControlRect(true, texture.height);
                //EditorGUI.DrawTextureTransparent(rect, texture, ScaleMode.ScaleToFit);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Result", result.ToString());
        }

        if (GUILayout.Button("Recalculate"))
            cachedFunctionResults.RemoveAll(x => x.Key == fullFunctionText);
    }
}


