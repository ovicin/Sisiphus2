using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorCameraManagerMenuItems : EditorWindow
{
	[MenuItem( "Window/Editor Camera Manager/Open Manager %e" )]
	public static void Init()
	{
		EditorCameraManagerWindow window = (EditorCameraManagerWindow)EditorWindow.GetWindow( typeof( EditorCameraManagerWindow ), false, "Editor Camera Manager" );

		window.UpdateMinSize();

		if( EditorCameraManagerWindow.IsOpen )
		{
			window.Close();
			EditorCameraManagerWindow.IsOpen = false;
		}
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 1 &1" )]
	public static void GotoQuickslot1()
	{
		EditorCameraManagerWindow.GotoQuickslot( 1 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 2 &2" )]
	public static void GotoQuickslot2()
	{
		EditorCameraManagerWindow.GotoQuickslot( 2 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 3 &3" )]
	public static void GotoQuickslot3()
	{
		EditorCameraManagerWindow.GotoQuickslot( 3 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 4 &4" )]
	public static void GotoQuickslot4()
	{
		EditorCameraManagerWindow.GotoQuickslot( 4 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 5 &5" )]
	public static void GotoQuickslot5()
	{
		EditorCameraManagerWindow.GotoQuickslot( 5 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 6 &6" )]
	public static void GotoQuickslot6()
	{
		EditorCameraManagerWindow.GotoQuickslot( 6 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 7 &7" )]
	public static void GotoQuickslot7()
	{
		EditorCameraManagerWindow.GotoQuickslot( 7 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 8 &8" )]
	public static void GotoQuickslot8()
	{
		EditorCameraManagerWindow.GotoQuickslot( 8 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 9 &9" )]
	public static void GotoQuickslot9()
	{
		EditorCameraManagerWindow.GotoQuickslot( 9 );
	}

	[MenuItem( "Window/Editor Camera Manager/Goto Quickslot/Goto Quickslot 0 &0" )]
	public static void GotoQuickslot0()
	{
		EditorCameraManagerWindow.GotoQuickslot( 0 );
	}



	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 1 %1" )]
	public static void SaveQuickslot1()
	{
		EditorCameraManagerWindow.SaveQuickslot( 1 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 2 %2" )]
	public static void SaveQuickslot2()
	{
		EditorCameraManagerWindow.SaveQuickslot( 2 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 3 %3" )]
	public static void SaveQuickslot3()
	{
		EditorCameraManagerWindow.SaveQuickslot( 3 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 4 %4" )]
	public static void SaveQuickslot4()
	{
		EditorCameraManagerWindow.SaveQuickslot( 4 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 5 %5" )]
	public static void SaveQuickslot5()
	{
		EditorCameraManagerWindow.SaveQuickslot( 5 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 6 %6" )]
	public static void SaveQuickslot6()
	{
		EditorCameraManagerWindow.SaveQuickslot( 6 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 7 %7" )]
	public static void SaveQuickslot7()
	{
		EditorCameraManagerWindow.SaveQuickslot( 7 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 8 %8" )]
	public static void SaveQuickslot8()
	{
		EditorCameraManagerWindow.SaveQuickslot( 8 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 9 %9" )]
	public static void SaveQuickslot9()
	{
		EditorCameraManagerWindow.SaveQuickslot( 9 );
	}

	[MenuItem( "Window/Editor Camera Manager/Save Quickslot/Save Quickslot 0 %0" )]
	public static void SaveQuickslot0()
	{
		EditorCameraManagerWindow.SaveQuickslot( 0 );
	}
}
