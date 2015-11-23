//#define FUSE_USE_SSL
#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;

public static class FusePostProcess
{
	// Frameworks Ids  -  These ids have been generated by creating a project using Xcode then
	// extracting the values from the generated project.pbxproj.  The format of this
	// file is not documented by Apple so the correct algorithm for generating these
	// ids is unknown
	const string CORETELEPHONY_ID = "3F3EE17B1757FB570038DED5";
	const string ADSUPPORT_ID = "3F3EE1791757FB4D0038DED6";
	const string STOREKIT_ID = "3F3EE17D1757FB610038DED7";
	const string MESSAGEUI_ID = "2076D92010F4D46C00CEE78A";
	const string EVENTKIT_ID = "3FBCE7851816DD570057C056";
	const string EVENTKITUI_ID = "3FBCE7861816DD570057C058";
	const string TWITTER_ID = "3FBCE7861816DD570057C060";
	const string SOCIAL_ID = "3FBCE7861816DD570057C062";
	const string SECURITY_ID = "3FBCE7861816DD570057C065";
	const string SQLITE_ID = "3FBCE7861816DD570057C069";
	const string MCORESERVICES_ID = "3FD2BD0F1A253866002566B8";
	const string LIBXML_ID = "3FD2BD0F1A253866002566B9";
	const string WEBKIT_ID = "CDECA1A21B02531000CAA921";
    const string GAMEKIT_ID = "CDECA1A21B02531000CAA931";


    const string CORETELEPHONY_FW = "3F3EE17B1757FB570038DED8";
	const string ADSUPPORT_FW = "3F3EE1791757FB4D0038DED9";
	const string STOREKIT_FW = "3F3EE17D1757FB610038DED0";
	const string MESSAGEUI_FW = "2076D92010F4D46C00CEE78B";
	const string EVENTKIT_FW = "3FBCE7851816DD570057C057";
	const string EVENTKITUI_FW = "3FBCE7861816DD570057C059";
	const string TWITTER_FW = "3F3EE17B1757FB570038DED9";
	const string SOCIAL_FW = "3F3EE17B1757FB570038DEEE";
	const string SECURITY_FW = "3F3EE17B1757FB570038DEED";
	const string SQLITE_FW = "3F3EE17B1757FB570038FEED";
	const string MCORESERVICES_FW = "A4FBC40B1A23EC33004D9A01";
	const string LIBXML_FW = "A4FBC40B1A23EC33004D9A02";
	const string WEBKIT_FW = "CDECA1A11B02531000CAA922";
    const string GAMEKIT_FW = "CDECA1A11B02531000CAA932";

    // List of all the frameworks to be added to the project
    public struct framework
	{
		public string sName;
		public string sId;
		public string sFileId;

		public framework(string name, string myId, string fileid)
		{
			sName = name;
			sId = myId;
			sFileId = fileid;
		}
	}

	/// Processbuild Function
	[PostProcessBuild] // <- this is where the magic happens
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		UnityEngine.Debug.Log("FusePostProcess Build Step - START");
		// iOS post-process
#if UNITY_5
		if( target == BuildTarget.iOS )
#else
		if(target == BuildTarget.iPhone)
#endif
		{
			// 2: We init our tab and process our project
			framework[] myFrameworks = { new framework("CoreTelephony.framework", CORETELEPHONY_FW, CORETELEPHONY_ID),
										 new framework("AdSupport.framework", ADSUPPORT_FW, ADSUPPORT_ID),
										 new framework("StoreKit.framework", STOREKIT_FW, STOREKIT_ID),
										 new framework("MessageUI.framework", MESSAGEUI_FW, MESSAGEUI_ID),
										 new framework("EventKit.framework", EVENTKIT_FW, EVENTKIT_ID),
										 new framework("EventKitUI.framework", EVENTKITUI_FW, EVENTKITUI_ID),
										 new framework("Twitter.framework", TWITTER_FW, TWITTER_ID),
										 new framework("Social.framework", SOCIAL_FW, SOCIAL_ID),
										 new framework("Security.framework", SECURITY_FW, SECURITY_ID),
										 new framework("MobileCoreServices.framework", MCORESERVICES_FW, MCORESERVICES_ID),
										 new framework("libsqlite3.tbd", SQLITE_FW, SQLITE_ID),
										 new framework("libxml2.tbd", LIBXML_FW, LIBXML_ID),
										 new framework("WebKit.framework", WEBKIT_FW, WEBKIT_ID),
                                         new framework("GameKit.framework", GAMEKIT_FW, GAMEKIT_ID),
                                        };

			string xcodeprojPath = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);

