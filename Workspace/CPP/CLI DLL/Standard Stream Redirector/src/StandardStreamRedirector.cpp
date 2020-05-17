#include "StandardStreamRedirector.h"

using namespace CommonUtilities::StandardStreams::Redirectors;
using namespace System::Runtime::InteropServices;
using namespace System::Text;

bool StandardStreamRedirector::init(int streamFileDescriptor, String^ logFilePath)
{
	if (isValid(streamFileDescriptor) && !isInitiated(streamFileDescriptor))
	{
		isRedirected->Add(streamFileDescriptor, false);

		array<int>^ temp = gcnew array<int>(2);
		int tempType;

		if (String::IsNullOrWhiteSpace(logFilePath))
		{
			int pipe[2];
			_pipe(pipe, BUFFER_SIZE, O_TEXT);

			temp[WRITE_FD] = pipe[WRITE_FD];
			temp[READ_FD] = pipe[READ_FD];

			tempType = RedirectionType::Pipe;
		}
		else
		{
			char *tempLogFilePath = (char *)(Marshal::StringToHGlobalAnsi(logFilePath)).ToPointer();

			temp[WRITE_FD] = _sopen(tempLogFilePath, _O_TRUNC | _O_CREAT | _O_WRONLY, _SH_DENYWR, _S_IREAD | _S_IWRITE);
			temp[READ_FD] = _sopen(tempLogFilePath, _O_RDONLY, _SH_DENYNO);

			Marshal::FreeHGlobal(IntPtr((void *)tempLogFilePath));

			tempType = RedirectionType::File;
		}

		fdStdStreamPipe->Add(streamFileDescriptor, temp);
		fdStdStream->Add(streamFileDescriptor, _dup(_fileno(getStream(streamFileDescriptor))));

		redirectionType->Add(streamFileDescriptor, tempType);

		return true;
	}

	return false;
}

bool StandardStreamRedirector::dispose(int streamFileDescriptor)
{
	if (isValid(streamFileDescriptor) && isInitiated(streamFileDescriptor))
	{
		array<int>^ temp = fdStdStreamPipe[streamFileDescriptor];

		_close(fdStdStream[streamFileDescriptor]);


		_close(temp[WRITE_FD]);
		_close(temp[READ_FD]);

		fdStdStreamPipe->Remove(streamFileDescriptor);
		fdStdStream->Remove(streamFileDescriptor);

		isRedirected->Remove(streamFileDescriptor);

		return true;
	}

	return false;
}

FILE* StandardStreamRedirector::getStream(int streamFileDescriptor)
{
	switch (streamFileDescriptor)
	{
	case StreamFileDescriptor::Output:
		return stdout;

	case StreamFileDescriptor::Error:
		return stderr;

	default:
		return 0;
	}
}

bool StandardStreamRedirector::isValid(int streamFileDescriptor)
{
	return (streamFileDescriptor == StreamFileDescriptor::Output) || (streamFileDescriptor == StreamFileDescriptor::Error);
}

bool StandardStreamRedirector::isInitiated(int streamFileDescriptor)
{
	return isRedirected->ContainsKey(streamFileDescriptor);
}

int StandardStreamRedirector::open(int streamFileDescriptor, String^ logFilePath)
{
	init(streamFileDescriptor, logFilePath);
	
	if (isInitiated(streamFileDescriptor) && !isRedirected[streamFileDescriptor])
	{
		array<int> ^temp = fdStdStreamPipe[streamFileDescriptor];
		isRedirected[streamFileDescriptor] = true;
		
		fflush(getStream(streamFileDescriptor));
		CHECK(_dup2(temp[WRITE_FD], _fileno(getStream(streamFileDescriptor))));
		ios::sync_with_stdio();
		setvbuf(getStream(streamFileDescriptor), NULL, _IONBF, 0); // absolutely needed

		return true;
	}
	return false;
}

int StandardStreamRedirector::close(int streamFileDescriptor)
{
	bool stoped = false;

	if (isInitiated(streamFileDescriptor) && isRedirected[streamFileDescriptor])
	{
		isRedirected[streamFileDescriptor] = false;

		CHECK(_dup2(fdStdStream[streamFileDescriptor], _fileno(getStream(streamFileDescriptor))));
		ios::sync_with_stdio();

		stoped = true;
	}
	dispose(streamFileDescriptor);

	return stoped;
}

String^ StandardStreamRedirector::getBuffer(int streamFileDescriptor)
{
	if (isValid(streamFileDescriptor) && isInitiated(streamFileDescriptor))
	{
		bool isFileType = redirectionType[streamFileDescriptor] == RedirectionType::File;

		array<int>^ temp = fdStdStreamPipe[streamFileDescriptor];
		StringBuilder^ result = gcnew StringBuilder();
		
		char* buffer = new char[BUFFER_SIZE];
		int nOutRead = 0;
		
		if (isFileType)
			fflush(getStream(streamFileDescriptor));

		do
		{
			nOutRead = _read(temp[READ_FD], buffer, BUFFER_SIZE);
			buffer[nOutRead] = '\0';

			result->Append(gcnew String(buffer));

		} while (isFileType && nOutRead > 0);

		delete [] buffer;

		return result->ToString();
	}

	return String::Empty;
}

void StandardStreamRedirector::writeStandardStream(int streamFileDescriptor, String^ message)
{
	if (isValid(streamFileDescriptor) && isInitiated(streamFileDescriptor))
	{
		char *buffer = (char *)(Marshal::StringToHGlobalAnsi(message)).ToPointer();;
		_write(fdStdStream[streamFileDescriptor], buffer, strlen(buffer));

		Marshal::FreeHGlobal(IntPtr((void *)buffer));
	}
}