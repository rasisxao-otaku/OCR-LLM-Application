public class TextChunker
{
    public IEnumerable<string> Split(
        string text,
        int size = 1000)
    {
        for (
            int i = 0;
            i < text.Length;
            i += size)
        {
            yield return text
                .Substring(
                    i,
                    Math.Min(
                        size,
                        text.Length - i));
        }
    }
}