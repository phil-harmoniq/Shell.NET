using System;
using System.IO;
using Xunit;
using Shell.NET;

public class BashTests
{
    private readonly Bash _bash;
    
    public BashTests() =>
        _bash = new Bash();

    [Fact]
    public void CatTest() =>
        Assert.Equal(0, _bash.Cat("~/.bashrc", redirect: false).ExitCode);

    [Fact]
    public void LsTest() =>
        Assert.True(_bash.Ls("-lhaF", "~").Lines.Length > 1);

    [Fact]
    public void GrepTest()
    {
        var grep = _bash.Grep("export", "~/.bashrc");
        Assert.True(grep.Lines != null && grep.Lines.Length > 0);
    }

    [Fact]
    public void CpMvRmTest()
    {
        Assert.True(_bash.Cp("~/.bashrc", "/tmp/bashrc-backup").ExitCode == 0);
        Assert.True(_bash.Mv("/tmp/bashrc-backup", "/tmp/.bashrc").ExitCode == 0);
        Assert.True(_bash.Rm("/tmp/.bashrc").ExitCode == 0);
    }

    [Fact]
    public void BashOutput() =>
        Assert.False(string.IsNullOrEmpty(_bash.Cat("~/.bashrc").Output));
}
