#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
using Path = System.IO.Path;

/// <summary>
/// Written by GK. 
/// Editor toolset For easy development.
/// </summary>


namespace Helpers
{
	public static class ProjectPathHelper
	{

		[MenuItem("Play/Play Game %0")]
		public static void PlayGame()
		{

			if (EditorApplication.isPlaying == true)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene("Assets/TaxiDriving3D/Scenes/Splash.unity");
			EditorApplication.isPlaying = true;
		}


		[MenuItem("Assets/Get Loading Names")]
		public static void GetLoadNames()
		{
			var o = Selection.activeObject;
			//        Debug.Log( "Name = " + o.name);
			//        var guid = Selection.assetGUIDs[0];
			//        Debug.Log( "GUID = " + guid);

			string typeString = o.GetType().ToString();

			// this presumes you can get away with just the final name in the type,
			// e.g., you can use Material instead of UnityEngine.Material.
			int n = typeString.LastIndexOf('.');
			if (n >= 0)
			{
				typeString = typeString.Substring(n + 1);
			}

			//        Debug.Log( "typeString:" + typeString);

			var assetPath = AssetDatabase.GetAssetOrScenePath(o);
			Debug.Log(System.String.Format(
				"var myAsset = AssetDatabase.LoadAssetAtPath<{0}>(\"{1}\");",
				typeString, assetPath));

			var dirName = System.IO.Path.GetDirectoryName(assetPath);



			string resourcesDirName = null;
			const string s_Resources = "\\Resources";
			if (assetPath.EndsWith(s_Resources))
			{
				resourcesDirName = "";
			}
			else if (assetPath.Contains(s_Resources) || assetPath.Contains("Resources"))
			{

				n = dirName.IndexOf(s_Resources);

				if (n >= 0)
				{
					resourcesDirName = dirName.Substring(n + s_Resources.Length + 1) + "/";
				}

				resourcesDirName = resourcesDirName.Replace(System.IO.Path.DirectorySeparatorChar, '/');


			}
			if (resourcesDirName != null)
			{
				Debug.Log(System.String.Format(
					"var myAsset = Resources.Load<{0}>(\"{1}\");",
					typeString, resourcesDirName + o.name));



				Debug.Log(System.String.Format(
					"Resource path: \n {0}",
					resourcesDirName + o.name));

			}
			else
			{
				Debug.LogWarning("Asset not under Resources folder. Cannot be loaded with Resources.Load();");
			}
		}



		private static bool isValid(string str)
		{
			return Regex.IsMatch(str, @"^[a-zA-Z]+$");
		}

		public static string GetLoadingPath(Object _object)
		{
			string path = null;
			Object o = _object;
			//        Debug.Log( "Name = " + o.name);
			//        var guid = Selection.assetGUIDs[0];
			//        Debug.Log( "GUID = " + guid);

			string typeString = o.GetType().ToString();

			// this presumes you can get away with just the final name in the type,
			// e.g., you can use Material instead of UnityEngine.Material.
			int n = typeString.LastIndexOf('.');


			//        Debug.Log( "typeString:" + typeString);

			var assetPath = AssetDatabase.GetAssetOrScenePath(o);

			var dirName = System.IO.Path.GetDirectoryName(assetPath);

			string resourcesDirName = null;
			const string s_Resources = "\\Resources";
			if (assetPath.EndsWith(s_Resources))
			{
				resourcesDirName = "";
			}
			else
			{

				n = dirName.IndexOf(s_Resources);

				if (n >= 0)
				{
					resourcesDirName = dirName.Substring(n + s_Resources.Length + 1) + "/";
				}

				resourcesDirName = resourcesDirName.Replace(System.IO.Path.DirectorySeparatorChar, '/');


			}
			if (resourcesDirName != null)
			{

				path = resourcesDirName + o.name;
			}
			else
			{
				Debug.LogWarning("Asset not under Resources folder. Cannot be loaded with Resources.Load();");
			}

			return path;

		}

