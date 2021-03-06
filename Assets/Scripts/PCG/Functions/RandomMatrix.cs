using System;

namespace PCG
{
    public class RandomMatrix : FunctionMatrix
    {
        public const string NAME = "RandomMatrix";

        private int seed;
        private int min;
        private int max;

        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        public int Min
        {
            get { return this.min; }
            set { this.min = value; }
        }
        
        public int Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        public RandomMatrix()
            : this(256, 0, 0, 255)
        {
        }
        
        public RandomMatrix(int size, int seed, int min, int max) :
            base(
                NAME,
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", size, Int32.MinValue, Int32.MaxValue
            )
        {
            this.seed = seed;
            this.min = min;
            this.max = max;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            return HashXX.Range((uint) seed, min, max + 1, x, y);
        }
        
        public override string ToString()
        {
            return NAME + " " + size + "x" + size + " -> [" + min + ".." + max + "]";
        }
    }
}