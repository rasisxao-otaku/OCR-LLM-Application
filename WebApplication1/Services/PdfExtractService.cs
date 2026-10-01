using System.Text;
using UglyToad.PdfPig;


public class PdfExtractService
{
    public string Extract(string path)
    {
        using var pdf =
            PdfDocument.Open(path);

        var builder = new StringBuilder();


        foreach (var page in pdf.GetPages())
        {
            builder.AppendLine(page.Text);
        }


        return builder.ToString();
    }
}