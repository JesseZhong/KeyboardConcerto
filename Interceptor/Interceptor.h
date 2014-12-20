

#ifdef INTERCEPTOR_EXPORTS
#define INTERCEPTOR_API extern "C" __declspec(dllexport)
#else
#define INTERCEPTOR_API extern "C" __declspec(dllimport)
#endif

INTERCEPTOR_API BOOL InstallHook (HWND hwndParent);

INTERCEPTOR_API BOOL UninstallHook ();