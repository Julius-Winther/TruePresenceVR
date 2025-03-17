using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CrazyMinnow.SALSA
{
	/// <summary>
	/// ==========================================================================
	/// PURPOSE: Dumps appropriate SALSA/EmoteR, Eyes settings to create a OneClick
	///		file set. The OneClick file set will subsequently ONLY work on models
	///		with an identical hierarchy naming scheme below the root object, with
	///		identically named blendshapes/bones.
	///
	/// NOTE:
	///		This script has support limited to its current implementation scope. In other
	///		words, it does only what it does -- see caveats below:
	///		Operational Caveats:
	///			- Currently only recognizes shape and bone controller types and limited
	///				Eyes module template scenarios. It should produce a solid SALSA and
	///				EmoteR OneClick for any configuration when using only bones and/or
	///				blendshapes. Eyes is limited to head:bone, 2 eyes:bones,
	///				2 blinks/lids:shapes.
	///
	///		If you have problems with the operation of this software, within its scope
	///		of operations, please email us at assetsupport@crazyminnow.com with details of
	///		what the issue is with video and/or screenshots demonstrating the problem. Also
	///		include: your Invoice number, version of this script, your OS, full Unity version,
	///		version of OneClickBase, version of SALSA Suite, etc.
	///
	///		We are also happy to hear suggestions for improvement or better operation.
	/// ==========================================================================
	/// RELEASE NOTES:
	///		2.7.3 (2023-10-18):
	///			~ Minor changes to be more compatible with the latest OneClickBase and more
	///				consistent with official 2.7.2 OneClicks.
	///		2.7.2 (2023-06-14):
	///			+ NOTE: this OneClick REQUIRES OneClickBase v2.7.2.
	///			~ Add QueueProcessor as last step in the process. In some fringe cases,
	///				this resolves jittery animations when conflicting external influences
	///				are present.
	///			~ AudioSource configuration now called from Editor script.
	///		2.7.1	! If SilenceAnalyzer not used, it will not be added via OneClick.
	///				+ Now respects not adding AudioSource -- configures ExternalAnalysis instead.
	///				+ Now respects AudioSource PlayOnAwake and Loop settings.
	///				+ Now respects custom AudioClip assignment.
	///		2.7.0	+ New EmoteR parameters for: persistence, repeater delay/type.
	///		2.6.5	! Corrected additional float cultural conversions that were missed.
	///		2.6.4	! Now correctly uses dot-format floats when grabbing from comma-format cultural data.
	///		2.6.3	! Malformed bracing structure after SilenceAnalyzer was segregated in previous.
	///				~ updated summary verbiage.
	///				~ changed menu location to SALSA LipSync > Tools > Create OneClick File-Set.
	///				~ changed menu method name to match menu action.
	///				~ changed class/filename to CreateCustomOneClick.
	///		2.6.2	! SilenceAnalyzer guard for unused SilenceAnalyzer.
	///		2.6.1	+ add basic Eyes support - head: bone, eyes: bones, blink: blendshapes, track: blendshapes.
	///		2.6.0	! fix EmoteR error, when EmoteR not being configured.
	///				+ Now creates a generic Editor file which links to the OneClickRuntime.
	///				+ Added asset database update so not necessary to click out of Unity and back on it.
	///		2.5.2	- fix silence analyzer conditional -- removed ';' after if statement.
	///		2.5.1	- add Darrin's Tweaks...
	/// 	2.5.0	- a couple of small updates for Suite/Base v2.5.0.
	/// 	1.0.0	- initial release.
	/// ==========================================================================
	/// General Usage Notes:
	/// 	1. Appropriately name the game object root (will be OneClick Class and Filename).
	/// 	2. Select the root object...
	/// 	3. Select menu option: GameObject > Crazy Minnow Studio > SALSA LipSync > Tools > Create OneClick File-Set
	/// 	4. Creates a new OneClick file set in:
	///			Assets>Plugins>Crazy Minnow Studio>SALSA LipSync>Editor>OneClicks
	///			Assets>Plugins>Crazy Minnow Studio>SALSA LipSync>Plugins>OneClickRuntimes
	///		5. Creates a OneClick menu option in:
	///			GameObject > Crazy Minnow Studio > SALSA LipSync > My Custom OneClicks
	/// ==========================================================================
	/// DISCLAIMER: While every attempt has been made to ensure the safe content
	///		and operation of these files, they are provided as-is, without
	///		warranty or guarantee of any kind. By downloading and using these
	///		files you are accepting any and all risks associated and release
	///		Crazy Minnow Studio, LLC of any and all liability.
	/// ==========================================================================
	/// </summary>

	public class CreateCustomOneClick : MonoBehaviour
	{
		[MenuItem("GameObject/Crazy Minnow Studio/SALSA LipSync/Tools/Create OneClick File-Set")]
		public static void CreateOneClickFileSet()
		{
			var ver = "2.7.3";
			string scriptText = "";
			var selectedObject = Selection.activeGameObject;
			var useAudioSource = false;

			Salsa salsa = selectedObject.GetComponent<Salsa>();
			Emoter emoter = selectedObject.GetComponent<Emoter>();
			Eyes eyes = selectedObject.GetComponent<Eyes>();

			if (!salsa && !emoter && !eyes)
			{
				Debug.LogError("Salsa, Emoter, and Eyes not found, can't do anything...");
				return;
			}

			if (!salsa && !emoter) Debug.LogError("Salsa and Emoter not found on the selected object, won't create SALSA/EmoteR script...");
			if (!salsa) Debug.Log("Salsa not found on the selected object.");
			if (!emoter) Debug.Log("Emoter not found on the selected object.");
			if (!eyes) Debug.Log("Eyes not found on the selected object, won't create Eyes script...");


			#region +++++ SALSA / EmoteR Script +++++

			if (salsa || emoter)
			{
				List<SkinnedMeshRenderer> smrs = new List<SkinnedMeshRenderer>();

				scriptText = "using UnityEngine;" +
				             "\r" +
				             "\rnamespace CrazyMinnow.SALSA.OneClicks" +
				             "\r{" +
				             "\r	/// <summary>" +
				             "\r	/// RELEASE NOTES:" +
				             "\r	///		Script generated by CreateCustomOneClick: v" + ver + "." +
				             "\r	///		See CreateCustomOneClick for version details." +
				             "\r	/// ==========================================================================" +
				             "\r	/// PURPOSE: This script applies OneClick settings to a custom model system" +
				             "\r	///		for SALSA and/or EmoteR. This script was generated by CreateCustomOneClick" +
				             "\r	///		from a custom model and is not supported by Crazy Minnow Studio, LLC." +
				             "\r	///		unless made available via the Official downloads portal." +
				             "\r	/// ==========================================================================" +
				             "\r	/// DISCLAIMER: While every attempt has been made to ensure the safe content" +
				             "\r	///		and operation of these files, they are provided as-is, without" +
				             "\r	///		warranty or guarantee of any kind. By downloading and using these" +
				             "\r	///		files you are accepting any and all risks associated and release" +
				             "\r	///		Crazy Minnow Studio, LLC of any and all liability." +
				             "\r	/// ==========================================================================" +
				             "\r	/// </summary>" +
				             "\r	public class OneClick" + selectedObject.name + " : OneClickBase" +
				             "\r	{" +
				             "\r		public static void Setup(GameObject gameObject)" +
				             "\r		{" +
				             "\r			////////////////////////////////////////////////////////////////////////////////////////////////////////////" +
				             "\r			//	SETUP Requirements:" +
				             "\r			//		use NewExpression(\"expression name\") to start a new viseme/emote expression." +
				             "\r			//		use AddShapeComponent to add blendshape configurations, passing:" +
				             "\r			//			- string array of shape names to look for." +
				             "\r			//			  : string array can be a single element." +
				             "\r			//			  : string array can be a single regex search string." +
				             "\r			//			    note: useRegex option must be set true." +
				             "\r			//			- optional string name prefix for the component." +
				             "\r			//			- optional blend amount (default = 1.0f)." +
				             "\r			//			- optional regex search option (default = false)." +
				             "\r" +
				             "\r			Init();";

				if (salsa)
				{
					if (salsa && salsa.audioSrc && !salsa.useExternalAnalysis)
						useAudioSource = true;

					foreach (var viseme in salsa.visemes)
					{
						for (int i = 0; i < viseme.expData.controllerVars.Count; i++)
						{
							if (viseme.expData.components[i].controlType == ExpressionComponent.ControlType.Shape)
							{
								if (!smrs.Contains(viseme.expData.controllerVars[i].smr))
									smrs.Add(viseme.expData.controllerVars[i].smr);
							}
						}
					}

					scriptText += "\r\r\t\t\t#region SALSA-Configuration";
					scriptText += "\r\t\t\tNewConfiguration(OneClickConfiguration.ConfigType.Salsa);"+
					              "\r\t\t\t{";

					scriptText += "\r\t\t\t\t////////////////////////////////////////////////////////";
					scriptText += "\r\t\t\t\t// SMR regex searches (enable/disable/add as required).";
					foreach (var smr in smrs)
					{
						scriptText += "\r\t\t\t\tAddSmrSearch(\"^" + smr.name + "$\");";
					}

					scriptText += "\r\r\t\t\t\t////////////////////////////////////////////////////////";
					scriptText += "\r\t\t\t\t// Adjust SALSA settings to taste...";
					scriptText += "\r\t\t\t\t// - data analysis settings";
					scriptText += "\r\t\t\t\tautoAdjustAnalysis = " + salsa.autoAdjustAnalysis.ToString().ToLower() + ";";
					scriptText += "\r\t\t\t\tautoAdjustMicrophone = " + salsa.autoAdjustMicrophone.ToString().ToLower() + ";";
					scriptText += "\r\t\t\t\taudioUpdateDelay = " + salsa.audioUpdateDelay.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\r\t\t\t\t// - advanced dynamics settings";
					scriptText += "\r\t\t\t\tloCutoff = " + salsa.loCutoff.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\thiCutoff = " + salsa.hiCutoff.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\tuseAdvDyn = " + salsa.useAdvDyn.ToString().ToLower() + ";";
					scriptText += "\r\t\t\t\tadvDynPrimaryBias = " + salsa.advDynPrimaryBias.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\tuseAdvDynJitter = " + salsa.useAdvDynJitter.ToString().ToLower() + ";";
					scriptText += "\r\t\t\t\tadvDynJitterAmount = " + salsa.advDynJitterAmount.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\tadvDynJitterProb = " + salsa.advDynJitterProb.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\tadvDynSecondaryMix = " + salsa.advDynSecondaryMix.ToString(CultureInfo.InvariantCulture) + "f;";
					scriptText += "\r\t\t\t\temphasizerTrigger = " + salsa.emphasizerTrigger.ToString(CultureInfo.InvariantCulture) + "f;";

					scriptText += "\r\r\t\t\t\t////////////////////////////////////////////////////////";
					scriptText += "\r\t\t\t\t// Viseme setup...";
					foreach (var viseme in salsa.visemes)
					{
						scriptText += "\r\r\t\t\t\tNewExpression(\"" + viseme.expData.name + "\");";

						scriptText += ParseComponentTypes(viseme.expData);
					}

					scriptText += "\r\t\t\t}"+
					              "\r\t\t\t#endregion // SALSA-configuration\r";
				}

				if (emoter)
				{
					// clear smrs for emoter use...
					smrs.Clear();
					foreach (var emote in emoter.emotes)
					{
						for (int i = 0; i < emote.expData.controllerVars.Count; i++)
						{
							if (emote.expData.components[i].controlType == ExpressionComponent.ControlType.Shape)
							{
								if (!smrs.Contains(emote.expData.controllerVars[i].smr))
									smrs.Add(emote.expData.controllerVars[i].smr);
							}
						}
					}

					scriptText += "\r\t\t\t#region EmoteR-Configuration";
					scriptText += "\r\t\t\tNewConfiguration(OneClickConfiguration.ConfigType.Emoter);"+
					              "\r\t\t\t{";

					var smrSearches = "";
					foreach (var smr in smrs)
					{
						smrSearches += "\r\t\t\t\tAddSmrSearch(\"^" + smr.name + "$\");";
					}

					if (!String.IsNullOrEmpty(smrSearches))
					{
						scriptText += "\r\t\t\t\t////////////////////////////////////////////////////////";
						scriptText += "\r\t\t\t\t// SMR regex searches (enable/disable/add as required).";
						scriptText += smrSearches;
					}

					scriptText += "\r"+
					              "\r				useRandomEmotes = " + emoter.useRandomEmotes.ToString().ToLower() + ";"+
					              "\r				isChancePerEmote = " + emoter.isChancePerEmote.ToString().ToLower() + ";"+
					              "\r				numRandomEmotesPerCycle = " + emoter.NumRandomEmotesPerCycle + ";"+
					              "\r				randomEmoteMinTimer = " + emoter.randomEmoteMinTimer.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r				randomEmoteMaxTimer = " + emoter.randomEmoteMaxTimer.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r				randomChance = " + emoter.randomChance.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r				useRandomFrac = " + emoter.useRandomFrac.ToString().ToLower() + ";"+
					              "\r				randomFracBias = " + emoter.randomFracBias.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r				useRandomHoldDuration = " + emoter.useRandomHoldDuration.ToString().ToLower() + ";"+
					              "\r				randomHoldDurationMin = " + emoter.randomHoldDurationMin.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r				randomHoldDurationMax = " + emoter.randomHoldDurationMax.ToString(CultureInfo.InvariantCulture) + "f;"+
					              "\r"+
					              "\r				////////////////////////////////////////////////////////"+
					              "\r				// Emote setup...";

					foreach (var emote in emoter.emotes)
					{
						scriptText += "\r"+
						              "\r				NewExpression(\"" + emote.expData.name + "\");"+
						              "\r				AddEmoteFlags(" +
											emote.isRandomEmote.ToString().ToLower() + ", " +
											emote.isLipsyncEmphasisEmote.ToString().ToLower() + ", " +
											emote.isRepeaterEmote.ToString().ToLower() +", " +
											emote.frac.ToString(CultureInfo.InvariantCulture) + "f," +
											emote.isAlwaysEmphasisEmote.ToString().ToLower() + ", " +
											emote.isPersistent.ToString().ToLower() + ", " +
											emote.repeaterDelay.ToString(CultureInfo.InvariantCulture) + "f, " +
											"EmoteRepeater.StartDelay." + emote.startDelay.ToString() +
											");";

						scriptText += ParseComponentTypes(emote.expData);
					}
					scriptText += "\r			}"+
					              "\r			#endregion // EmoteR-configuration";
				}

				scriptText += "\r" +
				              "\r			DoOneClickiness(gameObject);";


				#region ===== SALSA / EmoteR Tweaks =====

				var silenceAnalyzer = selectedObject.GetComponent<SalsaAdvancedDynamicsSilenceAnalyzer>();
				if (silenceAnalyzer)
				{
					scriptText += "\r" +
					              "\r			if (selectedObject.GetComponent<SalsaAdvancedDynamicsSilenceAnalyzer>() == null)" +
					              "\r				selectedObject.AddComponent<SalsaAdvancedDynamicsSilenceAnalyzer>();";
				}

				if (salsa || emoter)
				{
					scriptText += "\r" +
					              "\r			//Darrin's Tweaks";
				}

				if (salsa)
				{
					scriptText += "\r			salsa.useTimingsOverride = " + salsa.useTimingsOverride.ToString().ToLower() + ";" +
					              "\r			salsa.globalDurON = " + salsa.globalDurON.ToString(CultureInfo.InvariantCulture) + "f;" +
					              "\r			salsa.globalDurOffBalance = " + salsa.globalDurOffBalance.ToString(CultureInfo.InvariantCulture) + "f;" +
					              "\r			salsa.globalNuanceBalance = " + salsa.globalNuanceBalance.ToString(CultureInfo.InvariantCulture) + "f;";
				}

				if (emoter)
				{
					scriptText += "\r"+
					              "\r			emoter.NumRandomEmphasizersPerCycle = " + emoter.NumRandomEmphasizersPerCycle + ";"+
					              "\r			EmoteExpression emote;";

					foreach (var emote in emoter.emotes)
					{
						if (emote.isLipsyncEmphasisEmote)
						{
							scriptText += "\r			emote = emoter.FindEmote(\"" + emote.expData.name + "\");"+
							              "\r			if (emote != null)"+
							              "\r				emote.frac = " + emote.frac.ToString(CultureInfo.InvariantCulture) + "f;";
						}
					}
				}

				if (salsa)
				{
					if (silenceAnalyzer)
					{
						scriptText += "\r" +
						              "\r			var silenceAnalyzer = selectedObject.GetComponent<SalsaAdvancedDynamicsSilenceAnalyzer>();" +
						              "\r			if (silenceAnalyzer)" +
						              "\r			{" +
						              "\r				silenceAnalyzer.silenceThreshold = " +
						              silenceAnalyzer.silenceThreshold.ToString(CultureInfo.InvariantCulture) + "f;" +
						              "\r				silenceAnalyzer.timingStartPoint = " +
						              silenceAnalyzer.timingStartPoint.ToString(CultureInfo.InvariantCulture) + "f;" +
						              "\r				silenceAnalyzer.timingEndVariance = " +
						              silenceAnalyzer.timingEndVariance.ToString(CultureInfo.InvariantCulture) + "f;" +
						              "\r				silenceAnalyzer.silenceSampleWeight = " +
						              silenceAnalyzer.silenceSampleWeight.ToString(CultureInfo.InvariantCulture) +
						              "f;" +
						              "\r				silenceAnalyzer.bufferSize = " + silenceAnalyzer.bufferSize + ";" +
						              "\r			}";
					}
					else
						Debug.Log("SilenceAnalyzer not found, assumed not desired in the OneClick.");
				}

				scriptText += "\r		}"+
				              "\r	}"+
				              "\r}\r";

				#endregion

				var fileName = "OneClick" + selectedObject.name + ".cs";
				var fileHandle = File.CreateText(Application.dataPath + "/Plugins/Crazy Minnow Studio/SALSA LipSync/Plugins/OneClickRuntimes/" + fileName);
				fileHandle.WriteLine(scriptText);
				fileHandle.Close();

				Debug.Log(fileName + " file written...");
			}

			#endregion


			#region +++++ Eyes Script +++++

			if (eyes)
			{
				var head = eyes.heads[0].expData.controllerVars[0].bone.name;
				var eyeL = eyes.eyes[0].expData.controllerVars[0].bone.name;
				var eyeR = eyes.eyes[1].expData.controllerVars[0].bone.name;
				var blinkSmr = eyes.blinklids[0].expData.controllerVars[0].smr.name;
				var blinkLid = eyes.blinklids[0].expData.controllerVars[0].blendIndex;
				var blinkL = eyes.blinklids[0].expData.controllerVars[0].smr.sharedMesh.GetBlendShapeName(blinkLid);
				var blinkRid = eyes.blinklids[1].expData.controllerVars[0].blendIndex;
				var blinkR = eyes.blinklids[1].expData.controllerVars[0].smr.sharedMesh.GetBlendShapeName(blinkRid);
				var blinkMax = eyes.blinklids[0].expData.controllerVars[0].maxShape;

				scriptText = "using UnityEngine;" +
				             "\rnamespace CrazyMinnow.SALSA.OneClicks" +
				             "\r{" +
				             "\r	/// <summary>"+
				             "\r	/// RELEASE NOTES:"+
				             "\r	///		Script generated by CreateCustomOneClick: v" + ver + "." +
				             "\r	///		See CreateCustomOneClick for version details." +
				             "\r	/// =========================================================================="+
				             "\r	/// PURPOSE: This script applies OneClick settings to a custom model system" +
				             "\r	///		for the Eyes module. This script was generated by CreateCustomOneClick" +
				             "\r	///		from a custom model and is not supported by Crazy Minnow Studio, LLC." +
				             "\r	///		unless made available via the Official downloads portal." +
				             "\r	/// =========================================================================="+
				             "\r	/// DISCLAIMER: While every attempt has been made to ensure the safe content"+
				             "\r	///		and operation of these files, they are provided as-is, without"+
				             "\r	///		warranty or guarantee of any kind. By downloading and using these"+
				             "\r	///		files you are accepting any and all risks associated and release"+
				             "\r	///		Crazy Minnow Studio, LLC of any and all liability."+
				             "\r	/// =========================================================================="+
				             "\r	/// </summary>" +
				             "\r	public class OneClick" + selectedObject.name + "Eyes : MonoBehaviour" +
				             "\r	{" +
				             "\r		public static void Setup(GameObject go)" +
				             "\r		{" +
				             "\r			string head = \"" + head + "\";" +
				             "\r			string[] blinkSmr = new string[] {\"" + blinkSmr + "\"};" +
				             "\r			string[] eyeL = new string[] {\"" + eyeL + "\"};" +
				             "\r			string[] eyeR = new string[] {\"" + eyeR + "\"};" +
				             "\r			string[] blinkL = new string[] {\"" + blinkL + "\"};" +
				             "\r			string[] blinkR = new string[] {\"" + blinkR + "\"};" +
				             "\r			if (go)" +
				             "\r			{" +
				             "\r				Eyes eyes = go.GetComponent<Eyes>();" +
				             "\r				if (eyes == null)" +
				             "\r				{" +
				             "\r					eyes = go.AddComponent<Eyes>();" +
				             "\r				}" +
				             "\r				else" +
				             "\r				{" +
				             "\r					DestroyImmediate(eyes);" +
				             "\r					eyes = go.AddComponent<Eyes>();" +
				             "\r				}" +
				             "\r				// System Properties" +
				             "\r                eyes.characterRoot = go.transform;" +
				             "\r                // Heads - Bone_Rotation" +
				             "\r                eyes.BuildHeadTemplate(Eyes.HeadTemplates.Bone_Rotation_XY);" +
				             "\r                eyes.heads[0].expData.name = \"head\";" +
				             "\r                eyes.heads[0].expData.components[0].name = \"head\";" +
				             "\r                eyes.heads[0].expData.controllerVars[0].bone = Eyes.FindTransform(eyes.characterRoot, head);" +
				             "\r                eyes.headTargetOffset.y = 0.065f;" +
				             "\r                eyes.CaptureMin(ref eyes.heads);" +
				             "\r                eyes.CaptureMax(ref eyes.heads);" +
				             "\r                // Eyes - Bone_Rotation" +
				             "\r                eyes.BuildEyeTemplate(Eyes.EyeTemplates.Bone_Rotation);" +
				             "\r                eyes.eyes[0].expData.controllerVars[0].bone = Eyes.FindTransform(eyes.characterRoot, eyeL);" +
				             "\r                eyes.eyes[0].expData.name = \"eyeL\";" +
				             "\r                eyes.eyes[0].expData.components[0].name = \"eyeL\";" +
				             "\r                eyes.eyes[1].expData.controllerVars[0].bone = Eyes.FindTransform(eyes.characterRoot, eyeR);" +
				             "\r                eyes.eyes[1].expData.name = \"eyeR\";" +
				             "\r                eyes.eyes[1].expData.components[0].name = \"eyeR\";" +
				             "\r                eyes.CaptureMin(ref eyes.eyes);" +
				             "\r                eyes.CaptureMax(ref eyes.eyes);" +
				             "\r                // Eyelids - Bone_Rotation" +
				             "\r                eyes.BuildEyelidTemplate(Eyes.EyelidTemplates.BlendShapes, Eyes.EyelidSelection.Upper); // includes left/right eyelid" +
				             "\r                float blinkMax = " + blinkMax.ToString(CultureInfo.InvariantCulture) + "f;" +
				             "\r                // Left eyelid" +
				             "\r                eyes.blinklids[0].expData.controllerVars[0].smr = Eyes.FindTransform(eyes.characterRoot,  blinkSmr).GetComponent<SkinnedMeshRenderer>();" +
				             "\r                eyes.blinklids[0].expData.controllerVars[0].blendIndex = Eyes.FindBlendIndex(eyes.blinklids[0].expData.controllerVars[0].smr, blinkL);" +
				             "\r                eyes.blinklids[0].expData.controllerVars[0].maxShape = blinkMax;" +
				             "\r                eyes.blinklids[0].expData.name = \"eyelidL\";" +
				             "\r                // Right eyelid" +
				             "\r                eyes.blinklids[1].expData.controllerVars[0].smr = eyes.blinklids[0].expData.controllerVars[0].smr;" +
				             "\r                eyes.blinklids[1].expData.controllerVars[0].blendIndex = Eyes.FindBlendIndex(eyes.blinklids[0].expData.controllerVars[0].smr, blinkR);" +
				             "\r                eyes.blinklids[1].expData.controllerVars[0].maxShape = blinkMax;" +
				             "\r                eyes.blinklids[1].expData.name = \"eyelidR\";" +
				             "\r                // Track lids" +
				             "\r                eyes.CopyBlinkToTrack();" +
				             "\r                if (eyes.tracklids.Count > 1)" +
				             "\r                {" +
				             "\r	                eyes.tracklids[0].referenceIdx = 0; // left eye" +
				             "\r	                eyes.tracklids[1].referenceIdx = 1; // right eye" +
				             "\r                }" +
				             "\r                if (eyes.tracklids.Count > 2)" +
				             "\r                {" +
				             "\r	                eyes.tracklids[2].referenceIdx = 0; // left eye" +
				             "\r	                eyes.tracklids[3].referenceIdx = 1; // right eye" +
				             "\r                }" +
				             "\r                // Initialize the Eyes module" +
				             "\r                eyes.Initialize();" +
				             "\r			}" +
				             "\r		}" +
				             "\r	}" +
				             "\r}";

				var fileName = "OneClick" + selectedObject.name + "Eyes.cs";
				var fileHandle = File.CreateText(Application.dataPath + "/Plugins/Crazy Minnow Studio/SALSA LipSync/Plugins/OneClickRuntimes/" + fileName);
				fileHandle.WriteLine(scriptText);
				fileHandle.Close();

				Debug.Log(fileName + " file written...");
			}

			#endregion


			#region +++++ Editor Script +++++

			if (salsa | emoter | eyes)
			{
				string clipPath = "null";
				string customAudioSourceScript = "";
				if (useAudioSource)
				{
					string audioSrcSettings = "PlayOnAwake is "+(salsa.audioSrc.playOnAwake?"enabled":"disabled")+" on the AudioSource. ";
					audioSrcSettings += "Loop is "+(salsa.audioSrc.loop ? "enabled":"disabled")+" on the AudioSource. ";
					audioSrcSettings += "The custom OneClick will likewise be configured.";
					Debug.Log(audioSrcSettings);

					customAudioSourceScript = "\r" +
					                          "\r			// custom AudioSource settings..." +
					                          "\r			var salsa = go.GetComponent<Salsa>();" +
					                          "\r			if (salsa)" +
					                          "\r			{" +
					                          "\r				salsa.audioSrc.playOnAwake = " + salsa.audioSrc.playOnAwake.ToString().ToLower() + ";" +
					                          "\r				salsa.audioSrc.loop = " + salsa.audioSrc.loop.ToString().ToLower() + ";" +
					                          "\r			}";

					if (salsa.audioSrc.clip)
					{
						clipPath = AssetDatabase.GetAssetPath(salsa.audioSrc.clip);
					}
					else
					{
						Debug.Log("An AudioSource AudioClip is not assigned. It is assumed this is desired and no clip will be assigned in the custom OneClick.");
					}
				}
				else
				{
					Debug.Log("An AudioSource was not defined. It is assumed an AudioSource was not desired. SALSA will be configured to use ExternalAnalysis when the custom OneClick is applied. You must ensure ExternalAnalysis is properly configured/utilized.");
				}

				scriptText = "using UnityEngine;" +
				             "\rusing UnityEditor;" +
				             "\rnamespace CrazyMinnow.SALSA.OneClicks" +
				             "\r{" +
				             "\r	/// <summary>" +
				             "\r	/// RELEASE NOTES:" +
				             "\r	///		Script generated by CreateCustomOneClick: v" + ver + "." +
				             "\r	///		See CreateCustomOneClick for version details." +
				             "\r	/// ==========================================================================" +
				             "\r	/// PURPOSE: This Editor script implements menu options and applies OneClick" +
				             "\r	///		settings to a custom model system via associated scripts for" +
				             "\r	///		SALSA/EmoteR and/or Eyes. This script was generated by CreateCustomOneClick" +
				             "\r	///		from a custom model and is not supported by Crazy Minnow Studio, LLC." +
				             "\r	///		unless made available via the Official downloads portal." +
				             "\r	/// ==========================================================================" +
				             "\r	/// DISCLAIMER: While every attempt has been made to ensure the safe content" +
				             "\r	///		and operation of these files, they are provided as-is, without" +
				             "\r	///		warranty or guarantee of any kind. By downloading and using these" +
				             "\r	///		files you are accepting any and all risks associated and release" +
				             "\r	///		Crazy Minnow Studio, LLC of any and all liability." +
				             "\r	/// ==========================================================================" +
				             "\r	/// </summary>" +
				             "\r	public class OneClick" + selectedObject.name + "Editor : Editor" +
				             "\r	{" +
				             "\r		[MenuItem(\"GameObject/Crazy Minnow Studio/SALSA LipSync/My Custom OneClicks/" +
				             selectedObject.name + "\")]" +
				             "\r		public static void OneClickSetup()" +
				             "\r		{" +
				             "\r			GameObject go = Selection.activeGameObject;" +
				             "\r			" + (salsa || emoter ? "" : "//") + "OneClick" + selectedObject.name +
				             ".Setup(go);" +
				             "\r			" + (eyes ? "" : "//") + "OneClick" + selectedObject.name + "Eyes.Setup(go);" +
				             "\r" +
				             "\r			// add QueueProcessor" +
				             "\r			OneClickBase.AddQueueProcessor(go);";

				if (salsa)
				{
					scriptText += "\r" +
					              "\r			// configure AudioSource" +
					              "\r			var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(\"" + clipPath + "\");" +
					              "\r			OneClickBase.ConfigureSalsaAudioSource(go, clip, " + useAudioSource.ToString().ToLower() +
					              ");" +
					              customAudioSourceScript;
				}

				scriptText += "\r		}" +
				             "\r	}" +
				             "\r}";


				var fileName = "OneClick" + selectedObject.name + "Editor.cs";
				var fileHandle = File.CreateText(Application.dataPath + "/Plugins/Crazy Minnow Studio/SALSA LipSync/Editor/OneClicks/" + fileName);
				fileHandle.WriteLine(scriptText);
				fileHandle.Close();

				Debug.Log(fileName + " file written...");
			}
			#endregion

			AssetDatabase.Refresh();
		}

		private static string ParseComponentTypes(Expression expData)
		{
			var componentScript = "";
			List<string> shapesUsedInExpression = new List<string>();
			List<UmaUepProxy.UepPose> posesUsedInExpression = new List<UmaUepProxy.UepPose>();
			shapesUsedInExpression.Clear();
			posesUsedInExpression.Clear();
			for (int i = 0; i < expData.components.Count; i++)
			{
				var component = expData.components[i];
				var helper = expData.controllerVars[i];
				switch (component.controlType)
				{
					case ExpressionComponent.ControlType.Shape:
						var shapes = SalsaUtil.GetBlendShapes(helper.smr);
						if (!shapesUsedInExpression.Contains(shapes[helper.blendIndex]))
						{
							componentScript += "\r\t\t\t\tAddShapeComponent(new []{\"" + shapes[helper.blendIndex] + "\"}, " + component.durationOn.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationHold.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationOff.ToString(CultureInfo.InvariantCulture) + "f, \"" + shapes[helper.blendIndex] + "\", " + helper.maxShape.ToString(CultureInfo.InvariantCulture) + "f, true);";
							shapesUsedInExpression.Add(shapes[helper.blendIndex]);
						}

						break;
					case ExpressionComponent.ControlType.UMA:
						if (!posesUsedInExpression.Contains(helper.umaUepProxy.Poses[i]))
						{
							componentScript += "\r\t\t\t\tAddUepPoseComponent(\"" + helper.umaUepProxy.Poses[helper.blendIndex].name + "\", " + component.durationOn.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationHold.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationOff.ToString(CultureInfo.InvariantCulture) + "f, \"" + helper.umaUepProxy.Poses[helper.blendIndex].name + "\", " + helper.uepAmount.ToString(CultureInfo.InvariantCulture) + "f);";
							posesUsedInExpression.Add(helper.umaUepProxy.Poses[helper.blendIndex]);
						}

						break;
					case ExpressionComponent.ControlType.Bone:
						componentScript += "\r\t\t\t\tAddBoneComponent(\"^" + helper.bone.name + "$\"," +
									  "\r\t\t\t\t\tnew TformBase(new Vector3(" + helper.endTform.pos.x.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.pos.y.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.pos.z.ToString(CultureInfo.InvariantCulture) + "f)," +
									  "\r\t\t\t\t\tnew Quaternion(" + helper.endTform.rot.x.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.rot.y.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.rot.z.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.rot.w.ToString(CultureInfo.InvariantCulture) + "f)," +
									  "\r\t\t\t\t\tnew Vector3(" + helper.endTform.scale.x.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.scale.y.ToString(CultureInfo.InvariantCulture) + "f, " + helper.endTform.scale.z.ToString(CultureInfo.InvariantCulture) + "f))," +
									  "\r\t\t\t\t\t" + component.durationOn.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationHold.ToString(CultureInfo.InvariantCulture) + "f, " + component.durationOff.ToString(CultureInfo.InvariantCulture) + "f," +
									  "\r\t\t\t\t\t\"" + helper.bone.name + "\"," +
									  "\r\t\t\t\t\t" + helper.fracPos.ToString().ToLower() + ", " + helper.fracRot.ToString().ToLower() + ", " + helper.fracScl.ToString().ToLower() + ");";
						break;
				}
			}

			return componentScript;
		}
	}
}
