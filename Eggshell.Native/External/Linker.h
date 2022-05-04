#pragma once
#include "Linker.Generated.h"
#include "../Eggshell.h"

namespace Eggshell
{
    // Function Pointers needed from external sources
    struct Imports
    {
        IMPORT_GENERATED()
    };

    // Function Pointers to give to external sources
    struct Exports
    {
        EXPORT_GENERATED()
    };

    class Linker
    {
        Imports _imported;
        Exports _exports;

    public:
        static Imports Imported();
        static Exports Link( Imports imports );
    };
}
