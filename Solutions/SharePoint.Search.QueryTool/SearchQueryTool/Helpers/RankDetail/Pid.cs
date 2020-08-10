namespace SearchQueryTool.Helpers
{
    internal class Pid
    {
        // Fields
        private float _avdl;
        private float _b;
        private float _dl;
        private float _dlavdl;
        private float _dlnorm;
        private string _Name;
        private int _pid;
        private float _tf;
        private float _tfnorm;
        private float _tfw;
        private float _weight;

        // Methods
        public Pid(int pid)
        {
            this._pid = pid;
            this.GetNameFromPid(pid);
        }

        public Pid(string name)
        {
            this.GetNameFromPid(GetPidFromPropertyName(name));
        }

        public Pid(int pid, float weight, float tf, float tfw, float dl, float avdl, float dlavdl, float dlnorm, float tfnorm, float b)
        {
            this._pid = pid;
            this.GetNameFromPid(pid);
            this._tf = tf;
            this._weight = weight;
            this._b = b;
            this._tfw = tfw;
            this._dl = dl;
            this._avdl = avdl;
            this._dlavdl = dlavdl;
            this._dlnorm = dlnorm;
            this._tfnorm = tfnorm;
        }

        private void GetNameFromPid(int pid)
        {
            switch (pid)
            {
                case 0x38:
                case 13:
                    this._Name = "DisplayName";
                    return;

                case 70:
                    this._Name = "TAUC-Click";
                    return;

                case 1:
                    this._Name = "Contents";
                    return;

                case 2:
                    this._Name = "Title";
                    return;

                case 3:
                    this._Name = "Author";
                    return;

                case 5:
                    this._Name = "Language";
                    return;

                case 7:
                    this._Name = "DocPath";
                    return;

                case 10:
                    this._Name = "AnchorText";
                    return;

                case 15:
                    this._Name = "AccountName";
                    return;

                case 0x13:
                    this._Name = "PreferredName";
                    return;

                case 0x15:
                    this._Name = "WorkEmail";
                    return;

                case 0x18:
                    this._Name = "JobTitle";
                    return;

                case 0x19:
                    this._Name = "Department";
                    return;

                case 0x1a:
                    this._Name = "AboutMe";
                    return;

                case 0x1b:
                    this._Name = "UserName";
                    return;

                case 0x23:
                    this._Name = "Memberships";
                    return;

                case 0x27:
                    this._Name = "Responsibilities";
                    return;

                case 40:
                    this._Name = "Skills";
                    return;

                case 0x29:
                    this._Name = "Interests";
                    return;

                case 0x60:
                    this._Name = "ClickDistance";
                    return;

                case 0x62:
                    this._Name = "InternalFileType";
                    return;

                case 100:
                case 0x2780:
                    this._Name = "ClickQueryTerms";
                    return;

                case 0x53:
                    this._Name = "TAUC-URL";
                    return;

                case 0xaf:
                    this._Name = "ContentsHidden";
                    return;

                case 0xb1:
                    this._Name = "RankingWeightName";
                    return;

                case 0xb2:
                    this._Name = "RankingWeightHigh";
                    return;

                case 0xb3:
                    this._Name = "RankingWeightLow";
                    return;

                case 180:
                    this._Name = "Pronunciations";
                    return;

                case 0x108:
                    this._Name = "SocialTag";
                    return;

                case 0x10b:
                    this._Name = "LevelsToTop";
                    return;

                case 0x10c:
                    this._Name = "WeightedMemberships";
                    return;

                case 0x185:
                    this._Name = "OrgNames";
                    return;

                case 0x187:
                    this._Name = "CombinedName";
                    return;

                case 0x155:
                    this._Name = "LastClicks";
                    return;

                case 0x12e:
                    this._Name = "ExtractedTitle";
                    return;

                case 0x12f:
                    this._Name = "UrlDepth";
                    return;

                case 0x132:
                    this._Name = "QueryLogClicks";
                    return;

                case 0x133:
                    this._Name = "QueryLogSkips";
                    return;

                case 0x139:
                    this._Name = "SipAddress";
                    return;

                case 0x18c:
                    this._Name = "OrgParentNames";
                    return;

                case 0x1f5:
                    this._Name = "AnchorTextCompleteMatch";
                    return;

                case 0x28b:
                    this._Name = "QueryLogSiteClicks";
                    return;

                case 0x28c:
                    this._Name = "QueryLogSiteSkips";
                    return;

                case 0x28d:
                    this._Name = "QueryLogSiteLastClicks";
                    return;

                case 0x291:
                    this._Name = "EventRate";
                    return;

                case 0x277f:
                    this._Name = "IntersiteAnchorText";
                    return;
                case 0x4:
                    this._Name = "LastModifiedTime";
                    return;
                case 607:
                    this._Name = "FirstLevelColleagues";
                    return;
                case 608:
                    this._Name = "SecondLevelColleagues";
                    return;

            }
            this._Name = pid.ToString();
        }

        private static int GetPidFromPropertyName(string name)
        {
            switch (name)
            {
                case "Content":
                    return 1;

                case "Title":
                    return 2;

                case "Author":
                    return 3;

                case "DetectedLanguageRanking":
                    return 5;

                case "DocPath":
                    return 7;

                case "AnchorText":
                    return 10;

                case "DisplayName":
                    return 0x38;

                case "AccountName":
                    return 15;

                case "PreferredName":
                    return 0x13;

                case "WorkEmail":
                    return 0x15;

                case "JobTitle":
                    return 0x18;

                case "Department":
                    return 0x19;

                case "AboutMe":
                    return 0x1a;

                case "UserName":
                    return 0x1b;

                case "Memberships":
                    return 0x23;

                case "Responsibilities":
                    return 0x27;

                case "Skills":
                    return 40;

                case "Interests":
                    return 0x29;

                case "clickdistance":
                    return 0x60;

                case "InternalFileType":
                    return 0x62;

                case "ContentsHidden":
                    return 0xaf;

                case "RankingWeightName":
                    return 0xb1;

                case "RankingWeightHigh":
                    return 0xb2;

                case "RankingWeightLow":
                    return 0xb3;

                case "Pronunciations":
                    return 180;

                case "SocialTag":
                    return 0x108;

                case "LevelsToTop":
                    return 0x10b;

                case "WeightedMemberships":
                    return 0x10c;

                case "ExtractedTitle":
                    return 0x12e;

                case "UrlDepth":
                    return 0x12f;

                case "QLogClicks":
                    return 0x132;

                case "QLogSkips":
                    return 0x133;

                case "SipAddress":
                    return 0x139;

                case "QLogLastClicks":
                    return 0x155;

                case "OrgNames":
                    return 0x185;

                case "CombinedName":
                    return 0x187;

                case "OrgParentNames":
                    return 0x18c;

                case "IntersiteAnchorText":
                    return 0x277f;

                case "ClickQueryTerms":
                    return 100;

                case "AnchorTextCompleteMatch":
                    return 0x1f5;

                case "QueryLogSiteClicks":
                    return 0x28b;

                case "QueryLogSiteSkips":
                    return 0x28c;

                case "QueryLogSiteLastClicks":
                    return 0x28d;

                case "EventRate":
                    return 0x291;

                case "LastModifiedTime":
                    return 0x4;
                case "FirstLevelColleagues":
                    return 607;
                case "SecondLevelColleagues":
                    return 608;
            }
            return 0;
        }

        // Properties
        public float AVDL
        {
            get
            {
                return this._avdl;
            }
        }

        public float B
        {
            get
            {
                return this._b;
            }
        }

        public float DL
        {
            get
            {
                return this._dl;
            }
        }

        public float DLAvdl
        {
            get
            {
                return this._dlavdl;
            }
        }

        public float DLNorm
        {
            get
            {
                return this._dlnorm;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
        }

        public int PID
        {
            get
            {
                return this._pid;
            }
        }

        public float TF
        {
            get
            {
                return this._tf;
            }
        }

        public float TFNorm
        {
            get
            {
                return this._tfnorm;
            }
        }

        public float TFW
        {
            get
            {
                return this._tfw;
            }
        }

        public float Weight
        {
            get
            {
                return this._weight;
            }
        }
    }

}