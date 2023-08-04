#ifndef __IPC_I_H__
#define __IPC_I_H__
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#ifdef IPCDLLAPI_EXPORT
#define IPCDLLAPI extern "C" __declspec(dllexport)
#else
#define IPCDLLAPI extern "C" __declspec(dllimport)
#endif

namespace kuaishou {
namespace ipc {
enum IPC_CS_TYPE { CLIENT, SERVER, UNKNOWN };

typedef void(DataReceivedCallback)(const char* data, size_t len, void* user_data);
typedef void(ConnectedCallback)(void* user_data);
typedef void(DisconnectCallback)(void* user_data);
typedef void(LogCallback)(const char* data, size_t len, void* user_data);

IPCDLLAPI int InitIpc(const char* name, size_t len, IPC_CS_TYPE type);
IPCDLLAPI int ReleaseIpc();

IPCDLLAPI int SendData(const char* data, size_t len);
IPCDLLAPI void SetDataReceivedCallback(DataReceivedCallback cb, void* user_data);
IPCDLLAPI void SetConnectedCallback(ConnectedCallback cb, void* user_data);
IPCDLLAPI void SetDisconnectCallback(DisconnectCallback cb, void* user_data);
IPCDLLAPI void SetLogCallback(LogCallback cb, void* user_data);
}
}

#endif
