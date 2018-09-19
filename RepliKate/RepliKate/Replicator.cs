using System;
using System.Collections;
using Codice.CmdRunner;

using log4net;

namespace repliKate
{
    internal class Replicator
    {
        private static readonly ILog mLog = LogManager.GetLogger("Replikate");
        private string mCmExec;
        private bool mStopOnError;

        public Replicator(string cmExec, bool stopOnError)
        {
            mCmExec = cmExec;
            mStopOnError = stopOnError;
        }

        internal void Replicate(
            IList branches, string src, string dst,
            string wkpath, int initBranch)
        {
            int init = Environment.TickCount;

            string cmd = "{3} replicate \"br:{0}@{1}\" \"rep:{2}\"";

            int count = 0;

            foreach (Branch branch in branches)
            {
                if (count < initBranch)
                {
                    ++count;
                    continue;
                }

                mLog.InfoFormat("Replicating branch {0}. {1} of {2}. " +
                    "Time so far {3}",
                    branch.Name, count++, branches.Count,
                    TimeSpan.FromMilliseconds(Environment.TickCount - init)
                        .ToString());

                string command = string.Format(
                    cmd, branch.Name, src, dst, mCmExec);

                //replicate
                try
                {
                    int ini = Environment.TickCount;
                    int cmdresult = CmdRunner.ExecuteCommandWithResult(
                        command, wkpath);
                    mLog.InfoFormat("Branch {0} replicated in {1} ms",
                        branch.Name, Environment.TickCount - ini);

                    if (!(cmdresult == 0) && mStopOnError)
                    {
                        throw new Exception(
                            "Replication didn't finished properly: Branch" +
                            branch.Name);
                    }

                }
                catch (Exception e)
                {
                    mLog.ErrorFormat("Failed to replicate branch {0}. {1}{2} " +
                        "at {3}",
                        branch.Name, e.Message,
                        Environment.NewLine, e.StackTrace);

                    if (mStopOnError)
                    {
                        throw e;
                    }
                    else
                        continue;
                }
            }
        }
    }
}
