using System;
using System.IO;

namespace CodeTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string mapName = args[0];
                ReadMap(mapName);
            }
            catch
            {
                Console.WriteLine("Please specify a map to use");
                string mapName = Console.ReadLine();
                ReadMap(mapName);
            }

        }
        public static void ReadMap(string mapName)
        {
            FileStream MapFS = new FileStream(mapName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            /*Console.WriteLine("What offset would you like to get?");
            string inputoffset = Console.ReadLine();
            long offset = long.Parse(inputoffset, NumberStyles.HexNumber);*/

            //File Header
            MapFS.Seek(0x4B4 + 0x8, 0); //mask offset
            byte[] maskoffset = new byte[0x4];
            MapFS.Read(maskoffset, 0, 4);

            MapFS.Seek(0x4C4 + 0x10, 0); //sections offset
            byte[] sectionoffset = new byte[0x4];
            MapFS.Read(sectionoffset, 0, 4);

            MapFS.Seek(0x2F8, 0); //virtual base
            byte[] virtualbaseaddress = new byte[0x8];
            MapFS.Read(virtualbaseaddress, 0, 8);

            MapFS.Seek(0x10, 0); //index header address
            byte[] indexheaderaddress = new byte[0x8];
            MapFS.Read(indexheaderaddress, 0, 8);

            int tag_section_offset = BitConverter.ToInt32(maskoffset, 0) + BitConverter.ToInt32(sectionoffset, 0);

            long address_mask = tag_section_offset - BitConverter.ToInt64(virtualbaseaddress, 0);

            long tags_header_offset = BitConverter.ToInt64(indexheaderaddress, 0) + address_mask;


            //Index table header
            MapFS.Seek(tags_header_offset, 0); //tags header offset
            byte[] group_tag_count = new byte[0x4];
            MapFS.Read(group_tag_count, 0, 4);

            MapFS.Seek(tags_header_offset + 0x8, 0);
            byte[] tag_group_table_address = new byte[0x8];
            MapFS.Read(tag_group_table_address, 0, 8);

            MapFS.Seek(tags_header_offset + 0x10, 0);
            byte[] tag_count = new byte[0x4];
            MapFS.Read(tag_count, 0, 4);

            MapFS.Seek(tags_header_offset + 0x18, 0);
            byte[] tag_table_address = new byte[0x8];
            MapFS.Read(tag_table_address, 0, 8);

            MapFS.Seek(tags_header_offset + 0x20, 0);
            byte[] global_tag_count = new byte[0x4];
            MapFS.Read(global_tag_count, 0, 4);

            MapFS.Seek(tags_header_offset + 0x28, 0);
            byte[] global_tag_table_address = new byte[0x8];
            MapFS.Read(global_tag_table_address, 0, 8);

            MapFS.Seek(tags_header_offset + 0x30, 0);
            byte[] tags_interop_count = new byte[0x4];
            MapFS.Read(tags_interop_count, 0, 4);

            MapFS.Seek(tags_header_offset + 0x38, 0);
            byte[] tags_interop_table_address = new byte[0x8];
            MapFS.Read(tags_interop_table_address, 0, 8);

            MapFS.Seek(tags_header_offset + 0x44, 0);
            byte[] index_table_magic = new byte[0x4];
            MapFS.Read(index_table_magic, 0, 4);

            long tag_group_table_address_real = BitConverter.ToInt64(tag_group_table_address) + address_mask;

            long tag_table_address_real = BitConverter.ToInt64(tag_table_address) + address_mask;

            long global_tag_table_address_real = BitConverter.ToInt64(global_tag_table_address) + address_mask;

            long tags_interop_table_address_real = BitConverter.ToInt64(tags_interop_table_address) + address_mask;

            //Tag group table
            MapFS.Seek(tag_group_table_address_real, 0);
            byte[] tag_group_magic = new byte[0x4];
            MapFS.Read(tag_group_magic, 0, 4);

            MapFS.Seek(tag_group_table_address_real + 0x4, 0);
            byte[] tag_group_parent_magic = new byte[0x4];
            MapFS.Read(tag_group_parent_magic, 0, 4);

            MapFS.Seek(tag_group_table_address_real + 0x8, 0);
            byte[] tag_group_grandparent_magic = new byte[0x4];
            MapFS.Read(tag_group_grandparent_magic, 0, 4);

            MapFS.Seek(tag_group_table_address_real + 0xC, 0);
            byte[] tag_group_stringid = new byte[0x4];
            MapFS.Read(tag_group_stringid, 0, 4);

            //Tag table
            MapFS.Seek(tag_table_address_real, 0); //int16
            byte[] tag_group_index = new byte[0x2];
            MapFS.Read(tag_group_index, 0, 2);

            MapFS.Seek(tag_table_address_real + 0x2, 0); //uint16
            byte[] tag_table_datum_index_salt = new byte[0x2];
            MapFS.Read(tag_table_datum_index_salt, 0, 2);

            MapFS.Seek(tag_table_address_real + 0x4, 0);
            byte[] tag_table_memory_address = new byte[0x4];
            MapFS.Read(tag_table_memory_address, 0, 4);

            //uint tag_datum_index = BitConverter.ToUInt16(tag_table_datum_index_salt) << 8 | BitConverter.ToUInt16(tag_group_index); //idk

            //Global tag table
            MapFS.Seek(global_tag_table_address_real, 0);
            byte[] global_tag_group_magic = new byte[0x4];
            MapFS.Read(global_tag_group_magic, 0, 4);

            MapFS.Seek(global_tag_table_address_real + 0x4, 0);
            byte[] global_tag_datum_index = new byte[0x4];
            MapFS.Read(global_tag_datum_index, 0, 4);

            //Tag interop
            MapFS.Seek(tags_interop_table_address_real, 0);
            byte[] tag_interop_pointer = new byte[0x4];
            MapFS.Read(tag_interop_pointer, 0, 4);

            MapFS.Seek(tags_interop_table_address_real + 0x4, 0);
            byte[] tag_interop_type = new byte[0x4];
            MapFS.Read(tag_interop_type, 0, 4);

            Console.WriteLine("Index Header Address: " + BitConverter.ToInt64(indexheaderaddress, 0));

            Console.WriteLine("Virtual Base Address: " + BitConverter.ToInt64(virtualbaseaddress, 0));

            Console.WriteLine("Mask offset: " + BitConverter.ToInt32(maskoffset, 0));

            Console.WriteLine("Section offset: " + BitConverter.ToInt32(sectionoffset, 0));

            Console.WriteLine("Tag section offset: " + tag_section_offset);

            Console.WriteLine("Address mask: " + address_mask);

            Console.WriteLine("Tag header offset: " + tags_header_offset);

            Console.WriteLine("Tag group table address(real): " + tag_group_table_address_real);

            Console.WriteLine("Tag table address (real): " + tag_table_address_real);

            Console.WriteLine("Global tag group table address(real): " + global_tag_table_address_real);

            //Console.WriteLine("Okay, what tag do you want the group for?");

            //long tag_instance_tag_group = tag_table_address_real + 0x2 ;


            //the plus 0x0 can be replaced by the offset we want to go to
            //byte[] tag_instance_group_index = new byte[0x2];
            //MapFS.Read(tag_instance_group_index, 0, 2);

            //MapFS.Seek(tag_group_table_address_real + (tag_instance_tag_group * 0x64), 0);

            int ZoneTagGroup = ZoneGroupTagLookUp(MapFS, tag_group_table_address_real, group_tag_count);

            ZoneTagLookUp(MapFS, address_mask, tag_table_address_real, tag_count, ZoneTagGroup);

        }
        public static string ArrayToHex(byte[] array)
        {
            uint myInt = BitConverter.ToUInt32(array);
            return myInt.ToString("X");
        }
        public static string ArrayToString(byte[] array)
        {
            uint myInt = BitConverter.ToUInt32(array);
            return myInt.ToString();
        }

        public static int TagGroupLookUp(FileStream MapFS, long tag_group_table_address_real, int groupIndex)
        {
            long groupIndexOffset = tag_group_table_address_real + (groupIndex * 0x10);
            for (long i = tag_group_table_address_real; i <= groupIndexOffset; i += 0x10) //loops through each element in the tag table looking for the group index
            {

                if (i == groupIndexOffset)
                {
                    MapFS.Seek(i, 0);
                    byte[] group_magic = new byte[0x4]; //magic means tag type? idk
                    MapFS.Read(group_magic, 0, 4);
                    //Console.WriteLine("Tag group is: {0}", BitConverter.ToString(group_magic));
                    return BitConverter.ToInt32(group_magic);
                }
            }
            return 0;
        }
        public static int TagLookUp(FileStream MapFS, long tag_table_address_real, short tagIndex)
        {
            long IndexOffset = tag_table_address_real + (tagIndex * 0x8);
            for (long i = tag_table_address_real; i <= IndexOffset; i += 0x8) //loops through each element in the tag table looking for the tag index
            {

                if (i == IndexOffset)
                {
                    MapFS.Seek(i, 0);
                    byte[] group_magic = new byte[0x2]; //magic means tag type? idk
                    MapFS.Read(group_magic, 0, 2);
                    //Console.WriteLine("Tag group index is: {0}", BitConverter.ToString(group_magic));
                    return BitConverter.ToInt16(group_magic);
                }
            }
            return 0;
        }
        public static int ZoneGroupTagLookUp(FileStream MapFS, long tag_group_table_address_real, byte[] group_tag_count)
        {
            int iterations = 0;
            for (long i = tag_group_table_address_real; i < tag_group_table_address_real + (0x10 * BitConverter.ToInt32(group_tag_count)); i += 0x10) //loops through each element in the tag table looking for the group index
            {
                byte[] enoz = { 0x65, 0x6E, 0x6F, 0x7A };
                MapFS.Seek(i, 0); //has to use i and i has to hhe the tag table address. dont think too much about it
                byte[] group_magic = new byte[0x4]; //magic means tag type? idk
                MapFS.Read(group_magic, 0, 4);
                if (BitConverter.ToInt32(enoz) == BitConverter.ToInt32(group_magic))
                {
                    Console.WriteLine("Zone is: {0}", iterations);
                    return iterations;
                }
                iterations++;
            }
            return 0;
        }
        public static int ZoneTagLookUp(FileStream MapFS, long address_mask, long tag_table_address_real, byte[] tag_count, int ZoneTagGroup)
        {
            //long groupIndexOffset = tag_table_address_real + (groupIndex * 0x10);
            for (long i = tag_table_address_real; i < tag_table_address_real + (0x8 * BitConverter.ToInt32(tag_count)); i += 0x8) //loops through each element in the tag table looking for the group index
            {
                MapFS.Seek(i, 0); //has to use i and i has to hhe the tag table address. dont think too much about it
                byte[] group_index = new byte[0x2]; 
                MapFS.Read(group_index, 0, 2);
                //Console.WriteLine(group_index[0] + group_index[1]);
                int group_index_int = BitConverter.ToInt16(group_index);
                if (group_index_int == ZoneTagGroup)
                {
                    byte[] tag_datum = new byte[0x2];
                    MapFS.Read(tag_datum, 0, 2);
                    byte[] tag_memory_address = new byte[0x4];
                    MapFS.Read(tag_memory_address, 0, 4);
                    long tag_memory_address_int = BitConverter.ToInt32(tag_memory_address);
                    //Console.WriteLine(tag_memory_address_int);
                    long tag_memory_address_4x = tag_memory_address_int * 4;
                    //Console.WriteLine(tag_memory_address_4x);
                    long tag_memory_address_real = tag_memory_address_4x + address_mask;
                    Console.WriteLine("Zone Tag Group: {0} \nTag Datum: {1} \nTag Address {2}", BitConverter.ToString(group_index), BitConverter.ToUInt16(tag_datum), tag_memory_address_real);
                    return BitConverter.ToInt16(group_index);
                }
            }
            return 0;
        }
    }
}
