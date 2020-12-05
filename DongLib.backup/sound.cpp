#include "header.h"

static LPDIRECTSOUND8 pds = NULL;

DWORD APIENTRY DongSoundInit(HWND hWnd)
{
    DONG_RETURN_IF_FAILED(DirectSoundCreate8(NULL, &pds, NULL), 1);
    DONG_RETURN_IF_FAILED(pds->SetCooperativeLevel(hWnd, DSSCL_NORMAL), 1);
    return 0;
}

LPDIRECTSOUNDBUFFER* APIENTRY DongSoundLoad(char* path)
{
    int size = MultiByteToWideChar(CP_ACP, 0, path, -1, NULL, 0);
    LPWSTR path_ = new WCHAR[size];
    MultiByteToWideChar(CP_ACP, 0, path, -1, path_, size);
    HMMIO hmmio = mmioOpen(path_, NULL, MMIO_READ | MMIO_ALLOCBUF);
    delete[] path_;
    DONG_RETURN_IF_NULL(hmmio, NULL);
    MMCKINFO mmckriff;
    if (0 != mmioDescend(hmmio, &mmckriff, NULL, 0))
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    if (mmckriff.ckid != FOURCC_RIFF
        || mmckriff.fccType != mmioFOURCC('W', 'A', 'V', 'E'))
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    MMCKINFO mmckin;
    mmckin.ckid = mmioFOURCC('f', 'm', 't', ' ');
    if (0 != mmioDescend(hmmio, &mmckin, &mmckriff, MMIO_FINDCHUNK))
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    PCMWAVEFORMAT pwfm;
    if (mmioRead(hmmio, (HPSTR)&pwfm, sizeof(pwfm)) != sizeof(pwfm))
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    if (pwfm.wf.wFormatTag != WAVE_FORMAT_PCM)
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    WAVEFORMATEX wfmx;
    memcpy(&wfmx, &pwfm, sizeof(pwfm));
    wfmx.cbSize = 0;
    if (0 != mmioAscend(hmmio, &mmckin, 0))
    {
        mmioClose(hmmio, 0);
        return NULL;
    }
    mmckin.ckid = mmioFOURCC('d', 'a', 't', 'a');
    if (0 != mmioDescend(hmmio, &mmckin, &mmckriff, MMIO_FINDCHUNK))
    {
        mmioClose(hmmio, 0);
        return NULL;
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
    LPDIRECTSOUNDBUFFER *lpbuffer = new LPDIRECTSOUNDBUFFER;
    if (FAILED(pds->CreateSoundBuffer(&dsbd, lpbuffer, NULL)))
    {
        delete[] wave;
        return NULL;
    }
    LPVOID lBuffer = NULL;
    DWORD lBufferSize = 0;
    if (FAILED((*lpbuffer)->Lock(0, mmckin.cksize, &lBuffer, &lBufferSize, NULL, NULL, 0)))
    {
        delete[] wave;
        return NULL;
    }
    memcpy(lBuffer, wave, mmckin.cksize);
    delete[] wave;
    DONG_RETURN_IF_FAILED((*lpbuffer)->Unlock(lBuffer, lBufferSize, NULL, 0), NULL);
    return lpbuffer;
}

DWORD APIENTRY DongSoundPlay(LPDIRECTSOUNDBUFFER* buffer)
{
    DONG_RETURN_IF_FAILED((*buffer)->SetCurrentPosition(0), 1);
    DONG_RETURN_IF_FAILED((*buffer)->Play(0, 0, 0), 1);
    return 0;
}

DWORD APIENTRY DongSoundLoadAndPlay(char* path)
{
    LPDIRECTSOUNDBUFFER *buffer = NULL;
    DONG_RETURN_IF_NULL(buffer = DongSoundLoad(path), 1);
    DONG_RETURN_IF_NOT_ZERO(DongSoundPlay(buffer), 2);
    return 0;
}
