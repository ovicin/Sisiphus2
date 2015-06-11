using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EditorCameraData
{
	public string Name;
	public int QuickSlot = -1;

	public bool AttachToScene;
	public string SceneName;

	public bool IsOrthographic;
	public Vector3 Position;
	public Quaternion Rotation;
	public float Size;
}

public class EditorCamerasObject : ScriptableObject
{
	public List<EditorCameraData> Cameras = new List<EditorCameraData>();
}
