
#include "stdafx.h"
#include <stdio.h>
#include <iostream>
#include "Interceptor.h"

#pragma data_seg (".SHARED")
// Windows message for communication between main executable and DLL module
UINT const WM_HOOK = WM_APP + 1;
// HWND of the main executable (managing application)
HWND hwndServer = NULL;
#pragma data_seg ()
#pragma comment (linker, "/section:.SHARED,RWS")

static HINSTANCE instanceHandle;
static HHOOK hookHandle;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		instanceHandle = hModule;
		hookHandle = NULL;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

// Keyboard Hook procedure
LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam) {
	if (code < 0) {
		return CallNextHookEx(hookHandle, code, wParam, lParam);
	}

	// Report the event to the main window. If the return value is 1, block the input; otherwise pass it along the Hook Chain
	int val = 0;
	if (val = SendMessage(hwndServer, WM_HOOK, wParam, lParam)) {
		std::cout << (int)val << std::endl;
		return 1;
	}

	return CallNextHookEx(hookHandle, code, wParam, lParam);
}

BOOL InstallHook(HWND hwndParent) {
	if (hwndServer != NULL) {
		// Already hooked
		return false;
	}

	// Register keyboard Hook
	hookHandle = SetWindowsHookEx(WH_KEYBOARD, (HOOKPROC)KeyboardProc, instanceHandle, 0);
	if (hookHandle == NULL) {
		return true;
	}
	hwndServer = hwndParent;
	return true;
}

BOOL UninstallHook() {
	if (hookHandle == NULL) {
		return true;
	}
	// If unhook attempt fails, check whether it is because of invalid handle (in that case continue)
	if (!UnhookWindowsHookEx(hookHandle)) {
		DWORD error = GetLastError();
		if (error != ERROR_INVALID_HOOK_HANDLE) {
			return false;
		}
	}
	hwndServer = NULL;
	hookHandle = NULL;
	return true;
}

LRESULT WindowProcess(UINT message, WPARAM wParam, LPARAM lParam) {

}