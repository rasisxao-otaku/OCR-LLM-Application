public class TextChunkService
{
    public List<DocumentChunk> Split(
        string filename,
        string text,
        int size = 500,
        int overlap = 100)
    {
        var chunks = new List<DocumentChunk>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return chunks;
        }

        text = text.Trim();

        int step = size - overlap;

        if (step <= 0)
        {
            throw new ArgumentException(
                "overlap must be smaller than size");
        }

        int chunkIndex = 0;

        for (int i = 0; i < text.Length; i += step)
        {
            int length = Math.Min(
                size,
                text.Length - i);

            var chunkText =
                text.Substring(i, length).Trim();

            if (string.IsNullOrWhiteSpace(chunkText))
            {
                continue;
            }

            chunks.Add(new DocumentChunk
            {
                Filename = filename,
                ChunkIndex = chunkIndex++,
                Text = chunkText
            });

            if (i + length >= text.Length)
            {
                break;
            }
        }

        return chunks;
    }
}