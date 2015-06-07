using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using XMLib;

namespace CryptoWeek3
{
    public class StreamHasher
    {
        private const int BlockSize = 1024;

        private HashAlgorithm m_hashAlgo = new SHA256Managed();

        public byte[] ComputeHash(Stream _stream)
        {
	        if (_stream == null)
	        {
		        throw new ArgumentNullException(nameof(_stream));
	        }

	        using (var reader = new BinaryReader(_stream))
            {

                var lastBlockLength = 0;
                var blocksCount = Math.DivRem((int)reader.BaseStream.Length, BlockSize, out lastBlockLength);

                reader.BaseStream.Seek(0, SeekOrigin.End);

                if (lastBlockLength == 0)
                {
                    lastBlockLength = BlockSize;
                    blocksCount--;
                }
                
                var lastBlock = ReadBytesFromStreamBackwards(reader, lastBlockLength);
                var lastBlockHash = m_hashAlgo.ComputeHash(lastBlock);

                for (var i = 0; i < blocksCount; i++)
                {
                    var block = ReadBytesFromStreamBackwards(reader, BlockSize);
                    var blockPlusHash = block.Concat(lastBlockHash).ToArray();
                    var blockHash = m_hashAlgo.ComputeHash(blockPlusHash);

                    lastBlockHash = blockHash;
                }

                return lastBlockHash;
            }
        }

	    private byte[] ReadBytesFromStreamBackwards(BinaryReader _stream, int _count)
        {
		    if (_stream == null)
		    {
			    throw new ArgumentNullException(nameof(_stream));
		    }
		    _stream.BaseStream.Seek(-1 * _count, SeekOrigin.Current);
            var block = _stream.ReadBytes(_count);
            _stream.BaseStream.Seek(-1 * _count, SeekOrigin.Current);
            return block;
        }
    }
}
