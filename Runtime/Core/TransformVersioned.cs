using System;
using Platonic.Version;

namespace Platonic.Core
{
    public class TransformVersioned<TSource, TTarget> : BaseTransform<TTarget>
        where TSource : IVersioned
    {
        private readonly TSource _source;
        private readonly Func<TSource, TTarget> _transform;

        public TransformVersioned(TSource source, IFieldName<TTarget> targetName, Func<TSource, TTarget> transform) : base(targetName)
        {
            _source = source;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _source.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_source);
        }
    }
}