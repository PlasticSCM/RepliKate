using System;

using Codice.CmdRunner;

namespace repliKate
{
    class ConsoleAccess : IConsoleWriter
    {
        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public void Write(char[] buf)
        {
            Console.Write(buf);
        }

        public void Write(char[] buf, int index, int count)
        {
            Console.Write(buf, index, count);
        }
    }
}
