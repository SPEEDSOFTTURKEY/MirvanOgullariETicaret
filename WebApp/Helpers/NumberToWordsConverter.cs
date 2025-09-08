using System.Text;

public static class NumberToWordsConverter
{
    public static string ToWords(decimal amount)
    {
        long wholePart = (long)amount;
        int decimalPart = (int)((amount - wholePart) * 100);

        string wholePartWords = ConvertWholeNumberToWords(wholePart);
        string decimalPartWords = decimalPart > 0 ? ConvertWholeNumberToWords(decimalPart) : "";

        return decimalPart > 0
            ? $"{wholePartWords}TL{decimalPartWords}Kr."
            : $"{wholePartWords}TL";
    }

    private static string ConvertWholeNumberToWords(long number)
    {
        if (number == 0)
            return "Sıfır";

        string[] ones = { "", "Bir", "İki", "Üç", "Dört", "Beş", "Altı", "Yedi", "Sekiz", "Dokuz" };
        string[] tens = { "", "On", "Yirmi", "Otuz", "Kırk", "Elli", "Altmış", "Yetmiş", "Seksen", "Doksan" };
        string[] hundreds = { "", "Yüz", "İkiYüz", "ÜçYüz", "DörtYüz", "BeşYüz", "AltıYüz", "YediYüz", "SekizYüz", "DokuzYüz" };

        StringBuilder sb = new StringBuilder();

        void AppendGroup(int groupValue, string groupSuffix)
        {
            if (groupValue == 0)
                return;

            int y = groupValue / 100;
            int z = (groupValue / 10) % 10;
            int t = groupValue % 10;

            if (y > 0)
                sb.Append(hundreds[y]);
            if (z > 0)
                sb.Append(tens[z]);
            if (t > 0)
                sb.Append(ones[t]);

            if (!string.IsNullOrEmpty(groupSuffix))
                sb.Append(groupSuffix);
        }

        long billions = number / 1_000_000_000;
        number %= 1_000_000_000;

        long millions = number / 1_000_000;
        number %= 1_000_000;

        long thousands = number / 1000;
        long remainder = number % 1000;

        if (billions > 0)
        {
            AppendGroup((int)billions, "Milyar");
        }

        if (millions > 0)
        {
            AppendGroup((int)millions, "Milyon");
        }

        if (thousands > 0)
        {
            if (thousands == 1)
            {
                sb.Append("Bin");
            }
            else
            {
                AppendGroup((int)thousands, "Bin");
            }
        }

        AppendGroup((int)remainder, "");

        return sb.ToString();
    }
}
