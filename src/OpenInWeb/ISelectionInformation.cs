namespace OpenInWeb
{
    public interface ISelectionInformation
    {
        int StartLineNumber      { get; }

        int StartCharacterNumber { get; }

        int EndLineNumber        { get; }

        int EndCharacterNumber   { get; }
    }
}
