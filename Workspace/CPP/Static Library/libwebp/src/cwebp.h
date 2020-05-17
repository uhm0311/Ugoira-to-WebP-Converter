#pragma once

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif

int cwebp_main(int argc, const char **argv);

class __declspec( dllexport ) cwebp_wrapper
{
public:
	static int call_cwebp_main(int argc, const char **argv)
	{
		return cwebp_main(argc, argv);
	}
};