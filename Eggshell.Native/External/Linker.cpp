#include "Linker.h"

namespace Eggshell
{
    static Linker Instance;

    Imports Linker::Imported()
    {
        return Instance._imported;
    }

    Exports Linker::Link( const Imports imports )
    {
        Instance._imported = imports;
        Log::Info( "Hello World from Eggshell!" );
        return Instance._exports;
    }

    // Export

    EXPORT_API Exports Link( const Imports imports )
    {
        return Linker::Link( imports );
    }
}
