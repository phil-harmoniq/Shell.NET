using System;
using System.IO;
using Xunit;
using Shell.NET;

namespace BashTests
{
    public class BashTester
    {
        private readonly Bash _bash;
        
        public BashTester()
        {
            _bash = new Bash();
        }

        [Fact]
        public void CatTest() =>
            Assert.Equal(0, _bash.Cat("~/.bashrc").ExitCode);

        [Fact]
        public void GrepTest() =>
            Assert.True(_bash.Grep("export", "~/.bashrc").Lines.Length > 0);

        [Fact]
        public void LsTest() =>
            Assert.True(_bash.Ls("-lhaF", "~").Lines.Length > 0);

        [Fact]
        public void CpMvRmTest()
        {
            var cp = _bash.Cp("~/.bashrc", "/tmp/bashrc-backup");
            var mv = _bash.Mv("/tmp/bashrc-backup", "~");
            var rm = _bash.Rm("~/bashrc-backup");
            
            Assert.Equal(0, cp.ExitCode);
            Assert.Equal(0, mv.ExitCode);
            Assert.Equal(0, rm.ExitCode);
        }
    }
}
