using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class EditorGUIHierarchyViewSceneViewExampleWindow : EditorWindow
{

	EditorGUIHierarchyView hierarchyView = new EditorGUIHierarchyView();

	Color activeColor = new Color(0.705f, 0.705f, 0.705f);
	Color inactiveColor = Color.gray;

	[MenuItem ("EditorGUIHierarchyViewExampleWindow/Show Scene View")]
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
			hierarchyView.BeginNode (go.name,GetUnselectedTextColorForGameObject (go), GetSelectedTextColorForGameObject (go));
			foreach (Transform child in go.transform) {
				DrawGameObject (child.gameObject);
			}
			hierarchyView.EndNode ();
		} else {
			hierarchyView.Node (go.name, GetUnselectedTextColorForGameObject (go), GetSelectedTextColorForGameObject (go));
		}
	}

	Color GetSelectedTextColorForGameObject(GameObject go) {
		if (go.activeInHierarchy)
			return Color.white;
		else
			return activeColor;
	}

	Color GetUnselectedTextColorForGameObject(GameObject go) {
		if (go.activeInHierarchy)
			return activeColor;
		else
			return inactiveColor;
	}
}

