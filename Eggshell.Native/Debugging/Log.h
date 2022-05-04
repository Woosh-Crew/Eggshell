#pragma once

namespace Eggshell
{
    class Log
    {
    public:
        static void Info( const char* input );
        static void Warning( const char* input );
        static void Error( const char* input );
    };
}
