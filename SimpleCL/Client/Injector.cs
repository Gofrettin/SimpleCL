﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SimpleCL.Util;

namespace SimpleCL.Client
{
    public class Injector
    {
        private readonly Process _srProcess;
        private readonly string _dllPath;

        public Injector(Process srProcess, string dllPath)
        {
            _srProcess = srProcess;
            _dllPath = Path.GetFullPath(dllPath);
        }

        public bool Inject()
        {
            CreateMutex(IntPtr.Zero, false, "Silkroad Online Launcher");
            CreateMutex(IntPtr.Zero, false, "Ready");
            
            // _srProcess.Start();
            // SuspendProcess(_srProcess.Id);
            // var handle = OpenProcess(0x1F0FFF, 1, (uint) _srProcess.Id);
            // Int32 bufferSize = _dllPath.Length + 1;
            // IntPtr allocMem = VirtualAllocEx(handle, IntPtr.Zero, (IntPtr) bufferSize, 4096, 4);
            // WriteProcessMemory(handle, allocMem, Encoding.Default.GetBytes(_dllPath), (uint) bufferSize, out _);
            // IntPtr injector = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            // IntPtr hThread = CreateRemoteThread(handle, (IntPtr)null, IntPtr.Zero, injector, allocMem, 0, (IntPtr) null);
            // CloseHandle(hThread);
            // ResumeProcess(_srProcess.Id);
            
            var handle = OpenProcess(0x2 | 0x8 | 0x10 | 0x20 | 0x400, 1, (uint) _srProcess.Id);
            
            if (handle == IntPtr.Zero)
            {
                return false;
            }
            
            var loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            
            if (loadLibraryAddr == IntPtr.Zero)
            {
                return false;
            }
            
            if (!File.Exists(_dllPath))
            {
                throw new DllNotFoundException("Dll not found");
            }
            
            var allocMemAddress = VirtualAllocEx(handle, (IntPtr) null, (IntPtr) _dllPath.Length,
                (uint) AllocationType.MemCommit |
                (uint) AllocationType.MemReserve,
                (uint) ProtectionConstants.PageExecuteReadwrite);
            
            if (allocMemAddress == IntPtr.Zero)
            {
                return false;
            }
            
            var bytes = Encoding.Default.GetBytes(_dllPath);
            WriteProcessMemory(handle, allocMemAddress, bytes, (uint) bytes.Length, out var bytesWritten);
            
            var ipThread = CreateRemoteThread(handle, (IntPtr) null, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0,
                (IntPtr) null);
            
            if (ipThread == IntPtr.Zero)
            {
                return false;
            }
            
            var result = WaitForSingleObject(ipThread, 10000);
            if (result == 0x00000080L || result == 0x00000102L || result == 0xFFFFFFFF)
            {
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                }
            
                return false;
            }
            
            if (handle != IntPtr.Zero)
            {
                CloseHandle(handle);
            }
            
            return true;
        }
        
        private void SuspendProcess(int pid)
        {
            Process proc = Process.GetProcessById(pid);

            if (proc.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SuspendResume, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }

                SuspendThread(pOpenThread);
            }
        }
        
        public void ResumeProcess(int pid)
        {
            Process proc = Process.GetProcessById(pid);

            if (proc.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SuspendResume, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }

                ResumeThread(pOpenThread);
            }
        }

        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, uint dwAddress, ref byte[] lpBuffer, int nSize,
            out int lpBytesRead);

        [DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize,
            out IntPtr iBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheiritHandle, IntPtr dwProcessId);

        public static uint Rights = 0x0010 | 0x0020 | 0x0008;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            Int32 bInheritHandle,
            UInt32 dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 CloseHandle(
            IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            IntPtr dwSize,
            uint flAllocationType,
            uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] buffer,
            uint size,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttribute,
            IntPtr dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        public enum AllocationType
        {
            MemCommit = 0x1000,
            MemReserve = 0x2000,
            MemReset = 0x80000,
        }

        public enum ProtectionConstants
        {
            PageExecute = 0X10,
            PageExecuteRead = 0X20,
            PageExecuteReadwrite = 0X40,
            PageExecuteWritecopy = 0X80,
            PageNoaccess = 0X01
        }
        
        public enum ThreadAccess : int
        {
            Terminate = 0x0001,
            SuspendResume = 0x0002,
            GetContext = 0x0008,
            SetContext = 0x0010,
            SetInformation = 0x0020,
            QueryInformation = 0x0040,
            SetThreadToken = 0x0080,
            Impersonate = 0x0100,
            DirectImpersonation = 0x0200
        }
    }
}