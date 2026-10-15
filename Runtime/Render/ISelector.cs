namespace Platonic.Render
{
    public interface ISelector<in TIn, out TOut>
    {
        TOut SelectFor(TIn data);
    }
}