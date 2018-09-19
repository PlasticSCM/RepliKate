using System;

using System.Collections;
using System.Reflection;
using System.IO;
using Codice.CmdRunner;

using log4net;
using log4net.Config;

namespace repliKate
{
    class RepliKate
    {
        private static string INITIAL_BRANCH_ARG = "--initialbranch=";
        private static string SYNC_DATE_ARG = "--syncdate=";
        private static string CM_EXEC_ARG = "--cmexe=";
        private static string STOP_ON_ERROR_ARG = "--stoponerror=";
        private static string ONLY_SHOW_UNSYNCED_ARG = "--onlyshowunsynced=";

        private static readonly ILog mLog = LogManager.GetLogger("Replikate");

        static void Main(string[] args)
        {
            RepliKateParams rParams = GetRepliKateParams(args);
            if (rParams == null)
            {
                PrintUsage();
                return;
            }

            ConfigureLogging();

            InitCmdRunner();

            IList branches = GetBranches(rParams);

            if (rParams == null) return;

            PrintBranchesToReplicate(branches);

            if (rParams.OnlyShowUnsynced) return;

            DoReplication(branches, rParams);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: replikate srcrepos@srcserver dstrepos@dstserver" +
                "\n\t[--initialbranch=number]" +
                "\n\t[--syncdate=initialdate(Month/Day/Year) | yesterday]" +
                "\n\t[--cmexe=cm|bcm]" +
                "\n\t[--stoponerror=true|false] (Default: true) " +
                "\n\t[--onlyshowunsynced=true|false (Only shows unsynced branches. Default: false)]");
        }

        private static RepliKateParams GetRepliKateParams(string[] args)
        {
            if (args.Length < 2)
                return null;

            RepliKateParams rParams = new RepliKateParams();

            rParams.SrcSrv = args[0];
            rParams.DstSrv = args[1];

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].StartsWith(INITIAL_BRANCH_ARG))
                {
                    rParams.InitBranch = int.Parse(args[i].Substring(
                        INITIAL_BRANCH_ARG.Length));

                    mLog.DebugFormat("Going to use InitBranch:{0}", rParams.InitBranch);
                }
                else if (args[i].StartsWith(SYNC_DATE_ARG))
                {
                    rParams.Syncdate = args[i].Substring(SYNC_DATE_ARG.Length);
                    if (rParams.Syncdate.ToLower().Trim().StartsWith("yesterday"))
                        rParams.Syncdate = GetYesterdayDate();

                    mLog.DebugFormat("Going to use SyncDate:{0}", rParams.Syncdate);
                }

                else if (args[i].StartsWith(CM_EXEC_ARG))
                {
                    rParams.CmExec = args[i].Substring(CM_EXEC_ARG.Length);
                    mLog.DebugFormat("Going to use cm executable:{0}", rParams.CmExec);
                }
                else if (args[i].StartsWith(STOP_ON_ERROR_ARG))
                {
                    if (args[i].Substring(STOP_ON_ERROR_ARG.Length)
                        .ToLower().Trim().StartsWith("false"))
                    {
                        rParams.StopOnError = false;
                    }
                }

                else if (args[i].StartsWith(ONLY_SHOW_UNSYNCED_ARG))
                {
                    if (args[i].Substring(ONLY_SHOW_UNSYNCED_ARG.Length)
                        .ToLower().Trim().StartsWith("true"))
                    {
                        rParams.OnlyShowUnsynced = true;
                    }
                }
            }

            return rParams;
        }

        private static void ConfigureLogging()
        {
            string log4netpath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "replikate.log.conf");

            XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netpath));
        }

        private static void InitCmdRunner()
        {
            ConsoleAccess console = new ConsoleAccess();

            CmdRunner.InitConsole(console as IConsoleWriter);
            CmdRunner.SetBotWorkingMode();
        }

        private static IList GetBranches(RepliKateParams rParams)
        {
            BranchHandler brHandler = new BranchHandler(
                rParams.SrcSrv, rParams.CmExec, Environment.CurrentDirectory);

            IList branches = brHandler.GetBranches();

            if (branches == null) return null;

            if (rParams.Syncdate != string.Empty)
            {
                branches = brHandler.FilterBranches(branches, rParams.Syncdate);
            }

            return branches;
        }

        private static void PrintBranchesToReplicate(IList branches)
        {
            Console.WriteLine("Going to replicate the following branches:");

            foreach (Branch branch in branches)
            {
                Console.WriteLine("br:{0} Id {1}", branch.Name, branch.Id);
            }
        }

        private static void DoReplication(IList branches, RepliKateParams rParams)
        {
            Replicator replicator = new Replicator(
                rParams.CmExec, rParams.StopOnError);
            try
            {
                replicator.Replicate(
                    branches, rParams.SrcSrv, rParams.DstSrv,
                    Environment.CurrentDirectory, rParams.InitBranch);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Error replicating branches: " +
                    e.Message +
                    Environment.NewLine +
                    e.StackTrace);
            }
        }

        private static string GetYesterdayDate()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            string res = string.Empty;
            res = String.Format("{0}/{1}/{2}",
                yesterday.Month, yesterday.Day, yesterday.Year);
            return res;
        }

        private class RepliKateParams
        {
            public String SrcSrv;
            public String DstSrv;
            public string Syncdate = string.Empty;
            public int InitBranch = 0;
            public string CmExec = "cm";
            public bool StopOnError = true;
            public bool OnlyShowUnsynced = false;
        }

    }
}
