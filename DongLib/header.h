#pragma once

#include <thread>
#include <mutex>
#include <Windows.h>
#include <xaudio2.h>
using namespace std;

#define DONG_RETURN_IF_EQUAL(x, val, ret) do { if ((val) == (x)) { return (ret); } } while (false)
#define DONG_RETURN_IF_NOT_EQUAL(x, val, ret) DONG_RETURN_IF_EQUAL((x) != (val), true, (ret))
#define DONG_RETURN_IF_FAILED(x, ret) DONG_RETURN_IF_EQUAL(FAILED(x), true, (ret))
#define DONG_RETURN_IF_NOT_ZERO(x, ret) DONG_RETURN_IF_NOT_EQUAL((x), 0, (ret))
#define DONG_RETURN_IF_NULL(x, ret) DONG_RETURN_IF_EQUAL((x), NULL, (ret))

// sound.cpp
struct DongSoundBuffer
{
	XAUDIO2_BUFFER* buffer;
	WAVEFORMATEX* wfx;
};

/// From https://docs.microsoft.com/en-us/windows/win32/xaudio2/how-to--load-audio-data-files-in-xaudio2
#define fourccRIFF 'FFIR'
#define fourccDATA 'atad'
#define fourccFMT ' tmf'
#define fourccWAVE 'EVAW'
HRESULT FindChunk(HANDLE hFile, DWORD fourcc, DWORD& dwChunkSize, DWORD& dwChunkDataPosition);
HRESULT ReadChunkData(HANDLE hFile, void* buffer, DWORD buffersize, DWORD bufferoffset);
/// End

__declspec(dllexport) DWORD APIENTRY DongSoundInit();
__declspec(dllexport) DongSoundBuffer* APIENTRY DongSoundLoad(LPCWSTR path);
__declspec(dllexport) DWORD APIENTRY DongSoundPlay(DongSoundBuffer* buffer);
__declspec(dllexport) void APIENTRY DongSoundUnload(DongSoundBuffer* buffer);
__declspec(dllexport) void APIENTRY DongSoundQuit();

// messages.cpp

// utilities.cpp
__declspec(dllexport) LPWSTR APIENTRY DongCreateText(LPCSTR cstr);
__declspec(dllexport) void APIENTRY DongDestroyText(LPWSTR str);
__declspec(dllexport) void APIENTRY DongBSOD(long code);
__declspec(dllexport) HICON APIENTRY DongGetWindow(LPCWSTR title);
__declspec(dllexport) HICON APIENTRY DongMoveWindow(HWND handle, int x, int y, int align);
__declspec(dllexport) HICON APIENTRY DongChangeTitle(HWND handle, LPCWSTR title);
__declspec(dllexport) HICON APIENTRY DongGetIcon(LPCWSTR path, int index);
__declspec(dllexport) int APIENTRY DongScreenSizeW();
__declspec(dllexport) int APIENTRY DongScreenSizeH();
__declspec(dllexport) void APIENTRY DongAbout(HWND handle, LPCWSTR title, LPCWSTR text1, LPCWSTR text2, HICON icon);
