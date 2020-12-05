#include "header.h"

int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow)
{
	InitSound(InitImage(hInstance));

	ShowImage(IDB_PNG1);

	Sleep(1000000);
	return 0;

	InitAndPlayResource(IDR_WAVE1);
	Sleep(300);
	InitAndPlayResource(IDR_WAVE2);

	int cx = GetSystemMetrics(SM_CXFULLSCREEN);
	int cy = GetSystemMetrics(SM_CYFULLSCREEN);
	MessageBoxXY(NULL, cx / 2, cy / 2, ALIGN_X_LEFT | ALIGN_Y_TOP, TEXT("加载中……"), TEXT(""), MB_OKCANCEL | MB_ICONWARNING | MB_TOPMOST | MB_SYSTEMMODAL);
	Sleep(1000);
	MessageBoxClear();
	for (int i = 30; i < cy / 2; i += 30)
	{
		MessageBoxXY(NULL, i, i, ALIGN_X_LEFT | ALIGN_Y_TOP, TEXT("如果保存此图片任何透明度将丢失。是否要继续？"), TEXT("画图"), MB_OKCANCEL | MB_ICONWARNING | MB_TOPMOST | MB_SYSTEMMODAL);
		Sleep(300);
	}
	for (int i = 30; i < cy / 2; i += 30)
	{
		MessageBoxXY(NULL, cx - i, i, ALIGN_X_RIGHT | ALIGN_Y_TOP, TEXT("如果保存此图片任何透明度将丢失。是否要继续？"), TEXT("画图"), MB_OKCANCEL | MB_ICONWARNING | MB_TOPMOST | MB_SYSTEMMODAL);
		Sleep(300);
	}
	Sleep(2000);
	MessageBoxClear();
	return 0;
}

int main()
{
	return wWinMain(NULL, NULL, NULL, 0);
}