			xcodeprojPath = xcodeprojPath + "/Unity-iPhone.xcodeproj";
			UnityEngine.Debug.Log("XcodeprojPath should be : " + xcodeprojPath);

			updateXcodeProject(xcodeprojPath, myFrameworks);
		}

		if(target == BuildTarget.Android)
		{
			var ver = Application.unityVersion;

			if((ver.Contains("4.6.1") && !ver.Contains("p5"))
				|| ver.Contains("4.6.2f1")
				|| ver.Contains("4.6.2p1"))
			{
				UnityEngine.Debug.LogError("There are known bugs in this version of Unity. This app will not run on Android 5.0.");
				UnityEngine.Debug.LogError("Please update your version of Unity to 4.6.1p5, 4.6.2p2 or higher. Visit http://unity3d.com/unity/qa/patch-releases for more detail (Bug 668393).");
			}
		}

#if UNITY_5
		if(target == BuildTarget.iOS)
		{
			var ver = Application.unityVersion;

			if(ver.Contains("5.1.1") || ver.Contains("5.1.2"))
			{
				UnityEngine.Debug.LogError("There are known bugs in this version of Unity. The app will not function properly on iOS 7.");
				UnityEngine.Debug.LogError("Please use Unity version 5.0.*");
			}
		}
#endif

		UnityEngine.Debug.Log("FusePostProcess - STOP");
	}

	[PostProcessScene] // <- for old version cleanup
	public static void OnPostProcessScene()
	{
		try
		{
			// delete older versions of API jar
			for(int i = 0; i < 10; i++)
			{
				string oldAPIjar = "Assets/Plugins/Android/FuseAndroidAPI_v1.2" + i + ".jar";
				AssetDatabase.DeleteAsset(oldAPIjar);
			}

			AssetDatabase.DeleteAsset("Assets/Plugins/Android/FuseAPI.jar");
			AssetDatabase.DeleteAsset("Assets/Plugins/Android/FuseUnityAPI.jar");

			AssetDatabase.DeleteAsset("Assets/Plugins/iOS/FuseAPI.h");
			AssetDatabase.DeleteAsset("Assets/Plugins/iOS/libFuseAPI.a");

			AssetDatabase.DeleteAsset("Assets/Plugins/FuseNativeAPI.dll");

			if(File.Exists(Application.dataPath + "/Plugins/FuseSDK.NET-Stub.dll") && File.Exists(Application.dataPath + "/Plugins/FuseSDK.NET.dll"))
			{
				AssetDatabase.DeleteAsset("Assets/Plugins/FuseSDK.NET.dll");
			}

			if(Directory.Exists(Application.dataPath + "/FuseAPI"))
			{
				UnityEngine.Debug.LogError("FuseSDK: Please remove the Assets/FuseAPI folder from your project. The FuseSDk is now located in Assets/FuseSDK.");
			}

			if(Directory.Exists(Application.dataPath + "/Plugins/Android/libs"))
			{
				if(Directory.Exists(Application.dataPath + "/Plugins/Android/libs/x86"))
				{
					if(File.Exists(Application.dataPath + "/Plugins/Android/libs/x86/libFuseCommonCore.so"))
					{
						AssetDatabase.DeleteAsset("Plugins/Android/libs/x86/libFuseCommonCore.so");
					}

					if(Directory.GetFiles(Application.dataPath + "/Plugins/Android/libs/x86").Length == 0 && Directory.GetDirectories(Application.dataPath + "/Plugins/Android/libs/x86").Length == 0)
					{
						Directory.Delete(Application.dataPath + "/Plugins/Android/libs/x86");
						if(File.Exists(Application.dataPath + "/Plugins/Android/libs/x86.meta"))
							File.Delete(Application.dataPath + "/Plugins/Android/libs/x86.meta");
                    }
				}

				if(Directory.Exists(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a"))
				{
					if(File.Exists(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a/libFuseCommonCore.so"))
					{
						AssetDatabase.DeleteAsset("Plugins/Android/libs/armeabi-v7a/libFuseCommonCore.so");
					}

					if(Directory.GetFiles(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a").Length == 0 && Directory.GetDirectories(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a").Length == 0)
					{
						Directory.Delete(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a");
						if(File.Exists(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a.meta"))
							File.Delete(Application.dataPath + "/Plugins/Android/libs/armeabi-v7a.meta");
					}
				}

				if(Directory.GetFiles(Application.dataPath + "/Plugins/Android/libs").Length == 0 && Directory.GetDirectories(Application.dataPath + "/Plugins/Android/libs").Length == 0)
				{
					Directory.Delete(Application.dataPath + "/Plugins/Android/libs");
					if(File.Exists(Application.dataPath + "/Plugins/Android/libs.meta"))
						File.Delete(Application.dataPath + "/Plugins/Android/libs.meta");
				}
			}

			if(Application.platform == RuntimePlatform.Android)
			{
				EditorApplication.ExecuteMenuItem("FuseSDK/Update Android Manifest");
			}
		}
		catch
		{ }
	}

	// MAIN FUNCTION
	// xcodeproj_filename - filename of the Xcode project to change
	// frameworks - list of Apple standard frameworks to add to the project
	static bool bFoundCore = false;
	static bool bFoundAd = false;
	static bool bFoundStore = false;
	static bool bFoundMessage = false;
	static bool bFoundEvent = false;
	static bool bFoundEventUI = false;
	static bool bFoundTwitter = false;
	static bool bFoundSocial = false;
	static bool bFoundSecurity = false;
	static bool bFoundSQLite = false;
	static bool bFoundMCS = false;
	static bool bFoundLibXML = false;
	static bool bFoundWebKit = false;
    static bool bFoundGameKit = false;
	public static void updateXcodeProject(string xcodeprojPath, framework[] listeFrameworks)
	{
		//Modify Info.plist
		string infoPath = xcodeprojPath + "/../Info.plist";
		if(File.Exists(infoPath))
		{
			string[] plistLines = File.ReadAllLines(infoPath);
			List<string> newPlist = new List<string>(plistLines);
			long len = plistLines.Length - 1;
			int insertLine = -1; //If the entry is found this will be kept at -1

			//Add an SSL flag if enabled
#if FUSE_USE_SSL
			for(int l = 0; l < len; l++)
			{
				if(newPlist[l].Contains("plist") && newPlist[l+1].Contains("dict"))
				{
					insertLine = l + 2;
				}

				if(newPlist[l].Contains("fuse_ssl"))
				{
					if(newPlist[l+1].Contains("false"))
					{
						newPlist[l+1] = newPlist[l+1].Replace("false", "true");
					}
					insertLine = -1;
					break;
				}
			}

			//If the flag doesn't exist, add it now
			if(insertLine != -1)
			{
				newPlist.Insert(insertLine, @"    <key>fuse_ssl</key>");
				newPlist.Insert(insertLine + 1, @"    <true/>");
			}
#endif
			//Add the NSAppTransportSecurity entry if it doesnt exist
			insertLine = -1;
			len = newPlist.Count - 1;
			for(int l = 0; l < len; l++)
			{
				if(newPlist[l].Contains("plist") && newPlist[l + 1].Contains("dict"))
				{
					insertLine = l + 2;
				}

				if(newPlist[l].Contains("NSAppTransportSecurity"))
				{
					insertLine = -1;
					break;
				}
			}

			//If the flag doesn't exist, add it now
			if(insertLine != -1)
			{
				newPlist.Insert(insertLine, @"    <key>NSAppTransportSecurity</key>");
				newPlist.Insert(insertLine + 1, @"    <dict>");
				newPlist.Insert(insertLine + 2, @"        <key>NSAllowsArbitraryLoads</key>");
				newPlist.Insert(insertLine + 3, @"        <true/>");
				newPlist.Insert(insertLine + 4, @"    </dict>");
			}


			//Write out the new plist
			File.WriteAllLines(infoPath, newPlist.ToArray());
		}
		else
		{
			UnityEngine.Debug.LogError("Could not find Info.plist. You will need to edit this file manually");
		}



		// STEP 1 : We open up the file generated by Unity and read into memory as
		// a list of lines for processing
		string project = xcodeprojPath + "/project.pbxproj";
		if(!System.IO.File.Exists(project))
		{
			UnityEngine.Debug.LogError("Could not find Xcode project at the expected location.  You will need to manually add CoreTelephony, AdSupport, MessageUI, EventKit, EventKitUI, Twitter, Social, and StoreKit frameworks and the -ObjC linker flag");
			return;
		}


#if UNITY_IOS
//		Uncomment to Test xml project settings
//		ProcessStartInfo proc = new ProcessStartInfo();
//		proc.FileName = "plutil";
//		proc.WorkingDirectory = xcodeprojPath;
//		proc.Arguments = "-convert xml1 project.pbxproj";
//		proc.WindowStyle = ProcessWindowStyle.Minimized;
//		proc.CreateNoWindow = true;
//		Process process = Process.Start(proc);
//		process.WaitForExit();
//		UnityEngine.Debug.Log("Converting project.pbxProj to xml");
#endif


		string[] lines = System.IO.File.ReadAllLines(project);

		// STEP 2 : We process only the missing frameworks

		bool isXML = (string.Compare(lines[0].Substring(2, 3), "xml") == 0) || (string.Compare(lines[0].Substring(2, 3), "Xml") == 0);

		int i = 0;
		bool bEnd = false;
		while(!bEnd && i < lines.Length)
		{
			if(lines[i].Length > 5 && (string.Compare(lines[i].Substring(3, 3), "End") == 0) && !isXML)
				bEnd = true;

			if(lines[i].Contains("CoreTelephony.framework"))
			{
				bFoundCore = true;
			}
			else if(lines[i].Contains("AdSupport.framework"))
			{
				bFoundAd = true;
			}
			else if(lines[i].Contains("StoreKit.framework"))
			{
				bFoundStore = true;
			}
			else if(lines[i].Contains("MessageUI.framework"))
			{
				bFoundMessage = true;
			}
			else if(lines[i].Contains("EventKit.framework"))
			{
				bFoundEvent = true;
			}
			else if(lines[i].Contains("EventKitUI.framework"))
			{
				bFoundEventUI = true;
			}
			else if(lines[i].Contains("Twitter.framework"))
			{
				bFoundTwitter = true;
			}
			else if(lines[i].Contains("Social.framework"))
			{
				bFoundSocial = true;
			}
			else if(lines[i].Contains("Security.framework"))
			{
				bFoundSecurity = true;
			}
			else if(lines[i].Contains("WebKit.framework"))
			{
				bFoundWebKit = true;
			}
            else if (lines[i].Contains("GameKit.framework"))
            {
                bFoundGameKit = true;
            }
            else if(lines[i].Contains("libsqlite3.tbd"))
			{
				bFoundSQLite = true;
			}
			else if(lines[i].Contains("MobileCoreServices.framework"))
			{
				bFoundMCS = true;
			}
			else if(lines[i].Contains("libxml2.tbd"))
			{
				bFoundLibXML = true;
			}

			++i;
		}


		// STEP 3 : We'll open/replace project.pbxproj for writing and iterate over the old
		// file in memory, copying the original file and inserting every extra we need

		if(isXML)
		{

			UnityEngine.Debug.Log("project.pbxProj is xml, using XML script");
			XmlDocument projXml = new XmlDocument();
			projXml.PreserveWhitespace = true;
			projXml.LoadXml(System.IO.File.ReadAllText(project));

			XmlNode outerDict = null;
			XmlNode innerDict = null;

			foreach(XmlNode node in projXml.ChildNodes)
				if(node.Name == "plist")
					foreach(XmlNode child in node.ChildNodes)
						if(child.Name == "dict")
							outerDict = child;

			if(outerDict != null)
			{
				foreach(XmlNode node in outerDict.ChildNodes)
					if(node.Name == "dict" && node.HasChildNodes)
						innerDict = node;
			}

			if(innerDict != null)
			{
				foreach(framework fr in listeFrameworks)
				{
					add_build_file_xml(innerDict, fr.sId, fr.sName, fr.sFileId);
					add_framework_file_reference_xml(innerDict, fr.sFileId, fr.sName);
					add_frameworks_build_phase_xml(innerDict, fr.sId, fr.sName);
					add_group_xml(innerDict, fr.sFileId, fr.sName);

				}

				add_ldFlags_xml(innerDict, "-ObjC");



				FileStream filestr = new FileStream(project, FileMode.Create); //Create new file and open it for read and write, if the file exists overwrite it.
				filestr.Close();
				projXml.Save(project);

				//this is gonna be a dirty, dirty hack.
				string[] lines2 = System.IO.File.ReadAllLines(project);
				lines2[1] = lines[1];
				UnityEngine.Debug.Log("fixing line: " + lines2[1]);
				filestr = new FileStream(project, FileMode.Create); //Create new file and open it for read and write, if the file exists overwrite it.
				filestr.Close();
				StreamWriter fCurrentXcodeProjFile = new StreamWriter(project); // will be used for writing
				foreach(string line in lines2)
				{
					fCurrentXcodeProjFile.WriteLine(line);
				}
				fCurrentXcodeProjFile.Close();
			}
			else
			{

				UnityEngine.Debug.Log("project.pbxProj is xml but not in expected format! Please contact fuse support for more information");

			}

		}
		else
		{

			FileStream filestr = new FileStream(project, FileMode.Create); //Create new file and open it for read and write, if the file exists overwrite it.
			filestr.Close();
			StreamWriter fCurrentXcodeProjFile = new StreamWriter(project); // will be used for writing


			// As we iterate through the list we'll record which section of the
			// project.pbxproj we are currently in
			string section = "";

			// We use this boolean to decide whether we have already added the list of
			// build files to the link line.  This is needed because there could be multiple
			// build targets and they are not named in the project.pbxproj
			bool bFrameworks_build_added = false;
			int iNbBuildConfigSet = 0; // can't be > 2

			i = 0;
			foreach(string line in lines)
			{
				// set Foundation.framework to weak linked
				if(line.Contains("/* Foundation.framework in Frameworks */ = {isa = PBXBuildFile;") && !line.Contains("settings = {ATTRIBUTES = (Weak, ); };"))
				{
					// rewrite the line to set the library as weak linked
					UnityEngine.Debug.Log("Setting Foundation.framkework to weak link.");
					string[] splitstring = line.Split(';');
					string output = splitstring[0] + ";" + splitstring[1] + "; " + "settings = {ATTRIBUTES = (Weak, ); }; };";
					fCurrentXcodeProjFile.WriteLine(output);
				}
				//Mark FuseUnitySDK.m as -fno-objc-arc
				else if(line.Contains("/* FuseUnitySDK.m in Sources */ = {isa = PBXBuildFile;") && !line.Contains(@"settings = {COMPILER_FLAGS = ""-fno-objc-arc""; };"))
				{
					// rewrite the line to set the library as weak linked
					UnityEngine.Debug.Log("Marking FuseUnitySDK.m as -fno-objc-arc.");
					string[] splitstring = line.Split(';');
					string output = string.Join(";", splitstring, 0, splitstring.Length - 2) + @"; settings = {COMPILER_FLAGS = ""-fno-objc-arc""; }; };";
					fCurrentXcodeProjFile.WriteLine(output);
				}
				//Mark FuseUnityDebug.m as -fno-objc-arc
				else if(line.Contains("/* FuseUnityDebug.m in Sources */ = {isa = PBXBuildFile;") && !line.Contains(@"settings = {COMPILER_FLAGS = ""-fno-objc-arc""; };"))
				{
					// rewrite the line to set the library as weak linked
					UnityEngine.Debug.Log("Marking FuseUnityDebug.m as -fno-objc-arc.");
					string[] splitstring = line.Split(';');

					string output = string.Join(";", splitstring, 0, splitstring.Length - 2) + @"; settings = {COMPILER_FLAGS = ""-fno-objc-arc""; }; };";
					fCurrentXcodeProjFile.WriteLine(output);
				}
				//Mark NSData-Base64.m as -fno-objc-arc
				else if(line.Contains("/* NSData-Base64.m in Sources */ = {isa = PBXBuildFile;") && !line.Contains(@"settings = {COMPILER_FLAGS = ""-fno-objc-arc""; };"))
				{
					// rewrite the line to set the library as weak linked
					UnityEngine.Debug.Log("Marking NSData-Base64.m as -fno-objc-arc.");
					string[] splitstring = line.Split(';');

					string output = string.Join(";", splitstring, 0, splitstring.Length - 2) + @"; settings = {COMPILER_FLAGS = ""-fno-objc-arc""; }; };";
					fCurrentXcodeProjFile.WriteLine(output);
				}
				else
				{
					fCurrentXcodeProjFile.WriteLine(line);
				}

				//////////////////////////////////
				//  STEP 2 : Include Framewoks  //
				//////////////////////////////////                    
				// Each section starts with a comment such as : /* Begin PBXBuildFile section */'
				if(lines[i].Length > 7 && string.Compare(lines[i].Substring(3, 5), "Begin") == 0)
				{
					section = line.Split(' ')[2];
					//Debug.Log("NEW_SECTION: "+section);
					if(section == "PBXBuildFile")
					{
						foreach(framework fr in listeFrameworks)
							add_build_file(fCurrentXcodeProjFile, fr.sId, fr.sName, fr.sFileId);
					}

					if(section == "PBXFileReference")
					{
						foreach(framework fr in listeFrameworks)
							add_framework_file_reference(fCurrentXcodeProjFile, fr.sFileId, fr.sName);
					}

					if(line.Length > 5 && string.Compare(line.Substring(3, 3), "End") == 0)
						section = "";
				}
				// The PBXResourcesBuildPhase section is what appears in XCode as 'Link
				// Binary With Libraries'.  As with the frameworks we make the assumption the
				// first target is always 'Unity-iPhone' as the name of the target itself is
				// not listed in project.pbxproj
				if(section == "PBXFrameworksBuildPhase" &&
						line.Trim().Length > 4 &&
						string.Compare(line.Trim().Substring(0, 5), "files") == 0 &&
						!bFrameworks_build_added)
				{
					foreach(framework fr in listeFrameworks)
						add_frameworks_build_phase(fCurrentXcodeProjFile, fr.sId, fr.sName);
					bFrameworks_build_added = true;
				}

				// The PBXGroup is the section that appears in XCode as 'Copy Bundle Resources'.
				if(section == "PBXGroup" &&
						line.Trim().Length > 7 &&
						string.Compare(line.Trim().Substring(0, 8), "children") == 0 &&
						lines[i - 2].Trim().Split(' ').Length > 0 &&
						string.Compare(lines[i - 2].Trim().Split(' ')[2], "CustomTemplate") == 0)
				{
					foreach(framework fr in listeFrameworks)
						add_group(fCurrentXcodeProjFile, fr.sFileId, fr.sName);
				}

				//////////////////////////////
				//  STEP 3 : Build Options  //
				//////////////////////////////
				if(section == "XCBuildConfiguration" &&
						line.StartsWith("\t\t\t\tOTHER_LDFLAGS") &&
						iNbBuildConfigSet < 2)
				{
					int j = 0;
					bool bFlagSet = false;
					while(string.Compare(lines[i + j].Trim(), "};") != 0)
					{
						if(lines[i + j].Contains("ObjC"))
						{
							bFlagSet = true;
						}
						j++;
					}
					if(!bFlagSet)
					{
						//fCurrentXcodeProjFile.Write("\t\t\t\t\t\"-all_load\",\n");
						fCurrentXcodeProjFile.Write("\t\t\t\t\t\"-ObjC\",\n");
						UnityEngine.Debug.Log("OnPostProcessBuild - Adding \"-ObjC\" flag to build options"); // \"-all_load\" and
					}
					++iNbBuildConfigSet;
				}
				i++;
			}
			fCurrentXcodeProjFile.Close();
		}



	}


	/////////////////
	///////////
	// ROUTINES
	///////////
	/////////////////

	// check to see if the framework has already been processed
	private static bool should_process_framework(string name)
	{
		if((bFoundCore && name.Equals("CoreTelephony.framework"))
			|| (bFoundAd && name.Equals("AdSupport.framework"))
			|| (bFoundStore && name.Equals("StoreKit.framework"))
			|| (bFoundMessage && name.Equals("MessageUI.framework"))
			|| (bFoundEvent && name.Equals("EventKit.framework"))
			|| (bFoundEventUI && name.Equals("EventKitUI.framework"))
			|| (bFoundTwitter && name.Equals("Twitter.framework"))
			|| (bFoundSocial && name.Equals("Social.framework"))
			|| (bFoundSecurity && name.Equals("Security.framework"))
			|| (bFoundWebKit && name.Equals("WebKit.framework"))
            || (bFoundGameKit && name.Equals("GameKit.framework"))
            || (bFoundSQLite && name.Equals("libsqlite3.tbd"))
			|| (bFoundLibXML && name.Equals("libxml2.tbd"))
			|| (bFoundMCS && name.Equals("mobileCoreServices.framework")))
		{
			// framework is already in the xcode project - do no process it
			return false;
		}

		// the framework doesn't exist in the xcode project
		return true;
	}


	// Adds a entry for  PBXBuildFile section into an xml
	private static void add_build_file_xml(XmlNode innerDict, string id, string name, string fileref)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			//Debug.Log(name + " already exists in xcode project");
			return;
		}

		UnityEngine.Debug.Log("OnPostProcessBuild - Adding build file (xml) - " + name);
		XmlDocument doc = innerDict.OwnerDocument;

		XmlNode newChildKey = doc.CreateElement("key");
		newChildKey.InnerText = id;
		innerDict.AppendChild(newChildKey);

		XmlNode newChildDict = doc.CreateElement("dict");
		{
			XmlNode fileRefKeyNode = doc.CreateElement("key");
			fileRefKeyNode.InnerText = "fileRef";
			XmlNode fileRefNode = doc.CreateElement("string");
			fileRefNode.InnerText = fileref;

			newChildDict.AppendChild(fileRefKeyNode);
			newChildDict.AppendChild(fileRefNode);

			XmlNode isaKeyNode = doc.CreateElement("key");
			isaKeyNode.InnerText = "isa";
			XmlNode isaNode = doc.CreateElement("string");
			isaNode.InnerText = "PBXBuildFile";

			newChildDict.AppendChild(isaKeyNode);
			newChildDict.AppendChild(isaNode);

			XmlNode settingsKeyNode = doc.CreateElement("key");
			settingsKeyNode.InnerText = "settings";
			XmlNode settingsDictNode = doc.CreateElement("dict");
			XmlNode attribKeyNode = doc.CreateElement("key");
			attribKeyNode.InnerText = "ATTRIBUTES";
			settingsDictNode.AppendChild(attribKeyNode);
			XmlNode attribArrayNode = doc.CreateElement("array");
			XmlNode attribNode = doc.CreateElement("string");
			attribNode.InnerText = "Weak";
			attribArrayNode.AppendChild(attribNode);
			settingsDictNode.AppendChild(attribArrayNode);

			newChildDict.AppendChild(settingsKeyNode);
			newChildDict.AppendChild(settingsDictNode);
		}

		innerDict.AppendChild(newChildDict);

	}

	// Adds a line into the PBXBuildFile section
	private static void add_framework_file_reference_xml(XmlNode innerDict, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}

		UnityEngine.Debug.Log("OnPostProcessBuild - Adding framework file reference (xml) - " + name);

		string path = "System/Library/Frameworks"; // all the frameworks come from here
		if(name == "libsqlite3.tbd" || name == "libxml2.tbd")           // except for tbds
		{
			path = "usr/lib";
		}

		XmlDocument doc = innerDict.OwnerDocument;

		XmlNode newChildKey = doc.CreateElement("key");
		newChildKey.InnerText = id;
		innerDict.AppendChild(newChildKey);

		XmlNode newChildDict = doc.CreateElement("dict");
		{

			XmlNode isaKeyNode = doc.CreateElement("key");
			isaKeyNode.InnerText = "isa";
			newChildDict.AppendChild(isaKeyNode);
			XmlNode isaNode = doc.CreateElement("string");
			isaNode.InnerText = "PBXFileReference";
			newChildDict.AppendChild(isaNode);

			XmlNode fileTypeKeyNode = doc.CreateElement("key");
			fileTypeKeyNode.InnerText = "lastKnownFileType";
			XmlNode fileTypeNode = doc.CreateElement("string");
			fileTypeNode.InnerText = "wrapper.framework";

			newChildDict.AppendChild(fileTypeKeyNode);
			newChildDict.AppendChild(fileTypeNode);

			XmlNode fileNameKeyNode = doc.CreateElement("key");
			fileNameKeyNode.InnerText = "name";
			newChildDict.AppendChild(fileNameKeyNode);
			XmlNode fileNameNode = doc.CreateElement("string");
			fileNameNode.InnerText = name;
			newChildDict.AppendChild(fileNameNode);


			XmlNode filePathKeyNode = doc.CreateElement("key");
			filePathKeyNode.InnerText = "path";
			XmlNode filePathNode = doc.CreateElement("string");
			filePathNode.InnerText = path + "/" + name;

			newChildDict.AppendChild(filePathKeyNode);
			newChildDict.AppendChild(filePathNode);

			XmlNode sourceTreeKeyNode = doc.CreateElement("key");
			sourceTreeKeyNode.InnerText = "sourceTree";
			XmlNode sourceTreeNode = doc.CreateElement("string");
			sourceTreeNode.InnerText = "SDKROOT";

			newChildDict.AppendChild(sourceTreeKeyNode);
			newChildDict.AppendChild(sourceTreeNode);

		}

		innerDict.AppendChild(newChildDict);


	}

	// Adds a line into the PBXFrameworksBuildPhase section
	private static void add_frameworks_build_phase_xml(XmlNode innerdict, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}
		UnityEngine.Debug.Log("OnPostProcessBuild - Adding build phase (xml) - " + name);

		XmlDocument doc = innerdict.OwnerDocument;

		//Find Build phase sections
		XmlNode targetNode = null;
		foreach(XmlNode nodes in innerdict.ChildNodes)
		{
			if(nodes.HasChildNodes && nodes.Name == "dict")
			{
				foreach(XmlNode node in nodes.ChildNodes)
				{
					if(node.Name == "string" && node.InnerText == "PBXFrameworksBuildPhase")
					{
						targetNode = nodes;
						break;
					}
				}
			}
			if(targetNode != null)
			{
				break;
			}
		}

		if(targetNode != null)
		{
			XmlNode arrayNode = null;
			foreach(XmlNode node in targetNode.ChildNodes)
			{
				if(node.Name == "array")
				{
					arrayNode = node;
					break;
				}
			}

			if(arrayNode != null)
			{
				XmlNode insert = doc.CreateElement("string");
				insert.InnerText = id;
				arrayNode.AppendChild(insert);
			}
		}
	}

	// Adds a line into the PBXGroup section
	private static void add_group_xml(XmlNode innerdict, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}
		UnityEngine.Debug.Log("OnPostProcessBuild - Add group (xml) - " + name);

		XmlDocument doc = innerdict.OwnerDocument;

		//Find group section

		XmlNode targetNode = null;
		foreach(XmlNode nodes in innerdict.ChildNodes)
		{
			if(nodes.HasChildNodes && nodes.Name == "dict")
			{
				foreach(XmlNode node in nodes.ChildNodes)
				{
					if(node.Name == "string" && node.InnerText == "CustomTemplate")
					{
						targetNode = nodes;
						break;
					}
				}
			}
			if(targetNode != null)
			{
				break;
			}
		}

		if(targetNode != null)
		{
			XmlNode arrayNode = null;
			foreach(XmlNode node in targetNode.ChildNodes)
			{
				if(node.Name == "array")
				{
					arrayNode = node;
					break;
				}
			}

			if(arrayNode != null)
			{
				XmlNode insert = doc.CreateElement("string");
				insert.InnerText = id;
				arrayNode.AppendChild(insert);
			}
		}
	}


	private static void add_ldFlags_xml(XmlNode innerdict, string flag)
	{
		XmlDocument doc = innerdict.OwnerDocument;

		UnityEngine.Debug.Log("Inserting Link flag: " + flag);

		//Find group section

		XmlNode targetNode = null;

		foreach(XmlNode nodes in innerdict.ChildNodes)
		{
			if(nodes.HasChildNodes && nodes.Name == "dict")
			{
				foreach(XmlNode node in nodes.ChildNodes)
				{
					if(node.Name == "key" && node.InnerText == "buildSettings")
					{
						targetNode = nodes;
						break;
					}
				}
			}
			if(targetNode != null)
			{

				//We have found a target node, add ldFlag and proceed
				foreach(XmlNode node in targetNode.ChildNodes)
				{
					if(node.HasChildNodes && node.Name == "dict")
					{
						bool processNextArray = false;
						foreach(XmlNode subnode in node.ChildNodes)
						{
							if(processNextArray && subnode.Name == "array")
							{
								bool noExists = true;
								foreach(XmlNode arrayItem in subnode.ChildNodes)
								{
									if(arrayItem.Name == "string" && arrayItem.InnerText == flag)
									{
										noExists = false;
									}
								}
								if(noExists)
								{
									XmlNode insert = doc.CreateElement("string");
									insert.InnerText = flag;
									subnode.AppendChild(insert);
								}
								processNextArray = false;
							}
							if(subnode.Name == "key" && subnode.InnerText == "OTHER_LDFLAGS")
							{
								processNextArray = true;
							}
						}
					}
				}

				targetNode = null;
			}
		}
	}


	// Adds a line into the PBXBuildFile section
	private static void add_build_file(StreamWriter file, string id, string name, string fileref)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			//Debug.Log(name + " already exists in xcode project");
			return;
		}

		UnityEngine.Debug.Log("OnPostProcessBuild - Adding build file " + name);
		string subsection = "Frameworks";

		// optional frameworks (currently all)
		file.Write("\t\t" + id + " /* " + name + " in " + subsection + " */ = {isa = PBXBuildFile; fileRef = " + fileref + " /* " + name + " */; settings = {ATTRIBUTES = (Weak, ); }; };\n");
		//        else // required frameworks
		//            file.Write("\t\t"+id+" /* "+name+" in "+subsection+" */ = {isa = PBXBuildFile; fileRef = "+fileref+" /* "+name+" */; };\n");
	}

	// Adds a line into the PBXBuildFile section
	private static void add_framework_file_reference(StreamWriter file, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}

		UnityEngine.Debug.Log("OnPostProcessBuild - Adding framework file reference " + name);

		string path = "System/Library/Frameworks"; // all the frameworks come from here
		string type = "wrapper.framework";
		if(name == "libsqlite3.tbd" || name == "libxml2.tbd")           // except for tbds
		{
			path = "usr/lib";
			type = "\"compiled.mach-o.tbd\"";
		}

		file.Write("\t\t" + id + " /* " + name + " */ = {isa = PBXFileReference; lastKnownFileType = " + type + "; name = " + name + "; path = " + path + "/" + name + "; sourceTree = SDKROOT; };\n");
	}

	// Adds a line into the PBXFrameworksBuildPhase section
	private static void add_frameworks_build_phase(StreamWriter file, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}
		UnityEngine.Debug.Log("OnPostProcessBuild - Adding build phase " + name);

		file.Write("\t\t\t\t" + id + " /* " + name + " in Frameworks */,\n");
	}

	// Adds a line into the PBXGroup section
	private static void add_group(StreamWriter file, string id, string name)
	{
		// do not re-add the framework if it already exists in the xcode project
		if(!should_process_framework(name))
		{
			return;
		}
		UnityEngine.Debug.Log("OnPostProcessBuild - Add group " + name);

		file.Write("\t\t\t\t" + id + " /* " + name + " */,\n");
	}
}
#endif // UNITY_EDITOR