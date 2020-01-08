namespace PencilDurability
{
    public interface IPencil
    {
        int CurrentPointDurability { get; }

        int CurrentEraserDurability { get; }

        int CurrentLength { get; }

        void Write(IPaper paper, string text);

        void Erase(IPaper paper, string matchText);

        void Edit(IPaper paper, string editText, int startIndex);

        void Sharpen();
    }
}
