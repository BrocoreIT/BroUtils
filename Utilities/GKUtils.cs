using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



/// <summary>
/// Written by GK.
/// Extentions For easy development.
/// </summary>


public static class GKUtils
{

	public static string RemoveWhitespace(this string input)
	{
		return new string(input.ToCharArray()
			.Where(c => !Char.IsWhiteSpace(c))
			.ToArray());
	}


	public static void Log(string tag, string tag2, string message)
	{
		Log(tag, $"<{tag2}> {message}");
		Debug.Log($"<<{tag}>> {message}");
	}


	public static void Log(string tag, string message)
	{
		Debug.Log($"<<{tag}>> {message}");
	}

	public static List<T> Swap<T>(this List<T> list, int indexA, int indexB)
	{
		T tmp = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = tmp;
		return list;
	}

	//public static float Map(this double s, float fromMin, float fromMax, float toMin, float toMax) => toMin + ((float)s - fromMin) * (toMax - toMin) / (fromMax - fromMin);

	public static float Map(this float s, float fromMin, float fromMax, float toMin, float toMax) => toMin + (s - fromMin) * (toMax - toMin) / (fromMax - fromMin);

	public static int Map(this int s, float fromMin, float fromMax, float toMin, float toMax) => (int)Map((float)s, fromMin, fromMax, toMin, toMax);

	public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
	{
		parent.layer = layer;
		if (includeChildren)
		{
			foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = layer;
			}
		}
	}
	private static System.Random rng = new System.Random();

	public static void Blink(this MonoBehaviour mono, GameObject gameObject, int times = 10, float interval = 0.1f, int freq = 2)
	{
		mono.StartCoroutine(Blink(gameObject, times, interval, freq));
	}

	static IEnumerator Blink(GameObject go, int times, float interval, int freq)
	{

		var seconds = new WaitForSeconds(interval);
		for (int i = 0; i <= times; i++)
		{
			go.SetActive(i % freq == 0);
			yield return seconds;
		}
		go.SetActive(true);

	}

	public static void AnimatedMove(this MonoBehaviour mono, Transform transform, Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, System.Action done = null)
	{
		mono.StartCoroutine(AnimateMoveCo(transform, origin, target, duration, animationCurve, () => done?.Invoke()));
	}
	static IEnumerator AnimateMoveCo(Transform transform, Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, System.Action done = null)
	{
		float journey = 0f;
		float dist = float.MaxValue;
		while (journey <= duration)
		{
			dist = Vector3.Distance(origin, target);
			journey += Time.deltaTime;
			float percent = Mathf.Clamp01(journey / duration);

			float curvePercent = animationCurve.Evaluate(percent);
			transform.position = Vector3.LerpUnclamped(origin, target, curvePercent);

			yield return null;
		}
		done?.Invoke();
	}
	public static void AnimatedLocalMove(this MonoBehaviour mono, Transform transform, Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, System.Action done = null)
	{
		mono.StartCoroutine(AnimateLocalMoveCo(transform, origin, target, duration, animationCurve, () => done?.Invoke()));
	}
	static IEnumerator AnimateLocalMoveCo(Transform transform, Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, System.Action done = null)
	{
		float journey = 0f;
		float dist = float.MaxValue;
		while (journey <= duration)
		{
			dist = Vector3.Distance(origin, target);
			journey += Time.deltaTime;
			float percent = Mathf.Clamp01(journey / duration);

			float curvePercent = animationCurve.Evaluate(percent);
			transform.localPosition = Vector3.LerpUnclamped(origin, target, curvePercent);

			yield return null;
		}
		done?.Invoke();
	}
	public static void AnimatedMove(this MonoBehaviour mono, Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, Action<Vector3> newPos, System.Action done = null)
	{
		mono.StartCoroutine(AnimateMoveCo(origin, target, duration, animationCurve, newPos, () => done?.Invoke()));
	}
	static IEnumerator AnimateMoveCo(Vector3 origin, Vector3 target, float duration, AnimationCurve animationCurve, Action<Vector3> newPos, System.Action done = null)
	{
		float journey = 0f;
		float dist = float.MaxValue;
		Vector3 _newPos;
		while (journey <= duration)
		{
			dist = Vector3.Distance(origin, target);
			journey += Time.deltaTime;
			float percent = Mathf.Clamp01(journey / duration);

			float curvePercent = animationCurve.Evaluate(percent);
			_newPos = Vector3.LerpUnclamped(origin, target, curvePercent);
			newPos?.Invoke(_newPos);
			yield return null;
		}
		done?.Invoke();
	}

	public static void AnimatedFloat(this MonoBehaviour mono, float origin, float target, float duration, AnimationCurve animationCurve, Action<float> value = null, Action done = null)
	{
		mono.StartCoroutine(AnimatedFloatCo(origin, target, duration, animationCurve, (v) => value?.Invoke(v), () => done?.Invoke()));
	}
	static IEnumerator AnimatedFloatCo(float origin, float target, float duration, AnimationCurve animationCurve, Action<float> value = null, System.Action done = null)
	{
		float journey = 0f;
		float _val = origin;
		while (journey <= duration)
		{
			journey += Time.deltaTime;
			float percent = Mathf.Clamp01(journey / duration);

			float curvePercent = animationCurve.Evaluate(percent);
			_val = Mathf.LerpUnclamped(origin, target, curvePercent);
			value?.Invoke(_val);
			yield return null;
		}
		done?.Invoke();
	}



	public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
	{
		return k == 0 ? new[] { new T[0] } :
		  elements.SelectMany((e, i) =>
			elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
	}



	public static void Shuffle<T>(this List<T> list)
	{

		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static List<T> GetShuffled<T>(this List<T> list)
	{
		var newList = new List<T>(list.Count);

		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n);
			newList[k] = list[n];
			newList[n] = list[k];
		}
		return newList;

	}
	public static T GetRandomShuffled<T>(this List<T> list)
	{
		Shuffle(list);

		var rand = rng.Next(0, list.Count);

		return list[rand];
	}

	/* public static T GetRandomSeededShuffed<T>(this List<T> list)
	 {
		 SeedShuffle(list);

		 var rand = DungeonGenerator.Random.Instance.Next(0, list.Count-1);

		 return list[rand];
	 }
	 public static T GetRandomSeeded<T>(this List<T> list)
	 {
		 var rand = DungeonGenerator.Random.Instance.Next(0, list.Count - 1);

		 return list[rand];
	 }

	 public static void SeedShuffle<T>(this List<T> list)
	 {
		 int n = list.Count;
		 while (n > 1)
		 {
			 n--;
			 int k = DungeonGenerator.Random.Instance.Next(n);
			 T value = list[k];
			 list[k] = list[n];
			 list[n] = value;
		 }
	 }

	 public static void SeedShuffle<T>(this T[] list)
	 {
		 int n = list.Length;
		 while (n > 1)
		 {
			 n--;
			 int k = DungeonGenerator.Random.Instance.Next(n);
			 T value = list[k];
			 list[k] = list[n];
			 list[n] = value;
		 }
	 }*/

	public static T DeepCopy<T>(this T other)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(ms, other);
			ms.Position = 0;
			return (T)formatter.Deserialize(ms);
		}
	}

	public static float GetRandom(this Vector2 vector2) => UnityEngine.Random.Range(vector2.x, vector2.y);


	public static int[] GetSlots(int slots, int max)
	{
		return new System.Random().Values(1, max)
						   .Take(slots - 1)
						   .Append(0, max)
						   .OrderBy(i => i)
						   .Pairwise((x, y) => y - x)
						   .ToArray();
	}

	public static List<int> GetListSlots(int slots, int max)
	{
		return new System.Random().Values(1, max)
						   .Take(slots - 1)
						   .Append(0, max)
						   .OrderBy(i => i)
						   .Pairwise((x, y) => y - x)
						   .ToList();
	}


	public static IEnumerable<int> Values(this System.Random random, int minValue, int maxValue)
	{
		while (true)
			yield return random.Next(minValue, maxValue);
	}

	public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
	{
		TSource previous = default(TSource);

		using (var it = source.GetEnumerator())
		{
			if (it.MoveNext())
				previous = it.Current;

			while (it.MoveNext())
				yield return resultSelector(previous, previous = it.Current);
		}
	}

	public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] args)
	{
		return source.Concat(args);
	}

	// Shared array used to receive result of RectTransform.GetWorldCorners
	static Vector3[] corners = new Vector3[4];

	/// <summary>
	/// Transform the bounds of the current rect transform to the space of another transform.
	/// </summary>
	/// <param name="source">The rect to transform</param>
	/// <param name="target">The target space to transform to</param>
	/// <returns>The transformed bounds</returns>
	public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
	{
		// Based on code in ScrollRect's internal GetBounds and InternalGetBounds methods
		var bounds = new Bounds();
		if (source != null)
		{
			source.GetWorldCorners(corners);

			var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			var matrix = target.worldToLocalMatrix;
			for (int j = 0; j < 4; j++)
			{
				Vector3 v = matrix.MultiplyPoint3x4(corners[j]);
				vMin = Vector3.Min(v, vMin);
				vMax = Vector3.Max(v, vMax);
			}

			bounds = new Bounds(vMin, Vector3.zero);
			bounds.Encapsulate(vMax);
		}
		return bounds;
	}

	/// <summary>
	/// Normalize a distance to be used in verticalNormalizedPosition or horizontalNormalizedPosition.
	/// </summary>
	/// <param name="axis">Scroll axis, 0 = horizontal, 1 = vertical</param>
	/// <param name="distance">The distance in the scroll rect's view's coordiante space</param>
	/// <returns>The normalized scoll distance</returns>
	public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
	{
		// Based on code in ScrollRect's internal SetNormalizedPosition method
		var viewport = scrollRect.viewport;
		var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
		var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

		var content = scrollRect.content;
		var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

		var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
		return distance / hiddenLength;
	}

	/// <summary>
	/// Scroll the target element to the vertical center of the scroll rect's viewport.
	/// Assumes the target element is part of the scroll rect's contents.
	/// </summary>
	/// <param name="scrollRect">Scroll rect to scroll</param>
	/// <param name="target">Element of the scroll rect's content to center vertically</param>
	public static void ScrollToCeneter(this ScrollRect scrollRect, RectTransform target)
	{
		// The scroll rect's view's space is used to calculate scroll position
		var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

		// Calcualte the scroll offset in the view's space
		var viewRect = view.rect;
		var elementBounds = target.TransformBoundsTo(view);
		var offset = viewRect.center.y - elementBounds.center.y;

		// Normalize and apply the calculated offset
		var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);
		scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
	}

