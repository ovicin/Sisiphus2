using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class EditorCameraManagerWindow : EditorWindow 
{
	public static bool IsOpen;

	EditorCamerasObject EditorCameras;
	Vector2 SavedCamerasScrollPosition = Vector2.zero;
	EditorCameraData DeleteCamera = null;
	bool ResortCameraList = false;

	string AddCameraName = "";
	bool AddCameraSaveSceneToggle = true;

	GUIStyle HeadlineText;
	GUIStyle BoldToolbarText;
	GUIStyle CameraDataFoldout;
	GUIStyle ItalicText;
	GUIStyle RedText;
	GUIStyle BlackText;

	Texture2D CameraIcon;
	Texture2D CameraIconEmpty;
	Texture2D CameraIconDrag;

	Texture2D WebsiteImage;
	Texture2D MailImage;
	Texture2D FacebookImage;
	Texture2D TwitterImage;
	Texture2D YoutubeImage;
	Texture2D WebsiteImageHover;
	Texture2D MailImageHover;
	Texture2D FacebookImageHover;
	Texture2D TwitterImageHover;
	Texture2D YoutubeImageHover;

	Dictionary<EditorCameraData, bool> ShowCameraFoldouts = new Dictionary<EditorCameraData, bool>();

	string LastScene;

	EditorCameraData DragData;
	bool IsDragging = false;

	public static SceneView LastActiveSceneView = null;
	static SceneView ShakeSceneView;
	static float ShakeSceneViewStartTime;
	static Vector3 ShakeSceneViewStartPosition;

	public static void GotoQuickslot( int slot )
	{
		EditorCameraData data = GetQuickslotData( slot );

		if( data != null )
		{
			GotoEditorCamera( data );
		}
	}

	public static void GotoEditorCamera( EditorCameraData data )
	{
		EditorCamera.SetPosition( data.Position, LastActiveSceneView );
		EditorCamera.SetRotation( data.Rotation, LastActiveSceneView );
		EditorCamera.SetOrthographic( data.IsOrthographic, LastActiveSceneView );
		EditorCamera.SetSize( data.Size, LastActiveSceneView );
	}

	public static void SaveQuickslot( int slot )
	{
		EditorCamerasObject cameras = (EditorCamerasObject)AssetDatabase.LoadAssetAtPath( "Assets/Editor/EditorCameraManager/EditorCameras.asset", typeof( EditorCamerasObject ) );

		EditorCameraData data = cameras.Cameras.Find( item => item.Name == "Quickslot " + slot && item.SceneName == EditorApplication.currentScene );

		if( data == null )
		{
			data = new EditorCameraData();

			data.Name = "Quickslot " + slot;

			data.AttachToScene = true;
			data.SceneName = EditorApplication.currentScene;

			cameras.Cameras.Add( data );
		}

		data.Position = EditorCamera.GetPosition( LastActiveSceneView );
		data.Rotation = EditorCamera.GetRotation( LastActiveSceneView );
		data.Size = EditorCamera.GetSize( LastActiveSceneView );
		data.IsOrthographic = EditorCamera.GetOrthographic( LastActiveSceneView );

		SetQuickslot( data, slot );

		ShakeActiveSceneView();

		if( EditorCameraManagerWindow.IsOpen == true )
		{
			EditorCameraManagerWindow window = (EditorCameraManagerWindow)EditorWindow.GetWindow( typeof( EditorCameraManagerWindow ) );
			window.Repaint();
		}
	}

	void OnEnable()
	{
		wantsMouseMove = true;

		ConnectToEditorCamerasObject();

		UpdateMinSize();
	}

	void OnDisable()
	{
		IsOpen = false;
	}

	void ConnectToEditorCamerasObject()
	{
		EditorCameras = (EditorCamerasObject)AssetDatabase.LoadAssetAtPath( "Assets/Editor/EditorCameraManager/EditorCameras.asset", typeof( EditorCamerasObject ) );

		if( EditorCameras == null )
		{
			EditorCameras = ScriptableObject.CreateInstance<EditorCamerasObject>();

			AssetDatabase.CreateAsset( EditorCameras, "Assets/Editor/EditorCameraManager/EditorCameras.asset" );
		}
	}

	void SetupStyles()
	{
		if( HeadlineText == null )
		{
			HeadlineText = new GUIStyle( EditorStyles.boldLabel );
			HeadlineText.padding.top = 0;
			HeadlineText.fontStyle = FontStyle.Bold;
			HeadlineText.fontSize = 17;
			HeadlineText.margin.top = 10;
			HeadlineText.margin.left = 6;
		}

		if( BoldToolbarText == null )
		{
			BoldToolbarText = new GUIStyle( "Label" );
			BoldToolbarText.padding.top = 0;
			BoldToolbarText.fontStyle = FontStyle.Normal;
			BoldToolbarText.padding.left = 1;
		}

		if( CameraDataFoldout == null )
		{
			CameraDataFoldout = new GUIStyle( "Foldout" );
			CameraDataFoldout.margin.top = 3;
			CameraDataFoldout.margin.left = 39;
		}

		if( ItalicText == null )
		{
			ItalicText = new GUIStyle( "Label" );
			ItalicText.fontStyle = FontStyle.Italic;
		}

		if( RedText == null )
		{
			RedText = new GUIStyle( "Label" );
			RedText.normal.textColor = Color.red;
		}

		if( BlackText == null )
		{
			BlackText = new GUIStyle( "Label" );
		}
	}

	void LoadImages()
	{
		if( CameraIcon == null )
		{
			CameraIcon = (Texture2D)Resources.Load( "EditorCameraManagerIcon", typeof( Texture2D ) );
		}

		if( CameraIconEmpty == null )
		{
			CameraIconEmpty = (Texture2D)Resources.Load( "EditorCameraManagerIconEmpty", typeof( Texture2D ) );
		}

		if( CameraIconDrag == null )
		{
			CameraIconDrag = (Texture2D)Resources.Load( "EditorCameraManagerDragIcon", typeof( Texture2D ) );
		}

		if( WebsiteImage == null )
		{
			WebsiteImage = (Texture2D)Resources.Load( "Website", typeof( Texture2D ) );
		}

		if( MailImage == null )
		{
			MailImage = (Texture2D)Resources.Load( "email", typeof( Texture2D ) );
		}

		if( FacebookImage == null )
		{
			FacebookImage = (Texture2D)Resources.Load( "facebook", typeof( Texture2D ) );
		}

		if( TwitterImage == null )
		{
			TwitterImage = (Texture2D)Resources.Load( "Twitter", typeof( Texture2D ) );
		}

		if( YoutubeImage == null )
		{
			YoutubeImage = (Texture2D)Resources.Load( "YouTube", typeof( Texture2D ) );
		}

		if( WebsiteImageHover == null )
		{
			WebsiteImageHover = (Texture2D)Resources.Load( "WebsiteHover", typeof( Texture2D ) );
		}

		if( MailImageHover == null )
		{
			MailImageHover = (Texture2D)Resources.Load( "emailHover", typeof( Texture2D ) );
		}

		if( FacebookImageHover == null )
		{
			FacebookImageHover = (Texture2D)Resources.Load( "facebookHover", typeof( Texture2D ) );
		}

		if( TwitterImageHover == null )
		{
			TwitterImageHover = (Texture2D)Resources.Load( "TwitterHover", typeof( Texture2D ) );
		}

		if( YoutubeImageHover == null )
		{
			YoutubeImageHover = (Texture2D)Resources.Load( "YouTubeHover", typeof( Texture2D ) );
		}
	}

	void OnGUI()
	{
		SetupStyles();
		LoadImages();

		if( IsOpen == false )
		{
			IsOpen = true;
		}

		DeleteSelectedCamera();
		SortCameraList();
		RepaintOnUndoRedo();

		DrawHeader();

		GUILayout.Space( 5 );

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginVertical( GUILayout.Width( Screen.width / 3f ) );
			{
				DrawQuickSlots();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical( GUILayout.Width( Screen.width * 2f / 3f ) );
			{
				DrawSavedCameras();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		if( Event.current.type == EventType.MouseMove )
		{
			Repaint();
		}

		if( Event.current.type == EventType.MouseUp && IsDragging == true )
		{
			OnDragStop();
		}

		if( IsDragging == true )
		{
			Rect drawRect = new Rect( Event.current.mousePosition.x - 13, Event.current.mousePosition.y - 13, 26, 26 );
			Rect textRect = new Rect( Event.current.mousePosition.x + 16, Event.current.mousePosition.y - 10, 150, 20 );

			GUI.DrawTexture( drawRect, CameraIconDrag );
			GUI.Label( textRect, DragData.Name );
			Repaint();
		}
	}

	static void SetQuickslot( EditorCameraData data, int slot )
	{
		EditorCamerasObject cameras = (EditorCamerasObject)AssetDatabase.LoadAssetAtPath( "Assets/Editor/EditorCameraManager/EditorCameras.asset", typeof( EditorCamerasObject ) );

		if( data.AttachToScene == true )
		{
			EditorCameraData otherData = GetQuickslotData( slot );

			if( otherData != null )
			{
				otherData.QuickSlot = -1;
			}
		}
		else
		{
			EditorCameraData otherData = cameras.Cameras.Find( item => item.QuickSlot == slot );

			if( otherData != null )
			{
				otherData.QuickSlot = -1;
			}
		}

		data.QuickSlot = slot;

		EditorUtility.SetDirty( cameras );
	}

	void OnDragStop()
	{
		for( int i = 0; i < 10; ++i )
		{
			if( IsHoveringQuickslot( i ) == true )
			{
				int slot = i + 1;

				if( slot == 10 )
				{
					slot = 0;
				}

				SetQuickslot( DragData, slot );
			}
		}

		IsDragging = false;
		DragData = null;
	}

	static EditorCameraData GetQuickslotData( int slot )
	{
		EditorCamerasObject cameras = (EditorCamerasObject)AssetDatabase.LoadAssetAtPath( "Assets/Editor/EditorCameraManager/EditorCameras.asset", typeof( EditorCamerasObject ) );

		return cameras.Cameras.Find( item => item.QuickSlot == slot && ( item.AttachToScene == false || item.SceneName == EditorApplication.currentScene ) );
	}

	bool IsHoveringQuickslot( int slot )
	{
		if( Event.current == null )
		{
			return false;
		}

		if( IsDragging == false )
		{
			return false;
		}

		Rect quickslotRect = new Rect( 0, 61 + slot * 30, 210, 30 );

		return quickslotRect.Contains( Event.current.mousePosition );
	}

	void Update()
	{
		if( LastScene != EditorApplication.currentScene )
		{
			Repaint();
			LastScene = EditorApplication.currentScene;
		}

		if( EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == typeof( SceneView ) )
		{
			LastActiveSceneView = (SceneView)EditorWindow.focusedWindow;
		}
	}

	static void ShakeActiveSceneView()
	{
		if( LastActiveSceneView == null )
		{
			ShakeSceneView = EditorCamera.GetActiveSceneView();
		}
		else
		{
			ShakeSceneView = LastActiveSceneView;
		}

		ShakeSceneViewStartTime = Time.realtimeSinceStartup;
		ShakeSceneViewStartPosition = EditorCamera.GetPosition( LastActiveSceneView );

		Vector3 shakeToRightPosition = ShakeSceneViewStartPosition + EditorCamera.GetRotation( LastActiveSceneView ) * Vector3.right * EditorCamera.GetSize( LastActiveSceneView ) * 0.05f;

		EditorCamera.SetPosition( shakeToRightPosition, LastActiveSceneView );

		EditorApplication.update += UpdateCameraShake;
	}

	static void UpdateCameraShake()
	{
		if( ShakeSceneView != null )
		{
			if( Time.realtimeSinceStartup - ShakeSceneViewStartTime > 0.15f )
			{
				EditorCamera.SetPosition( ShakeSceneViewStartPosition, LastActiveSceneView );
				ShakeSceneView = null;
				EditorApplication.update -= UpdateCameraShake;
			}
			else if( Time.realtimeSinceStartup - ShakeSceneViewStartTime > 0.05f )
			{
				Vector3 shakeToLeftPosition = ShakeSceneViewStartPosition - EditorCamera.GetRotation( LastActiveSceneView ) * Vector3.right * EditorCamera.GetSize( LastActiveSceneView ) * 0.05f;
				EditorCamera.SetPosition( shakeToLeftPosition, LastActiveSceneView );
			}
		}
	}

	void RepaintOnUndoRedo()
	{
		if( Event.current.type == EventType.ValidateCommand )
		{
			switch( Event.current.commandName )
			{
			case "UndoRedoPerformed":
				Repaint();
				break;
			}
		}
	}

	void DeleteSelectedCamera()
	{
		if( DeleteCamera != null )
		{
			Undo.RegisterUndo( EditorCameras, "Delete Camera '" + DeleteCamera.Name + "'" );
			EditorCameras.Cameras.Remove( DeleteCamera );
			DeleteCamera = null;
			EditorUtility.SetDirty( EditorCameras );
		}
	}

	void SortCameraList()
	{
		if( ResortCameraList == true )
		{
			SortEditorCamerasByName();
			ResortCameraList = false;
		}
	}

	public void UpdateMinSize()
	{
		this.minSize = new Vector2( 650, 363 );
		Repaint();
	}

	void DrawHeader()
	{
		GUILayout.Label( "Editor Camera Manager", HeadlineText );

		GUI.Label( new Rect( 445, 12, 100, 20 ), "Contact me:" );

		DrawIconButton( 30, WebsiteImage, WebsiteImageHover, "http://www.olivereberlei.com" );
		DrawIconButton( 60, MailImage, MailImageHover, "mailto: oliver@eberlei.de" );
		DrawIconButton( 90, TwitterImage, TwitterImageHover, "http://www.twitter.com/olivereberlei" );
		DrawIconButton( 120, YoutubeImage, YoutubeImageHover, "http://www.youtube.com/user/OliverEberlei/videos" );
	}

	void DrawIconButton( int left, Texture2D image, Texture2D imageHover, string url )
	{
		bool isMouseOver = false;
		Rect drawRect = new Rect( 498 + left, 8, 24, 24 );

		if( Event.current != null )
		{
			if( drawRect.Contains( Event.current.mousePosition ) )
			{
				isMouseOver = true;
			}
		}

		
		EditorGUIUtility.AddCursorRect( drawRect, MouseCursor.Link );

		if( isMouseOver == true )
		{
			GUI.DrawTexture( drawRect, imageHover );
		}
		else
		{
			GUI.DrawTexture( drawRect, image );
		}

		if( Event.current != null && Event.current.type == EventType.MouseDown )
		{
			if( isMouseOver == true )
			{
				Application.OpenURL( url );
			}
		}
	}

	void DrawQuickSlots()
	{
		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		{
			GUILayout.Label( "Quick Slots", BoldToolbarText );
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );

		for( int i = 0; i < 10; ++i )
		{
			int quickSlotIndex = i + 1;

			if( quickSlotIndex == 10 )
			{
				quickSlotIndex = 0;
			}

			EditorCameraData data = GetQuickslotData( quickSlotIndex );

			float top = 62 + i * 30;

			GUI.Label( new Rect( 6, top + 4, 50, 20 ), "Alt+" + quickSlotIndex.ToString(), IsHoveringQuickslot( i ) ? RedText : BlackText );

			Rect iconDrawRect = new Rect( 48, top, 26, 26 );
			if( data == null )
			{
				GUI.DrawTexture( iconDrawRect, CameraIconEmpty );
			}
			else
			{
				EditorGUIUtility.AddCursorRect( iconDrawRect, MouseCursor.Link );

				if( Event.current != null && iconDrawRect.Contains( Event.current.mousePosition ) )
				{
					if( Event.current.type == EventType.MouseDown )
					{
						IsDragging = true;
						DragData = data;
					}

					GUI.DrawTexture( iconDrawRect, CameraIconDrag );
				}
				else
				{
					GUI.DrawTexture( iconDrawRect, CameraIcon );
				}
			}

			if( IsHoveringQuickslot( i ) == true )
			{
				GUI.Label( new Rect( 78, top + 4, 150, 20 ), DragData.Name, RedText );
			}
			else
			{
				if( data == null )
				{
					GUI.Label( new Rect( 78, top + 4, 150, 20 ), "<Drag an Icon here>", ItalicText );
				}
				else
				{
					GUI.Label( new Rect( 78, top + 4, 150, 20 ), data.Name, BlackText );
				}
			}
		}
	}

	void DrawSavedCameras()
	{
		DrawSavedCamerasList();
		DrawAddCamera();
	}

	void DrawSavedCamerasList()
	{
		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		{
			GUILayout.Label( "Saved Cameras", BoldToolbarText );
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );

		SavedCamerasScrollPosition = EditorGUILayout.BeginScrollView( SavedCamerasScrollPosition );
		{
			int offset = 0;

			foreach( EditorCameraData data in EditorCameras.Cameras )
			{
				if( data.AttachToScene == false || data.SceneName == EditorApplication.currentScene )
				{
					GUILayout.Space( 4 );

					DrawEditorCamera( data, offset );

					if( ShowCameraFoldouts[ data ] == true )
					{
						offset += 200;
					}
					else
					{
						offset += 30;
					}

					GUILayout.Space( 5 );
				}
			}
		}
		EditorGUILayout.EndScrollView();
	}

	void DrawEditorCamera( EditorCameraData data, int offset )
	{
		EditorGUILayout.BeginHorizontal();
		{
			Rect drawRect = new Rect( 9, offset, 26, 26 );

			EditorGUIUtility.AddCursorRect( drawRect, MouseCursor.Link );
			if( Event.current != null && drawRect.Contains( Event.current.mousePosition ) )
			{
				if( Event.current.type == EventType.MouseDown )
				{
					IsDragging = true;
					DragData = data;
				}

				GUI.DrawTexture( drawRect, CameraIconDrag );
			}
			else
			{
				GUI.DrawTexture( drawRect, CameraIcon );
			}
			

			if( ShowCameraFoldouts.ContainsKey( data ) == false )
			{
				ShowCameraFoldouts.Add( data, false );
			}

			ShowCameraFoldouts[ data ] = EditorGUILayout.Foldout( ShowCameraFoldouts[ data ], data.Name, CameraDataFoldout );

			if( GUILayout.Button( "Goto", GUILayout.Width( 50 ) ) )
			{
				GotoEditorCamera( data );
			}

			if( GUILayout.Button( "Save", GUILayout.Width( 50 ) ) )
			{
				EditorApplication.Beep();
				if( EditorUtility.DisplayDialog( "Overwrite '" + data.Name + "'?", "Do you want to overwrite the camera position '" + data.Name + "' with the current editor camera position?", "Yes", "No" ) )
				{
					Undo.RegisterUndo( EditorCameras, "Save current Position to '" + data.Name + "'" );

					data.Position = EditorCamera.GetPosition( LastActiveSceneView );
					data.Rotation = EditorCamera.GetRotation( LastActiveSceneView );
					data.Size = EditorCamera.GetSize( LastActiveSceneView );
					data.IsOrthographic = EditorCamera.GetOrthographic( LastActiveSceneView );

					ShakeActiveSceneView();

					EditorUtility.SetDirty( EditorCameras );
				}
			}

			if( GUILayout.Button( "Delete", GUILayout.Width( 50 ) ) )
			{
				EditorApplication.Beep();
				if( EditorUtility.DisplayDialog( "Delete '" + data.Name + "'?", "Do you really want to delete the camera position '" + data.Name + "'?", "Yes", "No" ) )
				{
					DeleteCamera = data;
				}
			}

			GUILayout.Space( 5 );
		}
		EditorGUILayout.EndHorizontal();


		if( ShowCameraFoldouts[ data ] == true )
		{
			GUILayout.Space( 5 );

			string newName = EditorGUILayout.TextField( "Name", data.Name );

			if( data.Name.Length > 10 && data.Name.Substring( 0, 10 ) == "Quickslot " )
			{
				GUI.enabled = false;
			}

			bool newAttached = EditorGUILayout.Toggle( "Attach To Scene", data.AttachToScene );
			string newSceneName = EditorGUILayout.TextField( "Scene Name", data.SceneName );

			GUI.enabled = true;
			
			float newSize = EditorGUILayout.FloatField( "Size", data.Size );
			bool newOrtho = EditorGUILayout.Toggle( "Is Orthographic", data.IsOrthographic );
			Vector3 newPosition = EditorGUILayout.Vector3Field( "Position", data.Position );
			Vector3 newRotation = EditorGUILayout.Vector3Field( "Rotation", data.Rotation.eulerAngles );

			if( newName != data.Name
				|| newAttached != data.AttachToScene
				|| newSceneName != data.SceneName
				|| newSize != data.Size
				|| newOrtho != data.IsOrthographic
				|| newPosition != data.Position
				|| newRotation != data.Rotation.eulerAngles )
			{
				Undo.RegisterUndo( EditorCameras, "Edit Editor Camera '" + data.Name + "'" );

				if( newName.Length > 10 && newName.Substring( 0, 10 ) == "Quickslot " )
				{
					newAttached = true;
					newSceneName = EditorApplication.currentScene;
				}

				if( newSceneName != data.SceneName )
				{
					newAttached = false;
				}

				if( newAttached == true )
				{
					Object scene = AssetDatabase.LoadMainAssetAtPath( data.SceneName );

					if( scene == null )
					{
						newAttached = false;
						Debug.LogWarning( "Scene '" + data.SceneName + "' does not exist! Did you foget the .unity extension?" );
					}
				}

				data.Name = newName;
				data.AttachToScene = newAttached;
				data.SceneName = newSceneName;
				data.Size = newSize;
				data.IsOrthographic = newOrtho;
				data.Position = newPosition;
				data.Rotation = Quaternion.Euler( newRotation );

				ResortCameraList = true;

				EditorUtility.SetDirty( EditorCameras );
			}
		}
	}

	void SortEditorCamerasByName()
	{
		EditorCameras.Cameras.Sort( new System.Comparison<EditorCameraData>( SortEditorCamerasByNameComparer ) );
	}

	int SortEditorCamerasByNameComparer( EditorCameraData obj1, EditorCameraData obj2 )
	{
		return obj1.Name.CompareTo( obj2.Name );
	}

	void DrawAddCamera()
	{
		bool isReturnPressed = false;

		if( Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return )
		{
			isReturnPressed = true;
		}

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Space( 7 );

			EditorGUILayout.BeginVertical();
			{
				GUILayout.Label( "Add Camera", EditorStyles.boldLabel );

				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label( "Name" );
					AddCameraName = EditorGUILayout.TextField( AddCameraName );
					AddCameraSaveSceneToggle = EditorGUILayout.Toggle( "Attach to Scene", AddCameraSaveSceneToggle, GUILayout.Width( 120 ) );

					if( GUILayout.Button( "Save", GUILayout.Width( 50 ) ) || ( isReturnPressed == true && AddCameraName != "" ) )
					{
						if( AddCameraName == "" )
						{
							EditorApplication.Beep();
							EditorUtility.DisplayDialog( "Y U GIV NO NAME?", "You have to enter a name for this camera", "Fine!" );
						}
						else
						{
							EditorCameraData cameraData = new EditorCameraData();

							cameraData.Name = AddCameraName;

							cameraData.SceneName = EditorApplication.currentScene;
							cameraData.AttachToScene = AddCameraSaveSceneToggle;

							cameraData.Position = EditorCamera.GetPosition( LastActiveSceneView );
							cameraData.Rotation = EditorCamera.GetRotation( LastActiveSceneView );
							cameraData.Size = EditorCamera.GetSize( LastActiveSceneView );
							cameraData.IsOrthographic = EditorCamera.GetOrthographic( LastActiveSceneView );

							ShakeActiveSceneView();

							Undo.RegisterUndo( EditorCameras, "Add Camera '" + AddCameraName + "'" );
							EditorCameras.Cameras.Add( cameraData );
							SortEditorCamerasByName();

							AddCameraName = "";
							EditorGUIUtility.keyboardControl = 0;

							EditorUtility.SetDirty( EditorCameras );
						}
					}

					GUILayout.Space( 5 );
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );
	}
}
