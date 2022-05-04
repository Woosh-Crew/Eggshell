#pragma once

class Project
{
public:
    virtual void OnReady();
    virtual void OnFrame();
    virtual void OnShutdown();
};
