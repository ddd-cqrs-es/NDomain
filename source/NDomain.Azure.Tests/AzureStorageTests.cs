﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Azure.Tests
{
    [SetUpFixture]
    public class StartStopAzureEmulator
    {
        private bool _wasUp;

        [SetUp]
        public void StartAzureBeforeAllTestsIfNotUp()
        {
            if (!AzureStorageEmulatorManager.IsProcessStarted())
            {
                AzureStorageEmulatorManager.StartStorageEmulator();
                _wasUp = false;
            }
            else
            {
                _wasUp = true;
            }

        }

        [TearDown]
        public void StopAzureAfterAllTestsIfWasDown()
        {
            if (!_wasUp)
            {
                AzureStorageEmulatorManager.StopStorageEmulator();
            }
            else
            {
                // Leave as it was before testing...
            }
        }
    }

    // Start/stop azure storage emulator from code:
    // http://stackoverflow.com/questions/7547567/how-to-start-azure-storage-emulator-from-within-a-program
    // Credits to David Peden http://stackoverflow.com/users/607701/david-peden for sharing this!
    public static class AzureStorageEmulatorManager
    {
        private const string _windowsAzureStorageEmulatorPath = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe";
        private const string _win7ProcessName = "AzureStorageEmulator";
        private const string _win8ProcessName = "WASTOR~1";

        private static readonly ProcessStartInfo startStorageEmulator = new ProcessStartInfo
        {
            FileName = _windowsAzureStorageEmulatorPath,
            Arguments = "start",
        };

        private static readonly ProcessStartInfo stopStorageEmulator = new ProcessStartInfo
        {
            FileName = _windowsAzureStorageEmulatorPath,
            Arguments = "stop",
        };

        private static Process GetProcess()
        {
            return Process.GetProcessesByName(_win7ProcessName).FirstOrDefault() ?? Process.GetProcessesByName(_win8ProcessName).FirstOrDefault();
        }

        public static bool IsProcessStarted()
        {
            return GetProcess() != null;
        }

        public static void StartStorageEmulator()
        {
            if (!IsProcessStarted())
            {
                using (Process process = Process.Start(startStorageEmulator))
                {
                    process.WaitForExit();
                }
            }
        }

        public static void StopStorageEmulator()
        {
            using (Process process = Process.Start(stopStorageEmulator))
            {
                process.WaitForExit();
            }
        }
    }
}
