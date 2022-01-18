using EditorKit;

namespace UPA
{
    [System.Serializable]
    public class AnalysistTreeElement : TreeElement
    {
        private bool m_Enabled;
        private AssetItem m_item;

        public AnalysistTreeElement()
        {
            m_Enabled = false;
        }

        public AnalysistTreeElement(int id, int deep) : base("", deep, id)
        {
            m_Enabled = false;
        }

        public AnalysistTreeElement(int id, int deep, AssetItem item) : base("", deep, id)
        {
            m_Enabled = true;
            m_item = item;
        }

        public bool enabled
        {
            set { m_Enabled = value; }
            get { return m_Enabled; }
        }

        public AssetItem item
        {
            set { m_item = value; }
            get { return m_item; }
        }
    }
}