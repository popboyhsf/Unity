#ifndef __IPC_I_WRAPPER_H__
#define __IPC_I_WRAPPER_H__
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#ifdef IPCDLLAPI_EXPORT
#define IPCDLLAPI extern "C" __declspec(dllexport)
#else
#define IPCDLLAPI extern "C" __declspec(dllimport)
#endif

#include "ipc_i.h"

namespace kuaishou {
namespace ipc {

class IpcListener {
 public:
  virtual void OnDataReceived(const char* message, size_t len) = 0;
  virtual void OnConnected() = 0;
  virtual void OnDisconnected() = 0;
  virtual void OnLog(const char* log, size_t len) {}
  virtual ~IpcListener() {}
};

class IpcInst {
 public:
  virtual int SendData(const char* message, size_t len) = 0;
  virtual ~IpcInst() {}
};

IPCDLLAPI IpcInst* KwaiCreateIpcInst(const char* name, size_t len, IPC_CS_TYPE type, IpcListener* listener);
IPCDLLAPI void KwaiReleaseIpcInst(IpcInst* inst);
IPCDLLAPI int KwaiSendDataByInst(IpcInst* inst, const char* data, size_t len);
}
}

#endif
