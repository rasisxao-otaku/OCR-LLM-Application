public class OcrResponse
{
    public string Filename { get; set; } = "";

    public List<OcrPage> Pages { get; set; } = [];
}

public class OcrPage
{
    public int Page { get; set; }

    public List<OcrLine> Lines { get; set; } = [];
}

public class OcrLine
{
    public string Text { get; set; } = "";

    public string Corrected { get; set; } = "";

    public double Confidence { get; set; }
}