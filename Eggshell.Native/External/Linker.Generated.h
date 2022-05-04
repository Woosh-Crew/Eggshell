#pragma once

#define IMPORT_GENERATED()\
    void(*func_log)(const char* message, int level);

#define EXPORT_GENERATED()\
    void(*func_onframe)(float delta);\
    void(*func_onshutdown)();
