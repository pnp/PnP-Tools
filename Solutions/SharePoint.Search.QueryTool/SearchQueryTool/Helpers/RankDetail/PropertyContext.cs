namespace SearchQueryTool.Helpers
{
    internal class PropertyContext
    {
        // Fields
        private int _iContext;
        private int _pid;
        private string _strUpdateGroup;

        // Methods
        public PropertyContext(int iContext, int pid, string strUpdateGroup)
        {
            this._iContext = iContext;
            this._strUpdateGroup = strUpdateGroup;
            this._pid = pid;
        }

        // Properties
        public int Context
        {
            get
            {
                return this._iContext;
            }
        }

        public int Pid
        {
            get
            {
                return this._pid;
            }
        }

        public string UpdateGroup
        {
            get
            {
                return this._strUpdateGroup;
            }
        }
    }

}