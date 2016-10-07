/**
	asset cleaner
	Copyright (c) 2015 Tatsuhiko Yamamura

    This software is released under the MIT License.
    http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace AssetClean
{
	public class AssetCollector
	{
		public List<string> deleteFileList = new List<string> ();
		ClassReferenceCollection classCollection = new ClassReferenceCollection ();
		ShaderReferenceCollection shaderCollection = new ShaderReferenceCollection ();
		
		public void Collection ()
		{
			try {
				deleteFileList.Clear ();
				
				classCollection.Collection ();
				shaderCollection.Collection ();
				
				// Find assets
				var files = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
					.Where (item => Path.GetExtension (item) != ".meta")
						.Where (item => Path.GetExtension (item) != ".js")
						.Where (item => Path.GetExtension (item) != ".dll")
						.Where (item => Regex.IsMatch (item, "[\\/\\\\]Gizmos[\\/\\\\]") == false)
						.Where (item => Regex.IsMatch (item, "[\\/\\\\]Plugins[\\/\\\\]Android[\\/\\\\]") == false)
						.Where (item => Regex.IsMatch (item, "[\\/\\\\]Plugins[\\/\\\\]iOS[\\/\\\\]") == false)
						.Where (item => Regex.IsMatch (item, "[\\/\\\\]Resources[\\/\\\\]") == false);
				foreach (var path in files) {
					var guid = AssetDatabase.AssetPathToGUID (path);
					deleteFileList.Add (guid);
				}
				EditorUtility.DisplayProgressBar ("checking", "collection all files", 0.2f);
				
				// Exclude objects that reference from Resources.
				var resourcesFiles = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Resources[\\/\\\\]") == true)
						.Where (item => Path.GetExtension (item) != ".meta")
						.ToArray ();
				foreach (var refs in AssetDatabase.GetDependencies (resourcesFiles)) {
					UnregistFromDelteList (refs);
				}
				
				EditorUtility.DisplayProgressBar ("checking", "check reference from resources", 0.4f);
				
				// Exclude objects that reference from scenes.
				var scenes = EditorBuildSettings.scenes
					.Where (item => item.enabled == true)
						.Select (item => item.path)
						.ToArray ();
				foreach (var refs in AssetDatabase.GetDependencies (scenes)) {
					UnregistFromDelteList (refs);
				} 
				EditorUtility.DisplayProgressBar ("checking", "check reference from scenes", 0.6f);
				
				// Exclude objects that reference from Editor API
				var editorcodes = Directory.GetFiles ("Assets", "*.cs", SearchOption.AllDirectories)
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Editor[\\/\\\\]") == true)
						.ToArray ();
				
				var undeleteClassList = classCollection.codeFileList.Keys
					.Where (type => Regex.IsMatch (classCollection.codeFileList [type], "[\\/\\\\]Editor[\\/\\\\]") == false)
						.Where (type => deleteFileList.Contains (AssetDatabase.AssetPathToGUID (classCollection.codeFileList [type])) == false);
				
				EditorUtility.DisplayProgressBar ("checking", "check reference from editor codes", 0.8f);
				
				foreach (var refs in editorcodes) {
					var code = File.ReadAllText (refs);
					if (Regex.IsMatch (code, "(\\[MenuItem|AssetPostprocessor|EditorWindow)")) {
						UnregistFromDelteList (refs);
						continue;
					}
					
					foreach (var undeleteClass in undeleteClassList) {
						if (Regex.IsMatch (code, string.Format ("\\[CustomEditor.*\\(\\s*{0}\\s*\\).*\\]", undeleteClass.Name))) {
							UnregistFromDelteList (refs);
							continue;
						}
					}
				}
			} finally {
				EditorUtility.ClearProgressBar ();
			}
			
		}
		
		void UnregistFromDelteList (string path)
		{
			var guid = AssetDatabase.AssetPathToGUID (path);
			if (deleteFileList.Contains (guid) == false) {
				return;
			}
			deleteFileList.Remove (guid);
			
			if (classCollection.references.ContainsKey (guid) == true) {
				
				foreach (var type in classCollection.references[guid]) {
					var codePath = classCollection.codeFileList [type];
					UnregistFromDelteList (codePath);
				}
			}
			
			if (shaderCollection.shaderFileList.ContainsValue (path)) {
				var shader = shaderCollection.shaderFileList.First (item => item.Value == path);
				var shaderAssets = shaderCollection.shaderReferenceList [shader.Key];
				foreach (var shaderPath in shaderAssets) {
					UnregistFromDelteList (shaderPath);
				}
			}
		}
	}
}
