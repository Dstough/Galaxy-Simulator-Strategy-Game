using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GalaxyServer.Networking
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        Welcome = 1,
        SpawnPlayer,
        PlayerPosition,
        PlayerRotation
    }

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        WelcomeReceived = 1,
        PlayerMovement
    }

    public class Packet : IDisposable
    {
        private List<byte> _buffer;
        private byte[] _readableBuffer;
        private int _readPosition;

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            _buffer = new List<byte>();
            _readPosition = 0;
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="id">The packet ID.</param>
        public Packet(int id)
        {
            _buffer = new List<byte>();
            _readPosition = 0;

            Write(id);
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public Packet(byte[] data)
        {
            _buffer = new List<byte>();
            _readPosition = 0;

            SetBytes(data);
        }

        #region Functions

        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] data)
        {
            Write(data);
            _readableBuffer = _buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public void WriteLength()
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="value">The int to insert.</param>
        public void InsertInt(int value)
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return _buffer.Count;
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - _readPosition;
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                _buffer.Clear();
                _readableBuffer = null;
                _readPosition = 0;
            }
            else
            {
                _readPosition -= 4; // "Unread" the last read int
            }
        }

        #endregion

        #region Write Data

        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="value">The byte to add.</param>
        public void Write(byte value) => _buffer.Add(value);

        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="value">The byte array to add.</param>
        public void Write(byte[] value) => _buffer.AddRange(value);

        /// <summary>Adds a short to the packet.</summary>
        /// <param name="value">The short to add.</param>
        public void Write(short value) => _buffer.AddRange(BitConverter.GetBytes(value));

        /// <summary>Adds an int to the packet.</summary>
        /// <param name="value">The int to add.</param>
        public void Write(int value) => _buffer.AddRange(BitConverter.GetBytes(value));

        /// <summary>Adds a long to the packet.</summary>
        /// <param name="value">The long to add.</param>
        public void Write(long value) => _buffer.AddRange(BitConverter.GetBytes(value));

        /// <summary>Adds a float to the packet.</summary>
        /// <param name="value">The float to add.</param>
        public void Write(float value) => _buffer.AddRange(BitConverter.GetBytes(value));

        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="value">The bool to add.</param>
        public void Write(bool value) => _buffer.AddRange(BitConverter.GetBytes(value));

        /// <summary>Adds a string to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            _buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
        }

        /// <summary>Adds a Vector3 to the packet.</summary>
        /// <param name="value">The Vector3 to add.</param>
        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        /// <summary>Adds a Quaternion to the packet.</summary>
        /// <param name="value">The Quaternion to add.</param>
        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        #endregion

        #region Read Data

        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'byte'!");

            var value = _readableBuffer[_readPosition];

            if (moveReadPosition)
                _readPosition += 1;

            return value;
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="length">The length of the byte array.</param>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int length, bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'byte[]'!");

            var value = _buffer.GetRange(_readPosition, length).ToArray();

            if (moveReadPosition)
                _readPosition += length;

            return value;
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'short'!");

            var value = BitConverter.ToInt16(_readableBuffer, _readPosition);

            if (moveReadPosition)
                _readPosition += 2;

            return value;
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'int'!");

            var value = BitConverter.ToInt32(_readableBuffer, _readPosition);

            if (moveReadPosition)
                _readPosition += 4;

            return value;
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'long'!");

            var value = BitConverter.ToInt64(_readableBuffer, _readPosition);

            if (moveReadPosition)
                _readPosition += 8;

            return value;
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'float'!");

            var value = BitConverter.ToSingle(_readableBuffer, _readPosition);

            if (moveReadPosition)
                _readPosition += 4;

            return value;
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool moveReadPosition = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new Exception("Could not read value of type 'bool'!");

            var value = BitConverter.ToBoolean(_readableBuffer, _readPosition);

            if (moveReadPosition)
                _readPosition += 1;

            return value;
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool moveReadPosition = true)
        {
            try
            {
                var length = ReadInt();
                var value = Encoding.ASCII.GetString(_readableBuffer, _readPosition, length);

                if (moveReadPosition && value.Length > 0)
                    _readPosition += length;

                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        /// <summary>Reads a Vector3 from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public Vector3 ReadVector3(bool moveReadPosition = true) =>
            new Vector3(ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition));

        /// <summary>Reads a Quaternion from the packet.</summary>
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        public Quaternion ReadQuaternion(bool moveReadPosition = true) =>
            new Quaternion(ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition));

        #endregion

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _buffer = null;
                _readableBuffer = null;
                _readPosition = 0;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}