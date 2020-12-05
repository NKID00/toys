#pragma once

#include "resource.h"
#include <Windows.h>
#include <mmsystem.h>
#include <dsound.h>
#include <cstdio>
#pragma comment(lib,"dsound.lib")
#pragma comment(lib,"winmm.lib")
using namespace std;

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

// AutoDing.cpp

// bsod.cpp
void BSODInit();
void BSODRun();

// utilities.cpp
DWORD SaveCheck(HWND hWnd);
DWORD InitResources(HWND hWnd);
void Start();
