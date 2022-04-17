namespace H.NotifyIcon;

internal static class PngToIcoConverter
{
    public static byte[] ConvertPngToIco(this byte[] data)
    {
        using var inStream = new MemoryStream(data);
        using var source = System.Drawing.Image.FromStream(inStream);
        using var outStream = new MemoryStream();
        
        // Header
        {
            // Reserved
            outStream.WriteByte(0);
            outStream.WriteByte(0);
            // File format (ico)
            outStream.WriteByte(1);
            outStream.WriteByte(0);
            // Image count (1)
            outStream.WriteByte(1);
            outStream.WriteByte(0);
        }

        // Image entry
        {
            // Width
            outStream.WriteByte((byte)source.Width);
            // Height
            outStream.WriteByte((byte)source.Height);
            // Number of colors (0 = No palette)
            outStream.WriteByte(0);
            // Reserved
            outStream.WriteByte(0);
            // Color plane (1)
            outStream.WriteByte(1);
            outStream.WriteByte(0);
            // Bits per pixel
            var bppAsLittle = IntToLittle2(System.Drawing.Image.GetPixelFormatSize(source.PixelFormat));
            outStream.Write(bppAsLittle, 0, 2);
            // Size of data in bytes
            var byteCountAsLittle = IntToLittle4(data.Length);
            outStream.Write(byteCountAsLittle, 0, 4);
            // Offset of data from beginning of file (data begins right here = 22)
            outStream.WriteByte(22);
            outStream.WriteByte(0);
            outStream.WriteByte(0);
            outStream.WriteByte(0);
            // Data
            outStream.Write(data, 0, data.Length);
        }
        return outStream.ToArray();
    }

    private static byte[] IntToLittle2(int input)
    {
        byte[] b = new byte[2];
        b[0] = (byte)input;
        b[1] = (byte)(((uint)input >> 8) & 0xFF);
        return b;
    }
    private static byte[] IntToLittle4(int input)
    {
        byte[] b = new byte[4];
        b[0] = (byte)input;
        b[1] = (byte)(((uint)input >> 8) & 0xFF);
        b[2] = (byte)(((uint)input >> 16) & 0xFF);
        b[3] = (byte)(((uint)input >> 24) & 0xFF);
        return b;
    }
}
