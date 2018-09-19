using System;
using System.Collections;
using System.IO;

using Codice.CmdRunner;
using log4net;

namespace repliKate
{
    struct Branch
    {
        internal string Name;
        internal long Id;

        internal Branch(string name, string id)
        {
            Name = name;
            Id = long.Parse(id);
        }

        public override bool Equals(object obj)
        {
            return ((Branch)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }

    }

    internal class BranchHandler
    {
        private static readonly ILog mLog = LogManager.GetLogger("Replikate");

        private string mSrcServer;
        private string mCmExec;
        private string mWkpath;

        public BranchHandler(string srcserver, string cmExec, string wkpath)
        {
            mSrcServer = srcserver;
            mCmExec = cmExec;
            mWkpath = wkpath;
        }

        internal IList GetBranches()
        {
            ArrayList result = new ArrayList();

            try
            {
                string cmdres = CmdRunner.ExecuteCommandWithStringResult(
                    mCmExec + " find branch --format={name}#{id} --nototal " +
                    string.Format("on repository '{0}'", mSrcServer), mWkpath);

                StringReader reader = new StringReader(cmdres);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == string.Empty)
                        continue;

                    // output will be like /main/SCM5754#2625
                    string[] values = line.Split('#');

                    Branch br = new Branch(values[0], values[1]);
                    result.Add(br);
                }

                result.Sort(new BranchComparer());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error listing branches: " + e.Message);
                mLog.ErrorFormat("Error listing branches:{0} at {1}{2}",
                    e.Message, Environment.NewLine, e.StackTrace);
                return null;
            }

            return result;
        }

        internal IList FilterBranches(IList branches, string initialdate)
        {

            string cmd = mCmExec +
                " find \"changeset where date >= '" + initialdate +
                "'\" --format={branch} --nototal on repository '" +
                mSrcServer + "'";

            string cmdres = CmdRunner.ExecuteCommandWithStringResult(cmd, mWkpath);

            StringReader reader = new StringReader(cmdres);

            ArrayList filtered = new ArrayList();

            // there will be many csets on some branches, only take on
            // branch for each

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim() == string.Empty)
                    continue;

                line = line.Trim();

                if (!filtered.Contains(line))
                    filtered.Add(line);
            }

            filtered.Sort();

            ArrayList result = new ArrayList();
            foreach (string branchname in filtered)
            {
                // find it on the original list... (it has the id list)
                foreach (Branch br in branches)
                {
                    if (br.Name == branchname)
                    {
                        if (!result.Contains(br))
                            result.Add(br);
                        break;
                    }
                }
            }

            result.Sort(new BranchComparer());

            return result;
        }
    }

    class BranchComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Branch brX = (Branch)x;
            Branch brY = (Branch)y;

            return brX.Id.CompareTo(brY.Id);
        }
    }
}
