// Copyright (c) 2015 The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace SpriterDotNetUnity
{
    [CustomEditor(typeof(SpriterDotNetBehaviour))]
    public class SpriterDotNetBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpriterDotNetBehaviour sdnb = target as SpriterDotNetBehaviour;

            EditorGUI.BeginChangeCheck();
            string[] layers = SortingLayer.layers.Select(l => l.name).ToArray();
            int currentIndex = Array.IndexOf(layers, sdnb.SortingLayer);
            if (currentIndex < 0) currentIndex = 0;
            int choiceIndex = EditorGUILayout.Popup("Sorting Layer", currentIndex, layers);
            sdnb.SortingLayer = layers[choiceIndex];
            sdnb.SortingOrder = EditorGUILayout.IntField("Sorting Order", sdnb.SortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(sdnb);
                EditorSceneManager.MarkSceneDirty(sdnb.gameObject.scene);
            }

            if (sdnb.Animator == null)
                sdnb.Init();

            if (sdnb.Animator != null)
            {
                var animations = sdnb.SpriterData.Spriter.Entities[sdnb.EntityIndex].Animations.ToList();
                var animationNames = animations.ConvertAll(x => x.Name).ToArray();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("PlayAnimation", GUILayout.Width(121));
                    sdnb.AnimationIndex = EditorGUILayout.Popup(sdnb.AnimationIndex, animationNames);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("PreviousAnim"))
                            sdnb.AnimationIndex--;
                        if (GUILayout.Button("NextAnim"))
                            sdnb.AnimationIndex++;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                {
                    if (sdnb.AnimationIndex < 0)
                        sdnb.AnimationIndex = animations.Count - 1;
                    if (sdnb.AnimationIndex >= animations.Count)
                        sdnb.AnimationIndex = 0;

                    sdnb.Animator.Play(animations[sdnb.AnimationIndex]);
                    SetFrameIndex(0, false);
                }

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUI.BeginChangeCheck();
                    sdnb.FrameIndex = EditorGUILayout.IntSlider("FrameIndex", sdnb.FrameIndex, 0, sdnb.Animator.CurrentAnimation.MainlineKeys.Length);
                    if (EditorGUI.EndChangeCheck())
                        SetFrameIndex(sdnb.FrameIndex, false);

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("PreviousFrame"))
                            SetFrameIndex(-1, true);
                        if (GUILayout.Button("NextFrame"))
                            SetFrameIndex(1, true);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();


                void SetFrameIndex(int frame, bool relative)
                {
                    sdnb.SetFrameIndex(frame, relative);
                    Repaint();
                }
            }
        }
    }
}

#endif
