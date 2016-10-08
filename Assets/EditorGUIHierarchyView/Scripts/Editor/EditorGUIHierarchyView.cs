using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class EditorGUIHierarchyView {

	List<string> OpenIds = new List<string>();
	List<string> SelectedIds = new List<string>();

	GUIStyle foldoutChildessStyle;
	GUIStyle selectedAreaStyle;
	GUIStyle selectedLabel;
	GUIStyle selectedFoldout;
	GUIStyle normalFoldout;

	Color defaultSelectedTextColorDarkSkin = Color.white;
	Color defaultUnselectedTextColorDarkSkin = new Color(0.705f, 0.705f, 0.705f);
	Color defaultSelectedTextColorWhiteSkin = Color.white;
	Color defaultUnselectedTextColorWhiteSkin = Color.black;

	Color defaultSelectedTextColor;
	Color defaultUnselectedTextColor;
	bool defaultTextColorsSet ;

	Vector2 scrollPosition;
	string previousNodeID;
	bool isCurrentNodeVisible = true;
	string lastVisibleNodeID;
	List<string> nodeIDBreadcrumb = new List<string>();

	public void BeginHierarchyView() {

		isCurrentNodeVisible = true;
		lastVisibleNodeID = null;
		nodeIDBreadcrumb.Clear ();
		EditorGUI.indentLevel = 0;

		if (foldoutChildessStyle == null)
			CreateStyles ();

		EditorGUILayout.BeginVertical ();
		scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);

		previousNodeID = null;
	}

	void CreateStyles() {
		foldoutChildessStyle = new GUIStyle (EditorStyles.label);
		var padding = foldoutChildessStyle.padding;
		padding.left = 16;
		foldoutChildessStyle.padding = padding;


		selectedAreaStyle = new GUIStyle (GUIStyle.none);
		selectedAreaStyle.normal.background = MakeTex (1, 1, new Color (0.24f, 0.49f, 0.91f));
		selectedAreaStyle.active.background = selectedAreaStyle.normal.background;

		selectedLabel = new GUIStyle(foldoutChildessStyle);
		selectedLabel.normal.textColor = Color.white;

		selectedFoldout = new GUIStyle (EditorStyles.foldout);
		selectedFoldout.normal.textColor = Color.white;
		selectedFoldout.active.textColor = Color.white;
		selectedFoldout.focused.textColor = Color.white;
		selectedFoldout.onNormal.textColor = Color.white;
		selectedFoldout.onActive.textColor = Color.white;
		selectedFoldout.onFocused.textColor = Color.white;

		normalFoldout = new GUIStyle (EditorStyles.foldout);
		normalFoldout.active = normalFoldout.normal;
		normalFoldout.focused = normalFoldout.normal;
		normalFoldout.onActive = normalFoldout.onNormal;
		normalFoldout.onFocused = normalFoldout.onNormal;

		SetDefaultTextColors ();
	}

	void SetDefaultTextColors() {
		if (defaultTextColorsSet)
			return;

		if (EditorGUIUtility.isProSkin) {
			defaultSelectedTextColor = defaultSelectedTextColorDarkSkin;
			defaultUnselectedTextColor = defaultUnselectedTextColorDarkSkin;
		} else {
			defaultSelectedTextColor = defaultSelectedTextColorWhiteSkin;
			defaultUnselectedTextColor = defaultUnselectedTextColorWhiteSkin;
		}

		defaultTextColorsSet = true;
	}

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];

		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}


	bool IsOpened(string id) {
		return OpenIds.Contains (id);
	}

	void Open(string id) {
		if (!IsOpened (id)) {
			OpenIds.Add (id);
		}
	}

	void Close(string id) {
		if (IsOpened (id)) {
			OpenIds.Remove (id);
		}
	}

	void AddToSelection(string id) {
		SelectedIds.Add (id);
	}

	bool IsSelected(string id) {
		return SelectedIds.Contains (id);
	}

	void RemoveFromSelection(string id) {
		SelectedIds.Remove (id);
	}

	void SetSelected(string id) {
		SelectedIds.Clear ();
		SelectedIds.Add (id);
		GUI.FocusControl(id);
	}

	//Returns true if this node is selected
	public bool BeginNode(string label) {
		return Node (label, true, defaultUnselectedTextColor,defaultSelectedTextColor);
	}

	//Returns true if this node is selected
	public bool BeginNode(string label, Color unselectedTextColor, Color selectedTextColor) {
		return Node (label, true,unselectedTextColor,selectedTextColor);
	}


	//Returns true if this node is selected
	public bool Node(string label, Color unselectedTextColor, Color selectedTextColor) {

		return Node (label, false, unselectedTextColor,selectedTextColor);
	}

	//Returns true if this node is selected
	public bool Node(string label) {

		return Node (label, false, defaultUnselectedTextColor,defaultSelectedTextColor);
	}

	bool Node(string label, bool isParent, Color unselectedTextColor, Color selectedTextColor) {

		var id = GetIDForLabel (label);

		if (isParent) {
			nodeIDBreadcrumb.Add (id);
		}

		if (!isCurrentNodeVisible)
			return false;


		bool wasOpened = IsOpened (id);
		bool isSelected = IsSelected (id);
		bool touchedInside = DrawNodeTouchableArea (id);
		GUI.SetNextControlName(id);
		bool opened = false;

		if (isParent) {
			GUIStyle styleToUse = isSelected ? selectedFoldout : normalFoldout;
			Color colorToUse = isSelected ? selectedTextColor : unselectedTextColor;
			styleToUse.normal.textColor = colorToUse;
			styleToUse.onNormal.textColor = colorToUse;
			styleToUse.active.textColor = colorToUse;
			styleToUse.onActive.textColor = colorToUse;
			styleToUse.focused.textColor = colorToUse;
			styleToUse.onFocused.textColor = colorToUse;

			opened = EditorGUILayout.Foldout (wasOpened, label, styleToUse);

			if (isSelected && Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.RightArrow) {
				opened = true;
			} else if (isSelected && Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.LeftArrow) {
				opened = false;
			}
		} else {
			GUIStyle styleToUse = isSelected ?selectedLabel:foldoutChildessStyle;
			Color colorToUse = isSelected ? selectedTextColor : unselectedTextColor;

			styleToUse.normal.textColor = colorToUse;
			styleToUse.active.textColor = colorToUse;

			EditorGUILayout.LabelField ( label, styleToUse);
		}

		EditorGUILayout.EndHorizontal ();

		bool useCurrentEvent = false;

		if (wasOpened != opened) {
			useCurrentEvent = true;
			if (opened)
				Open (id);
			else
				Close (id);
		} else if(touchedInside) {
			useCurrentEvent = true;
			if (Event.current.command) {
				if (IsSelected (id)) {
					RemoveFromSelection (id);
				} else {
					AddToSelection (id);
				}
			}
			else
				SetSelected (id);
		}

		HandleKeyboardCycle (previousNodeID, id);

		previousNodeID = id;

		if(useCurrentEvent) {
			Event.current.Use ();
		}


		if (isParent && !opened) {
			isCurrentNodeVisible = false;
			lastVisibleNodeID = id;
		}

		if (isParent) {
			EditorGUI.indentLevel++;
		}

		return IsSelected (id);
	}

	public void EndNode() {
		string endedNodeId =  nodeIDBreadcrumb [nodeIDBreadcrumb.Count - 1];
		if (endedNodeId == lastVisibleNodeID) {
			isCurrentNodeVisible = true;
			lastVisibleNodeID = null;
		}
		nodeIDBreadcrumb.RemoveAt (nodeIDBreadcrumb.Count - 1);

		if(isCurrentNodeVisible)
			EditorGUI.indentLevel--;
	}


	string GetIDForLabel(string label) {
		StringBuilder sb = new StringBuilder ();
		foreach (string id in nodeIDBreadcrumb) {
			sb.Append (id);
			sb.Append ("_");
		}

		sb.Append (label);
		return sb.ToString ();
	}

	void HandleKeyboardCycle(string previousNodeID, string currentNodeID) {
		if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.DownArrow) {
			if (IsSelected (previousNodeID)) {
				Event.current.Use ();
				SetSelected (currentNodeID);
			}
		}
		else if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.UpArrow) {
			if (IsSelected (currentNodeID)) {
				Event.current.Use ();
				SetSelected (previousNodeID);
			}
		}
	}

	bool DrawNodeTouchableArea(string id) {
		var area = EditorGUILayout.BeginHorizontal (IsSelected(id)?selectedAreaStyle:GUIStyle.none, GUILayout.ExpandWidth (true));
		Event currentEvent = Event.current;
		bool touchedInside = false;
		if (currentEvent.type == EventType.mouseUp) {
			Vector2 mousePosition = currentEvent.mousePosition;
			if (area.Contains (mousePosition)) {
				touchedInside = true;
			}
		}

		return touchedInside;
	}


	public void EndHierarchyView() {

		if (nodeIDBreadcrumb.Count > 0) {
			Debug.LogError ("Called EndHierarchyView with nodes still opened. Please ensure that you have a EndNode() for every BeginNode()");
		}

		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndScrollView ();
	}
}
