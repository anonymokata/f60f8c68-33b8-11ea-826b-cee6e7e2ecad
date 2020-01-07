namespace PencilDurability
{
    public interface IPencil
    {
        int CurrentEraserDurability { get; }
        int CurrentLength { get; }
        int CurrentPointDurability { get; }

        void Erase(IPaper paper, string matchText);
        void Sharpen();
        void Write(IPaper paper, string text);
    }
}
