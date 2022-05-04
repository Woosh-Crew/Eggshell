#include "Log.h"
#include "../External/Linker.h"

namespace Eggshell
{
    void Log::Info( const char* input )
    {
        Linker::Imported().func_log( input, 0 );
    }

    void Log::Warning( const char* input )
    {
        Linker::Imported().func_log( input, 1 );
    }

    void Log::Error( const char* input )
    {
        Linker::Imported().func_log( input, 2 );
    }
}
