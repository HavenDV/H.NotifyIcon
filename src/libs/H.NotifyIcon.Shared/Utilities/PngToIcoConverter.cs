﻿namespace H.NotifyIcon;

/// <summary>
/// Provides functionality to convert PNG image data to ICO format.
/// </summary>
public static class PngToIcoConverter
{
    /// <summary>
    /// Converts a PNG image byte array to an ICO image byte array.
    /// </summary>
    /// <param name="data">The byte array containing PNG image data.</param>
    /// <returns>A byte array containing the ICO image data.</returns>
    [SupportedOSPlatform("windows")]
    public static byte[] ConvertPngToIco(this byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty.");
        }
        
        using var inStream = new MemoryStream(data);
        var metadata = inStream.GetMetadata(); // Custom metadata retrieval
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
            outStream.WriteByte((byte)metadata.Width);
            // Height
            outStream.WriteByte((byte)metadata.Height);
            // Number of colors (0 = No palette)
            outStream.WriteByte(0);
            // Reserved
            outStream.WriteByte(0);
            // Color plane (1)
            outStream.WriteByte(1);
            outStream.WriteByte(0);
            // Bits per pixel
            var bppAsLittle = IntToLittle2(metadata.BitsPerPixel);
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

    /// <summary>
    /// Converts an integer to a 2-byte little-endian byte array.
    /// </summary>
    /// <param name="input">The integer to convert.</param>
    /// <returns>A 2-byte little-endian byte array.</returns>
    private static byte[] IntToLittle2(int input)
    {
        byte[] b = new byte[2];
        b[0] = (byte)input;
        b[1] = (byte)(((uint)input >> 8) & 0xFF);
        return b;
    }
    
    /// <summary>
    /// Converts an integer to a 4-byte little-endian byte array.
    /// </summary>
    /// <param name="input">The integer to convert.</param>
    /// <returns>A 4-byte little-endian byte array.</returns>
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
