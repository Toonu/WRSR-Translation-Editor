using System.Text;

namespace WRLang {
    public static class BTF {
        private const int HeaderSize = 12;
        private const int DefinitionSize = 10;

        public static Translation[] LoadBtf(string path) {
            using var reader = new BinaryReader(File.OpenRead(path));
            int translationCount = reader.ReadInt32BigEndian();
            var translations = new Translation[translationCount];

            for (int i = 0; i < translationCount; i++) {
                reader.BaseStream.Position = GetDefinitionOffset(i);

                int id = reader.ReadInt32BigEndian();
                int offset = reader.ReadInt32BigEndian();
                int length = reader.ReadInt16BigEndian();

                reader.BaseStream.Position = GetTextOffset(offset, translationCount);

                var chars = reader.ReadBytes(length * 2);
                translations[i] = new Translation {
                    Id = id,
                    Text = Encoding.BigEndianUnicode.GetString(chars)
                };
            }

            return translations;
        }

        public static void SaveBtf(string path, Translation[] translations) {
            using var writer = new BinaryWriter(File.OpenWrite(path));
            int currentOffset = 0;

            for (int i = 0; i < translations.Length; i++) {
                writer.BaseStream.Position = GetDefinitionOffset(i);

                //Write translation definition
                writer.WriteInt32BigEndian(translations[i].Id);
                writer.WriteInt32BigEndian(currentOffset);
                writer.WriteInt16BigEndian((short)(translations[i].Text.Length));

                //Write translation
                writer.BaseStream.Position = GetTextOffset(currentOffset, translations.Length);
                writer.Write(Encoding.BigEndianUnicode.GetBytes(translations[i].Text + '\0'));

                currentOffset += translations[i].Text.Length + 1;
            }

            writer.BaseStream.Position = 0;

            //Write header
            writer.WriteInt32BigEndian(translations.Length);
            writer.WriteInt32BigEndian((int)writer.BaseStream.Length);
            writer.WriteInt32BigEndian(currentOffset);
        }

        private static int GetDefinitionOffset(int definitionIndex) {
            return HeaderSize + definitionIndex * DefinitionSize;
        }

        private static int GetTextOffset(int translationOffset, int translationCount) {
            return HeaderSize + translationCount * DefinitionSize + (translationOffset * 2);
        }
    }
}
