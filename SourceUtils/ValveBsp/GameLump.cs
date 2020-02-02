using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Decoder = SevenZip.Sdk.Compression.Lzma.Decoder;

namespace SourceUtils
{
    partial class ValveBspFile
    {
        public class GameLump : ILump
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct Item
            {
                public readonly int Id;
                public readonly ushort Flags;
                public readonly ushort Version;
                public readonly int FileOffset;
                public readonly int FileLength;
            }

            private readonly ValveBspFile _bspFile;
            private Dictionary<string, Item> _items;

            public LumpType LumpType { get; }

            public GameLump( ValveBspFile bspFile, LumpType type )
            {
                _bspFile = bspFile;
                LumpType = type;
            }

            private string GetIdString( int id )
            {
                var str = "";

                for ( var i = 0; i < 4 && id >> (i << 3) > 0; ++i )
                {
                    str = (char) ((id >> (i << 3)) & 0x7f) + str;
                }

                return str;
            }

            public ushort GetItemFlags( string id )
            {
                EnsureLoaded();

                return _items[id].Flags;
            }

            public ushort GetItemVersion( string id )
            {
                EnsureLoaded();

                return _items[id].Version;
            }

            public Stream OpenItem( string id )
            {
                EnsureLoaded();

                var item = _items[id];
                //var info = _bspFile.GetLumpInfo( LumpType );

                var stream = _bspFile.GetSubStream( item.FileOffset, item.FileLength );

                try
                {
                    LzmaHeader lzmaHeader = LzmaHeader.Read( stream );

                    using ( var compressedStream =
                        _bspFile.GetSubStream( item.FileOffset + LzmaHeader.Size, lzmaHeader.LzmaSize ) )
                    {
                        using ( var uncompressedStream = new MemoryStream( ( int ) lzmaHeader.ActualSize ) )
                        {
                            Decoder decoder = new Decoder();
                            decoder.SetDecoderProperties( lzmaHeader.Properties );
                            decoder.Code( compressedStream, uncompressedStream, lzmaHeader.LzmaSize,
                                lzmaHeader.ActualSize, null );
                            stream = new MemoryStream( ( int ) stream.Length );
                            uncompressedStream.Seek( 0, SeekOrigin.Begin );
                            uncompressedStream.CopyTo( stream );
                        }
                    }

                    stream.Seek( 0, SeekOrigin.Begin );
                    return stream;
                }
                catch ( NotSupportedException e )
                {
                    stream.Seek( 0, SeekOrigin.Begin );
                    return stream;
                }
            }

            private void EnsureLoaded()
            {
                lock ( this )
                {
                    if ( _items != null ) return;

                    _items = new Dictionary<string, Item>();

                    using ( var reader = new BinaryReader( _bspFile.GetLumpStream( LumpType ) ) )
                    {
                        var count = reader.ReadInt32();
                        LumpReader<Item>.ReadLumpFromStream( reader.BaseStream, count, item => _items.Add( GetIdString( item.Id ), item ) );
                    }
                }
            }
        }
    }
}
