using System;
using System.IO;
using System.Text;

namespace FmdlTool
{
    public class Fmdl
    {
        private struct SectionInfo
        {
            public ushort id;
            public ushort numEntries;
            public uint offset;
        } //struct

        private struct Section1Entry
        {
            public ushort nameId;
            public ushort invisibilityFlag;
            public uint unknown;
        } //struct

        private struct Section2Entry
        {
            public ushort meshGroupId;
            public ushort numObjects;
            public ushort numPrecedingObjects;
            public ushort id;
            public ushort materialId;
        } //struct

        private struct Section7Entry
        {
            public ushort nameId;
            public ushort textureId;
        } //struct

        private struct Section8Entry
        {
            public ushort nameId;
            public ushort materialNameId;
        } //struct

        private uint signature;
        private uint unknown0;
        private ulong unknown1;
        private ulong unknown2;
        private ulong unknown3;
        private uint numSections;
        private uint unknown4;
        private uint headerLength;
        private uint dataOffset0;
        private uint dataOffset1;
        private uint dataLength;

        /*
         * There are 20 (0x14) sections in The Phantom Pain's models. Sections 0xC, 0xF and 0x13 do not exist.
         * 0 = 0x0
         * 1 = 0x1
         * 2 = 0x2
         * 3 = 0x3
         * 4 = 0x4
         * 5 = 0x5
         * 6 = 0x6
         * 7 = 0x7
         * 8 = 0x8
         * 9 = 0x9
         * 10 = 0xA
         * 11 = 0xB
         * 12 = 0xD
         * 13 = 0xE
         * 14 = 0x10
         * 15 = 0x11
         * 16 = 0x12
         * 17 = 0x14
         * 18 = 0x15
         * 19 = 0x16
         */
        private SectionInfo[] sectionInfo;

        private Section1Entry[] section1Entries;
        private Section2Entry[] section2Entries;
        private Section7Entry[] section7Entries;
        private Section8Entry[] section8Entries;
        private ulong[] section15Entries;
        private ulong[] section16Entries;

        public void Read(FileStream stream)
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.Default, true);

            signature = reader.ReadUInt32();
            unknown0 = reader.ReadUInt32();
            unknown1 = reader.ReadUInt64();
            unknown2 = reader.ReadUInt64();
            unknown3 = reader.ReadUInt64();
            numSections = reader.ReadUInt32();
            unknown4 = reader.ReadUInt32();
            headerLength = reader.ReadUInt32();
            dataOffset0 = reader.ReadUInt32();
            dataOffset1 = reader.ReadUInt32();
            dataLength = reader.ReadUInt32();
            reader.BaseStream.Position += 0x8; //8 bytes of padding here.

            sectionInfo = new SectionInfo[numSections];

            //get the section info.
            for(int i = 0; i < sectionInfo.Length; i++)
            {
                sectionInfo[i].id = reader.ReadUInt16();
                sectionInfo[i].numEntries = reader.ReadUInt16();
                sectionInfo[i].offset = reader.ReadUInt32();
            } //for

            section1Entries = new Section1Entry[sectionInfo[1].numEntries];
            section2Entries = new Section2Entry[sectionInfo[2].numEntries];
            section7Entries = new Section7Entry[sectionInfo[7].numEntries];
            section8Entries = new Section8Entry[sectionInfo[8].numEntries];
            section15Entries = new ulong[sectionInfo[18].numEntries];
            section16Entries = new ulong[sectionInfo[19].numEntries];

            //Do 0x15 and 0x16 before anything else as most other stuff references the names in these lists.

            //go to and get the section 0x15 entry info.
            reader.BaseStream.Position = sectionInfo[18].offset + headerLength;

            for (int i = 0; i < section15Entries.Length; i++)
            {
                section15Entries[i] = reader.ReadUInt64();
            } //for

            //go to and get the section 0x16 entry info.
            reader.BaseStream.Position = sectionInfo[19].offset + headerLength;

            for (int i = 0; i < section16Entries.Length; i++)
            {
                section16Entries[i] = reader.ReadUInt64();
            } //for

            //go to and get the section 0x1 entry info.
            reader.BaseStream.Position = sectionInfo[1].offset + headerLength;

            for(int i = 0; i < section1Entries.Length; i++)
            {
                section1Entries[i].nameId = reader.ReadUInt16();
                section1Entries[i].invisibilityFlag = reader.ReadUInt16();
                section1Entries[i].unknown = reader.ReadUInt32();
            } //for

            //go to and get the section 0x2 entry info.
            reader.BaseStream.Position = sectionInfo[2].offset + headerLength;

            for (int i = 0; i < section2Entries.Length; i++)
            {
                reader.BaseStream.Position += 0x4;
                section2Entries[i].meshGroupId = reader.ReadUInt16();
                section2Entries[i].numObjects = reader.ReadUInt16();
                section2Entries[i].numPrecedingObjects = reader.ReadUInt16();
                section2Entries[i].id = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section2Entries[i].materialId = reader.ReadUInt16();
                reader.BaseStream.Position += 0xE;
            } //for

            //go to and get the section 0x7 entry info.
            reader.BaseStream.Position = sectionInfo[7].offset + headerLength;

            for (int i = 0; i < section7Entries.Length; i++)
            {
                section7Entries[i].nameId = reader.ReadUInt16();
                section7Entries[i].textureId = reader.ReadUInt16();
            } //for

            //go to and get the section 0x8 entry info.
            reader.BaseStream.Position = sectionInfo[8].offset + headerLength;

            for (int i = 0; i < section8Entries.Length; i++)
            {
                section8Entries[i].nameId = reader.ReadUInt16();
                section8Entries[i].materialNameId = reader.ReadUInt16();
            } //for
        } //Read

        public void OutputSection2Info()
        {
            for(int i = 0; i < section2Entries.Length; i++)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Entry ID: " + section2Entries[i].id);
                Console.WriteLine("Mesh Group: " + section16Entries[section1Entries[section2Entries[i].meshGroupId].nameId].ToString("x"));
                Console.WriteLine("Number of Objects: " + section2Entries[i].numObjects);
                Console.WriteLine("Number of Preceding Objects: " + section2Entries[i].numPrecedingObjects);
                Console.WriteLine("Material ID: " + section2Entries[i].materialId);
            } //for
        } //OutputSection2Info

        public void OutputSection7Info()
        {
            for (int i = 0; i < section7Entries.Length; i++)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Entry No: " + i);
                Console.WriteLine("Texture Type Hash: " + section16Entries[section7Entries[i].nameId].ToString("x"));
                Console.WriteLine("Texture Hash: " + (section15Entries[section7Entries[i].textureId] - 0x1568000000000000).ToString("x"));
            } //for
        } //OutputSection2Info

        public void OutputSection8Info()
        {
            for (int i = 0; i < section8Entries.Length; i++)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Entry No: " + i);
                Console.WriteLine("Unknown Hash: " + (section16Entries[section8Entries[i].nameId]).ToString("x"));
                Console.WriteLine("Material Hash: " + (section16Entries[section8Entries[i].materialNameId]).ToString("x"));
            } //for
        } //OutputSection2Info

        public void OutputSection16Info()
        {
            for (int i = 0; i < section16Entries.Length; i++)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Entry No: " + i);
                Console.WriteLine("Hash: " + section16Entries[i].ToString("x"));
            } //for
        } //OutputSection2Info
    } //class
} //namespace
