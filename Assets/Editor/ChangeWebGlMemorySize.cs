using UnityEngine;
using UnityEditor;

public class ChangeWebGlMemeorySize : Editor
{
    [MenuItem("Tool/设置webgl内存为512")]
    public static void ChangeMemorySize()
    {
        PlayerSettings.WebGL.memorySize = 512;
    }

    [MenuItem("Tool/查看webgl内存大小")]
    public static void GetMemorySize()
    {
        Debug.Log(PlayerSettings.WebGL.memorySize);
    }

}