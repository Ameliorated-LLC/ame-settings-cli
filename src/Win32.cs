using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace amecs
{
    public static class Win32
    {
        public static class SystemInfo
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public class MEMORYSTATUSEX
            {
                public uint dwLength;
                public uint dwMemoryLoad;
                public ulong ullTotalPhys;
                public ulong ullAvailPhys;
                public ulong ullTotalPageFile;
                public ulong ullAvailPageFile;
                public ulong ullTotalVirtual;
                public ulong ullAvailVirtual;
                public ulong ullAvailExtendedVirtual;
                public MEMORYSTATUSEX()
                {
                    this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                }
            }

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
            
            [StructLayout(LayoutKind.Sequential)]
            public struct RTL_OSVERSIONINFOEX
            {
                internal uint dwOSVersionInfoSize;
                internal uint dwMajorVersion;
                internal uint dwMinorVersion;
                internal uint dwBuildNumber;
                internal uint dwPlatformId;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                internal string szCSDVersion;
            }
            [DllImport("ntdll")]
            public static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);

            public enum MachineType : ushort
            {
                IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
                IMAGE_FILE_MACHINE_ALPHA = 0x184, //Digital Equipment Corporation (DEC) Alpha (32-bit)
                IMAGE_FILE_MACHINE_AM33 = 0x1d3, //Matsushita AM33, now MN103 (32-bit) part of Panasonic Corporation
                IMAGE_FILE_MACHINE_AMD64 =
                    0x8664, //AMD (64-bit) - was Advanced Micro Devices, now means x64  - OVERLOADED _AMD64 = 0x8664 - http://msdn.microsoft.com/en-us/library/windows/desktop/ms680313(v=vs.85).aspx  
                IMAGE_FILE_MACHINE_ARM = 0x1c0, //ARM little endian (32-bit), ARM Holdings, later versions 6+ used in iPhone, Microsoft Nokia N900
                IMAGE_FILE_MACHINE_ARMV7 = 0x1c4, //ARMv7 or IMAGE_FILE_MACHINE_ARMNT (or higher) Thumb mode only (32 bit).
                IMAGE_FILE_MACHINE_ARM64 = 0xaa64, //ARM8+ (64-bit)
                IMAGE_FILE_MACHINE_EBC = 0xebc, //EFI byte code (32-bit), now (U)EFI or (Unified) Extensible Firmware Interface
                IMAGE_FILE_MACHINE_I386 = 0x14c, //Intel 386 or later processors and compatible processors (32-bit)
                IMAGE_FILE_MACHINE_I860 = 0x14d, //Intel i860 (aka 80860) (32-bit) was a RISC microprocessor design introduced by Intel in 1989, this was depricated in 90's
                IMAGE_FILE_MACHINE_IA64 = 0x200, //Intel Itanium architecture processor family, (64-bit)
                IMAGE_FILE_MACHINE_M68K = 0x268, //Motorola 68000 Series (32-bit) CISC microprocessors
                IMAGE_FILE_MACHINE_M32R = 0x9041, //Mitsubishi M32R little endian (32-bit) now owned by Renesas Electronics Corporation
                IMAGE_FILE_MACHINE_MIPS16 = 0x266, //MIPS16 (16-bit instruction codes, 8to32bit bus)- Microprocessor without Interlocked Pipeline Stages Architecture
                IMAGE_FILE_MACHINE_MIPSFPU = 0x366, //MIPS with FPU, MIPS Technologies (32-bit)
                IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466, //MIPS16 with FPU (Floating Point Unit aka a math co-processesor)(16-bit instruction codes, 8to32bit bus)
                IMAGE_FILE_MACHINE_POWERPC = 0x1f0, //Power PC little endian, Performance Optimization With Enhanced RISC – Performance Computing (32-bit) one of the first
                IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1, //Power PC with floating point support (FPU) (32-bit), designed by AIM Alliance (Apple, IBM, and Motorola)
                IMAGE_FILE_MACHINE_POWERPCBE = 0x01F2, //Power PC Big Endian (64?-bits)
                IMAGE_FILE_MACHINE_R3000 = 0x0162, //R3000 (32-bit) RISC processor
                IMAGE_FILE_MACHINE_R4000 = 0x166, //R4000 MIPS (64-bit) - claims to be first true 64-bit processor
                IMAGE_FILE_MACHINE_R10000 =
                    0x0168, //R10000 MIPS IV is a (64-bit) architecture, but the R10000 did not implement the entire physical or virtual address to reduce cost. Instead, it has a 40-bit physical address and a 44-bit virtual address, thus it is capable of addressing 1 TB of physical memory and 16 TB of virtual memory. These comments by metadataconsulting.ca
                IMAGE_FILE_MACHINE_SH3 = 0x1a2, //Hitachi SH-3 (32-bit) - SuperH processor (SH3) core family
                IMAGE_FILE_MACHINE_SH3DSP = 0x1a3, //Hitachi SH-3 DSP (32-bit)
                IMAGE_FILE_MACHINE_SH4 = 0x1a6, //Hitachi SH-4 (32-bit)
                IMAGE_FILE_MACHINE_SH5 = 0x1a8, //Hitachi SH-5, (64-bit) core with a 128-bit vector FPU (64 32-bit registers) and an integer unit which includes the SIMD support and 63 64-bit registers.
                IMAGE_FILE_MACHINE_TRICORE = 0x0520, //Infineon AUDO (Automotive unified processor) (32-bit) - Tricore architecture a unified RISC/MCU/DSP microcontroller core
                IMAGE_FILE_MACHINE_THUMB = 0x1c2, //ARM or Thumb (interworking), (32-bit) core instruction set, used in Nintendo Gameboy Advance
                IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169, //MIPS Windows Compact Edition v2
                IMAGE_FILE_MACHINE_ALPHA64 = 0x284 //DEC Alpha AXP (64-bit) or IMAGE_FILE_MACHINE_AXP64
            }
            
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool IsWow64Process2(
                IntPtr process,
                out MachineType processMachine,
                out MachineType nativeMachine
            );
        }

        public static class SystemInfoEx
        {
            private static WindowsVersionInfo? _windowsVersion;
            public static WindowsVersionInfo WindowsVersion => _windowsVersion ??= RtlGetVersion();

            private static ulong? _systemMemory;
            public static ulong SystemMemory => _systemMemory ??= GetSystemMemoryInBytes();
            
            private static Architecture? _systemArchitecture;
            public static Architecture SystemArchitecture => _systemArchitecture ??= GetArchitecture();
            
            public static ulong GetSystemMemoryInBytes()
            {
                SystemInfo.MEMORYSTATUSEX memStatus = new SystemInfo.MEMORYSTATUSEX();
                if (SystemInfo.GlobalMemoryStatusEx(memStatus))
                    return memStatus.ullTotalPhys;
                else
                    return 0;
            }

            public static Architecture GetArchitecture()
            {
                SystemInfo.MachineType processType = SystemInfo.MachineType.IMAGE_FILE_MACHINE_UNKNOWN;
                SystemInfo.MachineType hostType = SystemInfo.MachineType.IMAGE_FILE_MACHINE_UNKNOWN;
                SystemInfo.IsWow64Process2(Win32.Process.GetCurrentProcess().DangerousGetHandle(), out processType, out hostType);

                switch (hostType)
                {
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_ARMV7:
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_ARM:
                        return Architecture.Arm;
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_ARM64:
                        return Architecture.Arm64;
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_I386:
                        return Architecture.X86;
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_AMD64:
                    case SystemInfo.MachineType.IMAGE_FILE_MACHINE_I860:
                        return Architecture.X64;
                    default:
                        return RuntimeInformation.OSArchitecture;
                }
            }

            public class WindowsVersionInfo
            {
                public int MajorVersion { get; set; }
                public int BuildNumber { get; set; }
                public int UpdateNumber { get; set; }
                public string Edition { get; set; } = null!;
            }

            public static WindowsVersionInfo RtlGetVersion()
            {
                var result = new WindowsVersionInfo();

                bool failed = false;
                try
                {
                    result.BuildNumber = Int32.Parse((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", (string)"-1")!);
                    result.UpdateNumber = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", (int)0)!;
                    failed = result.BuildNumber == -1;
                }
                catch (Exception e)
                {
                    failed = true;
                }

                try
                {
                    SystemInfo.RTL_OSVERSIONINFOEX v = new SystemInfo.RTL_OSVERSIONINFOEX();
                    v.dwOSVersionInfoSize = (uint)Marshal.SizeOf<SystemInfo.RTL_OSVERSIONINFOEX>();
                    if (SystemInfo.RtlGetVersion(ref v) == 0)
                    {
                        result.BuildNumber = result.BuildNumber > (int)v.dwBuildNumber ? result.BuildNumber : (int)v.dwBuildNumber;
                        result.MajorVersion = result.BuildNumber < 22000 ? 10 : 11;
                        result.MajorVersion = result.MajorVersion > (int)v.dwMajorVersion ? result.MajorVersion : (int)v.dwMajorVersion;
                        failed = false;
                    }
                    else
                        result.MajorVersion = result.BuildNumber < 22000 ? 10 : 11;
                }
                catch (Exception e)
                {
                    result.MajorVersion = result.BuildNumber < 22000 ? 10 : 11;
                }
                if (failed)
                    throw new Exception("RtlGetVersion failed.");

                try
                {
                    var edition = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID", "Core")!;
                    result.Edition = edition switch
                    {
                        "Core" => "Home",
                        "Professional" => "Pro",
                        "ProfessionalWorkstation" => "Pro Workstation",
                        "Enterprise" => "Enterprise",
                        "EnterpriseN" => "Enterprise",
                        "EnterpriseG" => "Enterprise",
                        "EnterpriseS" => "Enterprise S",
                        "EnterpriseSN" => "Enterprise S",
                        "Education" => "Education",
                        "ProfessionalEducation" => "Pro Education",
                        "ServerStandard" => "Server",
                        "ServerDatacenter" => "Server",
                        "ServerSolution" => "Server",
                        "ServerStandardEval" => "Server Eval",
                        "ServerDatacenterEval" => "Server Eval",
                        "Cloud" => "Cloud",
                        "CloudN" => "Cloud S",
                        "CoreCountrySpecific" => "Home",
                        "CoreSingleLanguage" => "Home",
                        "IoTCore" => "IoT Core",
                        "IoTEnterprise" => "IoT Enterprise",
                        "IoTEnterpriseS" => "IoT Enterprise S",
                        "IoTUAP" => "IoT Enterprise",
                        "Team" => "Team",
                        _ => String.IsNullOrWhiteSpace(edition) ? "Home" : edition
                    };
                }
                catch (Exception e)
                {
                    result.Edition = "Home";
                }

                return result;
            }
        }

        public static class Process
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern SafeProcessHandle GetCurrentProcess();

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern SafeProcessHandle OpenProcess(ProcessAccessFlags dwDesiredAccess,
                bool bInheritHandle, int dwProcessId);

            [DllImport("ntdll.dll")]
            public static extern int NtQueryInformationProcess(SafeProcessHandle process, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength,
                out int returnLength);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool QueryFullProcessImageName(SafeProcessHandle process, uint dwFlags,
                [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName, ref uint lpdwSize);

            [DllImport("kernel32.dll")]
            public static extern bool TerminateProcess(SafeProcessHandle process, uint uExitCode);


            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_BASIC_INFORMATION
            {
                public IntPtr ExitStatus;
                public IntPtr PebBaseAddress;
                public IntPtr AffinityMask;
                public IntPtr BasePriority;
                public UIntPtr UniqueProcessId;
                public UIntPtr InheritedFromUniqueProcessId;
            }

            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            public enum LogonFlags
            {
                WithProfile = 1,
                NetCredentialsOnly
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public int dwProcessId;
                public int dwThreadId;
            }

            [Flags]
            public enum ProcessCreationFlags : uint
            {
                DEBUG_PROCESS = 0x00000001,
                DEBUG_ONLY_THIS_PROCESS = 0x00000002,
                CREATE_SUSPENDED = 0x00000004,
                DETACHED_PROCESS = 0x00000008,
                CREATE_NEW_CONSOLE = 0x00000010,
                CREATE_NEW_PROCESS_GROUP = 0x00000200,
                CREATE_UNICODE_ENVIRONMENT = 0x00000400,
                CREATE_SEPARATE_WOW_VDM = 0x00000800,
                CREATE_SHARED_WOW_VDM = 0x00001000,
                INHERIT_PARENT_AFFINITY = 0x00010000,
                CREATE_PROTECTED_PROCESS = 0x00040000,
                EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
                CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
                CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
                CREATE_DEFAULT_ERROR_MODE = 0x04000000,
                CREATE_NO_WINDOW = 0x08000000
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct STARTUPINFO
            {
                public int cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public int dwX;
                public int dwY;
                public int dwXSize;
                public int dwYSize;
                public int dwXCountChars;
                public int dwYCountChars;
                public int dwFillAttribute;
                public int dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }
        }
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public static class ProcessEx
        {
            [CanBeNull]
            public static System.Diagnostics.Process GetCurrentParentProcess()
            {
                return GetParentProcess(Process.GetCurrentProcess());
            }

            [CanBeNull]
            public static System.Diagnostics.Process GetParentProcess(SafeProcessHandle handle)
            {
                try
                {
                    Process.PROCESS_BASIC_INFORMATION pbi = new Process.PROCESS_BASIC_INFORMATION();
                    int status = Process.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
                    if (status != 0)
                        throw new ApplicationException("Could not get parent process.");

                    return System.Diagnostics.Process.GetProcessById((int)pbi.InheritedFromUniqueProcessId);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string GetCurrentProcessFileLocation()
            {
                var exe = new StringBuilder(1024);
                uint size = 1024;
                using var process = Process.GetCurrentProcess();
                if (!Process.QueryFullProcessImageName(process, 0, exe, ref size))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not fetch active process path.");

                return exe.ToString();
            }
            public static string GetProcessFileLocation(int processId)
            {
                using var process = Process.OpenProcess(Process.ProcessAccessFlags.QueryLimitedInformation, false, processId);
                if (process.DangerousGetHandle() == INVALID_HANDLE_VALUE)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not fetch process handle for path.");

                var exe = new StringBuilder(1024);
                uint size = 1024;
                if (!Process.QueryFullProcessImageName(process, 0, exe, ref size))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not fetch active process path.");

                return exe.ToString();
            }
        }
        
        public static class WTS
        {
            [DllImport("wtsapi32.dll", SetLastError = true)]
            public static extern int WTSEnumerateSessions(IntPtr hServer, int Reserved, int Version,
                ref IntPtr ppSessionInfo, ref int pCount);

            [DllImport("wtsapi32.dll", SetLastError = true)]
            public static extern bool WTSEnumerateProcesses(IntPtr serverHandle, Int32 reserved, Int32 version,
                ref IntPtr ppProcessInfo, ref Int32 pCount);

            [DllImport("kernel32.dll")]
            public static extern uint WTSGetActiveConsoleSessionId();

            [DllImport("wtsapi32.dll")]
            public static extern bool WTSQuerySessionInformation(IntPtr hServer, UInt32 sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

            [DllImport("wtsapi32.dll")]
            public static extern void WTSFreeMemory(IntPtr pMemory);

            public enum WTS_INFO_CLASS
            {
                WTSInitialProgram,
                WTSApplicationName,
                WTSWorkingDirectory,
                WTSOEMId,
                WTSSessionId,
                WTSUserName,
                WTSWinStationName,
                WTSDomainName,
                WTSConnectState,
                WTSClientBuildNumber,
                WTSClientName,
                WTSClientDirectory,
                WTSClientProductId,
                WTSClientHardwareId,
                WTSClientAddress,
                WTSClientDisplay,
                WTSClientProtocolType,
                WTSIdleTime,
                WTSLogonTime,
                WTSIncomingBytes,
                WTSOutgoingBytes,
                WTSIncomingFrames,
                WTSOutgoingFrames,
                WTSClientInfo,
                WTSSessionInfo
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WTS_CLIENT_ADDRESS
            {
                public uint AddressFamily;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
                public byte[] Address;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WTS_SESSION_INFO
            {
                public Int32 SessionID;
                [MarshalAs(UnmanagedType.LPStr)]
                public String pWinStationName;
                public WTS_CONNECTSTATE_CLASS State;
            }

            public enum WTS_CONNECTSTATE_CLASS
            {
                WTSActive,
                WTSConnected,
                WTSConnectQuery,
                WTSShadow,
                WTSDisconnected,
                WTSIdle,
                WTSListen,
                WTSReset,
                WTSDown,
                WTSInit
            }

            public struct WTS_PROCESS_INFO
            {
                public int SessionID;
                public int ProcessID;
                public IntPtr ProcessName;
                public IntPtr UserSid;
            }
        }
    }
}