#if UNITY_EDITOR

	public enum LabelIcon
	{
		Gray = 0,
		Blue,
		Teal,
		Green,
		Yellow,
		Orange,
		Red,
		Purple
	}

	public enum Icon
	{
		CircleGray = 0,
		CircleBlue,
		CircleTeal,
		CircleGreen,
		CircleYellow,
		CircleOrange,
		CircleRed,
		CirclePurple,
		DiamondGray,
		DiamondBlue,
		DiamondTeal,
		DiamondGreen,
		DiamondYellow,
		DiamondOrange,
		DiamondRed,
		DiamondPurple
	}

	private static GUIContent[] labelIcons;
	private static GUIContent[] largeIcons;

	public static void SetIcon(GameObject gObj, LabelIcon icon)
	{

		var iconContent = EditorGUIUtility.IconContent("sv_label_" + (int)icon);
		EditorGUIUtility.SetIconForObject(gObj, (Texture2D)iconContent.image);

	}

	public static void SetIcon(GameObject gObj, Icon icon)
	{
		if (largeIcons == null)
		{
			largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
		}

		SetIcon(gObj, largeIcons[(int)icon].image as Texture2D);
	}

	private static void SetIcon(GameObject gObj, Texture2D texture)
	{
		var ty = typeof(EditorGUIUtility);
		var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
		mi.Invoke(null, new object[] { gObj, texture });
	}

	private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
	{
		GUIContent[] guiContentArray = new GUIContent[count];

		var t = typeof(EditorGUIUtility);
		var mi = t.GetMethod("IconContent", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, null);

		for (int index = 0; index < count; ++index)
		{
			guiContentArray[index] = mi.Invoke(null, new object[] { baseName + (object)(startIndex + index) + postFix }) as GUIContent;
		}

		return guiContentArray;
	}

	/// <summary>
	/// Draw 
	/// </summary>
	/// <param name="serializedObject"></param>
	/// <param name="isLable"></param>
	public static void DrawSerializedObject(this UnityEditor.SerializedObject serializedObject, bool isLable = false)
	{

		// Draw a background that shows us clearly which fields are part of the ScriptableObject
		serializedObject.Update();
		UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
		{
			UnityEditor.EditorGUI.indentLevel++;

			// Iterate over all the values and draw them
			UnityEditor.SerializedProperty prop = serializedObject.GetIterator();

			if (prop.NextVisible(true))
			{
				do
				{
					// Don't bother drawing the class file
					if (prop.name == "m_Script") continue;
					UnityEditor.EditorGUIUtility.labelWidth = 200;
					if (!isLable)
					{

						//var result = (string)proper.GetValue(proper, null);
						var isNull = prop is null;
						if (isNull)
						{
							GUI.color = Color.red;
							UnityEditor.EditorGUILayout.PropertyField(prop);

						}
						else
						{
							UnityEditor.EditorGUILayout.PropertyField(prop);
						}

					}
					else
					{
						UnityEditor.EditorGUI.BeginDisabledGroup(isLable);
						UnityEditor.EditorGUILayout.PropertyField(prop);
						UnityEditor.EditorGUI.EndDisabledGroup();
					}
				}
				while (prop.NextVisible(false));
			}

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
			{
				UnityEditor.AssetDatabase.SaveAssets();
			}


			UnityEditor.EditorGUI.indentLevel--;
		}
		UnityEditor.EditorGUILayout.EndVertical();
	}

	/// <summary>
	/// Extends how ScriptableObject object references are displayed in the inspector
	/// Shows you all values under the object reference
	/// Also provides a button to create a new ScriptableObject if property is null.
	/// </summary>
	[CustomPropertyDrawer(typeof(ScriptableObject), true)]
	public class ExtendedScriptableObjectDrawer : PropertyDrawer
	{

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float totalHeight = EditorGUIUtility.singleLineHeight;
			if (property.objectReferenceValue == null || !AreAnySubPropertiesVisible(property))
			{
				return totalHeight;
			}
			if (property.isExpanded)
			{
				var data = property.objectReferenceValue as ScriptableObject;
				if (data == null) return EditorGUIUtility.singleLineHeight;
				SerializedObject serializedObject = new SerializedObject(data);
				SerializedProperty prop = serializedObject.GetIterator();
				if (prop.NextVisible(true))
				{
					do
					{
						if (prop.name == "m_Script") continue;
						var subProp = serializedObject.FindProperty(prop.name);
						float height = EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
						totalHeight += height;
					}
					while (prop.NextVisible(false));
				}
				// Add a tiny bit of height if open for the background
				totalHeight += EditorGUIUtility.standardVerticalSpacing;
			}
			return totalHeight;
		}

		const int buttonWidth = 66;

		static readonly List<string> ignoreClassFullNames = new List<string> { "TMPro.TMP_FontAsset" };

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var type = GetFieldType();

			if (type == null || ignoreClassFullNames.Contains(type.FullName))
			{
				EditorGUI.PropertyField(position, property, label);
				EditorGUI.EndProperty();
				return;
			}

			ScriptableObject propertySO = null;
			if (!property.hasMultipleDifferentValues && property.serializedObject.targetObject != null && property.serializedObject.targetObject is ScriptableObject)
			{
				propertySO = (ScriptableObject)property.serializedObject.targetObject;
			}

			var propertyRect = Rect.zero;
			var guiContent = new GUIContent(property.displayName);
			var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
			if (property.objectReferenceValue != null && AreAnySubPropertiesVisible(property))
			{
				property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true);
			}
			else
			{
				// So yeah having a foldout look like a label is a weird hack 
				// but both code paths seem to need to be a foldout or 
				// the object field control goes weird when the codepath changes.
				// I guess because foldout is an interactable control of its own and throws off the controlID?
				foldoutRect.x += 12;
				EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true, EditorStyles.label);
			}
			var indentedPosition = EditorGUI.IndentedRect(position);
			var indentOffset = indentedPosition.x - position.x;
			propertyRect = new Rect(position.x + (EditorGUIUtility.labelWidth - indentOffset), position.y, position.width - (EditorGUIUtility.labelWidth - indentOffset), EditorGUIUtility.singleLineHeight);

			if (propertySO != null || property.objectReferenceValue == null)
			{
				propertyRect.width -= buttonWidth;
			}

			EditorGUI.ObjectField(propertyRect, property, type, GUIContent.none);
			if (GUI.changed) property.serializedObject.ApplyModifiedProperties();

			var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

			if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
			{
				var data = (ScriptableObject)property.objectReferenceValue;

				if (property.isExpanded)
				{
					// Draw a background that shows us clearly which fields are part of the ScriptableObject
					GUI.Box(new Rect(0, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing - 1, Screen.width, position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing), "");

					EditorGUI.indentLevel++;
					SerializedObject serializedObject = new SerializedObject(data);

					// Iterate over all the values and draw them
					SerializedProperty prop = serializedObject.GetIterator();
					float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					if (prop.NextVisible(true))
					{
						do
						{
							// Don't bother drawing the class file
							if (prop.name == "m_Script") continue;
							float height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
							EditorGUI.PropertyField(new Rect(position.x, y, position.width - buttonWidth, height), prop, true);
							y += height + EditorGUIUtility.standardVerticalSpacing;
						}
						while (prop.NextVisible(false));
					}
					if (GUI.changed)
						serializedObject.ApplyModifiedProperties();

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				if (GUI.Button(buttonRect, "Create"))
				{
					string selectedAssetPath = "Assets";
					if (property.serializedObject.targetObject is MonoBehaviour)
					{
						MonoScript ms = MonoScript.FromMonoBehaviour((MonoBehaviour)property.serializedObject.targetObject);
						selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
					}

					property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
				}
			}
			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}

		public static T _GUILayout<T>(string label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject
		{
			return _GUILayout<T>(new GUIContent(label), objectReferenceValue, ref isExpanded);
		}

		public static T _GUILayout<T>(GUIContent label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject
		{
			Rect position = EditorGUILayout.BeginVertical();

			var propertyRect = Rect.zero;
			var guiContent = label;
			var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
			if (objectReferenceValue != null)
			{
				isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true);

				var indentedPosition = EditorGUI.IndentedRect(position);
				var indentOffset = indentedPosition.x - position.x;
				propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset, EditorGUIUtility.singleLineHeight);
			}
			else
			{
				// So yeah having a foldout look like a label is a weird hack 
				// but both code paths seem to need to be a foldout or 
				// the object field control goes weird when the codepath changes.
				// I guess because foldout is an interactable control of its own and throws off the controlID?
				foldoutRect.x += 12;
				EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true, EditorStyles.label);

				var indentedPosition = EditorGUI.IndentedRect(position);
				var indentOffset = indentedPosition.x - position.x;
				propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset - 60, EditorGUIUtility.singleLineHeight);
			}

			EditorGUILayout.BeginHorizontal();
			objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" "), objectReferenceValue, typeof(T), false) as T;

			if (objectReferenceValue != null)
			{

				EditorGUILayout.EndHorizontal();
				if (isExpanded)
				{
					DrawScriptableObjectChildFields(objectReferenceValue);
				}
			}
			else
			{
				if (GUILayout.Button("Create", GUILayout.Width(buttonWidth)))
				{
					string selectedAssetPath = "Assets";
					var newAsset = CreateAssetWithSavePrompt(typeof(T), selectedAssetPath);
					if (newAsset != null)
					{
						objectReferenceValue = (T)newAsset;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			return objectReferenceValue;
		}

		static void DrawScriptableObjectChildFields<T>(T objectReferenceValue) where T : ScriptableObject
		{
			// Draw a background that shows us clearly which fields are part of the ScriptableObject
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginVertical(GUI.skin.box);

			var serializedObject = new SerializedObject(objectReferenceValue);
			// Iterate over all the values and draw them
			SerializedProperty prop = serializedObject.GetIterator();
			if (prop.NextVisible(true))
			{
				do
				{
					// Don't bother drawing the class file
					if (prop.name == "m_Script") continue;
					EditorGUILayout.PropertyField(prop, true);
				}
				while (prop.NextVisible(false));
			}
			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;
		}

		public static T DrawScriptableObjectField<T>(GUIContent label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject
		{
			Rect position = EditorGUILayout.BeginVertical();

			var propertyRect = Rect.zero;
			var guiContent = label;
			var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
			if (objectReferenceValue != null)
			{
				isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true);

				var indentedPosition = EditorGUI.IndentedRect(position);
				var indentOffset = indentedPosition.x - position.x;
				propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset, EditorGUIUtility.singleLineHeight);
			}
			else
			{
				// So yeah having a foldout look like a label is a weird hack 
				// but both code paths seem to need to be a foldout or 
				// the object field control goes weird when the codepath changes.
				// I guess because foldout is an interactable control of its own and throws off the controlID?
				foldoutRect.x += 12;
				EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true, EditorStyles.label);

				var indentedPosition = EditorGUI.IndentedRect(position);
				var indentOffset = indentedPosition.x - position.x;
				propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset - 60, EditorGUIUtility.singleLineHeight);
			}

			EditorGUILayout.BeginHorizontal();
			objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" "), objectReferenceValue, typeof(T), false) as T;

			if (objectReferenceValue != null)
			{
				EditorGUILayout.EndHorizontal();
				if (isExpanded)
				{

				}
			}
			else
			{
				if (GUILayout.Button("Create", GUILayout.Width(buttonWidth)))
				{
					string selectedAssetPath = "Assets";
					var newAsset = CreateAssetWithSavePrompt(typeof(T), selectedAssetPath);
					if (newAsset != null)
					{
						objectReferenceValue = (T)newAsset;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			return objectReferenceValue;
		}

		// Creates a new ScriptableObject via the default Save File panel
		static ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
		{
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
			if (path == "") return null;
			ScriptableObject asset = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);
			return asset;
		}

		Type GetFieldType()
		{
			Type type = fieldInfo.FieldType;
			if (type.IsArray) type = type.GetElementType();
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
			return type;
		}

		static bool AreAnySubPropertiesVisible(SerializedProperty property)
		{
			var data = (ScriptableObject)property.objectReferenceValue;
			SerializedObject serializedObject = new SerializedObject(data);
			SerializedProperty prop = serializedObject.GetIterator();
			while (prop.NextVisible(true))
			{
				if (prop.name == "m_Script") continue;
				return true; //if theres any visible property other than m_script
			}
			return false;
		}
	}


	private static readonly Vector3 Vector3zero = Vector3.zero;
	private static readonly Vector3 Vector3one = Vector3.one;
	private static readonly Vector3 Vector3yDown = new Vector3(0, -1);

	public const int sortingOrderDefault = 5000;

	// Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
	public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
	{
		return (int)(baseSortingOrder - position.y) + offset;
	}


	// Get Main Canvas Transform
	private static Transform cachedCanvasTransform;
	public static Transform GetCanvasTransform()
	{
		if (cachedCanvasTransform == null)
		{
			Canvas canvas = MonoBehaviour.FindObjectOfType<Canvas>();
			if (canvas != null)
			{
				cachedCanvasTransform = canvas.transform;
			}
		}
		return cachedCanvasTransform;
	}

	// Get Default Unity Font, used in text objects if no font given
	public static Font GetDefaultFont()
	{
		return Resources.GetBuiltinResource<Font>("Arial.ttf");
	}


	// Create a Sprite in the World, no parent
	public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
	{
		return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
	}

	// Create a Sprite in the World
	public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
	{
		GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
		Transform transform = gameObject.transform;
		transform.SetParent(parent, false);
		transform.localPosition = localPosition;
		transform.localScale = localScale;
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		spriteRenderer.sortingOrder = sortingOrder;
		spriteRenderer.color = color;
		return gameObject;
	}

	/* // Create a Sprite in the World with Button_Sprite, no parent
	 public static Button_Sprite CreateWorldSpriteButton(string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
	 {
		 return CreateWorldSpriteButton(null, name, sprite, localPosition, localScale, sortingOrder, color);
	 }

	 // Create a Sprite in the World with Button_Sprite
	 public static Button_Sprite CreateWorldSpriteButton(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
	 {
		 GameObject gameObject = CreateWorldSprite(parent, name, sprite, localPosition, localScale, sortingOrder, color);
		 gameObject.AddComponent<BoxCollider2D>();
		 Button_Sprite buttonSprite = gameObject.AddComponent<Button_Sprite>();
		 return buttonSprite;
	 }
 */
	// Creates a Text Mesh in the World and constantly updates it
	public static GkUpdate CreateWorldTextUpdater(Func<string> GetTextFunc, Vector3 localPosition, Transform parent = null, int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
	{
		TextMesh textMesh = CreateWorldText(GetTextFunc(), parent, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder);
		return GkUpdate.Create(() =>
		{
			textMesh.text = GetTextFunc();
			return false;
		}, "WorldTextUpdater");
	}

	// Create Text in the World
	public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
	{
		if (color == null) color = Color.white;
		return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
	}

	// Create Text in the World
	public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
	{
		GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
		Transform transform = gameObject.transform;
		transform.SetParent(parent, false);
		transform.localPosition = localPosition;
		TextMesh textMesh = gameObject.GetComponent<TextMesh>();
		textMesh.anchor = textAnchor;
		textMesh.alignment = textAlignment;
		textMesh.text = text;
		textMesh.fontSize = fontSize;
		textMesh.color = color;
		textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
		return textMesh;
	}


	// Create a Text Popup in the World, no parent
	public static void CreateWorldTextPopup(string text, Vector3 localPosition, float popupTime = 1f)
	{
		CreateWorldTextPopup(null, text, localPosition, 40, Color.white, localPosition + new Vector3(0, 20), popupTime);
	}

	// Create a Text Popup in the World
	public static void CreateWorldTextPopup(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector3 finalPopupPosition, float popupTime)
	{
		TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, color, TextAnchor.LowerLeft, TextAlignment.Left, sortingOrderDefault);
		Transform transform = textMesh.transform;
		Vector3 moveAmount = (finalPopupPosition - localPosition) / popupTime;
		GkUpdate.Create(delegate ()
		{
			transform.position += moveAmount * Time.unscaledDeltaTime;
			popupTime -= Time.unscaledDeltaTime;
			if (popupTime <= 0f)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}, "WorldTextPopup");
	}

	// Create Text Updater in UI
	public static GkUpdate CreateUITextUpdater(Func<string> GetTextFunc, Vector2 anchoredPosition)
	{
		Text text = DrawTextUI(GetTextFunc(), anchoredPosition, 20, GetDefaultFont());
		return GkUpdate.Create(() =>
		{
			text.text = GetTextFunc();
			return false;
		}, "UITextUpdater");
	}


	// Draw a UI Sprite
	public static RectTransform DrawSprite(Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
	{
		RectTransform rectTransform = DrawSprite(null, color, parent, pos, size, name);
		return rectTransform;
	}

	// Draw a UI Sprite
	public static RectTransform DrawSprite(Sprite sprite, Transform parent, Vector2 pos, Vector2 size, string name = null)
	{
		RectTransform rectTransform = DrawSprite(sprite, Color.white, parent, pos, size, name);
		return rectTransform;
	}

	// Draw a UI Sprite
	public static RectTransform DrawSprite(Sprite sprite, Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
	{
		// Setup icon
		if (name == null || name == "") name = "Sprite";
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
		RectTransform goRectTransform = go.GetComponent<RectTransform>();
		goRectTransform.SetParent(parent, false);
		goRectTransform.sizeDelta = size;
		goRectTransform.anchoredPosition = pos;

		Image image = go.GetComponent<Image>();
		image.sprite = sprite;
		image.color = color;

		return goRectTransform;
	}

	public static Text DrawTextUI(string textString, Vector2 anchoredPosition, int fontSize, Font font)
	{
		return DrawTextUI(textString, GetCanvasTransform(), anchoredPosition, fontSize, font);
	}

	public static Text DrawTextUI(string textString, Transform parent, Vector2 anchoredPosition, int fontSize, Font font)
	{
		GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
		textGo.transform.SetParent(parent, false);
		Transform textGoTrans = textGo.transform;
		textGoTrans.SetParent(parent, false);
		textGoTrans.localPosition = Vector3zero;
		textGoTrans.localScale = Vector3one;

		RectTransform textGoRectTransform = textGo.GetComponent<RectTransform>();
		textGoRectTransform.sizeDelta = new Vector2(0, 0);
		textGoRectTransform.anchoredPosition = anchoredPosition;

		Text text = textGo.GetComponent<Text>();
		text.text = textString;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.alignment = TextAnchor.MiddleLeft;
		if (font == null) font = GetDefaultFont();
		text.font = font;
		text.fontSize = fontSize;

		return text;
	}


	// Parse a float, return default if failed
	public static float Parse_Float(string txt, float _default)
	{
		float f;
		if (!float.TryParse(txt, out f))
		{
			f = _default;
		}
		return f;
	}

	// Parse a int, return default if failed
	public static int Parse_Int(string txt, int _default)
	{
		int i;
		if (!int.TryParse(txt, out i))
		{
			i = _default;
		}
		return i;
	}

	public static int Parse_Int(string txt)
	{
		return Parse_Int(txt, -1);
	}



	// Get Mouse Position in World with Z = 0f
	public static Vector3 GetMouseWorldPosition()
	{
		Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
		vec.z = 0f;
		return vec;
	}

	public static Vector3 GetMouseWorldPositionWithZ()
	{
		return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
	}

	public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
	{
		return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
	}

	public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
	{
		Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
		return worldPosition;
	}

	public static Vector3 GetDirToMouse(Vector3 fromPosition)
	{
		Vector3 mouseWorldPosition = GetMouseWorldPosition();
		return (mouseWorldPosition - fromPosition).normalized;
	}



	// Is Mouse over a UI Element? Used for ignoring World clicks through UI
	public static bool IsPointerOverUI()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return true;
		}
		else
		{
			PointerEventData pe = new PointerEventData(EventSystem.current);
			pe.position = Input.mousePosition;
			List<RaycastResult> hits = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pe, hits);
			return hits.Count > 0;
		}
	}



	// Returns 00-FF, value 0->255
	public static string Dec_to_Hex(int value)
	{
		return value.ToString("X2");
	}

	// Returns 0-255
	public static int Hex_to_Dec(string hex)
	{
		return Convert.ToInt32(hex, 16);
	}

	// Returns a hex string based on a number between 0->1
	public static string Dec01_to_Hex(float value)
	{
		return Dec_to_Hex((int)Mathf.Round(value * 255f));
	}

	// Returns a float between 0->1
	public static float Hex_to_Dec01(string hex)
	{
		return Hex_to_Dec(hex) / 255f;
	}

	// Get Hex Color FF00FF
	public static string GetStringFromColor(Color color)
	{
		string red = Dec01_to_Hex(color.r);
		string green = Dec01_to_Hex(color.g);
		string blue = Dec01_to_Hex(color.b);
		return red + green + blue;
	}

	// Get Hex Color FF00FFAA
	public static string GetStringFromColorWithAlpha(Color color)
	{
		string alpha = Dec01_to_Hex(color.a);
		return GetStringFromColor(color) + alpha;
	}

	// Sets out values to Hex String 'FF'
	public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
	{
		red = Dec01_to_Hex(color.r);
		green = Dec01_to_Hex(color.g);
		blue = Dec01_to_Hex(color.b);
		alpha = Dec01_to_Hex(color.a);
	}

	// Get Hex Color FF00FF
	public static string GetStringFromColor(float r, float g, float b)
	{
		string red = Dec01_to_Hex(r);
		string green = Dec01_to_Hex(g);
		string blue = Dec01_to_Hex(b);
		return red + green + blue;
	}

	// Get Hex Color FF00FFAA
	public static string GetStringFromColor(float r, float g, float b, float a)
	{
		string alpha = Dec01_to_Hex(a);
		return GetStringFromColor(r, g, b) + alpha;
	}

	// Get Color from Hex string FF00FFAA
	public static Color GetColorFromString(string color)
	{
		float red = Hex_to_Dec01(color.Substring(0, 2));
		float green = Hex_to_Dec01(color.Substring(2, 2));
		float blue = Hex_to_Dec01(color.Substring(4, 2));
		float alpha = 1f;
		if (color.Length >= 8)
		{
			// Color string contains alpha
			alpha = Hex_to_Dec01(color.Substring(6, 2));
		}
		return new Color(red, green, blue, alpha);
	}

	// Return a color going from Red to Yellow to Green, like a heat map
	public static Color GetRedGreenColor(float value)
	{
		float r = 0f;
		float g = 0f;
		if (value <= .5f)
		{
			r = 1f;
			g = value * 2f;
		}
		else
		{
			g = 1f;
			r = 1f - (value - .5f) * 2f;
		}
		return new Color(r, g, 0f, 1f);
	}


	public static Color GetRandomColor()
	{
		return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
	}

	private static int sequencialColorIndex = -1;
	private static Color[] sequencialColors = new[] {
			GetColorFromString("26a6d5"),
			GetColorFromString("41d344"),
			GetColorFromString("e6e843"),
			GetColorFromString("e89543"),
			GetColorFromString("0f6ad0"),//("d34141"),
			GetColorFromString("b35db6"),
			GetColorFromString("c45947"),
			GetColorFromString("9447c4"),
			GetColorFromString("4756c4"),
		};

	public static void ResetSequencialColors()
	{
		sequencialColorIndex = -1;
	}

	public static Color GetSequencialColor()
	{
		sequencialColorIndex = (sequencialColorIndex + 1) % sequencialColors.Length;
		return sequencialColors[sequencialColorIndex];
	}

	public static Color GetColor255(float red, float green, float blue, float alpha = 255f)
	{
		return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
	}


	// Generate random normalized direction
	public static Vector3 GetRandomDir()
	{
		return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
	}

	// Generate random normalized direction
	public static Vector3 GetRandomDirXZ()
	{
		return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
	}


	public static Vector3 GetVectorFromAngle(int angle)
	{
		// angle = 0 -> 360
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	public static Vector3 GetVectorFromAngle(float angle)
	{
		// angle = 0 -> 360
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	public static Vector3 GetVectorFromAngleInt(int angle)
	{
		// angle = 0 -> 360
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	public static float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}

	public static float GetAngleFromVectorFloatXZ(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}

	public static int GetAngleFromVector(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;
		int angle = Mathf.RoundToInt(n);

		return angle;
	}

	public static int GetAngleFromVector180(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		int angle = Mathf.RoundToInt(n);

		return angle;
	}

	public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
	{
		return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
	}

	public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
	{
		return Quaternion.Euler(0, 0, angle) * vec;
	}

	public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
	{
		return Quaternion.Euler(0, angle, 0) * vec;
	}



	public static GkUpdate CreateMouseDraggingAction(Action<Vector3> onMouseDragging)
	{
		return CreateMouseDraggingAction(0, onMouseDragging);
	}

	public static GkUpdate CreateMouseDraggingAction(int mouseButton, Action<Vector3> onMouseDragging)
	{
		bool dragging = false;
		return GkUpdate.Create(() =>
		{
			if (Input.GetMouseButtonDown(mouseButton))
			{
				dragging = true;
			}
			if (Input.GetMouseButtonUp(mouseButton))
			{
				dragging = false;
			}
			if (dragging)
			{
				onMouseDragging(GetMouseWorldPosition());
			}
			return false;
		});
	}

	public static GkUpdate CreateMouseClickFromToAction(Action<Vector3, Vector3> onMouseClickFromTo, Action<Vector3, Vector3> onWaitingForToPosition)
	{
		return CreateMouseClickFromToAction(0, 1, onMouseClickFromTo, onWaitingForToPosition);
	}

	public static GkUpdate CreateMouseClickFromToAction(int mouseButton, int cancelMouseButton, Action<Vector3, Vector3> onMouseClickFromTo, Action<Vector3, Vector3> onWaitingForToPosition)
	{
		int state = 0;
		Vector3 from = Vector3.zero;
		return GkUpdate.Create(() =>
		{
			if (state == 1)
			{
				if (onWaitingForToPosition != null) onWaitingForToPosition(from, GetMouseWorldPosition());
			}
			if (state == 1 && Input.GetMouseButtonDown(cancelMouseButton))
			{
				// Cancel
				state = 0;
			}
			if (Input.GetMouseButtonDown(mouseButton) && !IsPointerOverUI())
			{
				if (state == 0)
				{
					state = 1;
					from = GetMouseWorldPosition();
				}
				else
				{
					state = 0;
					onMouseClickFromTo(from, GetMouseWorldPosition());
				}
			}
			return false;
		});
	}

	public static GkUpdate CreateMouseClickAction(Action<Vector3> onMouseClick)
	{
		return CreateMouseClickAction(0, onMouseClick);
	}

	public static GkUpdate CreateMouseClickAction(int mouseButton, Action<Vector3> onMouseClick)
	{
		return GkUpdate.Create(() =>
		{
			if (Input.GetMouseButtonDown(mouseButton))
			{
				onMouseClick(GetWorldPositionFromUI());
			}
			return false;
		});
	}

	public static GkUpdate CreateKeyCodeAction(KeyCode keyCode, Action onKeyDown)
	{
		return GkUpdate.Create(() =>
		{
			if (Input.GetKeyDown(keyCode))
			{
				onKeyDown();
			}
			return false;
		});
	}



	// Get UI Position from World Position
	public static Vector2 GetWorldUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera)
	{
		Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
		Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
		Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
		return new Vector2(localPos.x, localPos.y);
	}

	public static Vector3 GetWorldPositionFromUIZeroZ()
	{
		Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
		vec.z = 0f;
		return vec;
	}

	// Get World Position from UI Position
	public static Vector3 GetWorldPositionFromUI()
	{
		return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
	}

	public static Vector3 GetWorldPositionFromUI(Camera worldCamera)
	{
		return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
	}

	public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera)
	{
		Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
		return worldPosition;
	}

	public static Vector3 GetWorldPositionFromUI_Perspective()
	{
		return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
	}

	public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera)
	{
		return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
	}

	public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera)
	{
		Ray ray = worldCamera.ScreenPointToRay(screenPosition);
		Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
		float distance;
		xy.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}


	// Screen Shake
	public static void ShakeCamera(float intensity, float timer)
	{
		Vector3 lastCameraMovement = Vector3.zero;
		GkUpdate.Create(delegate ()
		{
			timer -= Time.unscaledDeltaTime;
			Vector3 randomMovement = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * intensity;
			Camera.main.transform.position = Camera.main.transform.position - lastCameraMovement + randomMovement;
			lastCameraMovement = randomMovement;
			return timer <= 0f;
		}, "CAMERA_SHAKE");
	}


	// Trigger an action next frame
	public static GkUpdate ActionNextFrame(Action action)
	{
		return GkUpdate.Create(() =>
		{
			action();
			return true;
		});
	}

	// Return random element from array
	public static T GetRandom<T>(T[] array)
	{
		return array[UnityEngine.Random.Range(0, array.Length)];
	}


	// Return a number with milli dots, 1000000 -> 1.000.000
	public static string GetMilliDots(float n)
	{
		return GetMilliDots((long)n);
	}

	public static string GetMilliDots(long n)
	{
		string ret = n.ToString();
		for (int i = 1; i <= Mathf.Floor(ret.Length / 4); i++)
		{
			ret = ret.Substring(0, ret.Length - i * 3 - (i - 1)) + "." + ret.Substring(ret.Length - i * 3 - (i - 1));
		}
		return ret;
	}


	// Return with milli dots and dollar sign
	public static string GetDollars(float n)
	{
		return GetDollars((long)n);
	}
	public static string GetDollars(long n)
	{
		if (n < 0)
			return "-$" + GetMilliDots(Mathf.Abs(n));
		else
			return "$" + GetMilliDots(n);
	}



	[System.Serializable]
	private class JsonDictionary
	{
		public List<string> keyList = new List<string>();
		public List<string> valueList = new List<string>();
	}

	// Take a Dictionary and return JSON string
	public static string SaveDictionaryJson<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
	{
		JsonDictionary jsonDictionary = new JsonDictionary();
		foreach (TKey key in dictionary.Keys)
		{
			jsonDictionary.keyList.Add(JsonUtility.ToJson(key));
			jsonDictionary.valueList.Add(JsonUtility.ToJson(dictionary[key]));
		}
		string saveJson = JsonUtility.ToJson(jsonDictionary);
		return saveJson;
	}

	// Take a JSON string and return Dictionary<T1, T2>
	public static Dictionary<TKey, TValue> LoadDictionaryJson<TKey, TValue>(string saveJson)
	{
		JsonDictionary jsonDictionary = JsonUtility.FromJson<JsonDictionary>(saveJson);
		Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
		for (int i = 0; i < jsonDictionary.keyList.Count; i++)
		{
			TKey key = JsonUtility.FromJson<TKey>(jsonDictionary.keyList[i]);
			TValue value = JsonUtility.FromJson<TValue>(jsonDictionary.valueList[i]);
			ret[key] = value;
		}
		return ret;
	}


	// Split a string into an array based on a Separator
	public static string[] SplitString(string save, string separator)
	{
		return save.Split(new string[] { separator }, System.StringSplitOptions.None);
	}


	// Destroy all children of this parent
	public static void DestroyChildren(Transform parent)
	{
		foreach (Transform transform in parent)
			GameObject.Destroy(transform.gameObject);
	}

	// Destroy all children and randomize their names, useful if you want to do a Find() after calling destroy, since they only really get destroyed at the end of the frame
	public static void DestroyChildrenRandomizeNames(Transform parent)
	{
		foreach (Transform transform in parent)
		{
			transform.name = "" + UnityEngine.Random.Range(10000, 99999);
			GameObject.Destroy(transform.gameObject);
		}
	}

	// Destroy all children except the ones with these names
	public static void DestroyChildren(Transform parent, params string[] ignoreArr)
	{
		foreach (Transform transform in parent)
		{
			if (System.Array.IndexOf(ignoreArr, transform.name) == -1) // Don't ignore
				GameObject.Destroy(transform.gameObject);
		}
	}


	// Set all parent and all children to this layer
	public static void SetAllChildrenLayer(Transform parent, int layer)
	{
		parent.gameObject.layer = layer;
		foreach (Transform trans in parent)
		{
			SetAllChildrenLayer(trans, layer);
		}
	}



	// Returns a random script that can be used to id
	public static string GetIdString()
	{
		string alphabet = "0123456789abcdefghijklmnopqrstuvxywz";
		string ret = "";
		for (int i = 0; i < 8; i++)
		{
			ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
		}
		return ret;
	}

	// Returns a random script that can be used to id (bigger alphabet)
	public static string GetIdStringLong()
	{
		return GetIdStringLong(10);
	}

	// Returns a random script that can be used to id (bigger alphabet)
	public static string GetIdStringLong(int chars)
	{
		string alphabet = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
		string ret = "";
		for (int i = 0; i < chars; i++)
		{
			ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
		}
		return ret;
	}



	// Get a random male name and optionally single letter surname
	public static string GetRandomName(bool withSurname = false)
	{
		List<string> firstNameList = new List<string>(){"Gabe","Cliff","Tim","Ron","Jon","John","Mike","Seth","Alex","Steve","Chris","Will","Bill","James","Jim",
										"Ahmed","Omar","Peter","Pierre","George","Lewis","Lewie","Adam","William","Ali","Eddie","Ed","Dick","Robert","Bob","Rob",
										"Neil","Tyson","Carl","Chris","Christopher","Jensen","Gordon","Morgan","Richard","Wen","Wei","Luke","Lucas","Noah","Ivan","Yusuf",
										"Ezio","Connor","Milan","Nathan","Victor","Harry","Ben","Charles","Charlie","Jack","Leo","Leonardo","Dylan","Steven","Jeff",
										"Alex","Mark","Leon","Oliver","Danny","Liam","Joe","Tom","Thomas","Bruce","Clark","Tyler","Jared","Brad","Jason"};

		if (!withSurname)
		{
			return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)];
		}
		else
		{
			string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
			return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)] + " " + alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
		}
	}




	public static string GetRandomCityName()
	{
		List<string> cityNameList = new List<string>(){"Alabama","New York","Old York","Bangkok","Lisbon","Vee","Agen","Agon","Ardok","Arbok",
							"Kobra","House","Noun","Hayar","Salma","Chancellor","Dascomb","Payn","Inglo","Lorr","Ringu",
							"Brot","Mount Loom","Kip","Chicago","Madrid","London","Gam",
							"Greenvile","Franklin","Clinton","Springfield","Salem","Fairview","Fairfax","Washington","Madison",
							"Georgetown","Arlington","Marion","Oxford","Harvard","Valley","Ashland","Burlington","Manchester","Clayton",
							"Milton","Auburn","Dayton","Lexington","Milford","Riverside","Cleveland","Dover","Hudson","Kingston","Mount Vernon",
							"Newport","Oakland","Centerville","Winchester","Rotary","Bailey","Saint Mary","Three Waters","Veritas","Chaos","Center",
							"Millbury","Stockland","Deerstead Hills","Plaintown","Fairchester","Milaire View","Bradton","Glenfield","Kirkmore",
							"Fortdell","Sharonford","Inglewood","Englecamp","Harrisvania","Bosstead","Brookopolis","Metropolis","Colewood","Willowbury",
							"Hearthdale","Weelworth","Donnelsfield","Greenline","Greenwich","Clarkswich","Bridgeworth","Normont",
							"Lynchbrook","Ashbridge","Garfort","Wolfpain","Waterstead","Glenburgh","Fortcroft","Kingsbank","Adamstead","Mistead",
							"Old Crossing","Crossing","New Agon","New Agen","Old Agon","New Valley","Old Valley","New Kingsbank","Old Kingsbank",
			"New Dover","Old Dover","New Burlington","Shawshank","Old Shawshank","New Shawshank","New Bradton", "Old Bradton","New Metropolis","Old Clayton","New Clayton"
		};
		return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
	}



	// Is this position inside the FOV? Top Down Perspective
	public static bool IsPositionInsideFov(Vector3 pos, Vector3 aimDir, Vector3 posTarget, float fov)
	{
		int aimAngle = GetAngleFromVector180(aimDir);
		int angle = GetAngleFromVector180(posTarget - pos);
		int angleDifference = (angle - aimAngle);
		if (angleDifference > 180) angleDifference -= 360;
		if (angleDifference < -180) angleDifference += 360;
		if (!(angleDifference < fov / 2f && angleDifference > -fov / 2f))
		{
			// Not inside fov
			return false;
		}
		else
		{
			// Inside fov
			return true;
		}
	}

	// Take two color arrays (pixels) and merge them
	public static void MergeColorArrays(Color[] baseArray, Color[] overlay)
	{
		for (int i = 0; i < baseArray.Length; i++)
		{
			if (overlay[i].a > 0)
			{
				// Not empty color
				if (overlay[i].a >= 1)
				{
					// Fully replace
					baseArray[i] = overlay[i];
				}
				else
				{
					// Interpolate colors
					float alpha = overlay[i].a;
					baseArray[i].r += (overlay[i].r - baseArray[i].r) * alpha;
					baseArray[i].g += (overlay[i].g - baseArray[i].g) * alpha;
					baseArray[i].b += (overlay[i].b - baseArray[i].b) * alpha;
					baseArray[i].a += overlay[i].a;
				}
			}
		}
	}

	// Replace color in baseArray with replaceArray if baseArray[i] != ignoreColor
	public static void ReplaceColorArrays(Color[] baseArray, Color[] replaceArray, Color ignoreColor)
	{
		for (int i = 0; i < baseArray.Length; i++)
		{
			if (baseArray[i] != ignoreColor)
			{
				baseArray[i] = replaceArray[i];
			}
		}
	}

	public static void MaskColorArrays(Color[] baseArray, Color[] mask)
	{
		for (int i = 0; i < baseArray.Length; i++)
		{
			if (baseArray[i].a > 0f)
			{
				baseArray[i].a = mask[i].a;
			}
		}
	}

	public static void TintColorArray(Color[] baseArray, Color tint)
	{
		for (int i = 0; i < baseArray.Length; i++)
		{
			// Apply tint
			baseArray[i].r = tint.r * baseArray[i].r;
			baseArray[i].g = tint.g * baseArray[i].g;
			baseArray[i].b = tint.b * baseArray[i].b;
		}
	}
	public static void TintColorArrayInsideMask(Color[] baseArray, Color tint, Color[] mask)
	{
		for (int i = 0; i < baseArray.Length; i++)
		{
			if (mask[i].a > 0)
			{
				// Apply tint
				Color baseColor = baseArray[i];
				Color fullyTintedColor = tint * baseColor;
				float interpolateAmount = mask[i].a;
				baseArray[i].r = baseColor.r + (fullyTintedColor.r - baseColor.r) * interpolateAmount;
				baseArray[i].g = baseColor.g + (fullyTintedColor.g - baseColor.g) * interpolateAmount;
				baseArray[i].b = baseColor.b + (fullyTintedColor.b - baseColor.b) * interpolateAmount;
			}
		}
	}

	public static Color TintColor(Color baseColor, Color tint)
	{
		// Apply tint
		baseColor.r = tint.r * baseColor.r;
		baseColor.g = tint.g * baseColor.g;
		baseColor.b = tint.b * baseColor.b;

		return baseColor;
	}

	public static bool IsColorSimilar255(Color colorA, Color colorB, int maxDiff)
	{
		return IsColorSimilar(colorA, colorB, maxDiff / 255f);
	}

	public static bool IsColorSimilar(Color colorA, Color colorB, float maxDiff)
	{
		float rDiff = Mathf.Abs(colorA.r - colorB.r);
		float gDiff = Mathf.Abs(colorA.g - colorB.g);
		float bDiff = Mathf.Abs(colorA.b - colorB.b);
		float aDiff = Mathf.Abs(colorA.a - colorB.a);

		float totalDiff = rDiff + gDiff + bDiff + aDiff;
		return totalDiff < maxDiff;
	}

	public static float GetColorDifference(Color colorA, Color colorB)
	{
		float rDiff = Mathf.Abs(colorA.r - colorB.r);
		float gDiff = Mathf.Abs(colorA.g - colorB.g);
		float bDiff = Mathf.Abs(colorA.b - colorB.b);
		float aDiff = Mathf.Abs(colorA.a - colorB.a);

		float totalDiff = rDiff + gDiff + bDiff + aDiff;
		return totalDiff;
	}



	public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax)
	{
		return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
	}

	public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight)
	{
		return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
	}





	public static string GetTimeHMS(float time, bool hours = true, bool minutes = true, bool seconds = true, bool milliseconds = true)
	{
		string h0, h1, m0, m1, s0, s1, ms0, ms1, ms2;
		GetTimeCharacterStrings(time, out h0, out h1, out m0, out m1, out s0, out s1, out ms0, out ms1, out ms2);
		string h = h0 + h1;
		string m = m0 + m1;
		string s = s0 + s1;
		string ms = ms0 + ms1 + ms2;

		if (hours)
		{
			if (minutes)
			{
				if (seconds)
				{
					if (milliseconds)
					{
						return h + ":" + m + ":" + s + "." + ms;
					}
					else
					{
						return h + ":" + m + ":" + s;
					}
				}
				else
				{
					return h + ":" + m;
				}
			}
			else
			{
				return h;
			}
		}
		else
		{
			if (minutes)
			{
				if (seconds)
				{
					if (milliseconds)
					{
						return m + ":" + s + "." + ms;
					}
					else
					{
						return m + ":" + s;
					}
				}
				else
				{
					return m;
				}
			}
			else
			{
				if (seconds)
				{
					if (milliseconds)
					{
						return s + "." + ms;
					}
					else
					{
						return s;
					}
				}
				else
				{
					return ms;
				}
			}
		}
	}

	public static void SetupTimeHMSTransform(Transform transform, float time)
	{
		string h0, h1, m0, m1, s0, s1, ms0, ms1, ms2;
		GetTimeCharacterStrings(time, out h0, out h1, out m0, out m1, out s0, out s1, out ms0, out ms1, out ms2);

		if (transform.Find("h0") != null && transform.Find("h0").GetComponent<Text>() != null)
			transform.Find("h0").GetComponent<Text>().text = h0;
		if (transform.Find("h1") != null && transform.Find("h1").GetComponent<Text>() != null)
			transform.Find("h1").GetComponent<Text>().text = h1;

		if (transform.Find("m0") != null && transform.Find("m0").GetComponent<Text>() != null)
			transform.Find("m0").GetComponent<Text>().text = m0;
		if (transform.Find("m1") != null && transform.Find("m1").GetComponent<Text>() != null)
			transform.Find("m1").GetComponent<Text>().text = m1;

		if (transform.Find("s0") != null && transform.Find("s0").GetComponent<Text>() != null)
			transform.Find("s0").GetComponent<Text>().text = s0;
		if (transform.Find("s1") != null && transform.Find("s1").GetComponent<Text>() != null)
			transform.Find("s1").GetComponent<Text>().text = s1;

		if (transform.Find("ms0") != null && transform.Find("ms0").GetComponent<Text>() != null)
			transform.Find("ms0").GetComponent<Text>().text = ms0;
		if (transform.Find("ms1") != null && transform.Find("ms1").GetComponent<Text>() != null)
			transform.Find("ms1").GetComponent<Text>().text = ms1;
		if (transform.Find("ms2") != null && transform.Find("ms2").GetComponent<Text>() != null)
			transform.Find("ms2").GetComponent<Text>().text = ms2;
	}

	public static void GetTimeHMS(float time, out int h, out int m, out int s, out int ms)
	{
		s = Mathf.FloorToInt(time);
		m = Mathf.FloorToInt(s / 60f);
		h = Mathf.FloorToInt((s / 60f) / 60f);
		s = s - m * 60;
		m = m - h * 60;
		ms = (int)((time - Mathf.FloorToInt(time)) * 1000);
	}

	public static void GetTimeCharacterStrings(float time, out string h0, out string h1, out string m0, out string m1, out string s0, out string s1, out string ms0, out string ms1, out string ms2)
	{
		int s = Mathf.FloorToInt(time);
		int m = Mathf.FloorToInt(s / 60f);
		int h = Mathf.FloorToInt((s / 60f) / 60f);
		s = s - m * 60;
		m = m - h * 60;
		int ms = (int)((time - Mathf.FloorToInt(time)) * 1000);

		if (h < 10)
		{
			h0 = "0";
			h1 = "" + h;
		}
		else
		{
			h0 = "" + Mathf.FloorToInt(h / 10f);
			h1 = "" + (h - Mathf.FloorToInt(h / 10f) * 10);
		}

		if (m < 10)
		{
			m0 = "0";
			m1 = "" + m;
		}
		else
		{
			m0 = "" + Mathf.FloorToInt(m / 10f);
			m1 = "" + (m - Mathf.FloorToInt(m / 10f) * 10);
		}

		if (s < 10)
		{
			s0 = "0";
			s1 = "" + s;
		}
		else
		{
			s0 = "" + Mathf.FloorToInt(s / 10f);
			s1 = "" + (s - Mathf.FloorToInt(s / 10f) * 10);
		}


		if (ms < 10)
		{
			ms0 = "0";
			ms1 = "0";
			ms2 = "" + ms;
		}
		else
		{
			// >= 10
			if (ms < 100)
			{
				ms0 = "0";
				ms1 = "" + Mathf.FloorToInt(ms / 10f);
				ms2 = "" + (ms - Mathf.FloorToInt(ms / 10f) * 10);
			}
			else
			{
				// >= 100
				int _i_ms0 = Mathf.FloorToInt(ms / 100f);
				int _i_ms1 = Mathf.FloorToInt(ms / 10f) - (_i_ms0 * 10);
				int _i_ms2 = ms - (_i_ms1 * 10) - (_i_ms0 * 100);
				ms0 = "" + _i_ms0;
				ms1 = "" + _i_ms1;
				ms2 = "" + _i_ms2;
			}
		}
	}

	public static void PrintTimeMilliseconds(float startTime, string prefix = "")
	{
		Debug.Log(prefix + GetTimeMilliseconds(startTime));
	}

	public static float GetTimeMilliseconds(float startTime)
	{
		return (Time.realtimeSinceStartup - startTime) * 1000f;
	}





	public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount)
	{
		List<Vector3> ret = new List<Vector3>();
		for (int i = 0; i < positionCount; i++)
		{
			int angle = i * (360 / positionCount);
			Vector3 dir = ApplyRotationToVector(new Vector3(0, 1), angle);
			Vector3 pos = position + dir * distance;
			ret.Add(pos);
		}
		return ret;
	}

	public static List<Vector3> GetPositionListAround(Vector3 position, float[] ringDistance, int[] ringPositionCount)
	{
		List<Vector3> ret = new List<Vector3>();
		for (int ring = 0; ring < ringPositionCount.Length; ring++)
		{
			List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance[ring], ringPositionCount[ring]);
			ret.AddRange(ringPositionList);
		}
		return ret;
	}

	public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount, Vector3 direction, int angleStart, int angleIncrease)
	{
		List<Vector3> ret = new List<Vector3>();
		for (int i = 0; i < positionCount; i++)
		{
			int angle = angleStart + angleIncrease * i;
			Vector3 dir = ApplyRotationToVector(direction, angle);
			Vector3 pos = position + dir * distance;
			ret.Add(pos);
		}
		return ret;
	}

	public static List<Vector3> GetPositionListAlongDirection(Vector3 position, Vector3 direction, float distancePerPosition, int positionCount)
	{
		List<Vector3> ret = new List<Vector3>();
		for (int i = 0; i < positionCount; i++)
		{
			Vector3 pos = position + direction * (distancePerPosition * i);
			ret.Add(pos);
		}
		return ret;
	}

	public static List<Vector3> GetPositionListAlongAxis(Vector3 positionStart, Vector3 positionEnd, int positionCount)
	{
		Vector3 direction = (positionEnd - positionStart).normalized;
		float distancePerPosition = (positionEnd - positionStart).magnitude / positionCount;
		return GetPositionListAlongDirection(positionStart + direction * (distancePerPosition / 2f), direction, distancePerPosition, positionCount);
	}

	public static List<Vector3> GetPositionListWithinRect(Vector3 lowerLeft, Vector3 upperRight, int positionCount)
	{
		List<Vector3> ret = new List<Vector3>();
		float width = upperRight.x - lowerLeft.x;
		float height = upperRight.y - lowerLeft.y;
		float area = width * height;
		float areaPerPosition = area / positionCount;
		float positionSquareSize = Mathf.Sqrt(areaPerPosition);
		Vector3 rowLeft, rowRight;
		rowLeft = new Vector3(lowerLeft.x, lowerLeft.y);
		rowRight = new Vector3(upperRight.x, lowerLeft.y);
		int rowsTotal = Mathf.RoundToInt(height / positionSquareSize);
		float increaseY = height / rowsTotal;
		rowLeft.y += increaseY / 2f;
		rowRight.y += increaseY / 2f;
		int positionsPerRow = Mathf.RoundToInt(width / positionSquareSize);
		for (int i = 0; i < rowsTotal; i++)
		{
			ret.AddRange(GetPositionListAlongAxis(rowLeft, rowRight, positionsPerRow));
			rowLeft.y += increaseY;
			rowRight.y += increaseY;
		}
		int missingPositions = positionCount - ret.Count;
		Vector3 angleDir = (upperRight - lowerLeft) / missingPositions;
		for (int i = 0; i < missingPositions; i++)
		{
			ret.Add(lowerLeft + (angleDir / 2f) + angleDir * i);
		}
		while (ret.Count > positionCount)
		{
			ret.RemoveAt(UnityEngine.Random.Range(0, ret.Count));
		}
		return ret;
	}



	public static List<Vector2Int> GetPosXYListDiamond(int size)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size - x; y++)
			{
				list.Add(new Vector2Int(x, y));
				list.Add(new Vector2Int(-x, y));
				list.Add(new Vector2Int(x, -y));
				list.Add(new Vector2Int(-x, -y));
			}
		}
		return list;
	}

	public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount, Vector3 dir)
	{
		List<Vector2Int> list = GetPosXYListOblong(width, dropXamount, increaseDropXamount);
		list = RotatePosXYList(list, GetAngleFromVector(dir));
		return list;
	}

	public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount)
	{
		List<Vector2Int> triangle = GetPosXYListTriangle(width, dropXamount, increaseDropXamount);
		List<Vector2Int> list = new List<Vector2Int>(triangle);
		foreach (Vector2Int posXY in triangle)
		{
			if (posXY.y == 0) continue;
			list.Add(new Vector2Int(posXY.x, -posXY.y));
		}
		foreach (Vector2Int posXY in new List<Vector2Int>(list))
		{
			if (posXY.x == 0) continue;
			list.Add(new Vector2Int(-posXY.x, posXY.y));
		}
		return list;
	}

	public static List<Vector2Int> GetPosXYListTriangle(int width, int dropXamount, int increaseDropXamount)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = 0; i > -999; i--)
		{
			for (int j = 0; j < width; j++)
			{
				list.Add(new Vector2Int(j, i));
			}
			width -= dropXamount;
			dropXamount += increaseDropXamount;
			if (width <= 0) break;
		}
		return list;
	}

	public static List<Vector2Int> RotatePosXYList(List<Vector2Int> list, int angle)
	{
		List<Vector2Int> ret = new List<Vector2Int>();
		for (int i = 0; i < list.Count; i++)
		{
			Vector2Int posXY = list[i];
			Vector3 vec = ApplyRotationToVector(new Vector3(posXY.x, posXY.y), angle);
			ret.Add(new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)));
		}
		return ret;
	}






	public static Transform CloneTransform(Transform transform, string name = null)
	{
		Transform clone = GameObject.Instantiate(transform, transform.parent);

		if (name != null)
			clone.name = name;
		else
			clone.name = transform.name;

		return clone;
	}

	public static Transform CloneTransform(Transform transform, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
	{
		Transform clone = CloneTransform(transform, name);
		clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
		if (setActiveTrue)
		{
			clone.gameObject.SetActive(true);
		}
		return clone;
	}

	public static Transform CloneTransform(Transform transform, Transform newParent, string name = null)
	{
		Transform clone = GameObject.Instantiate(transform, newParent);

		if (name != null)
			clone.name = name;
		else
			clone.name = transform.name;

		return clone;
	}

	public static Transform CloneTransform(Transform transform, Transform newParent, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
	{
		Transform clone = CloneTransform(transform, newParent, name);
		clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
		if (setActiveTrue)
		{
			clone.gameObject.SetActive(true);
		}
		return clone;
	}

	public static Transform CloneTransformWorld(Transform transform, Transform newParent, string name, Vector3 newLocalPosition)
	{
		Transform clone = CloneTransform(transform, newParent, name);
		clone.localPosition = newLocalPosition;
		return clone;
	}



	public static T[] ArrayAdd<T>(T[] arr, T add)
	{
		T[] ret = new T[arr.Length + 1];
		for (int i = 0; i < arr.Length; i++)
		{
			ret[i] = arr[i];
		}
		ret[arr.Length] = add;
		return ret;
	}

	public static void ShuffleArray<T>(T[] arr, int iterations)
	{
		for (int i = 0; i < iterations; i++)
		{
			int rnd = UnityEngine.Random.Range(0, arr.Length);
			T tmp = arr[rnd];
			arr[rnd] = arr[0];
			arr[0] = tmp;
		}
	}
	public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random)
	{
		for (int i = 0; i < iterations; i++)
		{
			int rnd = random.Next(0, arr.Length);
			T tmp = arr[rnd];
			arr[rnd] = arr[0];
			arr[0] = tmp;
		}
	}

	public static void ShuffleList<T>(List<T> list, int iterations)
	{
		for (int i = 0; i < iterations; i++)
		{
			int rnd = UnityEngine.Random.Range(0, list.Count);
			T tmp = list[rnd];
			list[rnd] = list[0];
			list[0] = tmp;
		}
	}


	public static void DebugDrawCircle(Vector3 center, float radius, Color color, float duration, int divisions)
	{
		for (int i = 0; i <= divisions; i++)
		{
			Vector3 vec1 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * i);
			Vector3 vec2 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * (i + 1));
			Debug.DrawLine(vec1, vec2, color, duration);
		}
	}

	public static void DebugDrawRectangle(Vector3 minXY, Vector3 maxXY, Color color, float duration)
	{
		Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(maxXY.x, minXY.y), color, duration);
		Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(minXY.x, maxXY.y), color, duration);
		Debug.DrawLine(new Vector3(minXY.x, maxXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
		Debug.DrawLine(new Vector3(maxXY.x, minXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
	}

	public static void DebugDrawText(string text, Vector3 position, Color color, float size, float duration)
	{
		text = text.ToUpper();
		float kerningSize = size * 0.6f;
		Vector3 basePosition = position;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			switch (c)
			{
				case '\n':
					// Newline
					position.x = basePosition.x;
					position.y += size;
					break;
				case ' ':
					position.x += kerningSize;
					break;
				default:
					DebugDrawChar(c, position, color, size, duration);
					position.x += kerningSize;
					break;
			}
		}
	}

	// Draw Characters using Debug DrawLine Gizmos
	public static void DebugDrawChar(char c, Vector3 position, Color color, float size, float duration)
	{
		switch (c)
		{
			default:
			case 'A':
				DebugDrawLines(position, color, size, duration, new[] {
				0.317f,0.041f, 0.5f,0.98f, 0.749f,0.062f, 0.625f,0.501f, 0.408f,0.507f }); break;
			case 'B':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.289f,0.069f, 0.274f,0.937f, 0.609f,0.937f, 0.801f,0.879f, 0.829f,0.708f, 0.756f,0.538f, 0.655f,0.492f, 0.442f,0.495f, 0.271f,0.495f, 0.567f,0.474f, 0.676f,0.465f, 0.722f,0.385f, 0.719f,0.181f, 0.664f,0.087f, 0.527f,0.053f, 0.396f,0.05f, 0.271f,0.078f }); break;
			case 'C':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.695f,0.946f, 0.561f,0.949f, 0.426f,0.937f, 0.317f,0.867f, 0.265f,0.733f, 0.262f,0.553f, 0.292f,0.27f, 0.323f,0.172f, 0.417f,0.12f, 0.512f,0.096f, 0.637f,0.093f, 0.743f,0.117f, }); break;
			case 'D':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.314f,0.909f, 0.329f,0.096f, 0.53f,0.123f, 0.594f,0.197f, 0.673f,0.334f, 0.716f,0.498f, 0.692f,0.666f, 0.609f,0.806f, 0.457f,0.891f, 0.323f,0.919f }); break;
			case 'E':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.344f,0.919f, 0.363f,0.078f, 0.713f,0.096f, 0.359f,0.096f, 0.347f,0.48f, 0.53f,0.492f, 0.356f,0.489f, 0.338f,0.913f, 0.625f,0.919f }); break;
			case 'F':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.682f,0.916f, 0.329f,0.909f, 0.341f,0.66f, 0.503f,0.669f, 0.341f,0.669f, 0.317f,0.087f }); break;
			case 'G':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.618f,0.867f, 0.399f,0.849f, 0.292f,0.654f, 0.241f,0.404f, 0.253f,0.178f, 0.481f,0.075f, 0.612f,0.078f, 0.725f,0.169f, 0.728f,0.334f, 0.71f,0.437f, 0.609f,0.462f, 0.463f,0.462f }); break;
			case 'H':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.277f,0.876f, 0.305f,0.133f, 0.295f,0.507f, 0.628f,0.501f, 0.643f,0.139f, 0.637f,0.873f }); break;
			case 'I':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.487f,0.906f, 0.484f,0.096f }); break;
			case 'J':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.628f,0.882f, 0.679f,0.242f, 0.603f,0.114f, 0.445f,0.066f, 0.317f,0.114f, 0.262f,0.209f, 0.253f,0.3f, 0.259f,0.367f }); break;
			case 'K':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.292f,0.879f, 0.311f,0.111f, 0.305f,0.498f, 0.594f,0.876f, 0.305f,0.516f, 0.573f,0.154f,  }); break;
			case 'L':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.311f,0.879f, 0.308f,0.133f, 0.682f,0.148f,  }); break;
			case 'M':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.262f,0.12f, 0.265f,0.909f, 0.509f,0.608f, 0.71f,0.919f, 0.713f,0.151f,  }); break;
			case 'N':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.737f,0.885f, 0.679f,0.114f, 0.335f,0.845f, 0.353f,0.175f,  }); break;
			case 'O':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.609f,0.906f, 0.396f,0.894f, 0.271f,0.687f, 0.232f,0.474f, 0.241f,0.282f, 0.356f,0.142f, 0.527f,0.087f, 0.655f,0.09f, 0.719f,0.181f, 0.737f,0.379f, 0.737f,0.638f, 0.71f,0.836f, 0.628f,0.919f, 0.582f,0.919f, }); break;
			case 'P':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.314f,0.129f, 0.311f,0.873f, 0.658f,0.906f, 0.746f,0.8f, 0.746f,0.66f, 0.673f,0.544f, 0.509f,0.51f, 0.359f,0.51f, 0.311f,0.516f,  }); break;
			case 'Q':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.497f,0.925f, 0.335f,0.894f, 0.228f,0.681f, 0.213f,0.379f, 0.25f,0.145f, 0.396f,0.096f, 0.573f,0.105f, 0.631f,0.166f, 0.542f,0.245f, 0.752f,0.108f, 0.628f,0.187f, 0.685f,0.261f, 0.728f,0.398f, 0.759f,0.605f, 0.722f,0.794f, 0.64f,0.916f, 0.475f,0.946f,  }); break;
			case 'R':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.347f,0.142f, 0.332f,0.9f, 0.667f,0.897f, 0.698f,0.699f, 0.655f,0.58f, 0.521f,0.553f, 0.396f,0.553f, 0.344f,0.553f, 0.564f,0.37f, 0.655f,0.206f, 0.71f,0.169f }); break;
			case 'S':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.695f,0.842f, 0.576f,0.882f, 0.439f,0.885f, 0.329f,0.8f, 0.289f,0.626f, 0.317f,0.489f, 0.439f,0.44f, 0.621f,0.434f, 0.695f,0.358f, 0.713f,0.224f, 0.646f,0.111f, 0.494f,0.093f, 0.338f,0.105f, 0.289f,0.151f, }); break;
			case 'T':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.497f,0.172f, 0.5f,0.864f, 0.286f,0.858f, 0.719f,0.852f,  }); break;
			case 'U':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.232f,0.858f, 0.247f,0.251f, 0.366f,0.105f, 0.466f,0.078f, 0.615f,0.084f, 0.704f,0.123f, 0.746f,0.276f, 0.74f,0.559f, 0.737f,0.806f, 0.722f,0.864f, }); break;
			case 'V':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.238f,0.855f, 0.494f,0.105f, 0.707f,0.855f,  }); break;
			case 'X':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.783f,0.852f, 0.256f,0.133f, 0.503f,0.498f, 0.305f,0.824f, 0.789f,0.117f,  }); break;
			case 'Y':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.299f,0.842f, 0.497f,0.529f, 0.646f,0.842f, 0.49f,0.541f, 0.487f,0.105f, }); break;
			case 'W':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.228f,0.815f, 0.381f,0.093f, 0.503f,0.434f, 0.615f,0.151f, 0.722f,0.818f,  }); break;
			case 'Z':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.25f,0.87f, 0.795f,0.842f, 0.274f,0.133f, 0.716f,0.142f }); break;


			case '0':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.536f,0.891f, 0.509f,0.891f, 0.42f,0.809f, 0.378f,0.523f, 0.372f,0.215f, 0.448f,0.087f, 0.539f,0.069f, 0.609f,0.099f, 0.637f,0.242f, 0.646f,0.416f, 0.646f,0.608f, 0.631f,0.809f, 0.554f,0.888f, 0.527f,0.894f,  }); break;
			case '1':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.652f,0.108f, 0.341f,0.114f, 0.497f,0.12f, 0.497f,0.855f, 0.378f,0.623f,  }); break;
			case '2':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.311f,0.714f, 0.375f,0.83f, 0.564f,0.894f, 0.722f,0.839f, 0.765f,0.681f, 0.634f,0.483f, 0.5f,0.331f, 0.366f,0.245f, 0.299f,0.126f, 0.426f,0.126f, 0.621f,0.136f, 0.679f,0.136f, 0.737f,0.139f,  }); break;
			case '3':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.289f,0.855f, 0.454f,0.876f, 0.606f,0.818f, 0.685f,0.702f, 0.664f,0.547f, 0.564f,0.459f, 0.484f,0.449f, 0.417f,0.455f, 0.53f,0.434f, 0.655f,0.355f, 0.664f,0.233f, 0.591f,0.105f, 0.466f,0.075f, 0.335f,0.084f, 0.259f,0.142f,  }); break;
			case '4':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.353f,0.836f, 0.262f,0.349f, 0.579f,0.367f, 0.5f,0.376f, 0.49f,0.471f, 0.509f,0.069f,  }); break;
			case '5':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.67f,0.852f, 0.335f,0.858f, 0.347f,0.596f, 0.582f,0.602f, 0.698f,0.513f, 0.749f,0.343f, 0.719f,0.187f, 0.561f,0.133f, 0.363f,0.151f,  }); break;
			case '6':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.567f,0.888f, 0.442f,0.782f, 0.35f,0.544f, 0.326f,0.288f, 0.39f,0.157f, 0.615f,0.142f, 0.679f,0.245f, 0.676f,0.37f, 0.573f,0.48f, 0.454f,0.48f, 0.378f,0.41f, 0.335f,0.367f,  }); break;
			case '7':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.286f,0.852f, 0.731f,0.864f, 0.417f,0.117f, 0.57f,0.498f, 0.451f,0.483f, 0.688f,0.501f, }); break;
			case '8':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.518f,0.541f, 0.603f,0.623f, 0.649f,0.748f, 0.612f,0.858f, 0.497f,0.888f, 0.375f,0.824f, 0.341f,0.708f, 0.381f,0.611f, 0.494f,0.55f, 0.557f,0.513f, 0.6f,0.416f, 0.631f,0.312f, 0.579f,0.178f, 0.509f,0.108f, 0.436f,0.102f, 0.335f,0.181f, 0.308f,0.279f, 0.347f,0.401f, 0.423f,0.486f, 0.497f,0.547f,  }); break;
			case '9':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.475f,0.129f, 0.573f,0.495f, 0.646f,0.824f, 0.509f,0.97f, 0.28f,0.94f, 0.189f,0.827f, 0.262f,0.708f, 0.396f,0.69f, 0.564f,0.745f, 0.646f,0.83f,  }); break;


			case '.':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f }); break;
			case ':':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f });
				DebugDrawLines(position, color, size, duration, new[] {
				0.515f,.5f+0.157f, 0.469f,.5f+0.148f, 0.469f,.5f+0.117f, 0.515f,.5f+0.123f, 0.503f,.5f+0.169f });
				break;
			case '-':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.277f,0.51f, 0.716f,0.51f,  }); break;
			case '+':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.265f,0.513f, 0.676f,0.516f, 0.497f,0.529f, 0.49f,0.699f, 0.497f,0.27f,  }); break;

			case '(':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.542f,0.934f, 0.411f,0.797f, 0.344f,0.587f, 0.341f,0.434f, 0.375f,0.257f, 0.457f,0.12f, 0.567f,0.075f, }); break;
			case ')':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.442f,0.94f, 0.548f,0.757f, 0.625f,0.568f, 0.64f,0.392f, 0.554f,0.129f, 0.472f,0.056f,  }); break;
			case ';':
			case ',':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.533f,0.239f, 0.527f,0.154f, 0.487f,0.099f, 0.451f,0.062f,  }); break;
			case '_':
				DebugDrawLines(position, color, size, duration, new[] {
			   0.274f,0.133f, 0.716f,0.142f }); break;

				/*
		 case ':': DebugDrawLines(position, color, size, duration, new [] {
				0f }); break;
				*/
		}
	}

	public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, Vector3[] points)
	{
		for (int i = 0; i < points.Length - 1; i++)
		{
			Debug.DrawLine(position + points[i] * size, position + points[i + 1] * size, color, duration);
		}
	}

	public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, float[] points)
	{
		List<Vector3> vecList = new List<Vector3>();
		for (int i = 0; i < points.Length; i += 2)
		{
			Vector3 vec = new Vector3(points[i + 0], points[i + 1]);
			vecList.Add(vec);
		}
		DebugDrawLines(position, color, size, duration, vecList.ToArray());
	}

	public static void ClearLogConsole()
	{
#if UNITY_EDITOR
		//Debug.Log("################# DISABLED BECAUSE OF BUILD!");
		/*
		Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
		System.Type logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
		MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
		clearConsoleMethod.Invoke(new object(), null);
		//*/
#endif
	}


	public static string GetPercentString(float f, bool includeSign = true)
	{
		return Mathf.RoundToInt(f * 100f) + (includeSign ? "%" : "");
	}



	public static string GetMonthName(int month)
	{
		switch (month)
		{
			default:
			case 0: return "January";
			case 1: return "February";
			case 2: return "March";
			case 3: return "April";
			case 4: return "May";
			case 5: return "June";
			case 6: return "July";
			case 7: return "August";
			case 8: return "September";
			case 9: return "October";
			case 10: return "November";
			case 11: return "December";
		}
	}

	public static string GetMonthNameShort(int month)
	{
		return GetMonthName(month).Substring(0, 3);
	}




	public static class ReflectionTools
	{

		public static object CallMethod(string typeName, string methodName)
		{
			return System.Type.GetType(typeName).GetMethod(methodName).Invoke(null, null);
		}
		public static object GetField(string typeName, string fieldName)
		{
			System.Reflection.FieldInfo fieldInfo = System.Type.GetType(typeName).GetField(fieldName);
			return fieldInfo.GetValue(null);
		}
		public static System.Type GetNestedType(string typeName, string nestedTypeName)
		{
			return System.Type.GetType(typeName).GetNestedType(nestedTypeName);
		}

	}



	public static bool TestChance(int chance, int chanceMax = 100)
	{
		return UnityEngine.Random.Range(0, chanceMax) < chance;
	}

	public static T[] RemoveDuplicates<T>(T[] arr)
	{
		List<T> list = new List<T>();
		foreach (T t in arr)
		{
			if (!list.Contains(t))
			{
				list.Add(t);
			}
		}
		return list.ToArray();
	}

	public static List<T> RemoveDuplicates<T>(List<T> arr)
	{
		List<T> list = new List<T>();
		foreach (T t in arr)
		{
			if (!list.Contains(t))
			{
				list.Add(t);
			}
		}
		return list;
	}



