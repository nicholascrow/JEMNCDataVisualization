/**
	asset cleaner
	Copyright (c) 2015 Tatsuhiko Yamamura

    This software is released under the MIT License.
    http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;

namespace AssetClean
{
	public class ClassReferenceCollection
	{
		// type : path
		public Dictionary<System.Type, string> codeFileList = new Dictionary<System.Type, string> ();
		// guid : types
		public Dictionary<string, List<System.Type>> references = new Dictionary<string, List<System.Type>> ();

		public void Collection ()
		{
			references.Clear ();

			EditorUtility.DisplayProgressBar ("checking", "collection all type", 0);

			// Connect the files and class.
			var codes = Directory.GetFiles ("Assets", "*.cs", SearchOption.AllDirectories);

			var alltypes = CollectionAllClasses ();
			codeFileList = CollectionCodeFileDictionary (alltypes, codes);

			// connect each classes.
			int count = 0;
			foreach (var codepath in codes) {

				CollectionReferenceClasses (AssetDatabase.AssetPathToGUID (codepath), alltypes);
				EditorUtility.DisplayProgressBar ("checking", "analytics codes", (float)++count / codes.Length);
			}
		}

		Dictionary<System.Type, string>  CollectionCodeFileDictionary (List<System.Type> alltypes, string[] codes)
		{
			Dictionary<System.Type, string> codeFileList = new Dictionary<System.Type, string> ();

			float count = 1;
			foreach (var codePath in codes) {
				EditorUtility.DisplayProgressBar ("checking", "search files", count++ / codes.Length);

				// connect file and classes.
				var code = System.IO.File.ReadAllText (codePath);
				foreach (var type in alltypes) {
				
					if (codeFileList.ContainsKey (type))
						continue;
				
					if (string.IsNullOrEmpty (type.Namespace) == false) {
						var namespacepattern = string.Format ("namespace[\\s.]{0}[{{\\s\\n]", type.Namespace);
						if (Regex.IsMatch (code, namespacepattern) == false) {
							continue;
						}
					}

					string typeName = type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition ().Name.Split ('`') [0] : type.Name;
					if (Regex.IsMatch (code, string.Format ("class[\\s*]{0}[\\s\\n\\r:<{{]", typeName))) {
						codeFileList.Add (type, codePath);
						continue;
					}
				
					if (Regex.IsMatch (code, string.Format ("struct[\\s*]{0}[\\s\\n\\r:<{{]", typeName))) {
						codeFileList.Add (type, codePath);
						continue;
					}
				
					if (Regex.IsMatch (code, string.Format ("enum[\\s*]{0}[\\s\\n\\r{{]", type.Name))) {
						codeFileList.Add (type, codePath);
						continue;
					}
				
					if (Regex.IsMatch (code, string.Format ("delegate\\s*{0}\\s*\\(", type.Name))) {
						codeFileList.Add (type, codePath);
						continue;
					}
				}
			}
			return codeFileList;
		}

		List<System.Type> CollectionAllClasses ()
		{
			List<System.Type> alltypes = new List<System.Type> ();
		
			if (File.Exists ("Library/ScriptAssemblies/Assembly-CSharp.dll"))
				alltypes.AddRange (Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp.dll").GetTypes ());
			if (File.Exists ("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll"))
				alltypes.AddRange (Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll").GetTypes ());
			if (File.Exists ("Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll"))
				alltypes.AddRange (Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll").GetTypes ());

			return alltypes	.ToList ();
		}

		void CollectionReferenceClasses (string guid, List<System.Type> alltypes)
		{
			var codePath = AssetDatabase.GUIDToAssetPath (guid);
			if (string.IsNullOrEmpty (codePath) || references.ContainsKey (guid)) {
				return;
			}

			var code = System.IO.File.ReadAllText (codePath);
			var list = new List<System.Type> ();

			references [guid] = list;
	
			foreach (var type in alltypes) {

				if (string.IsNullOrEmpty (type.Namespace) == false) {
					var namespacepattern = string.Format ("[namespace|using][\\s\\.]{0}[{{\\s\\r\\n\\r;]", type.Namespace);
					if (Regex.IsMatch (code, namespacepattern) == false) {
						continue;
					}
				}

				if (codeFileList.ContainsKey (type) == false) {
					continue;
				}

				string match = string.Empty;
				if (type.IsGenericTypeDefinition) {
					string typeName = type.GetGenericTypeDefinition ().Name.Split ('`') [0];
					match = string.Format ("[\\]\\.\\s<(]{0}[\\.\\s\\n\\r>,<(){{]", typeName);
				} else {
					match = string.Format ("[\\]\\.\\s<(]{0}[\\.\\s\\n\\r>,<(){{]", type.Name);
				}
				if (Regex.IsMatch (code, match)) {
					list.Add (type);
					var path = codeFileList [type];
					CollectionReferenceClasses (AssetDatabase.AssetPathToGUID (path), alltypes);
				}
			}
		}
	}
}
