using System;
using lib.Ai;

namespace lib
{
    public class AiFactory
    {
        private readonly Func<IAi> create;
        public string Name;

        public AiFactory(string name, Func<IAi> create)
        {
            Name = name;
            this.create = create;
        }

        public IAi Create()
        {
            return create();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}