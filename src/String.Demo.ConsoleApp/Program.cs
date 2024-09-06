namespace String.Demo.ConsoleApp;

using System;

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
        unsafe
        {
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
        }
    }
}
