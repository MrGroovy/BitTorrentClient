using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib.Bittorrent.Bencoding
{
    public class BenValueParser
    {
        private Stream stream;
        private long streamOrgPos;

        public BenValueParser(Stream stream)
        {
            this.stream = stream;
            streamOrgPos = stream.Position;
        }

        public BenValue Parse()
        {
            BenValueKind valueKind = ReadValueType();

            switch (valueKind)
            {
                case BenValueKind.Integer:
                    return ReadInteger();
                case BenValueKind.ByteString:
                    return ReadByteString();
                case BenValueKind.List:
                    return ReadList();
                case BenValueKind.Dictionary:
                    return ReadDictionary();
                default:
                    break;
            }

            return null;
        }

        private BenInteger ReadInteger()
        {
            stream.Seek(streamOrgPos, SeekOrigin.Begin);

            int[] bytes = ReadUntilIncluding('e');

            if (!(bytes.Length >= 3))
                throw new InvalidOperationException("Invalid integer value.");

            if (!(bytes.First() == 'i'))
                throw new InvalidOperationException("Integer value should start with 'i'.");

            if (!(SubArray(bytes, 1, bytes.Length - 2))
                .All(b => IsDecimalChar(b)))
                throw new InvalidOperationException("Integer value should only have decimals between 'i' and 'e'.");

            if (!(bytes.Last() == 'e'))
                throw new InvalidOperationException("Integer value should end with 'e'.");
            
            string[] intStrParts = bytes
                .Skip(1)
                .Take(bytes.Length - 2)
                .Select(b => (char)b)
                .Select(c => c.ToString())
                .ToArray();
            string intStr = string.Concat(intStrParts);
            long intVal = long.Parse(intStr);

            return new BenInteger(intVal);
        }

        private BenByteString ReadByteString()
        {
            stream.Seek(streamOrgPos, SeekOrigin.Begin);

            int[] lengthBytes = ReadUntilIncluding(':');

            if (!(lengthBytes.Length >= 2))
                throw new InvalidOperationException("Invalid byte string value.");

            if (!(lengthBytes.Take(lengthBytes.Length - 1)
                .All(b => IsDecimalChar(b))))
                throw new InvalidOperationException("Byte string length should consist of only decimals.");

            if (!(lengthBytes.Last() == ':'))
                throw new InvalidOperationException("Integer value should end with ':'.");

            string[] lengthStrParts = lengthBytes
                .Take(lengthBytes.Length - 1)
                .Select(b => (char)b)
                .Select(c => c.ToString())
                .ToArray();
            string lengthStr = string.Concat(lengthStrParts);
            int length = int.Parse(lengthStr);

            stream.Seek(streamOrgPos, SeekOrigin.Begin);
            stream.Seek(lengthStr.Length + 1, SeekOrigin.Current);

            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return new BenByteString(bytes);
        }

        private BenList ReadList()
        {
            stream.Seek(streamOrgPos, SeekOrigin.Begin);
            stream.Seek(1, SeekOrigin.Current);

            var list = new List<BenValue>();

            while (true)
            {
                int currByte = stream.ReadByte();

                if (currByte == 'e')
                {
                    break;
                }
                else if (currByte == -1)
                {
                    throw new InvalidOperationException("Unexpected end of stream.");
                }
                else
                {
                    stream.Seek(-1, SeekOrigin.Current);

                    var valueParser = new BenValueParser(stream);
                    BenValue listItem = valueParser.Parse();
                    list.Add(listItem);
                }
            }

            return new BenList(list);
        }

        private BenDictionary ReadDictionary()
        {
            stream.Seek(streamOrgPos, SeekOrigin.Begin);
            stream.Seek(1, SeekOrigin.Current);

            var dict = new Dictionary<BenByteString, BenValue>();

            while (true)
            {
                int currByte = stream.ReadByte();

                if (currByte == 'e')
                {
                    break;
                }
                else if (currByte == -1)
                {
                    throw new InvalidOperationException("Unexpected end of stream.");
                }
                else
                {
                    stream.Seek(-1, SeekOrigin.Current);

                    var keyParser = new BenValueParser(stream);
                    BenByteString key = (BenByteString)keyParser.Parse();

                    var valueParser = new BenValueParser(stream);
                    BenValue value = valueParser.Parse();

                    dict.Add(key, value);
                }
            }

            return new BenDictionary(dict);
        }

        private BenValueKind ReadValueType()
        {
            stream.Seek(streamOrgPos, SeekOrigin.Begin);

            int firstByte = stream.ReadByte();

            if (firstByte == 'i')
            {
                return BenValueKind.Integer;
            }
            else if (IsDecimalChar(firstByte))
            {
                return BenValueKind.ByteString;
            }
            else if (firstByte == 'l')
            {
                return BenValueKind.List;
            }
            else if (firstByte == 'd')
            {
                return BenValueKind.Dictionary;
            }
            else
            {
                throw new InvalidOperationException("Could not detect value kind.");
            }
        }

        private int[] ReadUntilIncluding(char terminator)
        {
            var intBytes = new List<int>();

            while (true)
            {
                int currByte = stream.ReadByte();

                if (currByte == terminator)
                {
                    intBytes.Add(currByte);
                    break;
                }
                else if (currByte == -1)
                {
                    throw new InvalidOperationException("Unexpected end of stream.");
                }
                else
                {
                    intBytes.Add(currByte);
                }
            }

            return intBytes.ToArray();
        }

        private bool IsDecimalChar(int charByte)
        {
            switch (charByte)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }

        private (T[] PartOne, T[] PartTwo ) SplitArray<T>(T[] data, T separator)
        {
            int separatorIndex = Array.IndexOf(data, separator);

            if (separatorIndex == -1)
            {
                return (data, new T[0]);
            }
            else
            {
                T[] partOne = data.Take(separatorIndex).ToArray();
                T[] partTwo = data.Skip(separatorIndex + 1).ToArray();
                return (partOne, partTwo);
            }
        }

        private T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
