﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAPS2.Scan;

namespace NAPS2.ImportExport
{
    [Serializable]
    public class DirectProfileTransfer
    {
        public DirectProfileTransfer(ScanProfile profile)
        {
            ProcessID = Process.GetCurrentProcess().Id;
            ScanProfile = profile.Clone();
        }

        public int ProcessID { get; private set; }

        public ScanProfile ScanProfile { get; private set; }
    }
}