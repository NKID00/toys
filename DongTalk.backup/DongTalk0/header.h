#pragma once

#include "resource.h"

#include <cstdio>
#include <thread>
#include <mutex>
#include <Windows.h>
#include <dsound.h>
#include <gdiplus.h>
#pragma comment(lib,"dsound.lib")
#pragma comment(lib,"winmm.lib")
#pragma comment (lib,"Gdiplus.lib")
using namespace std;
using namespace Gdiplus;

#define RETURN_IF_FAILED(x, ret) do { \
    if (FAILED(x)) \
    { \
        return (ret);\
    } \
} while (false)
#define RETURN_IF_NOT_ZERO(x, ret) do { \
    if (0 != (x)) \
    { \
        return (ret);\
    } \
} while (false)
#define RETURN_IF_NOT_EQUAL(x, val, ret) do { \
    if ((val) != (x)) \
    { \
        return (ret);\
    } \
} while (false)

// sound.cpp
DWORD InitSound(HWND hWnd);
void PlayResource(LPDIRECTSOUNDBUFFER buffer);
void InitAndPlayResource(DWORD id);

// image.cpp
HWND InitImage(HINSTANCE hIns);
void ShowImage(DWORD id);
void ImageClose(HWND hWnd);
void ImageClear();

// messagebox.cpp
#define ALIGN_X_LEFT	0b1000
#define ALIGN_X_MIDDLE	0b1100
#define ALIGN_X_RIGHT	0b0100
#define ALIGN_Y_TOP		0b0010
#define ALIGN_Y_MIDDLE	0b0011
#define ALIGN_Y_BOTTOM	0b0001
#define ALIGN_X_CHECK(align) ((align) & 0b1100)
#define ALIGN_Y_CHECK(align) ((align) & 0b0011)
void MessageBoxXY(HWND hWnd, int x, int y, int align, LPCWSTR text, LPCWSTR caption, UINT option);
void MessageBoxClear();

// bsod.cpp
void BSODInit();
void BSODRun();
