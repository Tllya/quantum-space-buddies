﻿using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static EntitlementsManager;
using Debug = UnityEngine.Debug;

namespace EpicRerouter.ModSide
{
	public static class Interop
	{
		public static AsyncOwnershipStatus OwnershipStatus { get; private set; } = AsyncOwnershipStatus.NotReady;

		public static void Go()
		{
			if (typeof(EpicPlatformManager).GetField("_platformInterface", BindingFlags.NonPublic | BindingFlags.Instance) == null)
			{
				Log("not epic. don't reroute");
				// return;
			}

			Log("go");

			Patches.Apply();

			var processPath = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
				"EpicRerouter.exe"
			);
			Log($"process path = {processPath}");
			var gamePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(EpicPlatformManager).Assembly.Location)!, ".."));
			Log($"game path = {gamePath}");
			var args = new[]
			{
				Application.productName,
				Application.version,
				Path.Combine(gamePath, "Managed")
			};
			Log($"args = {args.Join()}");
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = processPath,
				WorkingDirectory = Path.Combine(gamePath, "Plugins", "x86_64"),
				Arguments = args.Join(x => $"\"{x}\"", " "),
				UseShellExecute = false
			});
			process!.WaitForExit();
			OwnershipStatus = (AsyncOwnershipStatus)process.ExitCode;
			Log($"ownership status = {OwnershipStatus}");
		}

		public static void Log(object msg) => Debug.LogError($"[interop] {msg}");
	}
}
