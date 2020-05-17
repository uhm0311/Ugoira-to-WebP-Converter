#pragma once

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif

int webpmux_main(int argc, const char** argv);

class __declspec( dllexport ) webpmux_wrapper
{
public:
	static int call_webpmux_main(int argc, const char **argv)
	{
		return webpmux_main(argc, argv);
	}
};