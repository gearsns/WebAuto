namespace WebAuto
{
    public partial class LoadingTabPage : TabPage
    {
        private bool _loading;
        public LoadingTabPage() : base()
        {
        }
        public LoadingTabPage(string s) : base(s)
        {
        }
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                if (null != Parent)
                {
                    Parent.Invalidate();
                }
            }
        }
    }
}
