using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace P2PNode
{
    class WebSocketrFrameWriter
    {

        public WebSocketFrame Read(Stream stream, Socket socket)
        {
            byte byte1;

            try
            {
                byte1 = (byte)stream.ReadByte();
            }
            catch (IOException)
            {
                if (socket.Connected)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }

            // process first byte
            byte finBitFlag = 0x80;
            byte opCodeFlag = 0x0F;
            bool isFinBitSet = (byte1 & finBitFlag) == finBitFlag;
            WebSocketOpCode opCode = (WebSocketOpCode)(byte1 & opCodeFlag);

            // read and process second byte
            byte byte2 = (byte)stream.ReadByte();
            byte maskFlag = 0x80;
            bool isMaskBitSet = (byte2 & maskFlag) == maskFlag;
            uint len = ReadLength(byte2, stream);
            byte[] payload;

            // use the masking key to decode the data if needed
            if (isMaskBitSet)
            {
                byte[] maskKey = BinaryReaderWriter.ReadExactly(WebSocketFrameCommon.MaskKeyLength, stream);
                payload = BinaryReaderWriter.ReadExactly((int)len, stream);

                // apply the mask key to the payload (which will be mutated)
                WebSocketFrameCommon.ToggleMask(maskKey, payload);
            }
            else
            {
                payload = BinaryReaderWriter.ReadExactly((int)len, stream);
            }

            WebSocketFrame frame = new WebSocketFrame(isFinBitSet, opCode, payload, true);
            return frame;
        }


        public void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame)
        {
            // best to write everything to a memory stream before we push it onto the wire
            // not really necessary but I like it this way
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte finBitSetAsByte = isLastFrame ? (byte)0x80 : (byte)0x00;
                byte byte1 = (byte)(finBitSetAsByte | (byte)opCode);
                memoryStream.WriteByte(byte1);

                // NB, set the mask flag if we are constructing a client frame
                byte maskBitSetAsByte = _isClient ? (byte)0x80 : (byte)0x00;

                // depending on the size of the length we want to write it as a byte, ushort or ulong
                if (payload.Length < 126)
                {
                    byte byte2 = (byte)(maskBitSetAsByte | (byte)payload.Length);
                    memoryStream.WriteByte(byte2);
                }
                else if (payload.Length <= ushort.MaxValue)
                {
                    byte byte2 = (byte)(maskBitSetAsByte | 126);
                    memoryStream.WriteByte(byte2);
                    BinaryReaderWriter.WriteUShort((ushort)payload.Length, memoryStream, false);
                }
                else
                {
                    byte byte2 = (byte)(maskBitSetAsByte | 127);
                    memoryStream.WriteByte(byte2);
                    BinaryReaderWriter.WriteULong((ulong)payload.Length, memoryStream, false);
                }

                // if we are creating a client frame then we MUST mack the payload as per the spec
                if (_isClient)
                {
                    byte[] maskKey = new byte[WebSocketFrameCommon.MaskKeyLength];
                    _random.NextBytes(maskKey);
                    memoryStream.Write(maskKey, 0, maskKey.Length);

                    // mask the payload
                    WebSocketFrameCommon.ToggleMask(maskKey, payload);
                }

                memoryStream.Write(payload, 0, payload.Length);
                byte[] buffer = memoryStream.ToArray();
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
