#include "header.h"

static mutex msg_mtx;

void _MessageBoxXY(ULONG id, HWND hWnd, int x, int y, int align, LPCWSTR text, LPCWSTR caption, UINT option)
{
    WCHAR tcaption[10];
    wsprintf(tcaption, TEXT("%08x"), id);
    thread t(MessageBox, hWnd, text, tcaption, option);
    t.detach();
    HWND handle = NULL;
    while (handle == NULL)
    {
        handle = FindWindow(NULL, tcaption);
    }
    RECT r;
    GetWindowRect(handle, &r);
    int w = r.right - r.left;
    int h = r.bottom - r.top;
    int align_x = ALIGN_X_CHECK(align);
    switch (align_x)
    {
    case ALIGN_X_LEFT:
        break;
    case ALIGN_X_MIDDLE:
        x -= w / 2;
        break;
    case ALIGN_X_RIGHT:
        x -= w;
    default:
        break;
    }
    int align_y = ALIGN_Y_CHECK(align);
    switch (align_y)
    {
    case ALIGN_Y_TOP:
        break;
    case ALIGN_Y_MIDDLE:
        y -= h / 2;
        break;
    case ALIGN_Y_BOTTOM:
        y -= h;
    default:
        break;
    }
    MoveWindow(handle, x, y, w, h, true);
    SetWindowText(handle, caption);
    HMENU hMenu = GetSystemMenu(handle, FALSE);
    EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED | MF_BYCOMMAND);
    msg_mtx.lock();
    msg_mtx.unlock();
    PostMessage(handle, WM_CLOSE, NULL, NULL);
}

static ULONG msgid = 0;
static bool msg_locked = false;

void MessageBoxXY(HWND hWnd, int x, int y, int align, LPCWSTR text, LPCWSTR caption, UINT option)
{
    if (!msg_locked)
    {
        msg_mtx.lock();
        msg_locked = true;
    }
    thread t(_MessageBoxXY, msgid, hWnd, x, y, align, text, caption, option);
    t.detach();
    msgid++;
}

void MessageBoxClear()
{
    if (msg_locked)
    {
        msg_mtx.unlock();
        msg_locked = false;
    }
}
