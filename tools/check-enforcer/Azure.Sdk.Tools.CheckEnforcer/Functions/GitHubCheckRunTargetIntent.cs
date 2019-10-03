using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Sdk.Tools.CheckEnforcer.Functions
{
    public enum GitHubCheckRunTargetIntent
    {
        Unknown,
        Reset,
        Create,
        Evaluate,
        SetInProgress,
        SetSuccess,
        SetQueued
    }
}
