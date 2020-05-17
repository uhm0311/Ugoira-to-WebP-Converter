// 기본 DLL 파일입니다.

#include "WebPLibraryWrapper.h"
#pragma managed(push, off)
#include <cwebp.h>
#include <webpmux.h>
#pragma comment (lib, "libwebp.lib")
#pragma managed(pop)

using namespace Pixiv::Utilities::Ugoira::NativeCode;
using namespace System::Runtime::InteropServices;

#define cwebp_name "cwebp.exe"
#define webpmux_name "webpmux.exe"

void WebPLibraryWrapper::callWebPLibrary(String^ name, array<String^> ^params)
{
	const int argc = params->Length + 1;
	const char **argv = new const char*[argc];

	argv[0] = (char *)(Marshal::StringToHGlobalAnsi(name)).ToPointer();
	for (int i = params->Length; i; i--)
		argv[i] = (char *)(Marshal::StringToHGlobalAnsi(params[i - 1])).ToPointer();

	if (name->Equals(cwebp_name))
		cwebp_wrapper::call_cwebp_main(argc, argv);

	else if (name->Equals(webpmux_name))
		webpmux_wrapper::call_webpmux_main(argc, argv);

	for (int i = params->Length; i >= 0; i--)
		Marshal::FreeHGlobal(IntPtr((void *)argv[i]));
}

void WebPLibraryWrapper::callWebPEncoder(array<String^> ^params)
{
	callWebPLibrary(cwebp_name, params);
}

void WebPLibraryWrapper::callWebMux(array<String^> ^params)
{
	callWebPLibrary(webpmux_name, params);
}