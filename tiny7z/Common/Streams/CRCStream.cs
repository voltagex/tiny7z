﻿using System;
using System.IO;

namespace pdj.tiny7z.Common
{
    /// <summary>
    /// Filter stream to calculate CRC32 on the fly.
    /// </summary>
    public class CRCStream : Stream
    {
        /// <summary>
        /// Access this once all stream has been read and it will be the stream's CRC32 value.
        /// </summary>
        public uint CRC => ~crc;

        private Stream internalStream;
        private uint crc;
        private bool leaveOpen;

        public override bool CanRead => internalStream is Stream && internalStream.CanRead;

        public override bool CanWrite => internalStream is Stream && internalStream.CanWrite;

        public override bool CanSeek => false;

        public override long Length => internalStream is Stream ? internalStream.Length : -1;

        public override long Position
        {
            get => internalStream is Stream ? internalStream.Position : -1;
            set => throw new NotImplementedException();
        }

        public CRCStream()
            : base()
        {
            internalStream = null;
            crc = 0xffffffff;
            leaveOpen = true;
        }

        public CRCStream(Stream internalStream, bool leaveOpen = true)
            : base()
        {
            this.internalStream = internalStream;
            this.crc = 0xffffffff;
            this.leaveOpen = leaveOpen;
        }

        public override void Flush()
        {
            if (internalStream is Stream)
            {
                internalStream.Flush();
            }
        }

        public override void Close()
        {
            if (internalStream != null && !leaveOpen)
            {
                internalStream.Close();
            }
            internalStream = null;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int r = internalStream.Read(buffer, offset, count);
            if (r > 0)
                crc = Common.CRC.Calculate(buffer, offset, r, crc);
            return r;
        }

        public override int ReadByte()
        {
            int y = internalStream.ReadByte();
            if (y != -1)
                crc = Common.CRC.Calculate((byte)y, crc);
            return y;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (internalStream is Stream)
            {
                internalStream.Write(buffer, offset, count);
                crc = Common.CRC.Calculate(buffer, offset, count, crc);
            }
        }

        public override void WriteByte(byte value)
        {
            if (internalStream is Stream)
            {
                internalStream.WriteByte(value);
                crc = Common.CRC.Calculate(value, crc);
            }
        }
    }
}
