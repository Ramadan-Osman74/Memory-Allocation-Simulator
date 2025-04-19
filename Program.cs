using System;

class MemoryAllocator
{
    const int MAX_BLOCKS = 100;
    const int NAME_LEN = 10;
    static Block[] memory = new Block[MAX_BLOCKS];
    static int blockCount = 1;
    static int MEMORY_SIZE;

    public struct Block
    {
        public int start;
        public int end;
        public string name;
    }

    // Initialize memory with given size
    static void InitializeMemory(int size)
    {
        MEMORY_SIZE = size;
        memory[0].start = 0;
        memory[0].end = size - 1;
        memory[0].name = "Unused";
    }

    // Print the memory blocks
    static void PrintMemory()
    {
        for (int i = 0; i < blockCount; i++)
        {
            Console.WriteLine($"Addresses [{memory[i].start}:{memory[i].end}] {memory[i].name}");
        }
    }

    // Allocate memory using First Fit strategy
    static void AllocateFirstFit(string name, int size)
    {
        for (int i = 0; i < blockCount; i++)
        {
            if (memory[i].name == "Unused")
            {
                int holeSize = memory[i].end - memory[i].start + 1;
                if (holeSize >= size)
                {
                    int oldEnd = memory[i].end;

                    // Allocate current block
                    memory[i].end = memory[i].start + size - 1;
                    memory[i].name = name;

                    // Create new block if space is left
                    if (holeSize > size)
                    {
                        for (int j = blockCount; j > i + 1; j--)
                        {
                            memory[j] = memory[j - 1];
                        }
                        memory[i + 1].start = memory[i].end + 1;
                        memory[i + 1].end = oldEnd;
                        memory[i + 1].name = "Unused";
                        blockCount++;
                    }
                    return;
                }
            }
        }
        Console.WriteLine("Error: Not enough memory.");
    }

    // Allocate memory using Best Fit strategy
    static void AllocateBestFit(string name, int size)
    {
        int bestIndex = -1;
        int bestHoleSize = int.MaxValue;

        for (int i = 0; i < blockCount; i++)
        {
            if (memory[i].name == "Unused")
            {
                int holeSize = memory[i].end - memory[i].start + 1;
                if (holeSize >= size && holeSize < bestHoleSize)
                {
                    bestHoleSize = holeSize;
                    bestIndex = i;
                }
            }
        }

        if (bestIndex != -1)
        {
            int oldEnd = memory[bestIndex].end;

            // Allocate best fit block
            memory[bestIndex].end = memory[bestIndex].start + size - 1;
            memory[bestIndex].name = name;

            // Create new block if space is left
            if (bestHoleSize > size)
            {
                for (int j = blockCount; j > bestIndex + 1; j--)
                {
                    memory[j] = memory[j - 1];
                }
                memory[bestIndex + 1].start = memory[bestIndex].end + 1;
                memory[bestIndex + 1].end = oldEnd;
                memory[bestIndex + 1].name = "Unused";
                blockCount++;
            }
            return;
        }
        Console.WriteLine("Error: Not enough memory.");
    }

    // Allocate memory using Worst Fit strategy
    static void AllocateWorstFit(string name, int size)
    {
        int worstIndex = -1;
        int worstHoleSize = -1;

        for (int i = 0; i < blockCount; i++)
        {
            if (memory[i].name == "Unused")
            {
                int holeSize = memory[i].end - memory[i].start + 1;
                if (holeSize >= size && holeSize > worstHoleSize)
                {
                    worstHoleSize = holeSize;
                    worstIndex = i;
                }
            }
        }

        if (worstIndex != -1)
        {
            int oldEnd = memory[worstIndex].end;

            // Allocate worst fit block
            memory[worstIndex].end = memory[worstIndex].start + size - 1;
            memory[worstIndex].name = name;

            // Create new block if space is left
            if (worstHoleSize > size)
            {
                for (int j = blockCount; j > worstIndex + 1; j--)
                {
                    memory[j] = memory[j - 1];
                }
                memory[worstIndex + 1].start = memory[worstIndex].end + 1;
                memory[worstIndex + 1].end = oldEnd;
                memory[worstIndex + 1].name = "Unused";
                blockCount++;
            }
            return;
        }
        Console.WriteLine("Error: Not enough memory.");
    }

    // Release the memory for a given process
    static void Release(string name)
    {
        for (int i = 0; i < blockCount; i++)
        {
            if (memory[i].name == name)
            {
                memory[i].name = "Unused";

                // Merge with previous block if unused
                if (i > 0 && memory[i - 1].name == "Unused")
                {
                    memory[i - 1].end = memory[i].end;
                    for (int j = i; j < blockCount - 1; j++)
                    {
                        memory[j] = memory[j + 1];
                    }
                    blockCount--;
                    i--;
                }

                // Merge with next block if unused
                if (i < blockCount - 1 && memory[i + 1].name == "Unused")
                {
                    memory[i].end = memory[i + 1].end;
                    for (int j = i + 1; j < blockCount - 1; j++)
                    {
                        memory[j] = memory[j + 1];
                    }
                    blockCount--;
                }

                return;
            }
        }
        Console.WriteLine("Error: Process not found.");
    }

    // Main method
    static void Main(string[] args)
    {
        int size;
        string command;
        string name;
        int reqSize;
        bool exitFlag = false;

        do
        {
            // Prompt to restart the process or exit
            Console.Write("Enter memory size: ");
            size = int.Parse(Console.ReadLine());
            InitializeMemory(size);

            // Choose allocation strategy
            Console.WriteLine("Choose the memory allocation strategy:");
            Console.WriteLine("1. First Fit");
            Console.WriteLine("2. Best Fit");
            Console.WriteLine("3. Worst Fit");

            int choice = int.Parse(Console.ReadLine());

            while (true)
            {
                Console.WriteLine("\nallocator> ");
                command = Console.ReadLine();

                if (command.StartsWith("RQ"))
                {
                    var parts = command.Split(' ');
                    name = parts[1];
                    reqSize = int.Parse(parts[2]);

                    // Allocate based on user's chosen strategy
                    switch (choice)
                    {
                        case 1:
                            AllocateFirstFit(name, reqSize);
                            break;
                        case 2:
                            AllocateBestFit(name, reqSize);
                            break;
                        case 3:
                            AllocateWorstFit(name, reqSize);
                            break;
                        default:
                            Console.WriteLine("Invalid choice!");
                            break;
                    }
                }
                else if (command.StartsWith("RL"))
                {
                    name = command.Split(' ')[1];
                    Release(name);
                }
                else if (command.StartsWith("STAT"))
                {
                    PrintMemory();
                }
                else if (command.StartsWith("X"))
                {
                    break;
                }
                else if (command.StartsWith("EXIT"))
                {
                    // Restart the process
                    Console.WriteLine("Restarting the process...");
                    break;
                }
                else if (command.StartsWith("QUIT"))
                {
                    // Exit the program
                    exitFlag = true;
                    break;
                }
                else
                {
                    Console.WriteLine("Unknown command.");
                }
            }

        } while (!exitFlag);  // Loop until the user chooses to quit

        Console.WriteLine("Exiting program...");
    }
}
