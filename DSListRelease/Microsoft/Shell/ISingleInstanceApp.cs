namespace Microsoft.Shell
{
    using System;
    using System.Collections.Generic;

    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IList<string> args);
    }
}

