#include "header.h"

/// From https://docs.microsoft.com/en-us/windows/win32/xaudio2/how-to--load-audio-data-files-in-xaudio2
HRESULT FindChunk(HANDLE hFile, DWORD fourcc, DWORD& dwChunkSize, DWORD& dwChunkDataPosition)
{
    HRESULT hr = S_OK;
    if (INVALID_SET_FILE_POINTER == SetFilePointer(hFile, 0, NULL, FILE_BEGIN))
        return HRESULT_FROM_WIN32(GetLastError());

    DWORD dwChunkType;
    DWORD dwChunkDataSize;
    DWORD dwRIFFDataSize = 0;
    DWORD dwFileType;
    DWORD bytesRead = 0;
    DWORD dwOffset = 0;

    while (hr == S_OK)
    {
        DWORD dwRead;
        if (0 == ReadFile(hFile, &dwChunkType, sizeof(DWORD), &dwRead, NULL))
            hr = HRESULT_FROM_WIN32(GetLastError());

        if (0 == ReadFile(hFile, &dwChunkDataSize, sizeof(DWORD), &dwRead, NULL))
            hr = HRESULT_FROM_WIN32(GetLastError());

        switch (dwChunkType)
        {
        case fourccRIFF:
            dwRIFFDataSize = dwChunkDataSize;
            dwChunkDataSize = 4;
            if (0 == ReadFile(hFile, &dwFileType, sizeof(DWORD), &dwRead, NULL))
                hr = HRESULT_FROM_WIN32(GetLastError());
            break;

        default:
            if (INVALID_SET_FILE_POINTER == SetFilePointer(hFile, dwChunkDataSize, NULL, FILE_CURRENT))
                return HRESULT_FROM_WIN32(GetLastError());
        }

        dwOffset += sizeof(DWORD) * 2;

        if (dwChunkType == fourcc)
        {
            dwChunkSize = dwChunkDataSize;
            dwChunkDataPosition = dwOffset;
            return S_OK;
        }

        dwOffset += dwChunkDataSize;

        if (bytesRead >= dwRIFFDataSize) return S_FALSE;

    }

    return S_OK;

}

HRESULT ReadChunkData(HANDLE hFile, void* buffer, DWORD buffersize, DWORD bufferoffset)
{
    HRESULT hr = S_OK;
    if (INVALID_SET_FILE_POINTER == SetFilePointer(hFile, bufferoffset, NULL, FILE_BEGIN))
        return HRESULT_FROM_WIN32(GetLastError());
    DWORD dwRead;
    if (0 == ReadFile(hFile, buffer, buffersize, &dwRead, NULL))
        hr = HRESULT_FROM_WIN32(GetLastError());
    return hr;
}
/// End

IXAudio2* pXAudio2 = nullptr;
IXAudio2MasteringVoice* pMasterVoice = nullptr;

DWORD APIENTRY DongSoundInit()
{
    DONG_RETURN_IF_FAILED(XAudio2Create(&pXAudio2), 1);
    DONG_RETURN_IF_FAILED(pXAudio2->CreateMasteringVoice(&pMasterVoice), 1);
    return 0;
}

DongSoundBuffer* APIENTRY DongSoundLoad(LPCWSTR path)
{
    DongSoundBuffer* buffer_ret = new DongSoundBuffer;

    HANDLE file;
    DONG_RETURN_IF_EQUAL(file = CreateFile(
        path, GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, 0, nullptr
        ), INVALID_HANDLE_VALUE, nullptr);
    DONG_RETURN_IF_EQUAL(SetFilePointer(file, 0, nullptr, FILE_BEGIN), INVALID_SET_FILE_POINTER, nullptr);

    DWORD chk_size;
    DWORD chk_position;
    DONG_RETURN_IF_FAILED(FindChunk(file, fourccRIFF, chk_size, chk_position), nullptr);
    DWORD filetype;
    DONG_RETURN_IF_FAILED(ReadChunkData(file, &filetype, sizeof(DWORD), chk_position), nullptr);
    if (filetype != fourccWAVE)
        return nullptr;

    DONG_RETURN_IF_FAILED(FindChunk(file, fourccFMT, chk_size, chk_position), nullptr);
    WAVEFORMATEX* wfx = new WAVEFORMATEX;
    memset(wfx, 0, sizeof(wfx));
    DONG_RETURN_IF_FAILED(ReadChunkData(file, &wfx, chk_size, chk_position), nullptr);
    buffer_ret->wfx = wfx;

    DONG_RETURN_IF_FAILED(FindChunk(file, fourccDATA, chk_size, chk_position), nullptr);
    BYTE* buffer_data = new BYTE[chk_size];
    DONG_RETURN_IF_FAILED(ReadChunkData(file, buffer_data, chk_size, chk_position), nullptr);
    XAUDIO2_BUFFER* buffer = new XAUDIO2_BUFFER;
    memset(buffer, 0, sizeof(buffer));
    buffer->AudioBytes = chk_size;
    buffer->pAudioData = buffer_data;
    buffer->Flags = XAUDIO2_END_OF_STREAM;
    buffer_ret->buffer = buffer;

    return buffer_ret;
}

DWORD APIENTRY DongSoundPlay(DongSoundBuffer* buffer)
{
    IXAudio2SourceVoice* pSourceVoice = nullptr;
    DONG_RETURN_IF_FAILED(pXAudio2->CreateSourceVoice(&pSourceVoice, buffer->wfx), 1);
    DONG_RETURN_IF_FAILED(pSourceVoice->SubmitSourceBuffer(buffer->buffer), 1);
    DONG_RETURN_IF_FAILED(pSourceVoice->Start(), 1);
    XAUDIO2_VOICE_STATE state;
    pSourceVoice->GetState(&state);
    pSourceVoice->DestroyVoice();
    return 0;
}

void APIENTRY DongSoundUnload(DongSoundBuffer* buffer)
{
    delete buffer->buffer;
    delete buffer->wfx;
    delete buffer;
}

void APIENTRY DongSoundQuit()
{
    pMasterVoice->DestroyVoice();
    pMasterVoice = nullptr;
    pXAudio2->Release();
    pXAudio2 = nullptr;
}
