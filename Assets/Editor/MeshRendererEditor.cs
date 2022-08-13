using UnityEngine;
using UnityEditor;
 
[CustomEditor(typeof(MeshRenderer))]
public class MeshRendererEditor : Editor
{
    MeshRenderer meshRenderer;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        meshRenderer = target as MeshRenderer;
 
        string[] layerNames = new string[SortingLayer.layers.Length];
        for (int i = 0; i < SortingLayer.layers.Length; i++)
            layerNames[i] = SortingLayer.layers[i].name;
 
        int layerValue = SortingLayer.GetLayerValueFromID(meshRenderer.sortingLayerID);
        layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, layerNames);
 
        SortingLayer layer = SortingLayer.layers[layerValue];
        meshRenderer.sortingLayerName = layer.name;
        meshRenderer.sortingLayerID = layer.id;
        meshRenderer.sortingOrder = EditorGUILayout.IntField("Order in Layer", meshRenderer.sortingOrder);
    }
}