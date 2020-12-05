#include "header.h"

static HINSTANCE hInstance;

LRESULT CALLBACK ImageWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	PAINTSTRUCT paintstruct;
	HDC g_hdc;
	HBITMAP g_hbitmap;
	HDC g_mdc;
	switch (message)
	{
	case WM_PAINT: 
		g_hdc = BeginPaint(hWnd, &paintstruct);
		g_hdc = GetDC(hWnd);
		g_hbitmap = (HBITMAP)LoadImage(hInstance, MAKEINTRESOURCE(IDB_PNG1), IMAGE_BITMAP, 800, 600, LR_LOADFROMFILE);
		g_mdc = CreateCompatibleDC(g_hdc);
		SelectObject(g_mdc, g_hbitmap);
		BitBlt(g_hdc, 0, 0, 800, 600, g_mdc, 0, 0, SRCCOPY);
		ReleaseDC(hWnd, g_hdc);
		EndPaint(hWnd, &paintstruct);
		ValidateRect(hWnd, NULL);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

LRESULT CALLBACK EmptyWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

static LPCWSTR szClassNameEmpty = TEXT("DongTalkEmptyWindow");
static LPCWSTR szClassNameImage = TEXT("DongTalkImageWindow");
static WNDCLASSEX wc_empty;
static WNDCLASSEX wc_image;


HWND InitImage(HINSTANCE hIns)
{
	hInstance = hIns;

	ULONG_PTR m_pGdiplusToken;
	GdiplusStartupInput gdiplusStartupInput;
	GdiplusStartup(&m_pGdiplusToken, &gdiplusStartupInput, NULL);

	memset(&wc_empty, 0, sizeof(wc_empty));
	wc_empty.cbSize = sizeof(wc_empty);
	wc_empty.style = CS_NOCLOSE;
	wc_empty.lpfnWndProc = EmptyWndProc;
	wc_empty.hInstance = hInstance;
	wc_empty.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc_empty.lpszClassName = szClassNameEmpty;
	RegisterClassEx(&wc_empty);

	memset(&wc_image, 0, sizeof(wc_image));
	wc_image.cbSize = sizeof(wc_image);
	wc_image.style = CS_NOCLOSE;
	wc_image.lpfnWndProc = EmptyWndProc;
	wc_image.hInstance = hInstance;
	wc_image.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc_image.lpszClassName = szClassNameImage;
	RegisterClassEx(&wc_image);

	HWND hWnd = CreateWindowEx(WS_EX_LAYERED, szClassNameEmpty, TEXT("DongTalk"), WS_POPUP,
		100, 100, 100, 100, NULL, NULL, hInstance, NULL);
	ShowWindow(hWnd, SW_HIDE);
	UpdateWindow(hWnd);
	return hWnd;
}

static mutex image_mtx;

void _ShowImage(DWORD id)
{
	HWND hWnd = CreateWindowEx(/*WS_EX_LAYERED*/ 0, szClassNameEmpty, TEXT("DongTalk"), WS_OVERLAPPEDWINDOW,
		100, 100, 100, 100, NULL, NULL, hInstance, NULL);
	ShowWindow(hWnd, SW_SHOWNORMAL);
	UpdateWindow(hWnd);
	MSG msg;
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	image_mtx.lock();
	image_mtx.unlock();
	DestroyWindow(hWnd);
}

static bool image_locked = false;

void ShowImage(DWORD id)
{
	if (!image_locked)
	{
		image_mtx.lock();
	}
	thread t(_ShowImage, id);
	t.detach();
}

void ImageClose(HWND hWnd)
{
	DestroyWindow(hWnd);
}

void ImageClear()
{
	if (image_locked)
	{
		image_mtx.unlock();
	}
}
