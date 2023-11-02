﻿using System.Windows.Threading;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

public static class DispatcherUtil
{
    public static void DoEvents()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(ExitFrame), frame);
        Dispatcher.PushFrame(frame);
    }

    public static void DoEventsSync()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(ExitFrame), frame);
        Dispatcher.PushFrame(frame);
    }

    private static object? ExitFrame(object frame)
    {
        ((DispatcherFrame)frame).Continue = false;
        return null;
    }
}
