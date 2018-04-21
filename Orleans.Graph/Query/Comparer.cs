namespace Orleans.Graph.Query
{
    public class Comparer
    {
        private readonly string comparer;

        public Comparer(string comparer) => this.comparer = comparer;

        public static implicit operator string(Comparer d) => d.ToString();

        public override string ToString() => comparer;
    }
}