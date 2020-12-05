#pragma once

#include <Windows.h>
#include <dsound.h>
#pragma comment(lib,"dsound.lib")
#pragma comment(lib,"winmm.lib")
using namespace std;

extern "C"
{
    #define DONG_RETURN_IF_EQUAL(x, val, ret) do { if ((val) == (x)) { return (ret); } } while (false)
    #define DONG_RETURN_IF_NOT_EQUAL(x, val, ret) DONG_RETURN_IF_EQUAL((x) != (val), true, (ret))
    #define DONG_RETURN_IF_FAILED(x, ret) DONG_RETURN_IF_EQUAL(FAILED(x), true, (ret))
    #define DONG_RETURN_IF_NOT_ZERO(x, ret) DONG_RETURN_IF_NOT_EQUAL((x), 0, (ret))
    #define DONG_RETURN_IF_NULL(x, ret) DONG_RETURN_IF_EQUAL((x), NULL, (ret))

    // sound.cpp
    __declspec(dllexport) DWORD APIENTRY DongSoundInit(HWND hWnd);
    __declspec(dllexport) LPDIRECTSOUNDBUFFER* APIENTRY DongSoundLoad(char* path);
    __declspec(dllexport) DWORD APIENTRY DongSoundPlay(LPDIRECTSOUNDBUFFER* buffer);
    __declspec(dllexport) DWORD APIENTRY DongSoundLoadAndPlay(char* path);

    // bsod.cpp
    __declspec(dllexport) void APIENTRY DongBSOD(long code);
}
