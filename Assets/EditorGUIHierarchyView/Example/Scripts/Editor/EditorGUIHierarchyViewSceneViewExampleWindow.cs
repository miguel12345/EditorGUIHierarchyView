using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class EditorGUIHierarchyViewSceneViewExampleWindow : EditorWindow
{

	EditorGUIHierarchyView hierarchyView = new EditorGUIHierarchyView();

	[MenuItem ("Window/EditorGUIHierarchy - Show Scene View")]
	static void Init ()
	{
		EditorGUIHierarchyViewSceneViewExampleWindow window = (EditorGUIHierarchyViewSceneViewExampleWindow)GetWindow (typeof(EditorGUIHierarchyViewSceneViewExampleWindow));
		window.Show ();
	}

	struct SceneGameObjectComparer : IComparer<GameObject> {

		public int Compare(GameObject go1, GameObject go2) {
			return go1.transform.GetSiblingIndex ().CompareTo (go2.transform.GetSiblingIndex ());
		}
	}

	void OnEnable() {
		this.titleContent = new GUIContent ("Scene View");
	}

	public void OnGUI ()
	{

		var gameObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();
		Array.Sort (gameObjects,new SceneGameObjectComparer());

		hierarchyView.BeginHierarchyView ();
		foreach (GameObject gameObject in gameObjects) {
			gameObject.transform.GetSiblingIndex ();
			DrawGameObject (gameObject);
		}
		hierarchyView.EndHierarchyView ();

		Repaint ();
	}

	void DrawGameObject(GameObject go) {
		if (go.transform.childCount > 0) {

			if (go.activeInHierarchy) {
				hierarchyView.BeginNode (go.name);
			} else {
				hierarchyView.BeginNode (go.name,Color.gray,Color.gray);	
			}

			foreach (Transform child in go.transform) {
				DrawGameObject (child.gameObject);
			}
			hierarchyView.EndNode ();
		} else {

			if (go.activeInHierarchy) {
				hierarchyView.Node (go.name);
			} else {
				hierarchyView.Node (go.name,Color.gray,Color.gray);	
			}

		}
	}
}

