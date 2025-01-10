using System.Text;

namespace String.Demo.ConsoleApp;

using System;
using System.Runtime.InteropServices;

class Program
{
    public static void Main(string[] args)
    {
        try
        {
            new Program().Run(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Run(string[] args)
    {
        DemoString();
        DemoStringBuilder();
        DemoBstr();
    }

    public void DemoString()
    {
        unsafe
        {
            Console.WriteLine("============");
            Console.WriteLine("DemoString()");
            Console.WriteLine("============");

            /* string0 and string1 have different addresses on the stack
             * but point to the same object on the heap.
             *
             * string2 points to a different object on the heap because
             * it is a reference to a string literal.
             */
            ulong i = ulong.MaxValue;
            var string0 = new string("Hello World!");
            // string1[0] = 'h';
            var string1 = string0;
            var string2 = "Hello World!";
            ulong j = ulong.MaxValue;

            Console.WriteLine($" &{nameof(string0)}: 0x{(long)&string0:x16}");
            Console.WriteLine($" &{nameof(string1)}: 0x{(long)&string1:x16}");
            Console.WriteLine($" &{nameof(string2)}: 0x{(long)&string2:x16}");

            Console.WriteLine($"*&{nameof(string0)}: 0x{(long)*(void**)&string0:x16}");
            Console.WriteLine($"*&{nameof(string1)}: 0x{(long)*(void**)&string1:x16}");
            Console.WriteLine($"*&{nameof(string2)}: 0x{(long)*(void**)&string2:x16}");
            Console.WriteLine();

            /* string0 and string1 have different addresses on the stack
             * and point to different objects on the heap.
             */
            string1 = string1.ToLower();

            Console.WriteLine($" &{nameof(string0)}: 0x{(long)&string0:x16}");
            Console.WriteLine($" &{nameof(string1)}: 0x{(long)&string1:x16}");

            Console.WriteLine($"*&{nameof(string0)}: 0x{(long)*(void**)&string0:x16}");
            Console.WriteLine($"*&{nameof(string1)}: 0x{(long)*(void**)&string1:x16}");
            Console.WriteLine();

            /* string2 is reallocated on the heap each time a character
             * is concatenated.
             */
            for (int k = 0; k < 10; k++)
            {
                string2 = string2 + '!';
                Console.WriteLine($"*&{nameof(string2)}: 0x{(long)*(void**)&string2:x16}");
            }

            Console.WriteLine("============");
            Console.WriteLine();
        }
    }

    public void DemoStringBuilder()
    {
        unsafe
        {
            Console.WriteLine("===================");
            Console.WriteLine("DemoStringBuilder()");
            Console.WriteLine("===================");

            var string1 = "Hello World!";
            var sb = new StringBuilder(string1);
            long addressOfSbOnHeap;
            long addressOfm_ChunkPrevious;
            Console.WriteLine($" &{nameof(sb)}: 0x{(long)&sb:x16}");
            addressOfSbOnHeap = (long)*(void**)&sb;
            Console.WriteLine($"*&{nameof(sb)}: 0x{addressOfSbOnHeap:x16}");
            Console.WriteLine($"*&{nameof(sb)}.m_ChunkChars: 0x{((long)*(void**)(addressOfSbOnHeap + sizeof(IntPtr))):x16}");
            Console.WriteLine($"*&{nameof(sb)}.m_ChunkPrevious: 0x{((long)*(void**)(addressOfSbOnHeap + 2 * sizeof(IntPtr))):x16}");
            for (int k = 0; k < 10; k++)
            {
                sb.Append('!');

                Console.WriteLine($"===== AFTER APPEND #{k} =====");
                Console.WriteLine($" &{nameof(sb)}: 0x{(long)&sb:x16}");
                addressOfSbOnHeap = (long)*(void**)&sb;
                Console.WriteLine($"*&{nameof(sb)}: 0x{addressOfSbOnHeap:x16}");
                string indent = string.Empty;
                while (addressOfSbOnHeap != 0)
                {
                    Console.WriteLine($"{indent}*&{nameof(sb)}.m_ChunkChars: 0x{((long)*(void**)(addressOfSbOnHeap + sizeof(IntPtr))):x16}");
                    addressOfm_ChunkPrevious = (long)*(void**)(addressOfSbOnHeap + 2 * sizeof(IntPtr));
                    Console.WriteLine($"{indent}*&{nameof(sb)}.m_ChunkPrevious: 0x{addressOfm_ChunkPrevious:x16}");
                    addressOfSbOnHeap = addressOfm_ChunkPrevious;
                    indent += "  ";
                }
            }

            Console.WriteLine("===================");
            Console.WriteLine();
        }
    }

    [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr SysAllocString([MarshalAs(UnmanagedType.LPWStr)] string s);

    [DllImport("oleaut32.dll", SetLastError = true)]
    public static extern void SysFreeString(IntPtr bstr);

    [DllImport("oleaut32.dll", SetLastError = true)]
    public static extern uint SysStringLen(IntPtr bstr);

    public void DemoBstr()
    {
        Console.WriteLine("==========");
        Console.WriteLine("DemoBstr()");
        Console.WriteLine("==========");

        IntPtr bstr = SysAllocString("Hello World!");

        if (bstr == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocate BSTR.");
            return;
        }
        
        uint length = SysStringLen(bstr);
        Console.WriteLine($"BSTR Length: {length}");
        
        string managedString = Marshal.PtrToStringBSTR(bstr);
        Console.WriteLine($"Managed String: {managedString}");

        SysFreeString(bstr);

        Console.WriteLine("==========");
        Console.WriteLine();
    }
}
