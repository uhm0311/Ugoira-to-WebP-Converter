// CLI Wrapper DLL.h

#pragma once

#include <stdio.h>

using namespace System;

namespace Pixiv
{
	namespace Utilities
	{
		namespace Ugoira
		{
			namespace NativeCode
			{
				public ref class WebPLibraryWrapper
				{
				private:
					static void callWebPLibrary(String^ name, array<String^> ^params);

				public:
					static void callWebPEncoder(array<String^> ^params);
					static void callWebMux(array<String^> ^params);
				};
			}
		}
	}
}