#endif
}

public class Colors
{
	// NOTE: The follwing color names come from the CSS3 specification, Section 4.3 Extended Color Keywords
	// http://www.w3.org/TR/css3-color/#svg-color


	public static readonly Color AliceBlue = new Color32(240, 248, 255, 255);
	public static readonly Color AntiqueWhite = new Color32(250, 235, 215, 255);
	public static readonly Color Aqua = new Color32(0, 255, 255, 255);
	public static readonly Color Aquamarine = new Color32(127, 255, 212, 255);
	public static readonly Color Azure = new Color32(240, 255, 255, 255);
	public static readonly Color Beige = new Color32(245, 245, 220, 255);
	public static readonly Color Bisque = new Color32(255, 228, 196, 255);
	public static readonly Color Black = new Color32(0, 0, 0, 255);
	public static readonly Color BlanchedAlmond = new Color32(255, 235, 205, 255);
	public static readonly Color Blue = new Color32(0, 0, 255, 255);
	public static readonly Color BlueViolet = new Color32(138, 43, 226, 255);
	public static readonly Color Brown = new Color32(165, 42, 42, 255);
	public static readonly Color Burlywood = new Color32(222, 184, 135, 255);
	public static readonly Color CadetBlue = new Color32(95, 158, 160, 255);
	public static readonly Color Chartreuse = new Color32(127, 255, 0, 255);
	public static readonly Color Chocolate = new Color32(210, 105, 30, 255);
	public static readonly Color Coral = new Color32(255, 127, 80, 255);
	public static readonly Color CornflowerBlue = new Color32(100, 149, 237, 255);
	public static readonly Color Cornsilk = new Color32(255, 248, 220, 255);
	public static readonly Color Crimson = new Color32(220, 20, 60, 255);
	public static readonly Color Cyan = new Color32(0, 255, 255, 255);
	public static readonly Color DarkBlue = new Color32(0, 0, 139, 255);
	public static readonly Color DarkCyan = new Color32(0, 139, 139, 255);
	public static readonly Color DarkGoldenrod = new Color32(184, 134, 11, 255);
	public static readonly Color DarkGray = new Color32(169, 169, 169, 255);
	public static readonly Color DarkGreen = new Color32(0, 100, 0, 255);
	public static readonly Color DarkKhaki = new Color32(189, 183, 107, 255);
	public static readonly Color DarkMagenta = new Color32(139, 0, 139, 255);
	public static readonly Color DarkOliveGreen = new Color32(85, 107, 47, 255);
	public static readonly Color DarkOrange = new Color32(255, 140, 0, 255);
	public static readonly Color DarkOrchid = new Color32(153, 50, 204, 255);
	public static readonly Color DarkRed = new Color32(139, 0, 0, 255);
	public static readonly Color DarkSalmon = new Color32(233, 150, 122, 255);
	public static readonly Color DarkSeaGreen = new Color32(143, 188, 143, 255);
	public static readonly Color DarkSlateBlue = new Color32(72, 61, 139, 255);
	public static readonly Color DarkSlateGray = new Color32(47, 79, 79, 255);
	public static readonly Color DarkTurquoise = new Color32(0, 206, 209, 255);
	public static readonly Color DarkViolet = new Color32(148, 0, 211, 255);
	public static readonly Color DeepPink = new Color32(255, 20, 147, 255);
	public static readonly Color DeepSkyBlue = new Color32(0, 191, 255, 255);
	public static readonly Color DimGray = new Color32(105, 105, 105, 255);
	public static readonly Color DodgerBlue = new Color32(30, 144, 255, 255);
	public static readonly Color FireBrick = new Color32(178, 34, 34, 255);
	public static readonly Color FloralWhite = new Color32(255, 250, 240, 255);
	public static readonly Color ForestGreen = new Color32(34, 139, 34, 255);
	public static readonly Color Fuchsia = new Color32(255, 0, 255, 255);
	public static readonly Color Gainsboro = new Color32(220, 220, 220, 255);
	public static readonly Color GhostWhite = new Color32(248, 248, 255, 255);
	public static readonly Color Gold = new Color32(255, 215, 0, 255);
	public static readonly Color Goldenrod = new Color32(218, 165, 32, 255);
	public static readonly Color Gray = new Color32(128, 128, 128, 255);
	public static readonly Color Green = new Color32(0, 128, 0, 255);
	public static readonly Color GreenYellow = new Color32(173, 255, 47, 255);
	public static readonly Color Honeydew = new Color32(240, 255, 240, 255);
	public static readonly Color HotPink = new Color32(255, 105, 180, 255);
	public static readonly Color IndianRed = new Color32(205, 92, 92, 255);
	public static readonly Color Indigo = new Color32(75, 0, 130, 255);
	public static readonly Color Ivory = new Color32(255, 255, 240, 255);
	public static readonly Color Khaki = new Color32(240, 230, 140, 255);
	public static readonly Color Lavender = new Color32(230, 230, 250, 255);
	public static readonly Color Lavenderblush = new Color32(255, 240, 245, 255);
	public static readonly Color LawnGreen = new Color32(124, 252, 0, 255);
	public static readonly Color LemonChiffon = new Color32(255, 250, 205, 255);
	public static readonly Color LightBlue = new Color32(173, 216, 230, 255);
	public static readonly Color LightCoral = new Color32(240, 128, 128, 255);
	public static readonly Color LightCyan = new Color32(224, 255, 255, 255);
	public static readonly Color LightGoldenodYellow = new Color32(250, 250, 210, 255);
	public static readonly Color LightGray = new Color32(211, 211, 211, 255);
	public static readonly Color LightGreen = new Color32(144, 238, 144, 255);
	public static readonly Color LightPink = new Color32(255, 182, 193, 255);
	public static readonly Color LightSalmon = new Color32(255, 160, 122, 255);
	public static readonly Color LightSeaGreen = new Color32(32, 178, 170, 255);
	public static readonly Color LightSkyBlue = new Color32(135, 206, 250, 255);
	public static readonly Color LightSlateGray = new Color32(119, 136, 153, 255);
	public static readonly Color LightSteelBlue = new Color32(176, 196, 222, 255);
	public static readonly Color LightYellow = new Color32(255, 255, 224, 255);
	public static readonly Color LightYellow2 = new Color32(255, 241, 99, 255);
	public static readonly Color Lime = new Color32(0, 255, 0, 255);
	public static readonly Color LimeGreen = new Color32(50, 205, 50, 255);
	public static readonly Color Linen = new Color32(250, 240, 230, 255);
	public static readonly Color Magenta = new Color32(255, 0, 255, 255);
	public static readonly Color Maroon = new Color32(128, 0, 0, 255);
	public static readonly Color MediumAquamarine = new Color32(102, 205, 170, 255);
	public static readonly Color MediumBlue = new Color32(0, 0, 205, 255);
	public static readonly Color MediumOrchid = new Color32(186, 85, 211, 255);
	public static readonly Color MediumPurple = new Color32(147, 112, 219, 255);
	public static readonly Color MediumSeaGreen = new Color32(60, 179, 113, 255);
	public static readonly Color MediumSlateBlue = new Color32(123, 104, 238, 255);
	public static readonly Color MediumSpringGreen = new Color32(0, 250, 154, 255);
	public static readonly Color MediumTurquoise = new Color32(72, 209, 204, 255);
	public static readonly Color MediumVioletRed = new Color32(199, 21, 133, 255);
	public static readonly Color MidnightBlue = new Color32(25, 25, 112, 255);
	public static readonly Color Mintcream = new Color32(245, 255, 250, 255);
	public static readonly Color MistyRose = new Color32(255, 228, 225, 255);
	public static readonly Color Moccasin = new Color32(255, 228, 181, 255);
	public static readonly Color NavajoWhite = new Color32(255, 222, 173, 255);
	public static readonly Color Navy = new Color32(0, 0, 128, 255);
	public static readonly Color OldLace = new Color32(253, 245, 230, 255);
	public static readonly Color Olive = new Color32(128, 128, 0, 255);
	public static readonly Color Olivedrab = new Color32(107, 142, 35, 255);
	public static readonly Color Orange = new Color32(255, 165, 0, 255);
	public static readonly Color Orangered = new Color32(255, 69, 0, 255);
	public static readonly Color Orchid = new Color32(218, 112, 214, 255);
	public static readonly Color PaleGoldenrod = new Color32(238, 232, 170, 255);
	public static readonly Color PaleGreen = new Color32(152, 251, 152, 255);
	public static readonly Color PaleTurquoise = new Color32(175, 238, 238, 255);
	public static readonly Color PaleVioletred = new Color32(219, 112, 147, 255);
	public static readonly Color PapayaWhip = new Color32(255, 239, 213, 255);
	public static readonly Color PeachPuff = new Color32(255, 218, 185, 255);
	public static readonly Color Peru = new Color32(205, 133, 63, 255);
	public static readonly Color Pink = new Color32(255, 192, 203, 255);
	public static readonly Color Plum = new Color32(221, 160, 221, 255);
	public static readonly Color PowderBlue = new Color32(176, 224, 230, 255);
	public static readonly Color Purple = new Color32(128, 0, 128, 255);
	public static readonly Color Red = new Color32(255, 0, 0, 255);
	public static readonly Color RosyBrown = new Color32(188, 143, 143, 255);
	public static readonly Color RoyalBlue = new Color32(65, 105, 225, 255);
	public static readonly Color SaddleBrown = new Color32(139, 69, 19, 255);
	public static readonly Color Salmon = new Color32(250, 128, 114, 255);
	public static readonly Color SandyBrown = new Color32(244, 164, 96, 255);
	public static readonly Color SeaGreen = new Color32(46, 139, 87, 255);
	public static readonly Color Seashell = new Color32(255, 245, 238, 255);
	public static readonly Color Sienna = new Color32(160, 82, 45, 255);
	public static readonly Color Silver = new Color32(192, 192, 192, 255);
	public static readonly Color SkyBlue = new Color32(135, 206, 235, 255);
	public static readonly Color SlateBlue = new Color32(106, 90, 205, 255);
	public static readonly Color SlateGray = new Color32(112, 128, 144, 255);
	public static readonly Color Snow = new Color32(255, 250, 250, 255);
	public static readonly Color SpringGreen = new Color32(0, 255, 127, 255);
	public static readonly Color SteelBlue = new Color32(70, 130, 180, 255);
	public static readonly Color Tan = new Color32(210, 180, 140, 255);
	public static readonly Color Teal = new Color32(0, 128, 128, 255);
	public static readonly Color Thistle = new Color32(216, 191, 216, 255);
	public static readonly Color Tomato = new Color32(255, 99, 71, 255);
	public static readonly Color Turquoise = new Color32(64, 224, 208, 255);
	public static readonly Color Violet = new Color32(238, 130, 238, 255);
	public static readonly Color Wheat = new Color32(245, 222, 179, 255);
	public static readonly Color White = new Color32(255, 255, 255, 255);
	public static readonly Color WhiteSmoke = new Color32(245, 245, 245, 255);
	public static readonly Color Yellow = new Color32(255, 255, 0, 255);
	public static readonly Color YellowGreen = new Color32(154, 205, 50, 255);
	public static readonly Color[] colors = { Black, AntiqueWhite, Aqua, Aquamarine, Blue, BlueViolet, Chocolate, Coral, Cornsilk, Crimson, Cyan };


}

