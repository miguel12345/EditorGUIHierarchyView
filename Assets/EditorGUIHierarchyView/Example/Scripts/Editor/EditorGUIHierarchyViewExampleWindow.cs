using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorGUIHierarchyViewExampleWindow : EditorWindow {


	EditorGUIHierarchyView hierarchyView = new EditorGUIHierarchyView();

	[MenuItem ("EditorGUIHierarchyViewExampleWindow/Show Example Window")]
	static void Init ()
	{
		EditorGUIHierarchyViewExampleWindow window = (EditorGUIHierarchyViewExampleWindow)GetWindow (typeof(EditorGUIHierarchyViewExampleWindow));
		window.Show ();
	}

	public void OnGUI ()
	{
		hierarchyView.BeginHierarchyView ();

		bool isParent1Selected = hierarchyView.BeginNode ("Select me");
		hierarchyView.Node ("Child");
		hierarchyView.BeginNode ("Another parent");
		hierarchyView.Node ("Grandson");
		hierarchyView.EndNode ();
		hierarchyView.EndNode ();

		hierarchyView.Node ("No children");
		hierarchyView.Node ("Hello");

		hierarchyView.EndHierarchyView ();

		if (isParent1Selected) {
			EditorGUILayout.LabelField ("First parent is selected");
		}

		Repaint ();
	}
}
