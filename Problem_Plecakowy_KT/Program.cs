using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
class Program
{
    static Random rnd = new Random();
    static void Main(string[] args)
    {
        int zle_proby = 0;
        int proby = 0;
        int backpack = 2500;
        Console.WriteLine($"Objętosc plecaka: {backpack}");
        Console.WriteLine("---------------------------------------------");

        int[,] backpack_items = new int[2, 100];

        //zostawiamy miejsce na zapamiętanie najbardziej optymalnego rozwiązania
        int[,] suboptimal_solution = new int[2, 100];
        int best_free_space = backpack;

        //generujemy losowe objętosci obiektów plecaka w 2 rzędzie tablicy
        //w 1 rzędzie będziemy oznaczać obecnosc obiektu w plecaku jako '1' a nieobecnosc jako '0'
        for (int i = 0; i < backpack_items.GetLength(1); i++)
        {
            backpack_items[1, i] = rnd.Next(50, 91);
        }

        // Odliczanie czasu działania pętli
        Stopwatch time = new Stopwatch();
        time.Start(); // rozpoczyna odliczanie
        TimeSpan time_limit = TimeSpan.FromMinutes(2);

    GenerateParent:
        //tworzymy rodzica, który jest sekwencją losowo wybranych przedmiotów 
        int[,] parent = GenerateParent(backpack_items);

    MutateChild:

        if (time.Elapsed > time_limit)
        {
            Console.WriteLine("Czas upłynął. Najlepsze znalezione rozwiązanie to:");
            ShowBackpack(suboptimal_solution);
            Console.WriteLine($"Zostawia {best_free_space} wolnej przestrzeni");
            return;
        }
        proby++;
        Console.WriteLine($"Próba: {proby}");
        ShowBackpack(parent);
        //sprawdzamy aktualną wolną przestrzeń w plecaku (u rodzica)
        int space_parent = BackpackFreeSpace(parent, backpack);
        Console.WriteLine($"Wolne miejsce w plecaku {space_parent}");

        //aktualizujemy jeśli aktualny rezultat jest lepszy od poprzedniego 
        if (space_parent >= 0 && space_parent < best_free_space)
        {
            best_free_space = space_parent;
            suboptimal_solution = CloneArray(parent);
        }
        //tworzymy potomka, który jest idealną kopią rodzica 
        int[,] child = CloneArray(parent);
        //mutujemy losowy obiekt w potomku 
        child = Mutate(child);

        //sprawdzamy aktualną wolną przestrzeń w plecaku (u potomka)
        int space_child = BackpackFreeSpace(child, backpack);

        //aktualizujemy jeśli aktualny rezultat jest lepszy od poprzedniego 
        if (space_child >= 0 && space_child < best_free_space)
        {
            best_free_space = space_child;
            suboptimal_solution = CloneArray(child);
        }
        //ustalamy warunki 
        if (space_parent < 0)
        {
            if (space_child < 0)
            {
                goto GenerateParent;
            }
            else if (space_child > 0)
            {
                parent = CloneArray(child);
                goto MutateChild;
            }
            else
            {
                Console.WriteLine("Znaleziono najbardziej optymalne rozwiązanie");
                ShowBackpack(child);
                Console.WriteLine($"Wolne miejsce w plecaku {space_child}");
                return;
            }
        }
        else
        {
            if (space_child < space_parent && space_child > 0)
            {
                parent = CloneArray(child);
                goto MutateChild;
            }
            else if (space_parent > 0)
            {
                zle_proby++;
                if (zle_proby % 1000 == 0) // Co 100 prób
                {
                    Console.WriteLine("Generowanie nowego rodzica.");
                    goto GenerateParent;
                }
                goto MutateChild;
            }
            else
            {
                Console.WriteLine("Znaleziono najbardziej optymalne rozwiązanie");
                ShowBackpack(parent);
                Console.WriteLine($"Wolne miejsce w plecaku {space_parent}");
                return;
            }
        }
    }
    static void ShowBackpack(int[,] backpack_items)
    {
        Console.WriteLine("Aktualne obiekty do wrzucenia do plecaka: ");
        for (int i = 0; i < backpack_items.GetLength(0); i++)
        {
            for (int j = 0; j < backpack_items.GetLength(1); j++)
            {
                if (i == 0)
                {
                    Console.Write(backpack_items[i, j] + "   |");
                }
                else
                {
                    Console.Write(backpack_items[i, j] + "  |");
                }
            }
            Console.WriteLine(" ");
        }
    }

    static int[,] GenerateParent(int[,] backpack_items)
    {
        for (int i = 0; i < backpack_items.GetLength(1); i++)
        {
            backpack_items[0, i] = rnd.Next(2);
        }
        return backpack_items;
    }

    static int BackpackFreeSpace(int[,] backpack_items, int backpack_space)
    {
        int backpackitemsspace = 0;
        for (int i = 0; i < backpack_items.GetLength(1); i++)
        {
            if (backpack_items[0, i] == 1)
            {
                backpackitemsspace += backpack_items[1, i];
            }
        }
        return backpack_space - backpackitemsspace;
    }

    static int[,] Mutate(int[,] base_values)
    {
        int random_index = rnd.Next(100);
        if (base_values[0, random_index] == 1)
        {
            base_values[0, random_index] = 0;
        }
        else
        {
            base_values[0, random_index] = 1;
        }
        return base_values;
    }
    static int[,] CloneArray(int[,] base_array)
    {
        int[,] newArray = new int[2, 100];
        for (int i = 0; i < base_array.GetLength(0); i++)
        {
            for (int j = 0; j < base_array.GetLength(1); j++)
            {
                newArray[i, j] = base_array[i, j];
            }
        }
        return newArray;
    }
}