[System.Serializable]
public class CustomList<T>
{
	public List<CustomListObj<T>> List = new List<CustomListObj<T>>();
	public T Get(string Name)
	{
		CustomListObj<T> anim = null;
		try
		{
			anim = List.SingleOrDefault(x => x.name == Name);
			return anim.value;
		}
		catch
		{
			return default;
		}


	}

	public bool TryGet(string Name, out T value)
	{
		foreach (CustomListObj<T> item in List)
		{
			if (item.name == Name)
			{
				value = item.value;
				return true;
			}
		}
		value = default;
		return false;
	}
	public void Add(string key, T value)
	{
		CustomListObj<T> newVal = new CustomListObj<T>(key, value);
		List.Add(newVal);
	}



	public void DeepAdd(T value)
	{
		CustomListObj<T> newVal = new CustomListObj<T>(value);
		List.Add(newVal);
	}
	public void Add(CustomListObj<T> value)
	{
		List.Add(value);
	}

	public void DeepAdd(CustomListObj<T> value)
	{
		CustomListObj<T> newVal = new CustomListObj<T>(value.name, value.value);
		List.Add(newVal);
	}

	public int Count => List.Count;

	public T GetRandom()
	{
		return List.GetRandomShuffled().value;
	}
	public CustomListObj<T> GetRandomobj()
	{
		return List.GetRandomShuffled();
	}
	/* public T GetSeededRandom()
	 {
		 return List.GetRandomSeededShuffed().value;

	 }*/

	
}

