using System;

namespace Common
{
    public class Chunk
    {
        public static int MaxChunkSize = 256 * 1024;
        public Guid Id { get; set; }

        public int Position { get; set; }

        public int Size { get; set; }

        public byte[] Body { get; set; }
    }
}
