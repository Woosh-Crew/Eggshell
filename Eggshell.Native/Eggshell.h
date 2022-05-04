#pragma once

// Includes
// --------------------------------------------------------------------------------------- //

#include "Debugging/Log.h"
#include "External/Linker.h"

// Debugging
// --------------------------------------------------------------------------------------- //

#define LOG( input, level )\
    Log::Info( #input );

// Export API
// --------------------------------------------------------------------------------------- //

#if defined(_WIN32)
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT
#endif

#define EXPORT_API extern "C" DLL_EXPORT