[System.Serializable]
public class CustomListObj<T>
{
	public string name;
	public T value;

	public CustomListObj()
	{
	}

	public CustomListObj(T value)
	{
		this.value = value;
	}

	public CustomListObj(string name, T value)
	{
		this.name = name;
		this.value = value;
	}
}

[System.Serializable]
public class DataBank<Key,T>
{
	public CustomList<Key, T> DataList;
	public Key currentKey;
	public T currentData;
	public T GetGetCurrentData()
	{
		return DataList.Get(currentKey);
	}
	public void SetCurrentData(Key key)
	{
		currentKey = key;
		SetCurrentData();
	}
	public void SetCurrentData()
	{
		currentData = DataList.Get(currentKey);
	}
}

[System.Serializable]
public class CustomList<Key, T>
{
	public List<CustomListObj<Key, T>> List = new List<CustomListObj<Key, T>>();


	public int Count => List.Count;

	public void Clear() => List.Clear();

	public void New() => List = new List<CustomListObj<Key, T>>();
	public void AddDeep(Key key, T value)
	{
		T newT = value.DeepCopy();
		CustomListObj<Key, T> newVal = new CustomListObj<Key, T>(key, newT);
		List.Add(newVal);
	}
	public void Add(Key key, T value)
	{
		CustomListObj<Key, T> newVal = new CustomListObj<Key, T>(key, value);
		List.Add(newVal);
	}
	public void Add(CustomListObj<Key, T> value)
	{
		List.Add(value);
	}
	public void Remove(Key key)
	{
		if (TryGet(key, out CustomListObj<Key, T> obj))
		{
			List.Remove(obj);
		}
	}

