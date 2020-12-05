#include "header.h"

void ShowAbout(HWND Handle, LPCWSTR Title, LPCWSTR Text, LPCWSTR OtherText, HICON Icon)
{
    int len_title = wcslen(Title);
    int len_text = wcslen(Text);
    LPWSTR app_text = new WCHAR[len_title + len_text + 2];
    wcscpy_s(app_text, len_title + 1, Title);
    app_text[len_title] = TEXT('#');
    wcscpy_s(app_text + len_title + 1, len_text + 1, Text);
    ShellAboutW(nullptr, app_text, OtherText, nullptr);
    delete app_text;
}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
{
    ShowAbout(nullptr, TEXT("关于“错误”"), TEXT("M1©r0$0ft W1nd0w$"), nullptr, nullptr);
    return 0;
}
