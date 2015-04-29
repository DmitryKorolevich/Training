using System;
using System.IO;

namespace VitalChoice.Business.Helpers
{
    public static class StreamsHelper
    {
        public static string ToString(Stream fStream)
        {
            if (fStream == null)
                return string.Empty;

            using (var tReader = new StreamReader(fStream))
            {
                return tReader.ReadToEnd();
            }

            return string.Empty;
        }

        public static byte[] ReadFully(Stream stream, int? initialLength = null)
        {
            if (!initialLength.HasValue)
            {
                initialLength = 64 * 1024;
            }

            byte[] buffer = new byte[initialLength.Value];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }
    }
}