	public void MoveUp(int index)
	{
		if(index != 0)
		{
			List.Swap(index, index - 1);
		}
		
	}
	public void MoveDown(int index)
	{
		if(index != List.Count -1)
		{
			List.Swap(index, index + 1);
		}
	}
	public List<T> ToList()
	{
		List<T> list = new List<T>();
		foreach (var item in List)
		{
			list.Add(item.value);
		}
		return list;

	}


	public T Get(Key key)
	{
		CustomListObj<Key, T> anim = null;
		try
		{
			anim = List.SingleOrDefault(x => x.key.Equals(key));
			return anim.value;
		}
		catch
		{
			return default;
		}
	}

	public bool Contains(Key key)
	{
		return TryGet(key, out CustomListObj<Key, T> obj);

	}
	public bool TryGet(Key key, out T value)
	{
		foreach (CustomListObj<Key, T> item in List)
		{
			if (item.key.Equals(key))
			{
				value = item.value;
				return true;
			}
		}
		value = default;
		return false;
	}
	public bool TryGet(Key key, out CustomListObj<Key, T> value)
	{
		foreach (CustomListObj<Key, T> item in List)
		{
			if (item.key.Equals(key))
			{
				value = item;
				return true;
			}
		}
		value = default;
		return false;
	}



	public T GetRandom()
	{
		return List.GetRandomShuffled().value;
	}
	/* public T GetSeededRandom()
	 {
		 return List.GetRandomSeededShuffed().value;
	 }
	public void SeededShuffle() => List.SeedShuffle();

	 */

	public void Shuffle() => List.Shuffle();


}
[Serializable]
public class CustomListObj<Key, T>
{
	public Key key;
	public T value;

	public CustomListObj(Key key, T value)
	{
		this.key = key;
		this.value = value;
	}
}