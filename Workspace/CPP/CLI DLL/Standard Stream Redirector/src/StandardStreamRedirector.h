#pragma once

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif

#define BUFFER_SIZE 4096

#define READ_FD 0
#define WRITE_FD 1

#define CHECK(a) if ((a)!= 0) return -1;

#include <stdio.h>
#include <fcntl.h>
#include <io.h>
#include <sys\stat.h>
#include <iostream>

#ifndef _USE_OLD_IOSTREAMS
using namespace std;
#endif

using namespace System;
using namespace System::Collections::Generic;

namespace CommonUtilities
{
	namespace StandardStreams
	{
		namespace Redirectors
		{
			public ref class StandardStreamRedirector
			{
			public:
				static ref class StreamFileDescriptor
				{ 
				public :
					static const int Output = 1;
					static const int Error = 2;
				};

				static ref class RedirectionType
				{
				public:
					static const int Pipe = 0;
					static const int File = 1;
				};

				static int open(int streamFileDescriptor, String^ logFilePath);
				static int close(int streamFileDescriptor);
				static String^ getBuffer(int streamFileDescriptor);

				static void writeStandardStream(int streamFileDescriptor, String^ message);

			private:
				StandardStreamRedirector() { }

				static Dictionary<int, int>^ redirectionType = gcnew Dictionary<int, int>();
				static Dictionary<int, bool>^ isRedirected = gcnew Dictionary<int, bool>();

				static Dictionary<int, int>^ fdStdStream = gcnew Dictionary<int, int>();
				static Dictionary<int, array<int>^>^ fdStdStreamPipe = gcnew Dictionary<int, array<int>^>();

				static bool init(int standardStream, String^ logFilePath);
				static bool dispose(int standardStream);

				static FILE* getStream(int streamFileDescriptor);

				static bool isValid(int value);
				static bool isInitiated(int streamFileDescriptor);
			};

			/*const int StandardStreamRedirector::StreamFileDescriptor::Output;
			const int StandardStreamRedirector::StreamFileDescriptor::Error;*/
		}
	}
}