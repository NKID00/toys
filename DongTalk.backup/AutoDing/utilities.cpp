#include "AutoDing.h"


DWORD SaveCheck(HWND hWnd)
{
    
    RETURN_IF_NOT_EQUAL(MessageBox(
        hWnd,
        TEXT("此功能可能会间接导致数据丢失，请确定已经保存了所有未保存的工作！单击确定继续。"),
        TEXT("警告"),
        MB_ICONWARNING | MB_OKCANCEL | MB_DEFBUTTON2 | MB_SYSTEMMODAL | MB_TOPMOST
    ), IDOK, 1);
    RETURN_IF_NOT_EQUAL(MessageBox(
        hWnd,
        TEXT("请再次确定已经保存了所有未保存的工作！程序作者不对任何间接的数据丢失负责！单击确定继续。"),
        TEXT("警告"),
        MB_ICONWARNING | MB_OKCANCEL | MB_DEFBUTTON2 | MB_SYSTEMMODAL | MB_TOPMOST
    ), IDOK, 1);
    return 0;
}

/*
void PlayResource(DWORD Res)
{
    HMODULE hExe = GetModuleHandle(NULL);
    HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(Res), TEXT("WAVE"));
    HGLOBAL hgRes = LoadResource(hExe, hRes);
    LPVOID pRes = LockResource(hgRes);
    PlaySound((LPCWSTR)pRes, NULL, SND_MEMORY | SND_ASYNC);
}
*/
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

DWORD InitResources(HWND hWnd)
{
    RETURN_IF_FAILED(DirectSoundCreate8(NULL, &pds, NULL), 1);
    RETURN_IF_FAILED(pds->SetCooperativeLevel(hWnd, DSSCL_NORMAL), 1);
    hExe = GetModuleHandle(NULL);
    GetTempPath(_MAX_PATH, tmp);
    return 0;
}

/*
struct PlayResourceInfo
{
    HWND hWnd;
    LPWSTR path;
};

DWORD CALLBACK PlayResource(LPVOID arg)
{
    PlayResourceInfo* info = (PlayResourceInfo*)arg;

    return 0;
}
*/

/*
DWORD CALLBACK PlayResource(LPVOID arg)
{
    MCI_OPEN_PARMS mciOpen;
    mciOpen.lpstrDeviceType = TEXT("waveaudio");
    mciOpen.lpstrElementName = (LPWSTR)arg;
    if (mciSendCommand(NULL, MCI_OPEN, MCI_WAIT | MCI_OPEN_TYPE | MCI_OPEN_TYPE_ID | MCI_OPEN_ELEMENT, (DWORD)&mciOpen))
    {
        return 1;
    }
    WCHAR msg[200];
    BOOL s;
    s = mciGetErrorString(mciOpen.wDeviceID, msg, 200);
    MCI_PLAY_PARMS mciPlay;
    mciPlay.dwFrom = 0;
    if (mciSendCommand(mciOpen.wDeviceID, MCI_PLAY, MCI_WAIT | MCI_FROM, (DWORD)&mciPlay))
    {
        return 1;
    }
    s = mciGetErrorString(mciOpen.wDeviceID, msg, 200);
    if (mciSendCommand(mciOpen.wDeviceID, MCI_CLOSE, NULL, NULL))
    {
        return 1;
    }
    s = mciGetErrorString(mciOpen.wDeviceID, msg, 200);
    return 0;
}

*/

void PlayResource(LPDIRECTSOUNDBUFFER buffer)
{
    buffer->SetCurrentPosition(0);
    buffer->Play(0, 0, 0);
}

void InitAndPlayResource(DWORD id)
{
    LPDIRECTSOUNDBUFFER buffer = NULL;
    InitResource(IDR_WAVE1, buffer);
    PlayResource(buffer);
}

void Start()
{
    //ShowWindow(hWnd, SW_HIDE);
    /*
    HANDLE hThread;
    hThread = CreateThread(NULL, 0, PlayResource, (LPVOID)path1, 0, NULL);
    CloseHandle(hThread);
    */
    LPDIRECTSOUNDBUFFER buffer1 = NULL;
    InitResource(IDR_WAVE1, buffer1);
    LPDIRECTSOUNDBUFFER buffer2 = NULL;
    InitResource(IDR_WAVE2, buffer2);
    PlayResource(buffer1);
    Sleep(100);
    PlayResource(buffer2);
}