		public static string GetFileName(Object _object)
		{

			Object o = _object;

			var assetPath = AssetDatabase.GetAssetOrScenePath(o);

			var dirName = System.IO.Path.GetFileNameWithoutExtension(assetPath);


			return dirName;

		}

		public static string GetDummyFileName(Object _object, int cont = 0)
		{

			Object o = _object;

			var assetPath = AssetDatabase.GetAssetOrScenePath(o);

			var dirName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

			if (dirName.Contains("dummy"))
			{

				dirName = dirName.Replace("dummy", "td");
			}
			else
			{
				dirName = "Car" + cont;
			}

			return dirName;

		}


		[MenuItem("Assets/Copy Resource Path %&c")]
		public static void CopyResourcePath()
		{
			var o = Selection.activeObject;
			//        Debug.Log( "Name = " + o.name);
			//        var guid = Selection.assetGUIDs[0];
			//        Debug.Log( "GUID = " + guid);

			string path = GetLoadingPath(o);

			TextEditor textEditor = new TextEditor();
			textEditor.text = path;
			textEditor.SelectAll();
			textEditor.Copy();
		}

		[MenuItem("Assets/Copy Asset name %&x")]
		public static void CopyAssetName()
		{
			var o = Selection.activeObject;


			TextEditor textEditor = new TextEditor
			{
				text = o.name
			};
			textEditor.SelectAll();
			textEditor.Copy();
		}
	}


	public static class GridSpriteSlicer
	{
		[MenuItem("Assets/Slice Spritesheets %&s")]
		public static void Slice()
		{
			var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

			foreach (var texture in textures)
			{
				ProcessTexture(texture);
			}
		}
		public static void Slice(Vector2 PixelSize)
		{
			var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

			foreach (var texture in textures)
			{
				ProcessTexture(texture, PixelSize);
			}
		}

		static void ProcessTexture(Texture2D texture)
		{
			string path = AssetDatabase.GetAssetPath(texture);
			var importer = AssetImporter.GetAtPath(path) as TextureImporter;

			//importer.isReadable = true;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.filterMode = FilterMode.Point;
			importer.spritePivot = Vector2.down;
			importer.textureCompression = TextureImporterCompression.Uncompressed;

			var textureSettings = new TextureImporterSettings(); // need this stupid class because spriteExtrude and spriteMeshType aren't exposed on TextureImporter
			importer.ReadTextureSettings(textureSettings);
			textureSettings.spriteMeshType = SpriteMeshType.Tight;
			textureSettings.spriteExtrude = 0;

			importer.SetTextureSettings(textureSettings);

			int minimumSpriteSize = 16;
			int extrudeSize = 0;

			Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(texture, minimumSpriteSize, extrudeSize);
			var rectsList = new List<Rect>(rects);
			rectsList = SortRects(rectsList, texture.width);

			string filenameNoExtension = System.IO.Path.GetFileNameWithoutExtension(path);
			var metas = new List<SpriteMetaData>();
			int rectNum = 0;

			foreach (Rect rect in rectsList)
			{
				var meta = new SpriteMetaData
				{
					pivot = Vector2.down,
					alignment = (int)SpriteAlignment.BottomCenter,
					rect = rect,
					name = filenameNoExtension + "_" + rectNum++
				};
				metas.Add(meta);
			}

			importer.spritesheet = metas.ToArray();

			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}

		static void ProcessTexture(Texture2D texture, Vector2 PixelSize)
		{
			string path = AssetDatabase.GetAssetPath(texture);
			var importer = AssetImporter.GetAtPath(path) as TextureImporter;

			//importer.isReadable = true;
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.mipmapEnabled = false;
			importer.filterMode = FilterMode.Point;
			importer.spritePivot = Vector2.down;
			importer.textureCompression = TextureImporterCompression.Uncompressed;

			var textureSettings = new TextureImporterSettings(); // need this stupid class because spriteExtrude and spriteMeshType aren't exposed on TextureImporter
			importer.ReadTextureSettings(textureSettings);
			textureSettings.spriteMeshType = SpriteMeshType.Tight;
			textureSettings.spriteExtrude = 0;

			importer.SetTextureSettings(textureSettings);


			Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, Vector2.zero, PixelSize, Vector2.zero);
			var rectsList = new List<Rect>(rects);
			rectsList = SortRects(rectsList, texture.width);

