namespace PencilDurability
{
    public interface IPencil
    {
        int CurrentPointDurability { get; }

        int CurrentEraserDurability { get; }

        int CurrentLength { get; }

        void Erase(IPaper paper, string matchText);

        void Write(IPaper paper, string text);

        void Sharpen();
    }
}
