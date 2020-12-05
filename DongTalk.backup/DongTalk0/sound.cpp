#include "header.h"

static HMODULE hExe;
static WCHAR tmp[_MAX_PATH];
static LPDIRECTSOUND8 pds = NULL;

DWORD InitResource(DWORD id, LPDIRECTSOUNDBUFFER& buffer)
{
    HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(id), TEXT("WAVE"));
    DWORD size = SizeofResource(hExe, hRes);
    HGLOBAL hgRes = LoadResource(hExe, hRes);
    LPVOID pRes = LockResource(hgRes);
    WCHAR path[_MAX_PATH + 15];
    wsprintf(path, TEXT("%s%ld.wav"), tmp, id);
    HANDLE hFile = CreateFile(path, GENERIC_WRITE, NULL, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_TEMPORARY, NULL);
    DWORD x;
    WriteFile(hFile, pRes, size, &x, NULL);
    CloseHandle(hFile);
    HMMIO hmmio = mmioOpen(path, NULL, MMIO_READ | MMIO_ALLOCBUF);
    if (hmmio == NULL)
    {
        return 1;
    }
    MMCKINFO mmckriff;
    if (0 != mmioDescend(hmmio, &mmckriff, NULL, 0))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    if (mmckriff.ckid != FOURCC_RIFF
        || mmckriff.fccType != mmioFOURCC('W', 'A', 'V', 'E'))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    MMCKINFO mmckin;
    mmckin.ckid = mmioFOURCC('f', 'm', 't', ' ');
    if (0 != mmioDescend(hmmio, &mmckin, &mmckriff, MMIO_FINDCHUNK))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    PCMWAVEFORMAT pwfm;
    if (mmioRead(hmmio, (HPSTR)&pwfm, sizeof(pwfm)) != sizeof(pwfm))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    if (pwfm.wf.wFormatTag != WAVE_FORMAT_PCM)
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    WAVEFORMATEX wfmx;
    memcpy(&wfmx, &pwfm, sizeof(pwfm));
    wfmx.cbSize = 0;
    if (0 != mmioAscend(hmmio, &mmckin, 0))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    mmckin.ckid = mmioFOURCC('d', 'a', 't', 'a');
    if (0 != mmioDescend(hmmio, &mmckin, &mmckriff, MMIO_FINDCHUNK))
    {
        mmioClose(hmmio, 0);
        return 1;
    }
    UCHAR* wave = new UCHAR[mmckin.cksize];
    mmioRead(hmmio, (HPSTR)wave, mmckin.cksize);
    mmioClose(hmmio, 0);
    DSBUFFERDESC dsbd;
    dsbd.dwSize = sizeof(DSBUFFERDESC);
    dsbd.dwBufferBytes = mmckin.cksize;
    dsbd.dwFlags = DSBCAPS_GLOBALFOCUS | DSBCAPS_STATIC;
    dsbd.lpwfxFormat = &wfmx;
    dsbd.dwReserved = 0;
    if (FAILED(pds->CreateSoundBuffer(&dsbd, &buffer, NULL)))
    {
        delete[] wave;
        return 1;
    }
    LPVOID lBuffer = NULL;
    DWORD lBufferSize = 0;
    if (FAILED(buffer->Lock(0, mmckin.cksize, &lBuffer, &lBufferSize, NULL, NULL, 0)))
    {
        delete[] wave;
        return 1;
    }
    memcpy(lBuffer, wave, mmckin.cksize);
    delete[] wave;
    RETURN_IF_FAILED(buffer->Unlock(lBuffer, lBufferSize, NULL, 0), 1);
    return 0;
}

DWORD InitSound(HWND hWnd)
{
    RETURN_IF_FAILED(DirectSoundCreate8(NULL, &pds, NULL), 1);
    RETURN_IF_FAILED(pds->SetCooperativeLevel(hWnd, DSSCL_NORMAL), 1);
    hExe = GetModuleHandle(NULL);
    GetTempPath(_MAX_PATH, tmp);
    return 0;
}

void PlayResource(LPDIRECTSOUNDBUFFER buffer)
{
    buffer->SetCurrentPosition(0);
    buffer->Play(0, 0, 0);
}

void InitAndPlayResource(DWORD id)
{
    LPDIRECTSOUNDBUFFER buffer = NULL;
    InitResource(id, buffer);
    PlayResource(buffer);
}
