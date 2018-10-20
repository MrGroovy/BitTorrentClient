using Lib.Bittorrent.StateManagement;
using Lib.Bencoding;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Lib.Bittorrent.MetainfoDecoding
{
    public class MetaInfoSerializer
    {
        public MetaInfo Deserialize(Stream bencoding)
        {
            var bencodingParser = new BenValueParser(bencoding);
            BenDictionary dict = (BenDictionary)bencodingParser.Parse();

            var info = new MetaInfo();
            info.Announce = dict.Get<BenByteString>("announce").AsString;

            info.Comment = dict.HasString("comment")
                ? dict.GetString("comment")
                : null;

            if (dict.HasDict("info"))
            {
                BenDictionary infoDict = dict.GetDict("info");

                info.PieceLength = infoDict.HasInt("piece length")
                    ? infoDict.GetInt("piece length").Value
                    : 0;

                if (infoDict.HasString("pieces"))
                {
                    byte[] piecesString = infoDict.GetStringBytes("pieces");
                    List<byte[]> pieces = Enumerable.Range(0, piecesString.Length / 20)
                        .Select(i => piecesString.Skip(i * 20).Take(20).ToArray())
                        .ToList();

                    foreach (byte[] piece in pieces)
                        info.AddPiece(piece);
                }

                if (infoDict.HasString("name") && infoDict.HasInt("length"))
                {
                    // Single
                    List<string> path = new List<string> { infoDict.GetString("name") };
                    long length = infoDict.GetInt("length").Value;
                    info.AddFile(path, length);
                }
                else if (infoDict.HasString("name") && infoDict.HasList("files"))
                {
                    // Multiple
                    List<BenDictionary> files = infoDict.GetListOfDicts("files");
                    string name = infoDict.GetString("name");

                    foreach (BenDictionary fileDict in files)
                    {
                        List<string> path = new List<string>();
                        path.Add(name);
                        path.AddRange(fileDict.GetListOfStrings("path"));

                        long length = fileDict.GetInt("length").Value;
                        info.AddFile(path, length);
                    }
                }

                using (var stream = new MemoryStream())
                using (var sha1 = SHA1.Create())
                {
                    infoDict.Encode(stream);
                    byte[] infoDictBytes = stream.ToArray();
                    info.InfoHash = sha1.ComputeHash(infoDictBytes);
                }
            }
            else
            {
            }

            return info;
        }
    }
}
