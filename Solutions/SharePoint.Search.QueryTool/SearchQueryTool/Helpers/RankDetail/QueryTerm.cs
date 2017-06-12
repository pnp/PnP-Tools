using System;
using System.Collections.Generic;

namespace SearchQueryTool.Helpers
{
    internal class QueryTerm
{
    // Fields
    private int _n;
    private int _N;
    private List<Pid> _PropHits;
    private float _RW;
    private float _Score;
    private string _strTerm;
    private float _tfw;

    // Methods
    public QueryTerm(string strTerm, float Score, int n, int N, float RW)
    {
        this._strTerm = strTerm;
        this._Score = Score;
        this._n = n;
        this._N = N;
        this._RW = RW;
        this._PropHits = new List<Pid>();
    }

    public void AddPid(Pid pid)
    {
        this._PropHits.Add(pid);
    }

    public Pid FindPid(string strName)
    {
        SearchPid pid = new SearchPid(strName);
        return this._PropHits.Find(new Predicate<Pid>(pid.MatchPid));
    }

    // Properties
    public int n
    {
        get
        {
            return this._n;
        }
    }

    public int N
    {
        get
        {
            return this._N;
        }
    }

    public List<Pid> Pids
    {
        get
        {
            return this._PropHits;
        }
    }

    public float RW
    {
        get
        {
            return this._RW;
        }
    }

    public float Score
    {
        get
        {
            return this._Score;
        }
    }

    public string TermName
    {
        get
        {
            return this._strTerm;
        }
    }

    public float TFW
    {
        get
        {
            return this._tfw;
        }
        set
        {
            this._tfw = value;
        }
    }

    // Nested Types
    internal class SearchPid
    {
        // Fields
        private string _strName;

        // Methods
        public SearchPid(string strName)
        {
            this._strName = strName;
        }

        public bool MatchPid(Pid target)
        {
            return (target.Name == this._strName);
        }
    }
}
}