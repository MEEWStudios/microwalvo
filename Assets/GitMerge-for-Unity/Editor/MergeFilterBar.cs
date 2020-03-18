﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class MergeFilterBar
{
    public MergeFilter filter { get; set; }

    private AnimBool filterAnimAlpha;

    public void Draw()
    {
        filter.useFilter = GUILayout.Toggle(filter.useFilter, "Filter");
        if (filter.useFilter)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                filter.filterMode = (MergeFilter.FilterMode)EditorGUILayout.EnumPopup("Mode", filter.filterMode, GUILayout.ExpandWidth(false));
                filter.isRegex = EditorGUILayout.Toggle("Regex?", filter.isRegex);
                if (!filter.isRegex)
                {
                    filter.isCaseSensitive = EditorGUILayout.Toggle("Case Sensitive", filter.isCaseSensitive);
                }
                filter.expression = EditorGUILayout.TextField("Expression", filter.expression, GUILayout.ExpandWidth(true));
            }
        }
    }
}
