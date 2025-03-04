﻿using AssetRipper.IO.Endian;

namespace Web.AssetBundles.BundleFix.Header.Models;

public sealed record BundleScene : IEndianReadableWritable
{
    public uint CompressedSize { get; set; }
    public uint DecompressedSize { get; set; }

    public void Read(EndianReader reader)
    {
        CompressedSize = reader.ReadUInt32();
        DecompressedSize = reader.ReadUInt32();
    }

    public void Write(EndianWriter writer)
    {
        writer.Write(CompressedSize);
        writer.Write(DecompressedSize);
    }
}
