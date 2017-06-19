namespace SearchQueryTool.Helpers
{
    internal class Bm25PropertyWeights
    {
        // Fields
        private readonly float _b;
        private readonly float _w;

        // Methods
        public Bm25PropertyWeights(float w, float b)
        {
            _w = w;
            _b = b;
        }

        // Properties
        public float B
        {
            get { return _b; }
        }

        public float W
        {
            get { return _w; }
        }
    }
}