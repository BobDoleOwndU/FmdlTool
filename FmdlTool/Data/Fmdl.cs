using System;
using System.IO;
using System.Text;

namespace FmdlTool
{
    public class Fmdl
    {
        private struct Section0Info
        {
            public ushort id;
            public ushort numEntries;
            public uint offset;
        } //struct

        private struct Section1Info
        {
            public uint id;
            public uint offset;
            public uint length;
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

        private struct Section3Entry
        {
            public ushort unknown0;
            public ushort unknown1;
            public ushort unknown2;
            public ushort numVertices;
            public uint faceOffset;
            public uint numFaceVertices;
            public ulong unknown3;
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

        private struct Vertex
        {
            public float x;
            public float y;
            public float z;
        } //struct

        private struct Object
        {
            public Vertex[] vertices;
            public Face[] faces;
        } //struct

        private struct Face
        {
            public ushort v1;
            public ushort v2;
            public ushort v3;
        } //struct

        //local variables
        private uint signature;
        private uint unknown0;
        private ulong unknown1;
        private ulong unknown2;
        private ulong unknown3;
        private uint numSections0;
        private uint numSections1;
        private uint section0Offset;
        private uint section0Length;
        private uint section1Offset;
        private uint section1Length;

        private Object[] objects;

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
        private Section0Info[] section0Info;
        private Section1Info[] section1Info;

        private Section1Entry[] section1Entries;
        private Section2Entry[] section2Entries;
        private Section3Entry[] section3Entries;
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
            numSections0 = reader.ReadUInt32();
            numSections1 = reader.ReadUInt32();
            section0Offset = reader.ReadUInt32();
            section0Length = reader.ReadUInt32();
            section1Offset = reader.ReadUInt32();
            section1Length = reader.ReadUInt32();
            reader.BaseStream.Position += 0x8; //8 bytes of padding here.
            
            section0Info = new Section0Info[numSections0];

            //get the section0 info.
            for(int i = 0; i < section0Info.Length; i++)
            {
                section0Info[i].id = reader.ReadUInt16();
                section0Info[i].numEntries = reader.ReadUInt16();
                section0Info[i].offset = reader.ReadUInt32();
            } //for

            section1Info = new Section1Info[numSections1];

            //get the section1 info.
            for (int i = 0; i < section1Info.Length; i++)
            {
                section1Info[i].id = reader.ReadUInt32();
                section1Info[i].offset = reader.ReadUInt32();
                section1Info[i].length = reader.ReadUInt32();
            } //for

            section1Entries = new Section1Entry[section0Info[1].numEntries];
            section2Entries = new Section2Entry[section0Info[2].numEntries];
            section3Entries = new Section3Entry[section0Info[3].numEntries];
            section7Entries = new Section7Entry[section0Info[7].numEntries];
            section8Entries = new Section8Entry[section0Info[8].numEntries];
            section15Entries = new ulong[section0Info[18].numEntries];
            section16Entries = new ulong[section0Info[19].numEntries];

            objects = new Object[section0Info[3].numEntries];

            /****************************************************************
             *
             * SECTION 0x1 - MESH GROUP DEFINITIONS
             *
             ****************************************************************/
            //go to and get the section 0x1 entry info.
            reader.BaseStream.Position = section0Info[1].offset + section0Offset;

            for(int i = 0; i < section1Entries.Length; i++)
            {
                section1Entries[i].nameId = reader.ReadUInt16();
                section1Entries[i].invisibilityFlag = reader.ReadUInt16();
                section1Entries[i].unknown = reader.ReadUInt32();
            } //for

            /****************************************************************
             *
             * SECTION 0x2 - OBJECT ASSIGNMENT
             *
             ****************************************************************/
            //go to and get the section 0x2 entry info.
            reader.BaseStream.Position = section0Info[2].offset + section0Offset;

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

            /****************************************************************
             *
             * SECTION 0x3 - OBJECT DATA
             *
             ****************************************************************/
            reader.BaseStream.Position = section0Info[3].offset + section0Offset;

            for(int i = 0; i < section3Entries.Length; i++)
            {
                reader.BaseStream.Position += 0x4;
                section3Entries[i].unknown0 = reader.ReadUInt16();
                section3Entries[i].unknown1 = reader.ReadUInt16();
                section3Entries[i].unknown2 = reader.ReadUInt16();
                section3Entries[i].numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section3Entries[i].faceOffset = reader.ReadUInt32();
                section3Entries[i].numFaceVertices = reader.ReadUInt32();
                section3Entries[1].unknown3 = reader.ReadUInt64();
                reader.BaseStream.Position += 0x10;
            } //for

            /****************************************************************
             *
             * SECTION 0x7 - TEXTURE TYPE ASSIGNMENT
             *
             ****************************************************************/
            //go to and get the section 0x7 entry info.
            reader.BaseStream.Position = section0Info[7].offset + section0Offset;

            for (int i = 0; i < section7Entries.Length; i++)
            {
                section7Entries[i].nameId = reader.ReadUInt16();
                section7Entries[i].textureId = reader.ReadUInt16();
            } //for

            /****************************************************************
             *
             * SECTION 0x8 - UNKNOWN
             *
             ****************************************************************/
            //go to and get the section 0x8 entry info.
            reader.BaseStream.Position = section0Info[8].offset + section0Offset;

            for (int i = 0; i < section8Entries.Length; i++)
            {
                section8Entries[i].nameId = reader.ReadUInt16();
                section8Entries[i].materialNameId = reader.ReadUInt16();
            } //for

            /****************************************************************
             *
             * SECTION Ox15 - TEXTURE HASH LIST
             *
             ****************************************************************/
            //go to and get the section 0x15 entry info.
            reader.BaseStream.Position = section0Info[18].offset + section0Offset;

            for (int i = 0; i < section15Entries.Length; i++)
            {
                section15Entries[i] = reader.ReadUInt64();
            } //for

            /****************************************************************
             *
             * SECTION 0x16 - NAME HASH LIST
             *
             ****************************************************************/
            //go to and get the section 0x16 entry info.
            reader.BaseStream.Position = section0Info[19].offset + section0Offset;

            for (int i = 0; i < section16Entries.Length; i++)
            {
                section16Entries[i] = reader.ReadUInt64();
            } //for

            /****************************************************************
             *
             * OBJECTS
             *
             ****************************************************************/
            reader.BaseStream.Position = section1Info[1].offset + section1Offset;

            for(int i = 0; i < section3Entries.Length; i++)
            {
                objects[i].vertices = new Vertex[section3Entries[i].numVertices];

                for(int j = 0; j < section3Entries[i].numVertices; j++)
                {
                    objects[i].vertices[j].x = reader.ReadSingle();
                    objects[i].vertices[j].y = reader.ReadSingle();
                    objects[i].vertices[j].z = reader.ReadSingle();
                } //for
            } //for

            /*
            Need offset code before the face loop can read its data.

            for(int i = 0; i < section3Entries.Length; i++)
            {
                objects[i].faces = new Face[section3Entries[i].numFaceVertices / 3];

                for(int j = 0; j < section3Entries[i].numFaceVertices / 3; j++)
                {
                    objects[i].faces[j].v1 = reader.ReadUInt16();
                    objects[i].faces[j].v2 = reader.ReadUInt16();
                    objects[i].faces[j].v3 = reader.ReadUInt16();
                } //for
            } //for
            */
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
