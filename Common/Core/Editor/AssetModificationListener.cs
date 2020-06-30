using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Events;

public class AssetModificationListener : UnityEditor.AssetModificationProcessor
{
    //保存文件时调用
    static string[] OnWillSaveAssets(string[] paths)
    {
        foreach (string path in paths)
        {
            if (path.EndsWith(".controller"))
            {
                MergeAnimationClipToAnimatorController(path);
            }
        }
        return paths;
    }

    //将AnimatorController中用到的AnimationClip自动导入Controller中
    private static void MergeAnimationClipToAnimatorController(string animatorPath)
    {
        AnimatorController controller = (AnimatorController)AssetDatabase.LoadAssetAtPath(animatorPath, typeof(AnimatorController));
        string controllerPath = AssetDatabase.GetAssetPath(controller);
        foreach (var layer in controller.layers)
        {
            AnimatorStateMachine stateMachine = layer.stateMachine;
            ChildAnimatorState[] childStates = stateMachine.states;
            foreach (var childState in childStates)
            {
                AnimatorState state = childState.state;
                if (state.motion != null)
                {
                    AnimationClip clip = (AnimationClip)state.motion;
                    string clipPath = AssetDatabase.GetAssetPath(clip);
                    if (!controllerPath.Equals(clipPath))
                    {
                        var newClip = UnityEngine.Object.Instantiate(clip) as AnimationClip;
                        newClip.name = state.name;
                        AssetDatabase.AddObjectToAsset(newClip, controller);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
                        AssetDatabase.DeleteAsset(clipPath);
                        state.motion = newClip;
                    }
                }
            }
        }
    }
}