			string filenameNoExtension = System.IO.Path.GetFileNameWithoutExtension(path);
			var metas = new List<SpriteMetaData>();
			int rectNum = 0;

			foreach (Rect rect in rectsList)
			{
				var meta = new SpriteMetaData
				{
					pivot = Vector2.down,
					alignment = (int)SpriteAlignment.BottomCenter,
					rect = rect,
					name = filenameNoExtension + "_" + rectNum++
				};
				metas.Add(meta);
			}

			importer.spritesheet = metas.ToArray();

			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}


		static List<Rect> SortRects(List<Rect> rects, float textureWidth)
		{
			List<Rect> list = new List<Rect>();
			while (rects.Count > 0)
			{
				Rect rect = rects[rects.Count - 1];
				Rect sweepRect = new Rect(0f, rect.yMin, textureWidth, rect.height);
				List<Rect> list2 = RectSweep(rects, sweepRect);
				if (list2.Count <= 0)
				{
					list.AddRange(rects);
					break;
				}
				list.AddRange(list2);
			}
			return list;
		}

		static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
		{
			List<Rect> result;
			if (rects == null || rects.Count == 0)
			{
				result = new List<Rect>();
			}
			else
			{
				List<Rect> list = new List<Rect>();
				foreach (Rect current in rects)
				{
					if (current.Overlaps(sweepRect))
					{
						list.Add(current);
					}
				}
				foreach (Rect current2 in list)
				{
					rects.Remove(current2);
				}
				list.Sort((a, b) => a.x.CompareTo(b.x));
				result = list;
			}
			return result;
		}
	}



	public static class ScriptableObjectUtility
	{
		/// <summary>
		//	This makes it easy to create, name and place unique new ScriptableObject asset files.
		/// </summary>
		public static T CreateAsset<T>(string name = "", string path = "") where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();


			if (path == "")
			{
				path = "Assets";
			}
			else if (System.IO.Path.GetExtension(path) != "")
			{
				path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

			AssetDatabase.CreateAsset(asset, assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;

			return asset;
		}

		public static T AddAsset<T>(object parentSO, string name = "") where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			asset.name = name;
			AssetDatabase.AddObjectToAsset(asset, (Object)parentSO);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;

			return asset;
		}
		[MenuItem("Assets/RemoveAsset")]
		public static void RemoveAsset(Object Object = null)
		{
			if (Object == null)
			{
				Object = Selection.activeObject;
			}
			AssetDatabase.RemoveObjectFromAsset(Object);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

		}


	}



}

public class SpriteImporter : AssetPostprocessor // AssetPostprocessor gives us a bunch of tools we can use to modifiy assets on import
{
	// This hooks in to texture Preprocessing. that means it happens *before* the texture is processed by unity so we can fiddle with its import settings!
	// This method will be run on ever asset as it is imported. For example, whenever a new png file is added to the project or if you right click -> reimport on a texture asset.
	void OnPreprocessTexture()
	{
		if (assetPath.Contains("Resources")) // Optional filtering. We can set it to only effect files with certain keywords in the path. Uncomment if useful
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter; // create a TextureImporter object, this gives us tools to let us tweak the import settings on the current asset

			if (textureImporter.textureType != TextureImporterType.Sprite) // if the type is already sprite, dont run this test so we dosnt screw existing settings
			{
				textureImporter.textureType = TextureImporterType.Sprite; // Turns the texture into a sprite..
				textureImporter.spriteImportMode = SpriteImportMode.Single; // .. lets assume that each sprite is singular. if its a sprite sheet you have to set that up yourself the normal way

				textureImporter.mipmapEnabled = false; // Turn off mip maps because they are *gross* 

				textureImporter.textureCompression = TextureImporterCompression.CompressedHQ; // and set compression mode to true colour cos it looks pretty~
			}
		}
	}
}





#endif