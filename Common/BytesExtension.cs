using System;
using System.Collections.Generic;

namespace Common
{
    public static class BytesExtension
    {
        public static IEnumerable<Chunk> GetChunks(this byte[] data)
        {
            var id = Guid.NewGuid();
            if (data.Length <= Chunk.MaxChunkSize)
            {
                yield return new Chunk
                {
                    Id = id,
                    Position = 0,
                    Size = 1,
                    Body = data
                };
            }

            var size = data.Length / Chunk.MaxChunkSize;

            for (var i = 0; i < size; i++)
            {
                var bodyLength = i != size - 1 ? Chunk.MaxChunkSize : data.Length % Chunk.MaxChunkSize;
                var body = new byte[bodyLength];

                Array.Copy(data, Chunk.MaxChunkSize * i, body, 0, bodyLength);

                yield return new Chunk
                {
                    Id = id,
                    Position = i,
                    Size = size,
                    Body = body
                };
            }
        }
    }
}
