/**
	asset cleaner
	Copyright (c) 2015 Tatsuhiko Yamamura

    This software is released under the MIT License.
    http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

namespace AssetClean
{
	public class FindUnusedAssets : EditorWindow
	{
		AssetCollector corrector = new AssetCollector ();
		List<DeleteAsset> deleteAssets = new List<DeleteAsset> ();
		Vector2 scroll;

		[MenuItem("Assets/Delete Unused Assets from game", false, 50)]
		static void Init ()
		{
			var window = FindUnusedAssets.CreateInstance<FindUnusedAssets> ();
			window.corrector.Collection ();

			foreach (var asset in window.corrector.deleteFileList) {
				var filePath = AssetDatabase.GUIDToAssetPath (asset);
				if (string.IsNullOrEmpty (filePath) == false) {
					window.deleteAssets.Add (new DeleteAsset (){	path = filePath});
				}
			}
			window.Show ();
		}

		void OnGUI ()
		{
			using (var horizonal = new EditorGUILayout.HorizontalScope("box")) {
				EditorGUILayout.LabelField ("delete unreference assets from buildsettings and resources");

				if (GUILayout.Button ("Delete", GUILayout.Width (120), GUILayout.Height (40)) && deleteAssets.Count != 0) {
					RemoveFiles ();
					Close ();
				}
			}

			using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll)) {
				scroll = scrollScope.scrollPosition;
				foreach (var asset in deleteAssets) {
					if (string.IsNullOrEmpty (asset.path)) {
						continue;
					}

					using (var horizonal = new EditorGUILayout.HorizontalScope()) {
						asset.isDelete = EditorGUILayout.Toggle (asset.isDelete, GUILayout.Width (20));
						var icon = AssetDatabase.GetCachedIcon (asset.path);
						GUILayout.Label (icon, GUILayout.Width (20), GUILayout.Height (20));
						if (GUILayout.Button (asset.path, EditorStyles.largeLabel)) {
							Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object> (asset.path);
						}
					}
				}
			}

		}

		void RemoveFiles ()
		{
			try {
				string exportDirectry = "BackupUnusedAssets";
				Directory.CreateDirectory (exportDirectry);
				var files = deleteAssets.Where (item => item.isDelete == true).Select (item => item.path).ToArray ();
				string backupPackageName = exportDirectry + "/package" + System.DateTime.Now.ToString ("yyyyMMddHHmmss") + ".unitypackage";
				EditorUtility.DisplayProgressBar ("export package", backupPackageName, 0);
				AssetDatabase.ExportPackage (files, backupPackageName);

				int i = 0;
				int length = deleteAssets.Count;

				foreach (var asset in deleteAssets) {
					i++;
					if (asset.isDelete == false) {
						continue;
					}
					EditorUtility.DisplayProgressBar ("delete unused assets", asset.path, (float)i / length);
					AssetDatabase.DeleteAsset (asset.path);
				}

				EditorUtility.DisplayProgressBar ("clean directory", "", 1);
				foreach (var dir in Directory.GetDirectories("Assets")) {
					RemoveEmptyDirectry (dir);
				}

				System.Diagnostics.Process.Start (exportDirectry);

				AssetDatabase.Refresh ();

			} finally {
				EditorUtility.ClearProgressBar ();
			}
		}

		void RemoveEmptyDirectry (string path)
		{
			var dirs = Directory.GetDirectories (path);
			foreach (var dir in dirs) {
				RemoveEmptyDirectry (dir);
			}

			var files = Directory.GetFiles (path, "*", SearchOption.TopDirectoryOnly).Where (item => Path.GetExtension (item) != ".meta");
			if (files.Count () == 0 && Directory.GetDirectories (path).Count () == 0) {
				UnityEditor.FileUtil.DeleteFileOrDirectory (path);
			}
	
		}

		class DeleteAsset
		{
			public bool isDelete = true;
			public string path;
		}
	}
}
