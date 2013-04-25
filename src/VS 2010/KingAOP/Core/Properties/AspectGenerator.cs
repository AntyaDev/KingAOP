using System;
using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Aspects;

namespace KingAOP.Core.Properties
{
    internal class AspectGenerator
    {
        private readonly DynamicMetaObject _origObj;
        private readonly IEnumerable _aspects;
        private readonly LocationInterceptionArgs _args;

        public AspectGenerator(DynamicMetaObject origObj, IEnumerable aspects, LocationInterceptionArgs args)
        {
            _origObj = origObj;
            _aspects = aspects;
            _args = args;
        }

        public Expression GenerateProperty()
        {
            throw new NotImplementedException();
        }
    }
}