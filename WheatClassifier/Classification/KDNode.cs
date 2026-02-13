using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public sealed class KDNode
    {
        public Grain Point { get; }
        public int Axis { get; }
        public KDNode? Left { get; set; }
        public KDNode? Right { get; set; }

        public KDNode(Grain point, int axis)
        {
            Point = point;
            Axis = axis;
        }
    }
}
